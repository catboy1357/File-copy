namespace File_copy
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
    
        public Worker(ILogger<Worker> logger)
        {
            _logger = logger;
        }
    
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                Program.OutputMsg(ConsoleColor.Blue, "[Info] ", $"Looking: {Program.source}");
                await Task.Delay(1000, stoppingToken);
    
                FileSystemWatcher watcher = new FileSystemWatcher();
                // watches a directory for changes and runs a program when a change is detected
                watcher.Path = Program.source ?? throw new ArgumentNullException(nameof(Program.source));
                watcher.NotifyFilter = NotifyFilters.LastAccess | NotifyFilters.LastWrite
                   | NotifyFilters.FileName | NotifyFilters.DirectoryName;
                watcher.Filter = "*.*";

                // filters out the temporary files created by the OS
                watcher.IncludeSubdirectories = true;
    
                // Add event handlers.
                watcher.Created += new FileSystemEventHandler(OnChanged);
                watcher.Changed += new FileSystemEventHandler(OnChanged);
                // watcher.Deleted += new FileSystemEventHandler(OnChanged);
                // watcher.Renamed += new RenamedEventHandler(OnRenamed);
                
                watcher.EnableRaisingEvents = true;
    
                // Wait for the user to quit the program.
                Program.OutputMsg(ConsoleColor.Cyan, "[Info] ", "Press 'q' to stop listening.");
                while (Console.Read() != 'q') ;
    
                watcher.EnableRaisingEvents = false;
    
                // Stop the application.
                Environment.Exit(0);
            }
        }
    
        private void OnChanged(object sender, FileSystemEventArgs e)
        {            
            Program.OutputMsg(ConsoleColor.Blue, "[Info] ", $"File Moved: {e.FullPath} {e.ChangeType} to {Program.destination}");
            Program.Files = Directory.GetFiles(Program.source ?? throw new ArgumentNullException(nameof(Program.source)));
            Program.MoveFiles();  
        }
    }
}