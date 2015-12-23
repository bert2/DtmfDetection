namespace DtmfDetection.NAudio
{
    using System;

    public class DtmfToneEnd
    {
        public DtmfToneEnd(DtmfTone dtmfTone, TimeSpan duration)
        {
            DtmfTone = dtmfTone;
            Duration = duration;
        }

        public DtmfTone DtmfTone { get; }

        public TimeSpan Duration { get; }
    }
}