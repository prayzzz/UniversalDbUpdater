using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Microsoft.Extensions.Configuration;
using UniversalDbUpdater.Common;

namespace UniversalDbUpdater
{
    public static class Program
    {
        private static readonly Settings Settings = new Settings();
        private static readonly CommandLibrary CommandLibrary;
        private static IConfigurationRoot _configuration;

        static Program()
        {
            CommandLibrary = new CommandLibrary();
        }

        public static void Main(string[] args)
        {
            var logo = ResourceHelper.Current.GetEmbeddedFile(Assembly.GetEntryAssembly(), "UniversalDbUpdater.Resources.logo.txt");
            Console.WriteLine(logo);
            Console.WriteLine();

            LoadSettings(args);

            var exitCode = EvaluateArguments(args);
            Environment.Exit(exitCode);
        }

        private static void LoadSettings(IReadOnlyList<string> args)
        {
            var builder = new ConfigurationBuilder();
            builder.SetBasePath(Directory.GetCurrentDirectory());
            builder.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

            // remove command from args
            var strippedArgs = args.ToArray();
            if (args.Count > 0 && !args[0].StartsWith("-") && !args[0].StartsWith("--"))
            {
                strippedArgs = strippedArgs.Skip(1).ToArray();
            }

            builder.AddCommandLine(strippedArgs, SwitchMappings);

            _configuration = builder.Build();

            _configuration.GetSection("Settings").Bind(Settings);
        }

        private static int EvaluateArguments(string[] args)
        {
            if (args.Length < 1)
            {
                Console.WriteLine("No command");
                return 1;
            }

            var commandName = args[0];
            var command = CommandLibrary.Get(Settings.Type, commandName);

            if (command == null)
            {
                Console.WriteLine($"Unknown command: {commandName}");
                return 1;
            }

            try
            {
                command.Execute(args.Skip(1), Settings);
                Console.WriteLine();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception occured");
                Console.WriteLine(ex);
                return 1;
            }

            return 0;
        }

        private static readonly Dictionary<string, string> SwitchMappings = new Dictionary<string, string>
        {
            {"-d", "Settings:Database" },
            {"--database", "Settings:Database" },

            {"-h", "Settings:Host" },
            {"--host", "Settings:Host" },

            {"-i", "Settings:IntegratedSecurity" },
            {"--iSecurity", "Settings:IntegratedSecurity" },

            {"-o", "Settings:Port" },
            {"--port", "Settings:Port" },

            {"-p", "Settings:Password" },
            {"--password", "Settings:Password" },

            {"-t", "Settings:Type" },
            {"--type", "Settings:Type" },

            {"-u", "Settings:User" },
            {"--user", "Settings:User" }
        };
    }
}