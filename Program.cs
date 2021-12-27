using System;
using System.IO;

namespace File_copy
{
    public class Program
    {
        public static string source;
        public static string destination;

        public static string[] Files;


        public static void Main(string[] args)
        {
            File.AppendAllText(@".\info.log", "\n\n");
            OutputMsg( ConsoleColor.Cyan, "[Info] ", $"Starting: {DateTime.Now}");

            StartChecks(args);

            MoveFiles();
            CreateHostBuilder(args).Build().Run();
        }

        private static void StartChecks(string[] args)
        {
            if (args.Length == 2)
            {
                source = args[0];
                destination = args[1];
                Files = Directory.GetFiles(source);

            }
            else if (args.Length == 0)
            {
                OutputMsg(ConsoleColor.Blue, "[None] ", "No files to copy");

                OutputMsg(ConsoleColor.Blue, "[Info] ", "Please enter source");
                source = Console.ReadLine().ToString();
                
                OutputMsg(ConsoleColor.Blue, "[Info] ", "Please enter destination directory");
                destination = Console.ReadLine().ToString();

                if (source.Contains("\"") || source.Contains("\'") || destination.Contains("\"") || destination.Contains("\'"))
                {
                    source = source.Replace("\"", "");
                    source = source.Replace("\'", "");
                    destination = destination.Replace("\"", "");
                    destination = destination.Replace("\'", "");
                }

                if (Directory.Exists(source) && Directory.Exists(destination))
                {
                    Files = Directory.GetFiles(source);
                }
                else
                {
                    OutputMsg(ConsoleColor.Red, "[Err ] ", "Source or destination directory does not exist");
                    Environment.Exit(0);
                }
            }
            else
            {
                OutputMsg(ConsoleColor.Red, "[Err ]", "Invalid number of arguments");
                Environment.Exit(0);
            }
            if (!Directory.Exists(source) || !Directory.Exists(destination))
            {
                OutputMsg(
                    ConsoleColor.Yellow, "[None] ", "The path does not exist"
                );
                Environment.Exit(0);
            }
        }

        private static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureServices((hostContext, services) =>
                {
                    services.AddHostedService<Worker>();
                });

        public static void MoveFiles()
        {
            foreach (String File in Files)
            {
                OutputMsg(
                    ConsoleColor.Blue, "[File] ", $"Located: {File}"
                );
                try
                {
                    System.IO.File.Copy(
                        File,
                        Path.Combine(destination , Path.GetFileName(File)),
                        false
                    );

                    System.IO.File.Delete(File);

                    OutputMsg(
                        ConsoleColor.Green, "[Copy] ", $"Coppied: {File}"
                    );
                }
                catch (System.IO.IOException ex)
                {
                    OutputMsg(
                        ConsoleColor.Red, "[Err ] ", $"{ex.Message}"
                    );
                    if (ex.Message.Contains("already exists"))
                    {
                        OutputMsg(
                            ConsoleColor.Yellow, 
                            "[File] ", 
                            $"File already exists. Removing Duplicates: {Path.GetFileName(File)}"
                        );
                        System.IO.File.Delete(File);
                    }
                }
            }

            OutputMsg(
                ConsoleColor.Green, "[Done] ", "Compleated: All files have been copied"
            );
        }

        public static void OutputMsg(ConsoleColor colour, string statusMessage, string message)
        {
            ConsoleColor originalColor = Console.ForegroundColor;
            Console.ForegroundColor = colour;
            Console.Write (statusMessage);

            Console.ForegroundColor = originalColor;
            Console.WriteLine (message);

            File.AppendAllText(
                @".\info.log",
                $"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")} {statusMessage} {message} \n"
            );
        }
    }
}
