using UnityEngine;

namespace Assets.Scripts.Models
{
    [System.Serializable]
    public class SerializableHash128
    {
        public string hashString;

        // Constructor for converting Hash128 to SerializableHash128
        public SerializableHash128(Hash128 hash)
        {
            hashString = hash.ToString();
        }

        // Implicit conversion from SerializableHash128 to Hash128
        public static implicit operator Hash128(SerializableHash128 serializableHash)
        {
            return Hash128.Parse(serializableHash.hashString);
        }

        // Implicit conversion from Hash128 to SerializableHash128
        public static implicit operator SerializableHash128(Hash128 hash)
        {
            return new SerializableHash128(hash);
        }
    }
}
