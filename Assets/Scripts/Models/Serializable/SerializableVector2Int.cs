using UnityEngine;

namespace Assets.Scripts.Models
{
    [System.Serializable]
    public class SerializableVector2Int
    {
        public int x, y;

        // Constructor for converting Vector2Int to SerializableVector2Int
        public SerializableVector2Int(Vector2Int vector)
        {
            x = vector.x;
            y = vector.y;
        }

        // Implicit conversion from SerializableVector2Int to Vector2Int
        public static implicit operator Vector2Int(SerializableVector2Int serializableVector)
        {
            return new Vector2Int(serializableVector.x, serializableVector.y);
        }

        // Implicit conversion from Vector2Int to SerializableVector2Int
        public static implicit operator SerializableVector2Int(Vector2Int vector)
        {
            return new SerializableVector2Int(vector);
        }
    }

}
