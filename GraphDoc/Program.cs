using System.Diagnostics;
using System.IO.Compression;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace GraphDoc;

public class Program
{
    public static async Task Main(string[] args)
    {
        Command = CommandLineArguments.Parse(args);

        var result = await Command.Execute();
        Console.WriteLine(result);
    }

    public static ICommand Command { get; private set; } = new HelpCommand();
}