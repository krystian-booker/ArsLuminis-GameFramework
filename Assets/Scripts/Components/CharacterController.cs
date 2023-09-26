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

        public PlayerData(Vector3 playerLocation, int health, int xp, bool isAlive)
        {
            this.playerLocation = playerLocation;
            this.health = health;
            this.xp = xp;
            this.isAlive = isAlive;
        }
    }

    public class CharacterController : SaveableMonoBehaviour<PlayerData>
    {
        public Vector3 playerLocation;
        public int health = 100;
        public int xp;
        public bool isAlive = true;

        protected override void Start()
        {
            base.Start();
            Debug.Log("Awake called in CharacterController");
        }

        public override PlayerData Save()
        {
            return new PlayerData(playerLocation, health, xp, isAlive);
        }

        public override void Load(PlayerData data)
        {
            if (data != null)
            {
                playerLocation = data.playerLocation;
                health = data.health;
                xp = data.xp;
                isAlive = data.isAlive;

                // Move the player to the saved location
                transform.position = playerLocation;
            }
            else
            {
                Debug.LogError("No data to load!");
            }
        }
    }
}
