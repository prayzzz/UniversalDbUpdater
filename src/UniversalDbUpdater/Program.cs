using System;
using System.IO;
using System.Linq;
using System.Reflection;
using Microsoft.Extensions.Configuration;
using UniversalDbUpdater.Common;

namespace UniversalDbUpdater
{
    public static class Program
    {
        public const string DateFormat = "yyyy-MM-dd_HH-mm-ss";

        private static IConfigurationRoot Configuration { get; set; }
        private static readonly Settings Settings = new Settings();
        private static readonly CommandLibrary CommandLibrary;

        static Program()
        {
            CommandLibrary = new CommandLibrary();
        }


        public static void Main(string[] args)
        {
            var logo = ResourceHelper.Current.GetEmbeddedFile(Assembly.GetEntryAssembly(), "UniversalDbUpdater.Resources.logo.txt");
            Console.WriteLine(logo);
            Console.WriteLine();

            LoadSettings();

            var exitCode = EvaluateArguments(args);
            Environment.Exit(exitCode);
        }

        private static void LoadSettings()
        {
            var builder = new ConfigurationBuilder();
            builder.SetBasePath(Directory.GetCurrentDirectory());
            builder.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

            Configuration = builder.Build();

            Configuration.GetSection("Settings").Bind(Settings);
        }

        private static int EvaluateArguments(string[] args)
        {
            if (args.Length < 1)
            {
                Console.WriteLine("No command");
                return -1;
            }

            var commandName = args[0];
            var typeString = FindArgument(args, "-t", "--type");

            DatabaseType type;
            if (!Enum.TryParse(typeString, true, out type))
            {
                type = DatabaseType.Unknow;
            }

            var command = CommandLibrary.Get(type, commandName);

            if (command == null)
            {
                Console.WriteLine($"Unknown command: {commandName}");
                return -1;
            }

            try
            {
                command.Execute(args.Skip(1), Settings);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception occured");
                Console.WriteLine(ex);
                return -1;
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
    }
}