using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using WebApplication2.Models;


namespace WebApplication2.Auth
{
    public class Auth : AuthInterface
    {
        private readonly IConfiguration _configuration;

        public Auth(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public String LoginGetToken(string email, string password,String role)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            //System.Console.WriteLine(_configuration["SecretKey:key"]!);
            var key = Encoding.ASCII.GetBytes(_configuration["SecretKey:key"]!);
            var claims = new List<Claim>
{
            new Claim("email", email), // Replace with actual email
            new Claim(ClaimTypes.Role, role) // Replace with actual role
};

            SigningCredentials credentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature);
            var token = new JwtSecurityToken(
          
          
                expires: DateTime.Now.AddDays(100), 
                claims: claims,
                signingCredentials: credentials
            );

            var tokenString = tokenHandler.WriteToken(token);
            return tokenString;
        }
    }
}
