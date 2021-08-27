using System;
using UnityEngine;

namespace Characters.models
{
    [Serializable]
    public class Animations
    {
        [Header("Locomotion")] 
        [SerializeField] public string planarSpeedParameter = "PlanarSpeed";
        [SerializeField] public string rotationParameter = "Rotation";
    }
}