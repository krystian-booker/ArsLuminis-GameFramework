using Saving;

namespace EventSystem.Models
{
    //These are just examples
    public class EventStates : ISaveable
    {
        public bool introCompleted;
        public bool tutorialCompleted;
        public bool secretOneFound;
        public bool secretTwoFound;
        
        public object CaptureState()
        {
            throw new System.NotImplementedException();
        }

        public void RestoreState(object state)
        {
            throw new System.NotImplementedException();
        }
    }
}