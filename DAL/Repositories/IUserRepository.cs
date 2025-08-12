using Entity;
using System.Threading.Tasks;

namespace DAL.Repositories
{
    public interface IUserRepository : IRepository<User>
    {
        Task<User> GetByEmailAsync(string email);
        Task<User> GetWithTransactionsAsync(int id); // örnek: user + transactions include
    }
}
