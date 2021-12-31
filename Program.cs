namespace File_copy
{
    public class Program
    {
        public static string source;
        public static string destination;

        public static string[] Files;

        public static bool Debug = false;

        public static void Main(string[] args)
        {
            StartChecks(args);

            MoveFiles();
            CreateHostBuilder(args).Build().Run();
        }

        private static void StartChecks(string[] args)
        {
            File.AppendAllText(@".\info.log", "\n\n");
            OutputMsg( ConsoleColor.Cyan, "[Info] ", $"Starting: {DateTime.Now}");

            if (args.Length == 2)
            {
                source = args[0];
                destination = args[1];
                Files = Directory.GetFiles(source);

            }
            else if (Debug)
            {
                source = @"D:\Programing\File copy\temp 2";
                destination = @"D:\Programing\File copy\temp 1";
                Files = Directory.GetFiles(source);
            }
            else if (args.Length == 0)
            {
                OutputMsg(ConsoleColor.Blue, "[None] ", "No files to Move");

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

        public static void MoveFiles(bool IsComplete = true)
        {
            int MoveCount = 0;
            foreach (String File in Files)
            {
                MoveCount++;

                if (IsComplete)
                OutputMsg(
                    ConsoleColor.Blue, "[File] ", $"Located: New file found {File}"
                );

                string FullPath = Path.GetFullPath(File);

                if (FullPath.Contains(".tmp".ToLower()) || FullPath.Contains("~") || FullPath.Contains("$"))
                {
                    OutputMsg(ConsoleColor.Yellow, "[Log ] ", $"Skip Temp: '{Path.GetFileName(File)}' is a temp file");
                    MoveCount--;
                    continue;
                }

                try
                {
                    System.IO.File.Move(
                        File,
                        Path.Combine(destination , Path.GetFileName(File)),
                        false
                    );

                    OutputMsg(
                        ConsoleColor.Green, "[Move] ", $"Moved: {File}"
                    );
                }
                catch (System.IO.IOException ex)
                {
                    OutputMsg(
                        ConsoleColor.Red, "[Err ] ", $"{ex.Message}"
                    );
                    if (ex.Message.Contains("already exists"))
                    {
                        UpdateFile(File, destination);
                    }

                }
            }

            if (IsComplete)
            OutputMsg(
                ConsoleColor.Green, "[Info] ", $"Completed: {MoveCount} files moved, {Files.Length - MoveCount} files Skipped"
            );
        }

        private static void UpdateFile(string File, string Destination)
        {
            OutputMsg (ConsoleColor.Yellow, "[Log ] ", $"Info: '{Path.GetFileName(File)}' already exists changing name");
            string[] _Destination = Directory.GetFiles(Destination);

            string FileName = Path.GetFileNameWithoutExtension(File);
            string Extension = Path.GetExtension(File);
            string FilePath = File.Split('.')[0];

            foreach (string item in _Destination)
            {
                if (item.Contains(Path.GetFileName(File)))
                {                    
                    if (Regex.IsMatch(_Destination[0], @"\([0-9]+\)"))
                    {
                        // find the biggest number in _Destination
                        int BiggestNum = 0;
                        for (int i = 0; i < _Destination.Length; i++)
                        {
                            if (Regex.IsMatch(_Destination[i], @"\([0-9]+\)"))
                            {
                                // contine if the file is different then the current file name
                                if (Path.GetFileName(_Destination[i].Split(" (")[0]) != Path.GetFileName(File).Split(".")[0])
                                {
                                    continue;
                                }
                                

                                int Number = Convert.ToInt32(Regex.Match(_Destination[i], @"\([0-9]+\)").Value.Replace("(", "").Replace(")", ""));
                                if (Number > BiggestNum)
                                {
                                    BiggestNum = Number;
                                }
                            }
                        }
                        BiggestNum++;

                        OutputMsg(
                            ConsoleColor.Yellow, "[Edit] ", $"Renamed: {Path.GetFileName(File)} to {FileName + Extension}"
                        );

                        // rename the file to file_ + number + extension
                        System.IO.File.Move(
                            File,
                            Path.Combine(FilePath + " (" + BiggestNum + ")" + Extension),
                            false
                        );

                        Program.Files = Directory.GetFiles(source);
                        MoveFiles(false);
                    }
                    else
                    {
                        OutputMsg(
                            ConsoleColor.Yellow, "[Edit] ", $"Renamed: '{Path.GetFileName(File)}' to '{FileName + Extension}'"
                        );
                        // rename the file to file_ + number + extension
                        System.IO.File.Move(
                            File,
                            Path.Combine(FilePath + " (1)" + Extension),
                            false
                        );

                        Program.Files = Directory.GetFiles(source);
                        MoveFiles(false);
                    }
                }
            }
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
