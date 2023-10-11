using UnityEngine;

namespace Assets.Scripts.Models.Serializables
{
    [System.Serializable]
    public class SerializableLayerMask
    {
        public int value;

        // Constructor for converting LayerMask to SerializableLayerMask
        public SerializableLayerMask(LayerMask layerMask)
        {
            value = layerMask.value;
        }

        // Implicit conversion from SerializableLayerMask to LayerMask
        public static implicit operator LayerMask(SerializableLayerMask serializableLayerMask)
        {
            return new LayerMask { value = serializableLayerMask.value };
        }

        // Implicit conversion from LayerMask to SerializableLayerMask
        public static implicit operator SerializableLayerMask(LayerMask layerMask)
        {
            return new SerializableLayerMask(layerMask);
        }
    }
}
