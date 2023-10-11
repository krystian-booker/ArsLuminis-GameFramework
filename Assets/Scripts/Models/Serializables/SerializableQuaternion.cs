using UnityEngine;

namespace Assets.Scripts.Models.Serializables
{
    [System.Serializable]
    public class SerializableQuaternion
    {
        public float x, y, z, w;

        // Constructor for converting Quaternion to SerializableQuaternion
        public SerializableQuaternion(Quaternion quaternion)
        {
            x = quaternion.x;
            y = quaternion.y;
            z = quaternion.z;
            w = quaternion.w;
        }

        // Implicit conversion from SerializableQuaternion to Quaternion
        public static implicit operator Quaternion(SerializableQuaternion serializableQuaternion)
        {
            return new Quaternion(serializableQuaternion.x, serializableQuaternion.y, serializableQuaternion.z, serializableQuaternion.w);
        }

        // Implicit conversion from Quaternion to SerializableQuaternion
        public static implicit operator SerializableQuaternion(Quaternion quaternion)
        {
            return new SerializableQuaternion(quaternion);
        }
    }
}
