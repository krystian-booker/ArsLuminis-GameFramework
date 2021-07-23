namespace EventSystem.Models.interfaces
{
    public interface IPauseEventExecution : IEventExecution
    {
        public void PauseExecution();
        public void ResumeExecution();
    }
}