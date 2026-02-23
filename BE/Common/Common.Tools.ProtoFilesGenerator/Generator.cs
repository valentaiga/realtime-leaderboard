using ProtoBuf.Grpc.Reflection;
using ProtoBuf.Meta;

namespace Common.Tools.ProtoFilesGenerator;

public static class Generator
{
    public static async Task GenerateAsync(string interfaceName, Type type)
    {
        Console.WriteLine($"ProtoFilesGen: Generating protobuf files for {interfaceName}...");
        var generator = new SchemaGenerator
        {
            ProtoSyntax = ProtoSyntax.Proto3
        };

        var saveFolder = Path.Combine(FindSolutionDirectory(), "BackOffice", "ProtoFiles");

        var output = generator.GetSchema(type);
        var filePath = Path.Combine(saveFolder, $"{interfaceName}.proto");
        await File.WriteAllTextAsync(filePath, output);
        Console.WriteLine($"ProtoFilesGen: Proto file '{interfaceName}.proto' updated ({filePath})");
    }
    
    private static string FindSolutionDirectory()
    {
        var currentDirectory = Directory.GetCurrentDirectory();
        var directory = new DirectoryInfo(currentDirectory);

        while (directory != null)
        {
            var solutionFiles = directory.GetFiles("*.sln*");
            if (solutionFiles.Any(f => f.Extension is ".sln" or ".slnx"))
                return directory.FullName;

            directory = directory.Parent;
        }

        throw new FileNotFoundException("Solution file not found");
    }
}