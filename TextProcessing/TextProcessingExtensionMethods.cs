using System;
using System.Collections.Generic;
using System.Linq;

namespace TextProcessing
{
    public static class TextProcessingExtensionMethods
    {
        /// <summary>
        /// Get incomplete character sequence from end of string
        /// </summary>
        /// <param name="text">Text to search</param>
        /// <param name="charSequence">String containing sequence of characters to match</param>
        /// <returns>String containing characters which exist in <paramref name="charSequence"/>, as long as it does not sequentially match <paramref name="charSequence"/></returns>
        public static string GetTrailingIncompleteCharSeq(this string text, string charSequence)
        {
            if (text == null || charSequence == null || charSequence.Length < 2)
                return "";

            return new String(TextProcessingExtensionMethods._GetTrailingIncompleteCharSeq(text, charSequence.ToCharArray()).ToArray());
        }

        /// <summary>
        /// Get incomplete character sequence from end of character array
        /// </summary>
        /// <param name="text">Text to search</param>
        /// <param name="charSequence">Sequence of characters to match</param>
        /// <returns>Characters which exist in <paramref name="charSequence"/>, as long as it does not sequentially match <paramref name="charSequence"/></returns>
        public static char[] GetTrailingIncompleteCharSeq(this IEnumerable<char> text, char[] charSequence)
        {
            if (text == null || charSequence == null || charSequence.Length < 2)
                return new char[0];

            return TextProcessingExtensionMethods._GetTrailingIncompleteCharSeq(text, charSequence);
        }

        private static char[] _GetTrailingIncompleteCharSeq(IEnumerable<char> text, char[] charSequence)
        {
            char[] potentialMatch = text.Reverse().Take(charSequence.Length).Reverse().ToArray();
            if (potentialMatch.Length == 0)
                return new char[0];

            if (potentialMatch.Length == charSequence.Length && potentialMatch.SequenceEqual(charSequence))
                return new char[0];

            while ((potentialMatch = potentialMatch.Skip(1).ToArray()).Length > 0)
            {
                if (charSequence.Take(potentialMatch.Length).SequenceEqual(potentialMatch))
                    break;
            }

            return potentialMatch;
        }

        /// <summary>
        /// Parses text for individual lines, according to the current environment NewLine character sequence.
        /// </summary>
        /// <param name="text">String to parse.</param>
        /// <returns>Collection of lines parsed from <paramref name="text"/>.</returns>
        public static IEnumerable<string> ToLines(this string text)
        {
            return text.ToLines(Environment.NewLine, true);
        }

        /// <summary>
        /// Parses a <see cref="System.String"/> for individual lines, according to the <paramref name="newLineSeparator"/> value.
        /// </summary>
        /// <param name="text">Text to parse.</param>
        /// <param name="newLineSeparator">string which represents a line separator.</param>
        /// <param name="isLoose">if true, then even partial matches to the character sequence in <paramref name="newLineSeparator"/> willbe interpreted as a line separator;
        /// otherwise, only complete matches will be interpreted as a line separator.</param>
        /// <returns>Collection of strings which represent lines parsed from <paramref name="text"/>.</returns>
        public static IEnumerable<string> ToLines(this string text, string newLineSeparator, bool isLoose)
        {
            if (text == null)
                return new string[0];

            if (newLineSeparator == null)
                return new string[] { text };

            return TextProcessingExtensionMethods._ToLines(text.ToCharArray(), newLineSeparator.ToArray(), isLoose).Select(c => new String(c));
        }

        /// <summary>
        /// Parses a collection of <see cref="System.Char"/>s for individual lines, according to the current environment NewLine character sequence.
        /// </summary>
        /// <param name="text">Collection of characters to parse.</param>
        /// <returns>Collection of character arrays which represent lines parsed from <paramref name="text"/>.</returns>
        public static IEnumerable<char[]> ToLines(this IEnumerable<char> text)
        {
            return text.ToLines(Environment.NewLine.ToCharArray(), true);
        }

        /// <summary>
        /// Parses a collection of <see cref="System.Char"/>s for individual lines, according to the <paramref name="newLineSeparator"/> value.
        /// </summary>
        /// <param name="text">Collection of characters to parse.</param>
        /// <param name="newLineSeparator">Sequence of characters which represent a line separator.</param>
        /// <param name="isLoose">if true, then even partial matches to the character sequence in <paramref name="newLineSeparator"/> willbe interpreted as a line separator;
        /// otherwise, only complete matches will be interpreted as a line separator.</param>
        /// <returns>Collection of character arrays which represent lines parsed from <paramref name="text"/>.</returns>
        public static IEnumerable<char[]> ToLines(this IEnumerable<char> text, char[] newLineSeparator, bool isLoose)
        {
            if (text == null)
                return new char[0][];

            if (newLineSeparator == null || newLineSeparator.Length == 0)
                return new char[][] { text.ToArray() };

            return TextProcessingExtensionMethods._ToLines(text, newLineSeparator, isLoose);
        }

        private static IEnumerable<char[]> _ToLines(IEnumerable<char> text, char[] newLineSeparator, bool isLoose)
        {
            char[] remaining = text.ToArray();
            Func<char[], char[][]> getNextLine;
            #region Set getNextLine according to isLoose value

            if (newLineSeparator.Length == 1)
            {
                getNextLine = (char[] ca) =>
                {
                    char[] l = ca.TakeWhile(c => c != newLineSeparator[0]).ToArray();
                    return new char[][] { l, ca.Skip(l.Length).Take(1).ToArray() };
                };
            }
            else if (isLoose)
            {
                getNextLine = (char[] ca) =>
                {
                    char[] l = ca.TakeWhile(c => !newLineSeparator.Any(n => c == n)).ToArray();
                    if (l.Length == ca.Length)
                        return new char[][] { l, new char[0] };
                    IEnumerable<char> m = ca.Take(1);
                    char[] r = ca.Skip(l.Length + 1).Take(newLineSeparator.Length - 1).ToArray();
                    IEnumerator<char> e = newLineSeparator.Skip(1).GetEnumerator();
                    return new char[][] { l, m.Concat(r.TakeWhile(c => e.MoveNext() && e.Current == c)).ToArray() };
                };
            }
            else
            {
                getNextLine = (char[] ca) =>
                {
                    char m = newLineSeparator[0];
                    char[] l = new char[0];
                    char[] r = ca;
                    do
                    {
                        char[] cc = r.TakeWhile(c => c != m).ToArray();
                        l = l.Concat(cc).ToArray();

                        if (cc.Length == r.Length)
                            break;

                        r = r.Skip(cc.Length).ToArray();

                        if (r.Length < newLineSeparator.Length)
                            return new char[][] { l.Concat(r).ToArray(), new char[0] };

                        if (r.Take(newLineSeparator.Length).SequenceEqual(newLineSeparator))
                            return new char[][] { l, newLineSeparator };

                        l = l.Concat(r.Take(1)).ToArray();
                        r = r.Skip(1).ToArray();
                    } while (r.Length > 0);

                    return new char[][] { l, new char[0] };
                };
            }

            #endregion

            bool lastLineEndedInNl = true;

            while (remaining.Length > 0)
            {
                char[][] line = getNextLine(remaining);
                lastLineEndedInNl = line[1].Length > 0;
                yield return line[0];
                if (line[0].Length == remaining.Length)
                    break;
                remaining = remaining.Skip(line[0].Length + line[1].Length).ToArray();
            }

            if (lastLineEndedInNl)
                yield return new char[0];
        }
    }
}
