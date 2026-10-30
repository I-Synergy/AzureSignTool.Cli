using AzureSignTool.Cli.Commands;
using System.CommandLine;
using System.Reflection;

namespace AzureSignTool.Cli;

public static class Program
{
    public static async Task<int> Main(string[] args)
    {
        var rootCommand = new RootCommand("Azure Sign Tool - Code signing tool using Azure Key Vault");

        rootCommand.Add(SignCommand.Create());
        rootCommand.Add(AboutCommand.Create());

        return await rootCommand.Parse(args).InvokeAsync();
    }

    internal static string GetVersion()
    {
        Assembly assembly = typeof(Program).Assembly;
        string? version = assembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>()?.InformationalVersion;

        if (string.IsNullOrEmpty(version))
        {
            version = assembly.GetName().Version?.ToString();
        }

        return version ?? "0.0.0";
    }
}
