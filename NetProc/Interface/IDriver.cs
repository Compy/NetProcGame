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
        void Patter(ushort on_time = 10, ushort off_time = 10, ushort orig_on_time = 0);
        void Tick();
        void Schedule(uint sch, int v1, bool v2);
        void Enable();
    }
}
