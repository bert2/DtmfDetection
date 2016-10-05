namespace DtmfDetection.NAudio
{
    using System;

    public class DtmfToneStart
    {
        public DtmfToneStart(DtmfOccurence dtmfTone, DateTime position)
        {
            DtmfTone = dtmfTone.Tone;
            Channel = dtmfTone.Channel;
            Position = position;
        }

        public DtmfTone DtmfTone { get; }

        public int Channel { get; }

        public DateTime Position { get; }
    }
}