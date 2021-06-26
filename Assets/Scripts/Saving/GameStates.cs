using System;
using System.Collections.Generic;
using Saving.Models;
using UnityEngine;
using UnityEngine.Serialization;

namespace Saving
{
    [CreateAssetMenu(fileName = "Data", menuName = "CheddyShakes/Saving/Create game state")]
    public class GameStates : ScriptableObject
    {
        [SerializeField]
        public List<EventStateValue> states;
    }
}