namespace DtmfDetection.NAudio
{
    using System;

    public class DtmfToneEnd
    {
        public DtmfToneEnd(DtmfTone dtmfTone, int channel, TimeSpan duration)
        {
            DtmfTone = dtmfTone;
            Channel = channel;
            Duration = duration;
        }

        public DtmfTone DtmfTone { get; }

        public int Channel { get; private set; }

        public TimeSpan Duration { get; }
    }
}