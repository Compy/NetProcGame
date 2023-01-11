using System.Collections.Generic;

namespace NetProc.Interface
{
    public interface IModeQueue
    {
        List<IMode> Modes { get; }

        void Add(IMode mode);
        void Clear();
        void handle_event(Event evt);
        void Remove(IMode mode);
        void tick();
    }
}