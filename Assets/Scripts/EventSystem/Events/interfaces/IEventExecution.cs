using System.Collections;
using EventSystem.Events.Models;
using EventSystem.Models;

namespace EventSystem.Events.interfaces
{
    public interface IEventExecution
    {
        /// <summary>
        /// Starts the execution of our event, this method needs to implement the time delay. Look at the
        /// CharacterMovement class as an example
        /// </summary>
        /// <param name="gameEvent">Event running containing parameters for the execution</param>
        public IEnumerator Execute(GameEvent gameEvent);
        
        /// <summary>
        /// Returns true if the event is finished. Definition of "finished" will depend on the event that is being run
        /// For example CharacterMovement, reaching the set destination would be finished.
        /// </summary>
        public bool IsFinished();
        
        /// <summary>
        /// This method will clear all variable used in the execution of this event.
        /// This will allow us to reuse the instance of the event without instantiating a new one.
        /// </summary>
        public void Dispose();
    }
}