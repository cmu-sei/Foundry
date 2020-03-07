/*
Foundry
Copyright 2020 Carnegie Mellon University.
NO WARRANTY. THIS CARNEGIE MELLON UNIVERSITY AND SOFTWARE ENGINEERING INSTITUTE MATERIAL IS FURNISHED ON AN "AS-IS" BASIS. CARNEGIE MELLON UNIVERSITY MAKES NO WARRANTIES OF ANY KIND, EITHER EXPRESSED OR IMPLIED, AS TO ANY MATTER INCLUDING, BUT NOT LIMITED TO, WARRANTY OF FITNESS FOR PURPOSE OR MERCHANTABILITY, EXCLUSIVITY, OR RESULTS OBTAINED FROM USE OF THE MATERIAL. CARNEGIE MELLON UNIVERSITY DOES NOT MAKE ANY WARRANTY OF ANY KIND WITH RESPECT TO FREEDOM FROM PATENT, TRADEMARK, OR COPYRIGHT INFRINGEMENT.
Released under a MIT (SEI)-style license, please see license.txt or contact permission@sei.cmu.edu for full terms.
[DISTRIBUTION STATEMENT A] This material has been approved for public release and unlimited distribution.  Please see Copyright notice for non-US Government use and distribution.
Carnegie Mellon(R) and CERT(R) are registered in the U.S. Patent and Trademark Office by Carnegie Mellon University.
DM20-0194
*/

using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Foundry.Analytics
{
    public static class StringExtensions
    {
        internal const string DefaultUrlStringValue = "_";
        internal const string UrlDelimiter = "-";
        class Replacement
        {
            public Replacement()
            {
                Bad = new List<string>();
            }

            public Replacement(string bad, string good)
                : this()
            {
                Bad.Add(bad);
                Good = good;
            }

            public List<string> Bad { get; set; }
            public string Good { get; set; }
        }

        static List<Replacement> Replacements = new List<Replacement>
        {
            new Replacement("'", ""),
            new Replacement { Bad = new List<string>{ "ö", "ð", "Ö", "ø", "Ø" }, Good = "o" },
            new Replacement { Bad = new List<string>{ "é", "è" }, Good = "e" },
            new Replacement("ä", "a"),
            new Replacement("ñ", "n"),
            new Replacement("ü", "u"),
            new Replacement { Bad = new List<string>{ UrlDelimiter, "_", ".", "/", ":" }, Good = " " }
        };

        public static string ToUrlString(this string value, bool truncate = true, int truncateAt = 100)
        {
            if (string.IsNullOrEmpty(value))
                return DefaultUrlStringValue;

            string v = truncate ? value.Truncate(truncateAt) : value;

            foreach (var replacement in Replacements)
            {
                foreach (var bad in replacement.Bad)
                {
                    v = v.Replace(bad, replacement.Good);
                }
            }

            string result = Regex.Replace(v, @"[^a-zA-Z0-9\s]", "")
                .Replace(" ", UrlDelimiter)
                .Replace(UrlDelimiter + UrlDelimiter + UrlDelimiter, UrlDelimiter)
                .Replace(UrlDelimiter + UrlDelimiter, UrlDelimiter)
                .Trim(UrlDelimiter.ToCharArray());

            return string.IsNullOrEmpty(result) ? DefaultUrlStringValue : result.ToLower();
        }

        public static string Truncate(this string value, int max)
        {
            if (string.IsNullOrEmpty(value))
                return string.Empty;

            string v = value.Trim();

            if (v.Length <= max)
                return v;

            return v.Substring(0, max - 3) + "...";
        }
    }
}

