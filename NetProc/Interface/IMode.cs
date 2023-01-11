using NetProc.Events;
using System;

namespace NetProc.Interface
{
    public interface IMode : IComparable
    {
        IGameController Game { get; set; }
        int Priority { get; set; }
        ILayer Layer { get; set; }
        void AddSwitchHandler(string Name, string Event_Type, double Delay = 0, SwitchAcceptedHandler Handler = null);
        void CancelDelayed(string Name);
        void Delay(string Name, EventType Event_Type, double Delay, Delegate Handler, object Param = null);
        void DispatchDelayed();
        bool HandleEvent(Event evt);
        void ModeStarted();
        void ModeStopped();
        void ModeTick();
        void ModeTopMost();
        string ToString();
        void UpdateLamps();
    }
}