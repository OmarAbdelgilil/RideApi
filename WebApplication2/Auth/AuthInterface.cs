namespace WebApplication2.Auth
{
    public interface AuthInterface
    {
        String LoginGetToken(string email, string password, String role);
    }
}
