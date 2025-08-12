using DAL.Context;
using Entity;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace DAL.Repositories
{
    public class UserRepository : Repository<User>, IUserRepository
    {
        public UserRepository(AppDbContext context) : base(context) { }

        public async Task<User> GetByEmailAsync(string email)
        {
            return await _context.Set<User>().FirstOrDefaultAsync(u => u.Email == email);
        }

        public async Task<User> GetWithTransactionsAsync(int id)
        {
            // User sınıfında Transactions koleksiyonu varsa kullan:
            return await _context.Users
                                 .Include(u => u.Transactions) // modeline göre property adı farklıysa düzelt
                                 .FirstOrDefaultAsync(u => u.Id == id);
        }
    }
}
