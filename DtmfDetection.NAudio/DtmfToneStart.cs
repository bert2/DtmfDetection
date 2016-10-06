namespace DtmfDetection.NAudio
{
    using System;

    public class DtmfToneStart
    {
        public DtmfToneStart(DtmfTone dtmfTone, int channel, DateTime position)
        {
            DtmfTone = dtmfTone;
            Channel = channel;
            Position = position;
        }

        public DtmfTone DtmfTone { get; }

        public int Channel { get; }

        public DateTime Position { get; }
    }
}