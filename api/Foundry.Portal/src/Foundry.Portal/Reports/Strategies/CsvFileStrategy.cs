/*
Foundry
Copyright 2020 Carnegie Mellon University.
NO WARRANTY. THIS CARNEGIE MELLON UNIVERSITY AND SOFTWARE ENGINEERING INSTITUTE MATERIAL IS FURNISHED ON AN "AS-IS" BASIS. CARNEGIE MELLON UNIVERSITY MAKES NO WARRANTIES OF ANY KIND, EITHER EXPRESSED OR IMPLIED, AS TO ANY MATTER INCLUDING, BUT NOT LIMITED TO, WARRANTY OF FITNESS FOR PURPOSE OR MERCHANTABILITY, EXCLUSIVITY, OR RESULTS OBTAINED FROM USE OF THE MATERIAL. CARNEGIE MELLON UNIVERSITY DOES NOT MAKE ANY WARRANTY OF ANY KIND WITH RESPECT TO FREEDOM FROM PATENT, TRADEMARK, OR COPYRIGHT INFRINGEMENT.
Released under a MIT (SEI)-style license, please see license.txt or contact permission@sei.cmu.edu for full terms.
[DISTRIBUTION STATEMENT A] This material has been approved for public release and unlimited distribution.  Please see Copyright notice for non-US Government use and distribution.
Carnegie Mellon(R) and CERT(R) are registered in the U.S. Patent and Trademark Office by Carnegie Mellon University.
DM20-0194
*/

using CsvHelper;
using Microsoft.AspNetCore.Mvc;
using System;
using System.IO;
using System.Linq;
using System.Text;

namespace Foundry.Portal.Reports
{
    public class CsvFileStrategy : IDataSetFileStrategy
    {
        public FileContentResult File(IReportModel model)
        {
            FileContentResult fileContentResult = null;

            using (MemoryStream memoryStream = new MemoryStream())
            {
                using (TextWriter writer = new StreamWriter(memoryStream, Encoding.UTF8))
                {
                    var csv = new CsvWriter(writer);

                    foreach (var column in model.DataSet.Columns)
                    {
                        csv.WriteField(column.Name, true);
                    }

                    csv.NextRecord();

                    var rows = model.DataSet.Rows.ToList();

                    var totalRows = model.DataSet.Rows.Count();

                    foreach (var row in rows)
                    {
                        foreach (var value in row.Values)
                        {
                            var v = Convert.ToString(value.Value) ?? string.Empty;

                            if (int.TryParse(v, out int intValue))
                            {
                                csv.WriteField("=\"" + v + "\"");
                            }
                            else
                            {
                                csv.WriteField(v, true);
                            }
                        }

                        csv.NextRecord();
                    }
                }

                fileContentResult = new FileContentResult(memoryStream.ToArray(), "text/plain");

                fileContentResult.FileDownloadName = model.FileName + ".csv";
            }

            return fileContentResult;
        }
    }
}

