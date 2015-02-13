using System;

namespace AngleSharp.Core
{
    internal static class PlatformExtensions
    {
        // The starting codepoint for Unicode plane 1.  Plane 1 contains 0x010000 ~ 0x01ffff.
        internal const int UNICODE_PLANE01_START = 0x10000;
        // The end codepoint for Unicode plane 16.  This is the maximum code point value allowed for Unicode.
        // Plane 16 contains 0x100000 ~ 0x10ffff.
        internal const int UNICODE_PLANE16_END = 0x10ffff;
        internal const int HIGH_SURROGATE_START = 0x00d800;
        internal const int LOW_SURROGATE_END = 0x00dfff;

        internal static class CharUnicodeInfo
        {
            internal const char HIGH_SURROGATE_START = '\ud800';
            internal const char HIGH_SURROGATE_END = '\udbff';
            internal const char LOW_SURROGATE_START = '\udc00';
            internal const char LOW_SURROGATE_END = '\udfff';
        }

        internal static String ConvertFromUtf32(this int utf32)
        {
#if !SILVERLIGHT
            return Char.ConvertFromUtf32(utf32);
#else
            // For UTF32 values from U+00D800 ~ U+00DFFF, we should throw.  They
            // are considered as irregular code unit sequence, but they are not illegal.
            if ((utf32 < 0 || utf32 > UNICODE_PLANE16_END) || (utf32 >= HIGH_SURROGATE_START && utf32 <= LOW_SURROGATE_END))
                throw new ArgumentOutOfRangeException("utf32", "ArgumentOutOfRange_InvalidUTF32");
            if (utf32 < UNICODE_PLANE01_START)
            {
                // This is a BMP character.
                return (Char.ToString((char)utf32));
            }
            // This is a sumplementary character.  Convert it to a surrogate pair in UTF-16.
            utf32 -= UNICODE_PLANE01_START;
            char[] surrogate = new char[2];
            surrogate[0] = (char)((utf32 / 0x400) + (int)CharUnicodeInfo.HIGH_SURROGATE_START);
            surrogate[1] = (char)((utf32 % 0x400) + (int)CharUnicodeInfo.LOW_SURROGATE_START);
            return (new String(surrogate));
#endif
        }
         
        /*=============================ConvertToUtf32===================================
       ** Convert a character or a surrogate pair starting at index of the specified string 
       ** to UTF32 value.
       ** The char pointed by index should be a surrogate pair or a BMP character.
       ** This method throws if a high-surrogate is not followed by a low surrogate.
       ** This method throws if a low surrogate is seen without preceding a high-surrogate.
       ==============================================================================*/

        internal static int ConvertToUtf32(String s, int index)
        {
            if (s == null)
                throw new ArgumentNullException("s");

            if (index < 0 || index >= s.Length)
                throw new ArgumentOutOfRangeException("index", "ArgumentOutOfRange_Index");
            
            // Check if the character at index is a high surrogate.
            int temp1 = (int)s[index] - CharUnicodeInfo.HIGH_SURROGATE_START;
            if (temp1 >= 0 && temp1 <= 0x7ff)
            {
                // Found a surrogate char.
                if (temp1 <= 0x3ff)
                {
                    // Found a high surrogate.
                    if (index < s.Length - 1)
                    {
                        int temp2 = (int)s[index + 1] - CharUnicodeInfo.LOW_SURROGATE_START;
                        if (temp2 >= 0 && temp2 <= 0x3ff)
                        {
                            // Found a low surrogate.
                            return ((temp1 * 0x400) + temp2 + UNICODE_PLANE01_START);
                        }
                        else
                        {
                            throw new ArgumentException("Argument_InvalidHighSurrogate", "s");
                        }
                    }
                    else
                    {
                        // Found a high surrogate at the end of the string.
                        throw new ArgumentException("Argument_InvalidHighSurrogate", "s");
                    }
                }
                else
                {
                    // Find a low surrogate at the character pointed by index.
                    throw new ArgumentException("Argument_InvalidLowSurrogate", "s");
                }
            }
            // Not a high-surrogate or low-surrogate. Genereate the UTF32 value for the BMP characters.
            return ((int)s[index]);
        }
    }
}
