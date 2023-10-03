using Assets.Scripts.Managers;
using Assets.Scripts.Models.Interfaces;
using UnityEngine;

namespace Assets.Scripts.Models.Abstract
{
    public abstract class SaveableMonoBehaviour<T> : MonoBehaviour, ISaveable where T : SaveableData
    {
        private string Guid = System.Guid.NewGuid().ToString();

        private int Priority = int.MaxValue;

        public int GetPriority() => Priority;

        public string GetGuid() => Guid;
        
        private void Start()
        {
            GameManager.Instance.SaveManager.RegisterSaveableObject(Guid, this);
        }

        public abstract T SaveData();
        public abstract void LoadData(T saveData);

        public SaveableData Save()
        {
            return SaveData();
        }

        public void Load(SaveableData data)
        {
            LoadData((T)data);
        }
    }
}
