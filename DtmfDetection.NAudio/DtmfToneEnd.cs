namespace DtmfDetection.NAudio
{
    using System;

    public class DtmfToneEnd
    {
        public DtmfToneEnd(DtmfOccurence dtmfTone, TimeSpan duration)
        {
            DtmfTone = dtmfTone.Tone;
            Channel = dtmfTone.Channel;
            Duration = duration;
        }

        public DtmfTone DtmfTone { get; }

        public int Channel { get; private set; }

        public TimeSpan Duration { get; }
    }
}