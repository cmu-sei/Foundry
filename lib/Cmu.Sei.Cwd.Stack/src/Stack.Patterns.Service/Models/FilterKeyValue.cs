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
using System.Collections.Generic;
using System.Linq;

namespace Stack.Patterns.Service.Models
{
    public class FilterKeyValue
    {
        /// <summary>
        /// lower case string
        /// </summary>
        public string Key { get; private set; }

        /// <summary>
        /// optionally delimited string
        /// ex: course
        /// ex: 1,2,3,4
        /// ex: test,up,down
        /// </summary>
        public string Value { get; private set; }

        /// <summary>
        /// pre parsed string collection
        /// </summary>
        public string[] StringValues { get; private set; } = new string[] { };

        /// <summary>
        /// string values converted to int values
        /// </summary>
        public int[] ToIntValues()
        {
            List<int> intValues = new List<int>();
            foreach (var s in StringValues)
            {
                if (int.TryParse(s, out int intValue))
                {
                    intValues.Add(intValue);
                }
            }

            return intValues.ToArray();
        }

        public TEnum[] ToEnumValues<TEnum>()
            where TEnum : struct
        {
            var enumTypes = new List<TEnum>();

            foreach (var v in StringValues)
            {
                if (Enum.TryParse<TEnum>(v, true, out TEnum enumType))
                {
                    enumTypes.Add(enumType);
                }
            }
            return enumTypes.ToArray();
        }

        public FilterKeyValue(string filter)
        {
            var key = string.Empty;
            var value = string.Empty;

            if (filter.Contains("="))
            {
                key = filter.Split('=')[0].ToLower();
                value = filter.Split('=')[1];

                StringValues = filter.Split('=')[1]
                    .Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                    .Select(v => v.Trim())
                    .ToArray();
            }
            else
            {
                key = filter.ToLower();
            }

            Key = key;
            Value = value;
        }
    }
}
