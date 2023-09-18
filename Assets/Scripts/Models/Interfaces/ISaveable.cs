namespace Assets.Scripts.Models.Interfaces
{
    public interface ISaveable<T> where T : class
    {
        T Save();
        void Load(T data);
        string UniqueId { get; }
    }
}
