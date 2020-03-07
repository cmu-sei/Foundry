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

namespace Foundry.Portal.Extensions
{
    public static class Formats
    {
        public const string Date = "M/d/yyyy";
        public const string Time = "h:mmtt";
    }

    public static class DateTimeExtensions
    {

        const int SECOND = 1;
        const int MINUTE = 60 * SECOND;
        const int HOUR = 60 * MINUTE;
        const int DAY = 24 * HOUR;
        const int MONTH = 30 * DAY;

        public static string ToAgo(this DateTime dt)
        {
            var ts = new TimeSpan(DateTime.UtcNow.Ticks - dt.Ticks);
            double delta = Math.Abs(ts.TotalSeconds);

            if (delta < 1 * MINUTE)
                return ts.Seconds == 1 ? "just now" : ts.Seconds + " seconds ago";

            if (delta < 2 * MINUTE)
                return "a minute ago";

            if (delta < 45 * MINUTE)
                return ts.Minutes + " minutes ago";

            if (delta < 90 * MINUTE)
                return "an hour ago";

            if (delta < 24 * HOUR)
                return ts.Hours + " hours ago";

            if (delta < 48 * HOUR)
                return "yesterday";

            if (delta < 30 * DAY)
                return ts.Days + " days ago";

            if (delta < 12 * MONTH)
            {
                int months = Convert.ToInt32(Math.Floor((double)ts.Days / 30));
                return months <= 1 ? "one month ago" : months + " months ago";
            }

            int years = Convert.ToInt32(Math.Floor((double)ts.Days / 365));
            return years <= 1 ? "one year ago" : years + " years ago";
        }

        public static string AsDateString(this DateTime date)
        {
            return date.ToString(Formats.Date);
        }

        public static string AsDateString(this DateTime? date)
        {
            if (date.HasValue)
            {
                return AsDateString(date.Value);
            }

            return string.Empty;
        }

        public static string AsTimeString(this DateTime date)
        {
            return date.ToString(Formats.Time);
        }

        public static string AsTimeString(this DateTime? date)
        {
            if (date.HasValue)
            {
                return AsTimeString(date.Value);
            }

            return string.Empty;
        }

        public static DateTime Set(this DateTime date, string dateString, string timeString)
        {
            var startDate = DateTime.Parse(dateString);
            var startTime = DateTime.Parse(timeString);

            date = new DateTime(startDate.Year, startDate.Month, startDate.Day, startTime.Hour, startTime.Minute, 0);

            return date;
        }

        public static DateTime? Set(this DateTime? date, string dateString, string timeString, string defaultDateString = null)
        {
            if (string.IsNullOrWhiteSpace(dateString) && string.IsNullOrWhiteSpace(timeString))
            {
                date = null;
                return date;
            }

            if (string.IsNullOrWhiteSpace(timeString))
            {
                // 1/1/2001
                var valueDate = new DateTime();

                if (string.IsNullOrWhiteSpace(dateString))
                {
                    valueDate = DateTime.Parse(defaultDateString);
                }
                else
                {
                    valueDate = DateTime.Parse(dateString);
                }

                date = new DateTime(valueDate.Year, valueDate.Month, valueDate.Day);
            }
            else
            {
                // 1/1/2001 12:00PM
                var valueDate = new DateTime();

                if (string.IsNullOrWhiteSpace(dateString))
                {
                    valueDate = DateTime.Parse(defaultDateString);
                }
                else
                {
                    valueDate = DateTime.Parse(dateString);
                }

                var valueTime = DateTime.Parse(timeString);

                date = new DateTime(valueDate.Year, valueDate.Month, valueDate.Day, valueTime.Hour, valueTime.Minute, 0);
            }

            return date;
        }
    }
}
