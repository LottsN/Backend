using lab2.Models;

namespace lab2.Services
{
    public interface ITokenService
    {
        Task AddToBlacklist(BlacklistToken model);
        bool IsTokenBlacklisted(string tokenToCheck);
    }

    public class TokenService : ITokenService
    {
        private readonly DatabaseContext _context;

        public TokenService(DatabaseContext context)
        {
            _context = context;
        }

        public async Task AddToBlacklist(BlacklistToken model)
        {
            await _context.BlacklistedTokens.AddAsync(new BlacklistToken
            (
                model.value
            ));
            await _context.SaveChangesAsync();
        }

        public bool IsTokenBlacklisted(string tokenToCheck)
        {
            var token = _context.BlacklistedTokens.FirstOrDefault(x => x.value == tokenToCheck);

            if (token == null)
            {
                return false;
            }

            return true;
        }

    }
}
