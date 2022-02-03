namespace NetProc
{
    public interface IVirtualDriver : IDriver
    {
        void ChangeState(bool newState);
        void IncSchedule();
        void UpdateState(DriverState newState);
    }
}