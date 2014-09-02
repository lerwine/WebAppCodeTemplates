using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TextProcessing.ExtensionMethods
{
    public static class TextManagmentExtensions
    {
        public static string GetTrailingPartialSequence(this string text, string sequence)
        {
            if (text == null || text.Length == 0 || sequence == null || sequence.Length == 0)
                return "";

            return new String(TextManagmentExtensions._GetTrailingPartialSequence(text.ToCharArray(), sequence.ToCharArray()));
        }

        public static char[] GetTrailingPartialSequence(this IEnumerable<char> buffer, char[] sequence)
        {
            if (buffer == null || sequence == null || sequence.Length == 0)
                return new char[0];

            return TextManagmentExtensions._GetTrailingPartialSequence(buffer, sequence);
        }

        private static char[] _GetTrailingPartialSequence(IEnumerable<char> buffer, char[] sequence)
        {
            char[] lastChars = buffer.Reverse().Take(sequence.Length).Reverse().ToArray();

            if (lastChars.Length == 0)
                return new char[0];

            char[] b;
            if (lastChars.Length < sequence.Length)
                b = lastChars;
            else
            {
                if (lastChars.SequenceEqual(sequence))
                    return new char[0];
                b = lastChars.Skip(1).ToArray();
            }

            while (b.Length > 0)
            {
                if (sequence.Take(b.Length).SequenceEqual(b))
                    break;
                b = b.Skip(1).ToArray();
            }

            return b;
        }

        public static IEnumerable<string> GetLines(this string text, string sequence, bool loose)
        {
            if (text == null)
                return new string[0];

            if (text.Length == 0 || sequence == null || sequence.Length == 0)
                return new string[] { text };

            return TextManagmentExtensions._GetLines(text.ToCharArray(), sequence.ToCharArray(), loose)
                .Select(c => new String(c));
        }

        public static IEnumerable<char[]> GetLines(this IEnumerable<char> buffer, char[] sequence, bool loose)
        {
            if (buffer == null)
                return new char[0][];

            if (sequence == null || sequence.Length == 0)
                return new char[][] { new char[0] };

            return TextManagmentExtensions._GetLines(buffer, sequence, loose);
        }

        public static IEnumerable<char[]> _GetLines(IEnumerable<char> buffer, char[] sequence, bool loose)
        {
            IEnumerable<char> remaining = buffer;

            bool endsWithNewLine = true;
            char[] newLineChars;
            do
            {
                char[] line = TextManagmentExtensions._GetNextLine(remaining, sequence, loose, out newLineChars);

                if (line.Length == 0)
                {
                    if (newLineChars.Length == 0)
                        continue;

                    remaining = remaining.Skip(newLineChars.Length);
                    yield return line;
                    continue;
                }

                remaining = remaining.Skip(newLineChars.Length + line.Length);

                endsWithNewLine = newLineChars.Length > 0;

                if (!loose)
                {
                    yield return line;
                    continue;
                }

                char[] lr = line;
                bool emitBlank = true;
                do
                {
                    char[] cc = lr.TakeWhile(c => !sequence.Any(s => s == c)).ToArray();
                    yield return cc;
                    lr = lr.Skip(cc.Length).ToArray();
                    if (lr.Length == 0)
                        emitBlank = false;
                    else
                        lr = lr.Skip(1).ToArray();
                } while (lr.Length > 0);

                if (emitBlank)
                    yield return new char[0];
            } while (newLineChars.Length > 0);

            if (endsWithNewLine)
                yield return new char[0];
        }

        public static char[] _GetNextLine(IEnumerable<char> buffer, char[] sequence, bool loose, out char[] newLineChars)
        {
            IEnumerable<char> remaining = buffer;
            char[] currentLine = new char[0];

            do
            {
                char[] cc = remaining.TakeWhile(c => c != sequence[0]).ToArray();
                currentLine = currentLine.Concat(cc).ToArray();
                remaining = remaining.Skip(cc.Length);
                char[] lookAhead = remaining.Take(sequence.Length).ToArray();
                if (lookAhead.Length == 0)
                {
                    newLineChars = new char[0];
                    return currentLine;
                }

                if (lookAhead.SequenceEqual(sequence))
                {
                    newLineChars = lookAhead;
                    return currentLine;
                }


                if (loose)
                {
                    while (lookAhead.Length > 2)
                    {
                        lookAhead = lookAhead.Take(lookAhead.Length - 1).ToArray();
                        if (lookAhead.SequenceEqual(sequence.Take(lookAhead.Length)))
                        {
                            newLineChars = lookAhead;
                            return currentLine.Concat(remaining.Take(sequence.Length - lookAhead.Length - 1)).ToArray();
                        }
                    }

                    newLineChars = remaining.Take(1).ToArray();
                    return currentLine;
                }

                currentLine = currentLine.Concat(remaining.Take(1)).ToArray();
                remaining = remaining.Skip(1);
            } while (true);
        }
    }
}
