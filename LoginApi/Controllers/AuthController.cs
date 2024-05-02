using LoginApi.Data;
using LoginApi.DTOs;
using LoginApi.Models;
using LoginApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.ComponentModel.DataAnnotations;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace LoginApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        public readonly ApplicationDbContext _context;
        private readonly IConfiguration _configuration;
        private readonly IUserService _userService;

        public AuthController(IConfiguration configuration, ApplicationDbContext context, IUserService userService)
        {
            _configuration = configuration;
            _context = context;
            _userService = userService;
        }

        [HttpGet, Authorize]
        public ActionResult<string> GetMyName()
        {
            return Ok(_userService.GetMyName());
        }

        [HttpPost("register")]
        public async Task<ActionResult<User>> Register(UserDto request)
        {
            if (_context.User.Any(u => u.UserName == request.UserName))
            {
                return BadRequest("User already exists.");
            }

            if (_context.User.Any(u => u.Email == request.Email))
            {
                return BadRequest("Email already exists.");
            }

            if (string.IsNullOrWhiteSpace(request.Email) || !new EmailAddressAttribute().IsValid(request.Email))
            {
                return BadRequest("Invalid email.");
            }

            var user = new User
            {
                UserName = request.UserName,
                Email = request.Email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
            };

            _context.User.Add(user);
            await _context.SaveChangesAsync();

            // Gérer l'affectation du rôle
            var role = await _context.Roles.FirstOrDefaultAsync(r => r.RoleName == request.Role);
            if (role == null)
            {
                role = new Role { RoleName = request.Role };
                _context.Roles.Add(role);
                await _context.SaveChangesAsync();
            }

            var userRole = new UserRole { UserId = user.UserId, RoleId = role.RoleId };
            _context.UserRoles.Add(userRole);
            await _context.SaveChangesAsync();

            return Ok(new { UserId = user.UserId, UserName = user.UserName, Email = user.Email, Role = role.RoleName });
        }



        [HttpPost("addrole")]
        public async Task<IActionResult> AddRole(RoleDto roleDto)
        {
            CreateRoleIfNotExists(roleDto.RoleName);

            return Ok(new { Message = $"Role {roleDto.RoleName} added successfully." });
        }

        private void CreateRoleIfNotExists(string roleName)
        {
            var roleExists = _context.Roles.Any(r => r.RoleName == roleName);
            if (!roleExists)
            {
                var role = new Role { RoleName = roleName };
                _context.Roles.Add(role);
                _context.SaveChanges();
            }
        }

        [HttpPost("login")]
        public ActionResult<UserDto> Login(UserDto request)
        {
            var user = _context.User.FirstOrDefault(u => u.UserName == request.UserName);
            if (user == null)
            {
                return BadRequest("User not found.");
            }

            if (!BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
            {
                return BadRequest("Wrong password.");
            }
            // Vérifier si le refreshToken est toujours valide
            if (user.RefreshToken != null && user.TokenExpires > DateTime.UtcNow)
            {
                if (!string.IsNullOrEmpty(user.LastIssuedJwtToken))
                {
                    return Ok(new { token = user.LastIssuedJwtToken, refreshToken = user.RefreshToken });

                }
            }
            // Générer de nouveaux tokens si le refreshToken a expiré
            var token = GenerateJwtToken(user);
            var refreshToken = GenerateRefreshToken();
            SetRefreshToken(refreshToken, user);
            user.LastIssuedJwtToken = token;
            _context.SaveChanges();

            return Ok(new { token, refreshToken = refreshToken.Token });
        }


        [HttpPost("refresh-token")]
        public async Task<IActionResult> RefreshToken(string refreshToken)
        {
            var user = await _context.User.FirstOrDefaultAsync(u => u.RefreshToken == refreshToken && u.TokenExpires > DateTime.UtcNow);
            if (user == null)
            {
                return Unauthorized("Invalid or expired refresh token");
            }

            // Générer un nouveau JWT
            var newToken = GenerateJwtToken(user);
            var newRefreshToken = GenerateRefreshToken();

            // Sauvegarder le nouveau refresh token
            SetRefreshToken(newRefreshToken, user);

            return Ok(new { token = newToken, refreshToken = newRefreshToken.Token });
        }

        private void SetRefreshToken(RefreshToken newRefreshToken, User user)
        {
         var cookieOptions = new CookieOptions
        {
          HttpOnly = true,
          Expires = newRefreshToken.Expires
         };
          Response.Cookies.Append("refreshToken", newRefreshToken.Token, cookieOptions);
          user.RefreshToken = newRefreshToken.Token;
          user.TokenCreated = newRefreshToken.Created;
          user.TokenExpires = newRefreshToken.Expires;
          _context.User.Update(user); // Assurez-vous d'utiliser le contexte approprié
          _context.SaveChanges();
}

        private RefreshToken GenerateRefreshToken()
        {
            var refreshToken =  new RefreshToken
            {
                Token = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64)),
                Expires = DateTime.Now.AddDays(5) 
            };
            return refreshToken;
        }
        private string GenerateJwtToken(User user)
        {
            var userRoles = _context.UserRoles
                .Where(ur => ur.UserId == user.UserId)
                .Include(ur => ur.Role)
                .Select(ur => ur.Role.RoleName)
                .ToList();

            var claims = new List<Claim>
    {
        new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()),
        new Claim(ClaimTypes.Name, user.UserName),
        new Claim(ClaimTypes.Email, user.Email)
    };

            userRoles.ForEach(role => claims.Add(new Claim(ClaimTypes.Role, role)));

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["AppSettings:Token"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.Now.AddHours(1),
                SigningCredentials = creds
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }


        [HttpPut("update/{userId}")]
        public async Task<IActionResult> UpdateUser(int userId, UserDto updateUserDto)
        {
            var user = await _context.User.Include(u => u.UserRoles).ThenInclude(ur => ur.Role).FirstOrDefaultAsync(u => u.UserId == userId);
            if (user == null)
            {
                return NotFound("User not found.");
            }

            user.UserName = updateUserDto.UserName ?? user.UserName;

            if (!string.IsNullOrEmpty(updateUserDto.Password))
            {
                user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(updateUserDto.Password);
            }

            if (!string.IsNullOrWhiteSpace(updateUserDto.Email))
            {
                // Validation de l'email
                if (new EmailAddressAttribute().IsValid(updateUserDto.Email))
                {
                    user.Email = updateUserDto.Email;
                }
                else
                {
                    return BadRequest("Invalid email address.");
                }
            }
            user.UserRoles ??= new List<UserRole>();
            // Mise à jour du rôle de l'utilisateur
            if (!string.IsNullOrWhiteSpace(updateUserDto.Role))
            {
                var newRole = await _context.Roles.FirstOrDefaultAsync(r => r.RoleName == updateUserDto.Role);
                if (newRole == null)
                {
                    return BadRequest("Role does not exist.");
                }

                // Supprimer tous les rôles existants (facultatif si l'utilisateur ne peut avoir qu'un seul rôle)
                _context.UserRoles.RemoveRange(user.UserRoles);

                // Ajouter le nouveau rôle
                user.UserRoles.Add(new UserRole { UserId = user.UserId, RoleId = newRole.RoleId });
            }


            _context.User.Update(user);
            await _context.SaveChangesAsync();

            return Ok(new { UserId = user.UserId, UserName = user.UserName, Email = user.Email, Roles = user.UserRoles.Select(ur => ur.Role.RoleName).ToList()
            });
        }

}
    }
