namespace DtmfDetection
{
    using System;

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