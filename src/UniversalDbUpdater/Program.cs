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

        private static readonly Dictionary<string, string> SwitchMappings = new Dictionary<string, string>
        {
            {"--backup", "Settings:BackupDirectory" },

            {"--database", "Settings:Database" },

            {"--host", "Settings:Host" },

            {"--isecurity", "Settings:IntegratedSecurity" },

            {"--port", "Settings:Port" },

            {"--password", "Settings:Password" },

            {"--schema", "Settings:Schema" },

            {"--scripts", "Settings:ScriptsDirectory" },

            {"--table", "Settings:Table" },

            {"--type", "Settings:Type" },

            {"--user", "Settings:User" }
        };

        static Program()
        {
            CommandLibrary = new CommandLibrary();
        }

        public static void Main(string[] args)
        {
            var logo = ResourceHelper.Current.GetEmbeddedFile(Assembly.GetEntryAssembly(), "UniversalDbUpdater.Resources.logo.txt");
            Console.WriteLine(logo);
            Console.WriteLine();

            var exitCode = LoadSettings(args);
            if (exitCode != 0)
            {
                Exit(exitCode);
            }

            exitCode = CheckSettings();
            if (exitCode != 0)
            {
                Exit(exitCode);
            }

            exitCode = EvaluateArguments(args);
            Exit(exitCode);
        }

        private static int CheckSettings()
        {
            if (!Directory.Exists(Settings.ScriptsDirectory))
            {
                Console.WriteLine($"Directory '{Settings.ScriptsDirectory}' doesn't exist");
                return 1;
            }

            return 0;
        }

        private static int LoadSettings(IReadOnlyList<string> args)
        {
            var builder = new ConfigurationBuilder();

            // remove command from args
            var strippedArgs = args.ToArray();
            if (args.Count > 0 && !args[0].StartsWith("-") && !args[0].StartsWith("--"))
            {
                strippedArgs = strippedArgs.Skip(1).ToArray();
            }

            builder.AddCommandLine(strippedArgs, SwitchMappings);

            try
            {
                _configuration = builder.Build();
            }
            catch (FormatException ex)
            {
                Console.WriteLine("Exception occured");
                Console.WriteLine(ex);
                return 1;
            }

            _configuration.GetSection("Settings").Bind(Settings);

            if (Settings.Type == CommandType.MsSql && string.IsNullOrEmpty(Settings.Schema))
            {
                Settings.Table = "[Infrastructure]";
            }

            if (Settings.Type == CommandType.MsSql && string.IsNullOrEmpty(Settings.Table))
            {
                Settings.Table = "[DbScripts]";
            }

            if (Settings.Type == CommandType.MySql && string.IsNullOrEmpty(Settings.Schema))
            {
                Settings.Table = "infrastructure";
            }

            if (Settings.Type == CommandType.MySql && string.IsNullOrEmpty(Settings.Table))
            {
                Settings.Table = "dbscripts";
            }

            return 0;
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
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception occured");
                Console.WriteLine(ex);
                return 1;
            }

            return 0;
        }

        private static void Exit(int exitCode)
        {
            Console.WriteLine();
            Console.WriteLine($"Exit Code: {exitCode}");
            Environment.Exit(exitCode);
        }
    }
}