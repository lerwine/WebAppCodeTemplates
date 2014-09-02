using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Erwine.Leonard.T.ExtensionMethods.MultilineText
{
    public static class StringExtensions
    {
        private static readonly Regex _lineSplitRegex1 = new Regex(@"\r\n");
        private static readonly Regex _lineSplitRegex2 = new Regex(@"[\r\n]");

        public static IEnumerable<string> ToLines(this string text)
        {
            if (text == null)
                return new string[0];

            return StringExtensions._lineSplitRegex1.Split(text).SelectMany(s => StringExtensions._lineSplitRegex2.Split(s));
        }

        public static IEnumerable<string> TrimEachEnd(this IEnumerable<string> source)
        {
            if (source == null)
                return new string[0];

            return source.Select(s => (s == null) ? "" : s.TrimEnd());
        }

        public static IEnumerable<string> PrependEach(this IEnumerable<string> source, string text)
        {
            if (source == null)
                return new string[0];

            if (String.IsNullOrEmpty(text))
                return source;

            return source.Select(s => (s == null) ? text : text + s.TrimEnd());
        }

        public static IEnumerable<string> ToLinesWithHeader(this string value, string header)
        {
            if (value == null)
                return new string[0];

            IEnumerable<string> result = value.Trim().ToLines();
            result = result.Take(1).Select(s => s.Trim()).Concat(result.Skip(1).Select(s => "\t" + s.TrimEnd()));

            if (String.IsNullOrWhiteSpace(header))
                return result.Take(1).Select(s => s.Trim()).Concat(result.Skip(1).Select(s => ("\t" + s).TrimEnd()));

            string v = result.FirstOrDefault();
            if (v == null)
                v = "";
            else
            {
                v = v.Trim();
                result = result.Skip(1).Select(s => ("\t" + s).TrimEnd());
            }

            IEnumerable<string> hdrLn = header.Trim().ToLines();
            hdrLn = hdrLn.Take(1).Select(s => s.Trim()).Concat(hdrLn.Skip(1).Select(s => ("\t\t" + s).TrimEnd())).Reverse();
            return hdrLn.Take(1).Select(s => String.Format("{0}: {1}", s, v)).Concat(hdrLn.Skip(1)).Reverse().Concat(result);
        }
    }
}
