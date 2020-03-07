/*
Foundry
Copyright 2020 Carnegie Mellon University.
NO WARRANTY. THIS CARNEGIE MELLON UNIVERSITY AND SOFTWARE ENGINEERING INSTITUTE MATERIAL IS FURNISHED ON AN "AS-IS" BASIS. CARNEGIE MELLON UNIVERSITY MAKES NO WARRANTIES OF ANY KIND, EITHER EXPRESSED OR IMPLIED, AS TO ANY MATTER INCLUDING, BUT NOT LIMITED TO, WARRANTY OF FITNESS FOR PURPOSE OR MERCHANTABILITY, EXCLUSIVITY, OR RESULTS OBTAINED FROM USE OF THE MATERIAL. CARNEGIE MELLON UNIVERSITY DOES NOT MAKE ANY WARRANTY OF ANY KIND WITH RESPECT TO FREEDOM FROM PATENT, TRADEMARK, OR COPYRIGHT INFRINGEMENT.
Released under a MIT (SEI)-style license, please see license.txt or contact permission@sei.cmu.edu for full terms.
[DISTRIBUTION STATEMENT A] This material has been approved for public release and unlimited distribution.  Please see Copyright notice for non-US Government use and distribution.
Carnegie Mellon(R) and CERT(R) are registered in the U.S. Patent and Trademark Office by Carnegie Mellon University.
DM20-0194
*/

using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Foundry.Portal.Data;
using Stack.Data;
using System;

namespace Foundry.Portal.Api
{
    public class Program
    {
        public static IWebHostBuilder CreateWebHostBuilder(string[] args) => WebHost.CreateDefaultBuilder(args).UseStartup<Startup>();

        public static void Main(string[] args)
        {
            var webHost = CreateWebHostBuilder(args)
                .ConfigureAppConfiguration((context, config) => {
                    var hostingEnvironment = context.HostingEnvironment;

                    config.AddJsonFile("appsettings.json", false, true);
                    config.AddJsonFile($"appsettings.{hostingEnvironment.EnvironmentName}.json", true, true);

                    if (hostingEnvironment.IsDevelopment())
                    {
                        config.AddJsonFile($"appsettings.{Environment.UserName}.json", true, true);
                    }

                    config.AddEnvironmentVariables();

                    if (args != null) config.AddCommandLine(args);
                })
                .Build();

            webHost.InitializeDatabase<SketchDbContext>((db) => webHost.Seed());

            webHost.Run();
        }
    }
}
