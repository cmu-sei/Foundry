/*
Foundry
Copyright 2020 Carnegie Mellon University.
NO WARRANTY. THIS CARNEGIE MELLON UNIVERSITY AND SOFTWARE ENGINEERING INSTITUTE MATERIAL IS FURNISHED ON AN "AS-IS" BASIS. CARNEGIE MELLON UNIVERSITY MAKES NO WARRANTIES OF ANY KIND, EITHER EXPRESSED OR IMPLIED, AS TO ANY MATTER INCLUDING, BUT NOT LIMITED TO, WARRANTY OF FITNESS FOR PURPOSE OR MERCHANTABILITY, EXCLUSIVITY, OR RESULTS OBTAINED FROM USE OF THE MATERIAL. CARNEGIE MELLON UNIVERSITY DOES NOT MAKE ANY WARRANTY OF ANY KIND WITH RESPECT TO FREEDOM FROM PATENT, TRADEMARK, OR COPYRIGHT INFRINGEMENT.
Released under a MIT (SEI)-style license, please see license.txt or contact permission@sei.cmu.edu for full terms.
[DISTRIBUTION STATEMENT A] This material has been approved for public release and unlimited distribution.  Please see Copyright notice for non-US Government use and distribution.
Carnegie Mellon(R) and CERT(R) are registered in the U.S. Patent and Trademark Office by Carnegie Mellon University.
DM20-0194
*/

using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Stack.Http.Options;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using io = System.IO;

namespace Foundry.Orders.Services
{
    public class FileService
    {
        IHostingEnvironment _env;
        FileOptions _fileOptions;

        public FileService(FileOptions fileOptions, IHostingEnvironment env)
        {
            _fileOptions = fileOptions;
            _env = env;
        }

        public async Task<bool> Save(IFormFile file, int orderId)
        {
            string fileName = System.Net.WebUtility.UrlDecode(file.FileName);
            string savePath = DestinationPath(fileName, orderId);

            using (var fileStream = new io.FileStream(savePath, io.FileMode.Create))
            {
                await file.CopyToAsync(fileStream);
            }

            return true;
        }

        public string DestinationPath(string fileName, int orderId)
        {
            var newFileName = WebUtility.UrlDecode(fileName);
            io.Path.GetInvalidFileNameChars().ToList().ForEach(c => newFileName = newFileName.Replace(c.ToString(), ""));

            var path = io.Path.Combine(_env.WebRootPath, _fileOptions.DestPath, orderId.ToString());

            if (!io.Directory.Exists(path))
                io.Directory.CreateDirectory(path);

            return io.Path.Combine(path, newFileName);
        }
    }
}
