using System.Collections.Generic;
using System.Xml.Serialization;
using UnityEngine;

namespace Saving.Models
{
    [XmlRoot("GameState", IsNullable = false)]
    [CreateAssetMenu(fileName = "Data", menuName = "CheddyShakes/ScriptableObjects/GameState template", order = 1)]
    public class GameState : ScriptableObject
    {
        public List<EventState> states;
    }
}