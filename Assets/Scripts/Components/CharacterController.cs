using Assets.Scripts.Models.Abstract;
using System;
using UnityEngine;

namespace Assets.Scripts.Components
{
    [Serializable]
    public class PlayerData
    {
        public Vector3 playerLocation;
        public int health;
        public int xp;

        public bool isAlive = true;

        public PlayerData(Vector3 playerLocation, int health, int xp)
        {
            this.playerLocation = playerLocation;
            this.health = health;
            this.xp = xp;
        }
    }

    public class CharacterController : SaveableMonoBehaviour<PlayerData>
    {
        public Vector3 playerLocation;
        public int health = 100;
        public int xp;
        public bool isAlive = true;

        public string test = "hello";

        public override void Load(PlayerData data)
        {
            throw new NotImplementedException();
        }

        public override PlayerData Save()
        {
            throw new NotImplementedException();
        }

        void Awake()
        {
            //print(health);
        }
    }
}
