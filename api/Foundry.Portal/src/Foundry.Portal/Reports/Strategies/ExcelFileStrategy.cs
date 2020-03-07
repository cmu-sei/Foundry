/*
Foundry
Copyright 2020 Carnegie Mellon University.
NO WARRANTY. THIS CARNEGIE MELLON UNIVERSITY AND SOFTWARE ENGINEERING INSTITUTE MATERIAL IS FURNISHED ON AN "AS-IS" BASIS. CARNEGIE MELLON UNIVERSITY MAKES NO WARRANTIES OF ANY KIND, EITHER EXPRESSED OR IMPLIED, AS TO ANY MATTER INCLUDING, BUT NOT LIMITED TO, WARRANTY OF FITNESS FOR PURPOSE OR MERCHANTABILITY, EXCLUSIVITY, OR RESULTS OBTAINED FROM USE OF THE MATERIAL. CARNEGIE MELLON UNIVERSITY DOES NOT MAKE ANY WARRANTY OF ANY KIND WITH RESPECT TO FREEDOM FROM PATENT, TRADEMARK, OR COPYRIGHT INFRINGEMENT.
Released under a MIT (SEI)-style license, please see license.txt or contact permission@sei.cmu.edu for full terms.
[DISTRIBUTION STATEMENT A] This material has been approved for public release and unlimited distribution.  Please see Copyright notice for non-US Government use and distribution.
Carnegie Mellon(R) and CERT(R) are registered in the U.S. Patent and Trademark Office by Carnegie Mellon University.
DM20-0194
*/

using Microsoft.AspNetCore.Mvc;
using OfficeOpenXml.Core.ExcelPackage;
using System.IO;
using System.Linq;

namespace Foundry.Portal.Reports
{
    public class ExcelFileStrategy : IDataSetFileStrategy
    {
        public FileContentResult File(IReportModel model)
        {
            var stream = new MemoryStream();
            using (var package = new ExcelPackage(stream))
            {
                var worksheet = package.Workbook.Worksheets.Add(model.Name);

                var columns = model.DataSet.Columns.ToList();

                worksheet.InsertRow(1);

                for (int c = 0; c < columns.Count(); c++)
                {
                    var column = columns[c];

                    worksheet.Cell(1, c + 1).Value = column.Name;

                    var values = column.Values.ToList();

                    for (int v = 0; v < values.Count(); v++)
                    {
                        var value = values[v];

                        worksheet.Cell(v + 2, c + 1).Value = value.Value.ToString();
                    }
                }

                package.Save();
            }

            var fileContentResult = new FileContentResult(stream.ToArray(), "application/xlsx");

            fileContentResult.FileDownloadName = model.FileName + ".xlsx";

            return fileContentResult;
        }
    }
}
