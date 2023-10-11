using Assets.Scripts.Managers;
using Assets.Scripts.Models.Interfaces;
using Assets.Scripts.Models.Attributes;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Models.Abstract
{
    public abstract class SaveableMonoBehaviour<T> : MonoBehaviour, ISaveable where T : SaveableData
    {
        [UniqueIdentifier, SerializeField]
        private string _guid;

        private int _priority = int.MaxValue;

        private void OnValidate()
        {
            if (string.IsNullOrEmpty(_guid))
            {
                _guid = System.Guid.NewGuid().ToString();
            }
        }

        private void Start()
        {
            GameManager.Instance.SaveManager.RegisterSaveableObject(_guid, this);
        }

        public int GetPriority() => _priority;
        public string GetGuid() => _guid;
        public abstract T SaveData();
        public abstract void LoadData(T saveData);

        public virtual async Task LoadAsync(SaveableData data)
        {
            //Not implemented
        }

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
