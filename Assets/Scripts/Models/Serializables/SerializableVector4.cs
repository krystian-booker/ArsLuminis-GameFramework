using UnityEngine;

namespace Assets.Scripts.Models.Serializables
{
    [System.Serializable]
    public class SerializableVector4
    {
        public float x, y, z, w;

        // Constructor for converting Vector4 to SerializableVector4
        public SerializableVector4(Vector4 vector)
        {
            x = vector.x;
            y = vector.y;
            z = vector.z;
            w = vector.w;
        }

        // Implicit conversion from SerializableVector4 to Vector4
        public static implicit operator Vector4(SerializableVector4 serializableVector)
        {
            return new Vector4(serializableVector.x, serializableVector.y, serializableVector.z, serializableVector.w);
        }

        // Implicit conversion from Vector4 to SerializableVector4
        public static implicit operator SerializableVector4(Vector4 vector)
        {
            return new SerializableVector4(vector);
        }
    }
}
