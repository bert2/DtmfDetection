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

        private object state;

        public void Handle<TState>(DtmfTone tone, Func<DtmfTone, TState> handleStart, Action<TState, DtmfTone> handleEnd)
        {
            switch (currentState)
            {
                case State.NoDtmf:
                    if (tone != DtmfTone.None)
                    {
                        state = handleStart(tone);
                        currentState = State.Dtmf;
                    }
                    break;
                case State.Dtmf:
                    if (tone == DtmfTone.None)
                    {
                        handleEnd((TState)state, lastTone);
                        currentState = State.NoDtmf;
                    }
                    else if (tone != lastTone)
                    {
                        handleEnd((TState)state, lastTone);
                        state = handleStart(tone);
                    }
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            lastTone = tone;
        }
    }
}