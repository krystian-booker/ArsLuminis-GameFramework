using UnityEngine;

namespace Assets.Scripts.Models.Serializables
{
    [System.Serializable]
    public class SerializableVector3
    {
        public float x, y, z;

        // Constructor for converting Vector3 to SerializableVector3
        public SerializableVector3(Vector3 vector)
        {
            x = vector.x;
            y = vector.y;
            z = vector.z;
        }

        // Implicit conversion from SerializableVector3 to Vector3
        public static implicit operator Vector3(SerializableVector3 serializableVector)
        {
            return new Vector3(serializableVector.x, serializableVector.y, serializableVector.z);
        }

        // Implicit conversion from Vector3 to SerializableVector3
        public static implicit operator SerializableVector3(Vector3 vector)
        {
            return new SerializableVector3(vector);
        }
    }
}
