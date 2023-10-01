using UnityEngine;

namespace Assets.Scripts.Models
{
    [System.Serializable]
    public class SerializableColor32
    {
        public byte r, g, b, a;

        // Constructor for converting Color32 to SerializableColor32
        public SerializableColor32(Color32 color)
        {
            r = color.r;
            g = color.g;
            b = color.b;
            a = color.a;
        }

        // Implicit conversion from SerializableColor32 to Color32
        public static implicit operator Color32(SerializableColor32 serializableColor)
        {
            return new Color32(serializableColor.r, serializableColor.g, serializableColor.b, serializableColor.a);
        }

        // Implicit conversion from Color32 to SerializableColor32
        public static implicit operator SerializableColor32(Color32 color)
        {
            return new SerializableColor32(color);
        }
    }
}
