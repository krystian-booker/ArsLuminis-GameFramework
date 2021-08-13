using XNode;

namespace EventSystem.Models.interfaces
{
    public interface IEventExecution
    {
        /// <summary>
        /// Starts the execution of our event, this method needs to implement the time delay. Look at the
        /// CharacterMovement class as an example
        /// </summary>
        /// <param name="gameEvent">Event running containing parameters for the execution</param>
        public void Execute(Node gameEvent);
        
        /// <summary>
        /// Returns true if the event is finished. Definition of "finished" will depend on the event that is being run
        /// For example CharacterMovement, reaching the set destination would be finished.
        /// </summary>
        public bool IsFinished();
    }
}