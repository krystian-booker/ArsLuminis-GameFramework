using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace Saving.Models
{
    [Serializable]
    [XmlRoot("GameState", IsNullable = false)]
    public class GameState
    {
        public string playerName;
        public int sceneId;
        
        public List<EventStateValue> states;
    }
}