using System;
using System.Collections.Generic;

namespace Saving.Models
{
    [Serializable]
    public class GameState
    {
        public List<EventState> states;
    }
}