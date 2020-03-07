/*
Foundry
Copyright 2020 Carnegie Mellon University.
NO WARRANTY. THIS CARNEGIE MELLON UNIVERSITY AND SOFTWARE ENGINEERING INSTITUTE MATERIAL IS FURNISHED ON AN "AS-IS" BASIS. CARNEGIE MELLON UNIVERSITY MAKES NO WARRANTIES OF ANY KIND, EITHER EXPRESSED OR IMPLIED, AS TO ANY MATTER INCLUDING, BUT NOT LIMITED TO, WARRANTY OF FITNESS FOR PURPOSE OR MERCHANTABILITY, EXCLUSIVITY, OR RESULTS OBTAINED FROM USE OF THE MATERIAL. CARNEGIE MELLON UNIVERSITY DOES NOT MAKE ANY WARRANTY OF ANY KIND WITH RESPECT TO FREEDOM FROM PATENT, TRADEMARK, OR COPYRIGHT INFRINGEMENT.
Released under a MIT (SEI)-style license, please see license.txt or contact permission@sei.cmu.edu for full terms.
[DISTRIBUTION STATEMENT A] This material has been approved for public release and unlimited distribution.  Please see Copyright notice for non-US Government use and distribution.
Carnegie Mellon(R) and CERT(R) are registered in the U.S. Patent and Trademark Office by Carnegie Mellon University.
DM20-0194
*/

using Microsoft.Extensions.Logging;
using Foundry.Buckets.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Foundry.Buckets.Monitors
{
    /// <summary>
    /// file upload monitor
    /// </summary>
    public class FileUploadMonitor : IFileUploadMonitor
    {
        /// <summary>
        /// create instance
        /// </summary>
        /// <param name="logger"></param>
        public FileUploadMonitor(ILogger<FileUploadMonitor> logger)
        {
            _logger = logger;
            _monitor = new Dictionary<string, FileProgress>();
            Task.Run(() => CleanupLoop());
        }
        private readonly ILogger<FileUploadMonitor> _logger;
        private Dictionary<string, FileProgress> _monitor;

        /// <summary>
        /// update progress
        /// </summary>
        /// <param name="key"></param>
        /// <param name="progress"></param>
        public void Update(string key, int progress)
        {
            if (_monitor.ContainsKey(key))
            {
                _monitor[key].Progress = progress;
                _monitor[key].Stop = DateTime.UtcNow;
            }
            else
            {
                _monitor.Add(key, new FileProgress
                {
                    Key = key,
                    Start = DateTime.UtcNow
                });
            }
        }

        /// <summary>
        /// get file progress
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public FileProgress Progress(string key)
        {
            if (_monitor.ContainsKey(key))
                return _monitor[key];

            return new FileProgress { Key = key, Progress = -1 };
        }

        private async Task CleanupLoop()
        {
            while (true)
            {
                DateTime now = DateTime.UtcNow;
                foreach (FileProgress item in _monitor.Values.ToArray())
                {
                    if (now.CompareTo(item.Stop.AddMinutes(2)) > 0)
                    {
                        _logger.LogDebug("removed monitor " + item.Key);
                        _monitor.Remove(item.Key);
                    }
                }
                await Task.Delay(60000);
            }
        }
    }
}

