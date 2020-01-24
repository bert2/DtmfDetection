#nullable disable

namespace DtmfDetection.LastRelease {
    using System;
    using System.Diagnostics.CodeAnalysis;

    [SuppressMessage("Design", "CA1062:Validate arguments of public methods")]
    public class DtmfChangeHandler
    {
        private enum State
        {
            NoDtmf,
            Dtmf
        }

        private State currentState = State.NoDtmf;

        private DtmfTone lastTone = DtmfTone.None;

        private object clientData;

        [SuppressMessage("Usage", "CA2208:Instantiate argument exceptions correctly")]
        public void Handle<T>(DtmfTone tone, Func<DtmfTone, T> handleStart, Action<T, DtmfTone> handleEnd)
        {
            switch (currentState)
            {
                case State.NoDtmf:
                    if (tone != DtmfTone.None)
                    {
                        clientData = handleStart(tone);
                        currentState = State.Dtmf;
                    }
                    break;
                case State.Dtmf:
                    if (tone == DtmfTone.None)
                    {
                        handleEnd((T)clientData, lastTone);
                        currentState = State.NoDtmf;
                    }
                    else if (tone != lastTone)
                    {
                        handleEnd((T)clientData, lastTone);
                        clientData = handleStart(tone);
                    }
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            lastTone = tone;
        }
    }
}