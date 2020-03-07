/*
Foundry
Copyright 2020 Carnegie Mellon University.
NO WARRANTY. THIS CARNEGIE MELLON UNIVERSITY AND SOFTWARE ENGINEERING INSTITUTE MATERIAL IS FURNISHED ON AN "AS-IS" BASIS. CARNEGIE MELLON UNIVERSITY MAKES NO WARRANTIES OF ANY KIND, EITHER EXPRESSED OR IMPLIED, AS TO ANY MATTER INCLUDING, BUT NOT LIMITED TO, WARRANTY OF FITNESS FOR PURPOSE OR MERCHANTABILITY, EXCLUSIVITY, OR RESULTS OBTAINED FROM USE OF THE MATERIAL. CARNEGIE MELLON UNIVERSITY DOES NOT MAKE ANY WARRANTY OF ANY KIND WITH RESPECT TO FREEDOM FROM PATENT, TRADEMARK, OR COPYRIGHT INFRINGEMENT.
Released under a MIT (SEI)-style license, please see license.txt or contact permission@sei.cmu.edu for full terms.
[DISTRIBUTION STATEMENT A] This material has been approved for public release and unlimited distribution.  Please see Copyright notice for non-US Government use and distribution.
Carnegie Mellon(R) and CERT(R) are registered in the U.S. Patent and Trademark Office by Carnegie Mellon University.
DM20-0194
*/

using System;
using System.Text;
using System.Text.RegularExpressions;

namespace Foundry.Portal.Extensions
{
    public static class StringExtensions
    {
        internal const string DefaultUrlStringValue = "_";
        internal const string UrlDelimiter = "-";
        public static string TagDelimiter = "|";

        public static string Wrap(this string value, string wrap = "\"")
        {
            return string.Format("{0}{1}{0}", wrap, (value ?? string.Empty).Replace(wrap, wrap + wrap), wrap);
        }

        /// <summary>
        /// Converts a <see cref="System.Guid"/> to a the base64 encoded string representation.
        /// </summary>
        /// <param name="guid">The unique identifier.</param>
        /// <returns></returns>
        public static string AsBase64String(this Guid guid)
        {
            var bytes = Encoding.UTF8.GetBytes(guid.ToString("N"));
            return Convert.ToBase64String(bytes);
        }

        /// <summary>
        /// Converts a base64 encoded string representation of a <see cref="System.Guid"/> to a <see cref="System.Guid"/>
        /// </summary>
        /// <param name="base64String">The base64 encoded string.</param>
        /// <returns></returns>
        public static Guid AsGuid(this string base64String)
        {
            var bytes = Convert.FromBase64String(base64String);
            var guidString = Encoding.UTF8.GetString(bytes);
            return Guid.Parse(guidString);
        }

        public static bool HasValue(this string s)
        {
            return !String.IsNullOrWhiteSpace(s);
        }

        public static string Before(this string s, char separator)
        {
            int x = s.IndexOf(separator);
            return (x >= 0) ? s.Substring(0, x) : "";
        }

        public static string After(this string s, char separator)
        {
            int x = s.IndexOf(separator);
            return (x >= 0) ? s.Substring(x + 1) : "";
        }

        public static string ToDisplay(this Enum e)
        {
            return e.ToString().Replace("_", " ");
        }

        public static string ExtractUrl(this string inputString)
        {
            if (string.IsNullOrEmpty(inputString))
            {
                return string.Empty;
            }

            if (Uri.IsWellFormedUriString(inputString, UriKind.Absolute))
            {
                return inputString;
            }

            string pattern = "(href|src)\\s*=\\s*(?:[\"'](?<1>[^\"']*)[\"']|(?<1>\\S+))";

            Match match = Regex.Match(inputString, pattern,
                                RegexOptions.IgnoreCase | RegexOptions.Compiled,
                                TimeSpan.FromSeconds(1));

            if (match.Success)
            {
                return match.Groups[1].ToString();
            }
            else
            {
                return string.Empty;
            }
        }

        public static string ToTitleCase(this string value)
        {
            var tokens = value.Split(new[] { " ", "-" }, StringSplitOptions.RemoveEmptyEntries);
            for (var i = 0; i < tokens.Length; i++)
            {
                var token = tokens[i];
                tokens[i] = token == token.ToUpper()
                    ? token
                    : token.Substring(0, 1).ToUpper() + token.Substring(1).ToLower();
            }

            return string.Join(" ", tokens);
        }
    }
}
