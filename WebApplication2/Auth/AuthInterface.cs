namespace WebApplication2.Auth
{
    public interface AuthInterface
    {
        String LoginGetToken(string email, string password, String role);
        Task<bool> Register(string email, string password, String role);

        public bool VerifyPasswordHash(string password, byte[] passwordHash, byte[] passwordSalt);
    }
}
