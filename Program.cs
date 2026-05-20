public class Lox
{
    static bool hadError = false;

    public static void Main(String[] args)
    {
        if (args.Length > 1)
        {
            Console.WriteLine("Usage: jlox [script]");
            Environment.Exit(64);
        }
        else if (args.Length == 1)
        {
            RunFile(args[0]);
        }
        else
        {
            RunPrompt();
        }
    }

    public static void RunFile(string path)
    {
        StreamReader sr = new StreamReader(path);

        Run(sr.ReadToEnd());

        if (hadError)
            Environment.Exit(65);
    }

    public static void RunPrompt()
    {
        while (true)
        {
            Console.Write("> ");
            Run(Console.ReadLine());
            hadError = false;
        }
    }

    private static void Run(String source)
    {
        Scanner scanner = new Scanner(source);
        List<Token> tokens = scanner.ScanTokens();

        foreach (Token token in tokens)
        {
            Console.WriteLine(token);
        }
    }

    public static void Error(int line, string message)
    {
        Report(line, " ", message);
    }

    private static void Report(int line, string where, string message)
    {
        Console.Error.WriteLine("[line " + line.ToString() + "] Error" + where + ": " + message);
        hadError = true;
    }

    private static void Testing(string message)
    {
        Run(message);
    }
}
