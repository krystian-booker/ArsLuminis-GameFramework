namespace Assets.Scripts.Models.Interfaces
{
    public interface ISaveable
    {
        void Load(SaveableData data);
        SaveableData Save();
        string GetGuid();
    }
}
