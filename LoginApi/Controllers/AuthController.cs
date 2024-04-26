using LoginApi.Data;
using LoginApi.DTOs;
using LoginApi.Models;
using LoginApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
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

            var user = new User
            {
                UserName = request.UserName,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
            };

            _context.User.Add(user);
            await _context.SaveChangesAsync();


            return Ok(new
            {
                UserId = user.UserId,
                UserName = request.UserName,

            });
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

        [HttpPost("assign-role")]
        public async Task<IActionResult> AssignRoleToUser(int userId, string roleName)
        {
            // Vérifier si l'utilisateur existe
            var user = await _context.User.FindAsync(userId);
            if (user == null)
            {
                return NotFound($"User with ID {userId} not found.");
            }

            // Vérifier si le rôle existe
            var role = await _context.Roles.FirstOrDefaultAsync(r => r.RoleName == roleName);
            if (role == null)
            {
                return NotFound($"Role '{roleName}' not found.");
            }

            // Vérifier si l'utilisateur a déjà ce rôle
            var userRoleExists = await _context.UserRoles.AnyAsync(ur => ur.UserId == userId && ur.RoleId == role.RoleId);
            if (userRoleExists)
            {
                return BadRequest($"User already has the '{roleName}' role.");
            }

            // Attribuer le rôle à l'utilisateur
            var userRole = new UserRole { UserId = userId, RoleId = role.RoleId };
            _context.UserRoles.Add(userRole);
            await _context.SaveChangesAsync();

            return Ok($"Role '{roleName}' assigned to user ID {userId} successfully.");
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
        public async Task<ActionResult<string>> RefreshToken( )
        {
            var refreshToken = Request.Cookies["refreshToken"];
            var user = await _context.User.Include(u => u.UserRoles).ThenInclude(ur => ur.Role).FirstOrDefaultAsync(u => u.RefreshToken == refreshToken && u.TokenExpires > DateTime.UtcNow);

            if (user == null )
            {
                return Unauthorized("Invalid or expired refresh token");
            }


            // Générer un nouveau JWT et un nouveau Refresh Token
            var token = GenerateJwtToken(user);
            var newRefreshToken = GenerateRefreshToken();

            // Sauvegarder le nouveau Refresh Token avec l'utilisateur
            SetRefreshToken(newRefreshToken, user);

            return Ok(new { token = token, refreshToken = newRefreshToken.Token });
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
            var userRoles = _context.UserRoles.Include(ur => ur.Role).FirstOrDefault(ur => ur.UserId == user.UserId)?.Role;
            // Check if a role was assigned, and use the role name.
            if (userRoles == null)
            {
                throw new Exception("No role assigned to the user.");
            }

            // Create a list of claims for the user.
            var claims = new List<Claim>
    {
        new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()),
        new Claim(ClaimTypes.Name, user.UserName ?? string.Empty),
        new Claim(ClaimTypes.Role, "Manager")
    };

            // Key for signing the JWT token, retrieved from configuration.
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
            var user = await _context.User.FindAsync(userId);
            if (user == null)
            {
                return NotFound("User not found.");
            }

            user.UserName = updateUserDto.UserName ?? user.UserName;
            // Update other fields as necessary

            _context.User.Update(user);
            await _context.SaveChangesAsync();

            return Ok(new { UserId = user.UserId, UserName = user.UserName });
        }
    }


}
