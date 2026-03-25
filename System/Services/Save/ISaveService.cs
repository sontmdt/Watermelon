public interface ISaveService : IService
{
    T Load<T>(string key) where T : new();
    void Save<T>(string key, T data);
    void Delete(string key);
    bool HasKey(string key);
}
