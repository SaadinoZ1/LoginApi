using Microsoft.AspNetCore.Identity;

namespace LoginApi.Models
{
    public class User
    {
        public int UserId { get; set; }
        public string? UserName { get; set; }
        public string? PasswordHash { get; set; }
        public string? RefreshToken { get; set; }    
        public DateTime TokenCreated { get; set; }
        public DateTime TokenExpires { get; set; }
        public string? LastIssuedJwtToken { get; set; }
        public ICollection<UserRole>? UserRoles { get; set; }
    }
}
