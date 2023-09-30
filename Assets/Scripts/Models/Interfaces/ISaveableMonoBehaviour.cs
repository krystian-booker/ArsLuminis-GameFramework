namespace Assets.Scripts.Models.Interfaces
{
    public interface ISaveableMonoBehaviour
    {
        void Save();
        void Load();
        string GetUniqueId();
        int LoadPriority { get; }
    }
}
