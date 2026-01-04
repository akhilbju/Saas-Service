using System;
using System.Threading.Tasks;

public interface ICacheService
{
    Task<T> GetOrCreateAsync<T>(string key,Func<Task<T>> factory,int expirationInMinutes);
    void Remove(string key);
}