using System.Threading.Tasks;

namespace Assets.Scripts.Models.Interfaces
{
    public interface ISaveable
    {
        void Load(SaveableData data);
        SaveableData Save();
        string GetGuid();
        int GetPriority();
        Task LoadAsync(SaveableData data);
    }
}
