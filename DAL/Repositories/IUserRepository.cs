using Entity;
using System.Threading.Tasks;

namespace DAL.Repositories
{
    public interface IUserRepository : IRepository<User>
    {
        Task<User> GetByEmailAsync(string email);
        Task<User> GetWithTransactionsAsync(int id); // örnek: user + transactions include
        Task<IEnumerable<User>> GetAdminsAsync();
        Task<(IEnumerable<User> Items, int TotalCount)> QueryAsync(
            string? search,
            string? role,
            bool? isBanned,
            string? sortBy,
            bool sortDesc,
            int page,
            int pageSize);
    }
}
