using System;
using UnityEngine;

namespace Characters
{
    [Serializable]
    public class CharacterStats
    {
        private string _id = Guid.NewGuid().ToString();

        [Tooltip("Characters display name, will be overwritten via localization")]
        public string displayName;

        public int level;
        public int exp;
        public int hp;
        public int maxHp;
        public int mp;
        public int maxMp;
        public int speed;
        public int magic;
        public int defence;
        public int evasion;
        public int magicDefence;
        public int magicEvasion;
    }
}