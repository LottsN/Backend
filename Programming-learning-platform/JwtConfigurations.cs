using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace lab2
{
    public class JwtConfigurations
    {
        public const string Issuer = "JwtTestIssuer"; // издатель токена
        public const string Audience = "JwtTestClient"; // потребитель токена
        private const string Key = "SuperSecretKeyBazingaLolKek!*228322";   // ключ для шифрации
        public const int Lifetime = 10; // время жизни токена - 10 дней
        public static SymmetricSecurityKey GetSymmetricSecurityKey()
        {
            return new SymmetricSecurityKey(Encoding.ASCII.GetBytes(Key));
        }

        public static int ReturnLifetime()
        {
            int lifetime = Lifetime;
            return lifetime;
        }
    }
}
