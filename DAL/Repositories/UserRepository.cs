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

        public async Task<IEnumerable<User>> GetAdminsAsync()
        {
            return await _context.Users
                                 .Where(u => u.Role == UserRole.Admin)
                                 .ToListAsync();
        }

        public async Task<(IEnumerable<User> Items, int TotalCount)> QueryAsync(
            string? search,
            string? role,
            bool? isBanned,
            string? sortBy,
            bool sortDesc,
            int page,
            int pageSize)
        {
            if (page <= 0) page = 1;
            if (pageSize <= 0) pageSize = 10;

            IQueryable<User> query = _context.Users.AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
            {
                string s = search.Trim();
                query = query.Where(u =>
                    u.FirstName.Contains(s) ||
                    u.LastName.Contains(s) ||
                    u.Email.Contains(s));
            }

            if (!string.IsNullOrWhiteSpace(role))
            {
                if (Enum.TryParse<UserRole>(role, true, out var roleEnum))
                {
                    query = query.Where(u => u.Role == roleEnum);
                }
            }

            if (isBanned.HasValue)
            {
                query = query.Where(u => u.IsBanned == isBanned.Value);
            }

            // Sıralama
            switch (sortBy?.ToLowerInvariant())
            {
                case "firstname":
                    query = sortDesc ? query.OrderByDescending(u => u.FirstName) : query.OrderBy(u => u.FirstName);
                    break;
                case "lastname":
                    query = sortDesc ? query.OrderByDescending(u => u.LastName) : query.OrderBy(u => u.LastName);
                    break;
                case "email":
                    query = sortDesc ? query.OrderByDescending(u => u.Email) : query.OrderBy(u => u.Email);
                    break;
                case "role":
                    query = sortDesc ? query.OrderByDescending(u => u.Role) : query.OrderBy(u => u.Role);
                    break;
                case "isbanned":
                    query = sortDesc ? query.OrderByDescending(u => u.IsBanned) : query.OrderBy(u => u.IsBanned);
                    break;
                case "id":
                default:
                    query = sortDesc ? query.OrderByDescending(u => u.Id) : query.OrderBy(u => u.Id);
                    break;
            }

            int total = await query.CountAsync();
            var items = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (items, total);
        }
    }
}
