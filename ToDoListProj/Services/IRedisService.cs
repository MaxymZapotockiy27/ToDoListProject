using System.Threading.Tasks;

namespace ToDoListProj.Services
{
    public interface IRedisService
    {
        Task SetCodeAsync(string key, string code);
        Task<string> GetCodeAsync(string key);
        Task<bool> IsCodeExpiredAsync(string key);
        Task RemoveCodeAsync(string key);
    }
}
