using System;
using UnityEditor;
using UnityEngine;

namespace Characters
{
    [CreateAssetMenu(fileName = "Data", menuName = "CheddyShakes/Party/Create Character")]
    public class Character : ScriptableObject
    {
        private string _id = Guid.NewGuid().ToString();
        
        [Tooltip("Characters display name, will be overwritten via localization")]
        public string displayName;
        public GameObject prefab;
        public int level;
        public int exp;
        public int hp;
        public int maxHp;
        public int mp;
        public int maxMp;
        public int limitBreak;
        public int strength;
        public int speed;
        public int magic;
        public int spirit;
        public int defence;
        public int evasion;
        public int magicDefence;
        public int magicEvasion;
    }
}
