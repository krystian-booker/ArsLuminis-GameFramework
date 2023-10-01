using UnityEngine;

namespace Assets.Scripts.Models
{
    [System.Serializable]
    public class SerializableVector3Int
    {
        public int x, y, z;

        // Constructor for converting Vector3Int to SerializableVector3Int
        public SerializableVector3Int(Vector3Int vector)
        {
            x = vector.x;
            y = vector.y;
            z = vector.z;
        }

        // Implicit conversion from SerializableVector3Int to Vector3Int
        public static implicit operator Vector3Int(SerializableVector3Int serializableVector)
        {
            return new Vector3Int(serializableVector.x, serializableVector.y, serializableVector.z);
        }

        // Implicit conversion from Vector3Int to SerializableVector3Int
        public static implicit operator SerializableVector3Int(Vector3Int vector)
        {
            return new SerializableVector3Int(vector);
        }
    }

}
