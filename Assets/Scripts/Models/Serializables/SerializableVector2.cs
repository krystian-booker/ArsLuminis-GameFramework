using UnityEngine;

namespace Assets.Scripts.Models.Serializables
{
    [System.Serializable]
    public class SerializableVector2
    {
        public float x, y;

        // Constructor for converting Vector2 to SerializableVector2
        public SerializableVector2(Vector2 vector)
        {
            x = vector.x;
            y = vector.y;
        }

        // Implicit conversion from SerializableVector2 to Vector2
        public static implicit operator Vector2(SerializableVector2 serializableVector)
        {
            return new Vector2(serializableVector.x, serializableVector.y);
        }

        // Implicit conversion from Vector2 to SerializableVector2
        public static implicit operator SerializableVector2(Vector2 vector)
        {
            return new SerializableVector2(vector);
        }
    }
}
