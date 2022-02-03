using NetProc;
using NetProcGame.Game;

namespace NetProcGame.Events
{
    public delegate void AnonDelayedHandler();

    public delegate bool DelayedHandler(object param);

    public delegate bool SwitchAcceptedHandler(Switch sw);
}
