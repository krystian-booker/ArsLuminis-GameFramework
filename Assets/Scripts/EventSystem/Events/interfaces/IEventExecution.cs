using System.Collections;
using EventSystem.Events.Models;
using EventSystem.Models;

namespace EventSystem.Events.interfaces
{
    public interface IEventExecution
    {
        public IEnumerator Execute(GameEvent gameEvent);
        public bool IsFinished();
    }
}