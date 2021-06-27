using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace Models.Saving
{
    [Serializable]
    [XmlRoot("GameState", IsNullable = false)]
    public class GameState
    {
        public string playerName;
        public int sceneId;
        
        public List<EventStateValue> states;
        
        //TODO: Extend, player stats
    }
}