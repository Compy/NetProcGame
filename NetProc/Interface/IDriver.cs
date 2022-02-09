using System;

namespace NetProc
{
    public interface IDriver
    {
        ushort Number { get; set; }
        DriverState State { get; set; }
        string Name { get; set; }
        double _last_time_changed { get; set; }

        void Disable();
        void Pulse(int milliseconds = -1);
        void FuturePulse(int milliseconds = -1, UInt16 futureTime = 100);
        void Patter(byte on_time = 10, byte off_time = 10, byte orig_on_time = 0);
        void Tick();
        void Schedule(uint schedule, int cycle_seconds = 0, bool now = true);
        void Enable();
    }
}
