namespace DtmfDetection {
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public static class Utils {
        public static IEnumerable<PhoneKey> PhoneKeys() => Enum
            .GetValues(typeof(PhoneKey))
            .Cast<PhoneKey>()
            .Where(k => k != PhoneKey.None);

        public static PhoneKey ToPhoneKey(this in (int high, int low) dtmfTone) => dtmfTone switch {
            (1336, 941) => PhoneKey.Zero,
            (1209, 697) => PhoneKey.One,
            (1336, 697) => PhoneKey.Two,
            (1477, 697) => PhoneKey.Three,
            (1209, 770) => PhoneKey.Four,
            (1336, 770) => PhoneKey.Five,
            (1477, 770) => PhoneKey.Six,
            (1209, 852) => PhoneKey.Seven,
            (1336, 852) => PhoneKey.Eight,
            (1477, 852) => PhoneKey.Nine,
            (1209, 941) => PhoneKey.Star,
            (1477, 941) => PhoneKey.Hash,
            (1633, 697) => PhoneKey.A,
            (1633, 770) => PhoneKey.B,
            (1633, 852) => PhoneKey.C,
            (1633, 941) => PhoneKey.D,
            _ => PhoneKey.None
        };

        public static (int high, int low) ToDtmfTone(this PhoneKey key) => key switch {
            PhoneKey.Zero => (1336, 941),
            PhoneKey.One => (1209, 697),
            PhoneKey.Two => (1336, 697),
            PhoneKey.Three => (1477, 697),
            PhoneKey.Four => (1209, 770),
            PhoneKey.Five => (1336, 770),
            PhoneKey.Six => (1477, 770),
            PhoneKey.Seven => (1209, 852),
            PhoneKey.Eight => (1336, 852),
            PhoneKey.Nine => (1477, 852),
            PhoneKey.Star => (1209, 941),
            PhoneKey.Hash => (1477, 941),
            PhoneKey.A => (1633, 697),
            PhoneKey.B => (1633, 770),
            PhoneKey.C => (1633, 852),
            PhoneKey.D => (1633, 941),
            PhoneKey.None => (-1, -1),
            _ => throw new ArgumentOutOfRangeException(nameof(key), key, $"Unhandled {nameof(PhoneKey)}")
        };
    }
}
