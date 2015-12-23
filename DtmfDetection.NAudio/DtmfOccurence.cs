namespace DtmfDetection.NAudio
{
    using System;

    public class DtmfOccurence
    {
        public DtmfOccurence(DtmfTone dtmfTone, TimeSpan position, TimeSpan duration)
        {
            DtmfTone = dtmfTone;
            Position = position;
            Duration = duration;
        }

        public DtmfTone DtmfTone { get; }

        public TimeSpan Position { get; }

        public TimeSpan Duration { get; }
    }
}
