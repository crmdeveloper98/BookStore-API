using System.Collections.Generic;
using System.Threading.Tasks;

namespace BookStore_UI.Contracts
{
    public interface IBaseRepository<T> where T : class
    {
        public Task<T> Get(string url, int id);
        public Task<IList<T>> Get(string url);
        public Task<bool> Create(string url, T obj);
        public Task<bool> Update(string url, T obj);
        public Task<bool> Delete(string url, int id);

    }
}
