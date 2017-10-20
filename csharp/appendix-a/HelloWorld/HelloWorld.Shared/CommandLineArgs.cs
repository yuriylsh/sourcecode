using System;

namespace HelloWorld.Shared
{
    public class CommandLineArgs
    {
        private const int DefaultCount = 10;
        public string Message { get; set; }
        public int Count { get; set; }

        public static CommandLineArgs Parse(string[] args)
        {
            switch (args.Length)
            {
                case 0:
                    Console.WriteLine("Must supply message text.");
                    Environment.Exit(-1);
                    return null;
                case 1:
                    return new CommandLineArgs {Message = args[0], Count = DefaultCount};
                default:
                    return new CommandLineArgs
                    {
                        Message = args[0],
                        Count = int.TryParse(args[1], out int parsedCount) ? parsedCount : DefaultCount
                    };
            }
        }
    }
}