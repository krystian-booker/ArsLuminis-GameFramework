using Assets.Scripts.Models;
using Assets.Scripts.Models.Abstract;
using System;
using UnityEngine;

namespace Assets.Scripts.Components
{
    [Serializable]
    public class PlayerData : SaveableData
    {
        public Vector2 vector2;
        public Vector2Int vector2Int;
        public Vector3 vector3;
        public Vector3Int vector3Int;
        public Vector4 vector4;
        public Quaternion quaternion;
        public Matrix4x4 matrix4X4;
        public LayerMask layerMask;
        public Hash128 hash128;
        public Color32 color32;

        public PlayerData(string guid, Vector2 vector2, Vector2Int vector2Int, Vector3 vector3, Vector3Int vector3Int, Vector4 vector4, Quaternion quaternion, Matrix4x4 matrix4X4, LayerMask layerMask, Hash128 hash128, Color32 color32) : base(guid)
        {
            this.vector2 = vector2;
            this.vector2Int = vector2Int;
            this.vector3 = vector3;
            this.vector3Int = vector3Int;
            this.vector4 = vector4;
            this.quaternion = quaternion;
            this.matrix4X4 = matrix4X4;
            this.layerMask = layerMask;
            this.hash128 = hash128;
            this.color32 = color32;
        }
    }

    public class CharacterController : SaveableMonoBehaviour<PlayerData>
    {
        public Vector3 playerLocation;
        public int health = 100;
        public int xp = 0;
        public bool isAlive = true;

        private void Start()
        {
            playerLocation = this.transform.position;
            health = 0;
            isAlive = false;
        }

        private void Update()
        {
            xp++;
            xp *= 2;
            health++;
        }

        public override PlayerData SaveData()
        {
            Vector2 vector2 = new Vector2(1, 2);
            Vector2Int vector2Int = new Vector2Int(3, 4);
            Vector3 vector3 = new Vector3(5, 6, 7);
            Vector3Int vector3Int = new Vector3Int(8, 9, 10);
            Vector4 vector4 = new Vector4(11, 12, 13, 14);
            Quaternion quaternion = new Quaternion(0.5f, 0.5f, 0.5f, 0.5f);
            Matrix4x4 matrix4x4 = Matrix4x4.identity;
            LayerMask layerMask = new LayerMask();
            Hash128 hash128 = Hash128.Compute("Some String");
            Color32 color32 = new Color32(255, 0, 0, 255);

            var playerData = new PlayerData(this.GetGuid(), vector2, vector2Int, vector3, vector3Int, vector4, quaternion, matrix4x4, layerMask, hash128, color32);
            return playerData;
        }

        public override void LoadData(PlayerData saveData)
        {
        }
    }
}
