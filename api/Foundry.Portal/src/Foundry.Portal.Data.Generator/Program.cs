/*
Foundry
Copyright 2020 Carnegie Mellon University.
NO WARRANTY. THIS CARNEGIE MELLON UNIVERSITY AND SOFTWARE ENGINEERING INSTITUTE MATERIAL IS FURNISHED ON AN "AS-IS" BASIS. CARNEGIE MELLON UNIVERSITY MAKES NO WARRANTIES OF ANY KIND, EITHER EXPRESSED OR IMPLIED, AS TO ANY MATTER INCLUDING, BUT NOT LIMITED TO, WARRANTY OF FITNESS FOR PURPOSE OR MERCHANTABILITY, EXCLUSIVITY, OR RESULTS OBTAINED FROM USE OF THE MATERIAL. CARNEGIE MELLON UNIVERSITY DOES NOT MAKE ANY WARRANTY OF ANY KIND WITH RESPECT TO FREEDOM FROM PATENT, TRADEMARK, OR COPYRIGHT INFRINGEMENT.
Released under a MIT (SEI)-style license, please see license.txt or contact permission@sei.cmu.edu for full terms.
[DISTRIBUTION STATEMENT A] This material has been approved for public release and unlimited distribution.  Please see Copyright notice for non-US Government use and distribution.
Carnegie Mellon(R) and CERT(R) are registered in the U.S. Patent and Trademark Office by Carnegie Mellon University.
DM20-0194
*/

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Stack.Validation.Handlers;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Foundry.Portal.Data.Generator
{
    class Program
    {
        static bool first = true;
        static void Main(string[] args)
        {
            if (first)
            {
                Console.WriteLine("Data Generator Commands:");
                Console.WriteLine("template <template-name>, create-content <number>, create-channels <number>, create-groups <number>, create-profiles <number>");
                Console.WriteLine();
            }

            Console.WriteLine("?");
            if (args == null || !args.Any())
            {
                var entry = Console.ReadLine();
                args = new string[] { entry };
            }

            var commands = ParseCommands(args);

            if (commands.Any())
            {
                var path = System.IO.Directory.GetCurrentDirectory();

                var configuration = new ConfigurationBuilder()
                    .SetBasePath(path)
                    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                    .AddEnvironmentVariables()
                    .Build();

                var c = UseConfiguredDatabase(configuration);
                var ctx = new SketchDbContext(c.Options);
                ctx.Database.EnsureDeleted();
                ctx.Database.Migrate();

                var logger = new LoggerFactory();
                var validationHandler = new StrictValidationHandler(ctx);
                //Lets try 10000 users, 2000 channels, 2000 groups, and 20000 contents. Later we can raise it an order of magnitude.
                var generator = new DataFactory(new CoreOptions { }, ctx, logger, validationHandler)
                {
                    IsConsole = true
                };

                var result = generator.Process(commands).Result;

                Main(null);
            }
            else
            {
                Console.WriteLine("??");
                Main(null);
            }

            first = false;
        }

        static DbContextOptionsBuilder<SketchDbContext> UseConfiguredDatabase(IConfigurationRoot root)
        {
            var builder = new DbContextOptionsBuilder<SketchDbContext>();
            string dbProvider = root.GetValue<string>("Database:Provider");
            var connectionString = root.GetConnectionString(dbProvider);
            var migrationsAssembly = "Foundry.Portal.Data." + dbProvider;

            switch (dbProvider)
            {
                case "Sqlite":
                    builder.UseSqlite(connectionString, options => options.MigrationsAssembly(migrationsAssembly));
                    break;

                case "SqlServer":
                    builder.UseSqlServer(connectionString, options => options.MigrationsAssembly(migrationsAssembly));
                    break;

            }
            return builder;
        }

        static Command[] ParseCommands(params string[] args)
        {
            var commands = new List<Command>();

            if (args != null && args.Any())
            {
                var process = args.Select(a => a.ToLower());

                if (args.Length == 1)
                {
                    process = args[0].ToLower().Split(' ');
                }

                CommandType type = CommandType.NotSet;

                foreach (var arg in process)
                {
                    if (type == CommandType.NotSet)
                    {
                        if (arg == "template")
                            type = CommandType.Template;
                        if (arg == "create-profiles")
                            type = CommandType.CreateProfiles;
                        if (arg == "create-content")
                            type = CommandType.CreateContent;
                    }
                    else
                    {
                        commands.Add(new Command { Type = type, Value = arg });
                        type = CommandType.NotSet;
                    }
                }
            }

            return commands.ToArray();
        }
    }
}
