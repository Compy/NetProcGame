using System;

namespace NetProc
{
    public interface ILampController
    {
        void PlayShow(string key, bool repeat = false, Delegate callback = null);
        void RegisterShow(string key, string show_file);
        void RestorePlayback();
        void RestoreState(string key);
        void SaveState(string key);
        void StopShow();
    }
}