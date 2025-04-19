namespace ReplicatedKeyValueStore.Interfaces;

public interface IMyCache
{
    bool Add(object key, object value);
    bool AddOrUpdate(object key, object value);
    object Get(object key);
    bool Remove(object key);
    void Clear();
}