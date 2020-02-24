namespace DtmfDetection {
    using System;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>Provides helpers to convert between `PhoneKey`s and their corresponding frequency encodings.</summary>
    public static class Utils {
        /// <summary>Enumerates all `PhoneKey`s except `PhoneKey.None`.</summary>
        /// <returns>An enumeration of all valid `PhoneKey`s.</returns>
        public static IEnumerable<PhoneKey> PhoneKeys() => Enum
            .GetValues(typeof(PhoneKey))
            .Cast<PhoneKey>()
            .Where(k => k != PhoneKey.None);

        /// <summary>Converts a frequency tuple to a `PhoneKey`.</summary>
        /// <param name="dtmfTone">The high and low frequencies as a `ValueTuple`.</param>
        /// <returns>The matching `PhoneKey` or `PhoneKey.None` in case the given frequencies don't encode a DTMF key.</returns>
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

        /// <summary>Converts a `PhoneKey` to the two frequencies it is encoded with in audio data.</summary>
        /// <param name="key">The key to convert.</param>
        /// <returns>A `ValueTuple` holding the key's high frequency in the first position and its low frequency in the second position.</returns>
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

        /// <summary>Converts a `PhoneKey` to its UTF-8 symbol.</summary>
        /// <param name="key">The key to convert.</param>
        /// <returns>A `char` representing the `PhoneKey`.</returns>
        public static char ToSymbol(this PhoneKey key) => key switch
        {
            PhoneKey.Zero => '0',
            PhoneKey.One => '1',
            PhoneKey.Two => '2',
            PhoneKey.Three => '3',
            PhoneKey.Four => '4',
            PhoneKey.Five => '5',
            PhoneKey.Six => '6',
            PhoneKey.Seven => '7',
            PhoneKey.Eight => '8',
            PhoneKey.Nine => '9',
            PhoneKey.Star => '*',
            PhoneKey.Hash => '#',
            PhoneKey.A => 'A',
            PhoneKey.B => 'B',
            PhoneKey.C => 'C',
            PhoneKey.D => 'D',
            PhoneKey.None => ' ',
            _ => throw new ArgumentOutOfRangeException(nameof(key), key, $"Unhandled {nameof(PhoneKey)}")
        };
    }
}
