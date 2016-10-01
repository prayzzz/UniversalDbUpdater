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
        public const string DateFormat = "yyyy-MM-dd_HH-mm-ss";

        private static IConfigurationRoot Configuration { get; set; }
        private static readonly Settings Settings = new Settings();
        private static readonly Dictionary<string, ICommand> Commands;

        static Program()
        {
            Commands = new Dictionary<string, ICommand>();
            //Commands.Add("-i", InitCommand.Current);
            //Commands.Add("-c", CreateCommand.Current);
            //Commands.Add("-s", ShowMissingScriptsCommand.Current);
            //Commands.Add("-b", BackupCommand.Current);
            //Commands.Add("-e", ExecuteMissingScriptsCommand.Current);

            //Commands.Add("/b", BackupCommand.Current);
            //Commands.Add("/e", ExecuteMissingScriptsCommand.Current);
            //Commands.Add("/m", ShowMissingScriptsCommand.Current);
        }


        public static void Main(string[] args)
        {

            var logo = ResourceHelper.Current.GetEmbeddedFile(Assembly.GetEntryAssembly(), "UniversalDbUpdater.Resources.logo.txt");
            Console.WriteLine(logo);
            Console.WriteLine();

            LoadSettings();

            EvaluateArguments(args);
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
            Console.WriteLine("How does it work?");
            Console.WriteLine();

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
                try
                {
                    command.Execute(args.Skip(1), Settings);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Exception occured");
                    Console.WriteLine(ex);
                }
            }
            else
            {
                ShowHelp(new string[0]);
            }
        }
    }
}