using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using WebApplication1.Data;
using WebApplication2.Models;


namespace WebApplication2.Auth
{
    public class Auth : AuthInterface
    {
        private readonly IConfiguration _configuration;
        private readonly DataContext _db;


        public Auth(IConfiguration configuration, DataContext db)
        {
            _configuration = configuration;
            _db = db;
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


        public async Task<bool>  Register(string email, string password,string role) {

            byte[] passwordHash;
            byte[] passwordSalt;
            CreatePasswordHash(password, out passwordHash, out passwordSalt);
            Credentials credentials = new Credentials()
            {
                Email = email,
                PasswordHash = passwordHash,
                PasswordSalt = passwordSalt,
                Role = role,
            };
            if (await(_db.Credentials.FindAsync(credentials.Email))!=null)
            {
                return false;
            }
            await _db.Credentials.AddAsync(credentials);
            await _db.SaveChangesAsync();

            return true;
        }

        private void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            using (var hmac = new System.Security.Cryptography.HMACSHA512())
            {
                passwordSalt = hmac.Key;
                passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
            }
        }

        public bool VerifyPasswordHash(string password, byte[] passwordHash, byte[] passwordSalt)
        {
            using (var hmac = new System.Security.Cryptography.HMACSHA512(passwordSalt))
            {
                var computedHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
                for (int i = 0; i < computedHash.Length; i++)
                {
                    if (computedHash[i] != passwordHash[i])
                        return false;
                }
            }
            return true;
        }


    }
}
