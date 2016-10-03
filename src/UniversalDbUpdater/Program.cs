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

        private static void LoadSettings(string[] args)
        {
            var builder = new ConfigurationBuilder();
            builder.SetBasePath(Directory.GetCurrentDirectory());
            builder.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
            builder.AddCommandLine(args, _switchMappings);

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
            var typeString = FindArgument(args, "-t", "--type");

            CommandType type;
            if (!Enum.TryParse(typeString, true, out type))
            {
                type = CommandType.Common;
            }

            var command = CommandLibrary.Get(type, commandName);
            if (command == null)
            {
                Console.WriteLine($"Unknown command: {commandName}");
                return 1;
            }

            try
            {
                command.Execute(args.Skip(1), Settings);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception occured");
                Console.WriteLine(ex);
                return 1;
            }

            return 0;
        }

        private static string FindArgument(string[] args, params string[] options)
        {
            foreach (var option in options)
            {
                var index = Array.IndexOf(args, option);
                if (index < 0)
                {
                    continue;
                }

                var valueIndex = index + 1;
                if (valueIndex < args.Length)
                {
                    return args[valueIndex];
                }
            }

            return "";
        }

        private static Dictionary<string, string> _switchMappings = new Dictionary<string, string>
        {
            {"-d", "Settings:Database" },
            {"--database", "Settings:Database" },
            {"-h", "Settings:Host" },
            {"--host", "Settings:Host" },
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