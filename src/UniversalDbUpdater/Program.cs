using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.Extensions.Configuration;
using UniversalDbUpdater.Common;
using UniversalDbUpdater.MySql;

namespace UniversalDbUpdater
{
    public static class Program
    {
        public const string DateFormat = "yyyy-MM-dd_HH-mm-ss";

        private static IConfigurationRoot Configuration { get; set; }
        private static readonly Settings Settings = new Settings();
        private static readonly Dictionary<string, ICommand> Commands;

        static Program()
        {
            Commands = new Dictionary<string, ICommand>();
            Commands.Add("-i", InitCommand.Current);

            //Commands.Add("/b", BackupCommand.Current);
            //Commands.Add("/e", ExecuteMissingScriptsCommand.Current);
            //Commands.Add("/m", ShowMissingScriptsCommand.Current);
        }


        public static void Main(string[] args)
        {
            Console.WriteLine("mobtima DbUpdater");
            Console.WriteLine();

            Console.WriteLine("Loading Settings");

            LoadSettings();
            EvaluateArguments(args);

            Console.WriteLine();
        }

        private static void LoadSettings()
        {
            var builder = new ConfigurationBuilder();
            builder.SetBasePath(Directory.GetCurrentDirectory());
            builder.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

            Configuration = builder.Build();

            Configuration.GetSection("Settings").Bind(Settings);
        }

        public static void PrintException(Exception ex)
        {
            Console.WriteLine("\t " + ex);
            Console.WriteLine("\t " + ex.Message);

            var inner = ex.InnerException;
            while (inner != null)
            {
                Console.WriteLine("\t " + inner.Message);
                inner = inner.InnerException;
            }
        }

        public static void ShowHelp(IEnumerable<string> arguments)
        {
            Console.WriteLine("Help");

            foreach (var command in Commands.Values)
            {
                command.HelpShort();
            }
        }

        private static void EvaluateArguments(IReadOnlyList<string> args)
        {
            if (args.Count < 1)
            {
                ShowHelp(new string[0]);
                return;
            }

            ICommand command;
            if (Commands.TryGetValue(args[0], out command))
            {
                command.Execute(args.Skip(1), Settings);
            }
            else
            {
                ShowHelp(new string[0]);
            }
        }
    }
}