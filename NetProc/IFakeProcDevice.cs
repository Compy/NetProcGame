namespace NetProc
{
    public interface IFakeProcDevice : IProcDevice
    {
        void AddSwitchEvent(ushort number, EventType event_type);
    }
}
