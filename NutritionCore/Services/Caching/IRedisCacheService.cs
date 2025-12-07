namespace NutritionCore.Services.Caching;

public interface IRedisCacheService
{
    T? GetData<T>(string key);
    void SetData<T>(string key, T data);
    void Delete<T>(string key);
}