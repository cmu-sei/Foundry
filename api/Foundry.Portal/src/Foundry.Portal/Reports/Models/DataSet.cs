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

namespace Foundry.Portal.Reports
{
    public class DataSet
    {
        public DataSet()
        {
            Columns = new List<DataColumn>();
            Rows = new List<DataRow>();
        }

        public List<DataColumn> Columns { get; }

        public List<DataRow> Rows { get; set; }

        internal DataColumn AddColumn(string name)
        {
            var dataColumn = new DataColumn(name);
            Columns.Add(dataColumn);
            return dataColumn;
        }

        internal DataColumn AddColumn(string name, string sort, bool isSortedBy, SortDirection sortDirection)
        {
            if (Columns.Any(h => h.Name.ToLower() == name.ToLower()))
                throw new ArgumentException(string.Format("A header with the name '{0}' already exists.", name));

            var dataColumn = new DataColumn(name, sort, isSortedBy, sortDirection);

            Columns.Add(dataColumn);
            return dataColumn;
        }

        public IEnumerable<DataValue> AddRow(params object[] values)
        {
            if (values.Length > Columns.Count())
                throw new ArgumentException("There are more values than columns");

            if (Columns.Count() > values.Length)
                throw new ArgumentException("There are more columns than values");

            var adding = values.ToList();

            var row = new DataRow();

            for (int i = 0; i < adding.Count(); i++)
            {
                row.AddDataValue(Columns[i], adding[i]);
            }

            Rows.Add(row);

            return row.Values;
        }
    }
}

