using System;

namespace Assets.Scripts.Models
{
    [Serializable]
    public class SaveableData
    {
        public string Guid;
        public int Priority;

        public SaveableData(string guid, int priority = int.MaxValue)
        {
            this.Guid = guid;
            this.Priority = priority;
        }
    }
}
