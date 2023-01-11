using System;

namespace NetProc.Interface
{
    public interface ILampShowMode : IMode
    {
        void Load(string filename, bool repeat = false, Delegate callback = null);
        void Restart();
    }
}