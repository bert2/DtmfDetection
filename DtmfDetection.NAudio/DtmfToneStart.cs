namespace DtmfDetection.NAudio
{
    using System;

    public class DtmfToneStart
    {
        public DtmfToneStart(DtmfTone dtmfTone, DateTime position)
        {
            DtmfTone = dtmfTone;
            Position = position;
        }

        public DtmfTone DtmfTone { get; }

        public DateTime Position { get; }
    }
}