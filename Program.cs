using Newtonsoft.Json.Linq;

/// <summary>
/// Example usage: SwaggerJsonFilter.exe endpoints.txt swagger.json filtered_swagger.json
///Format of endpoints.txt (actual names from swagger):
/// /api/Account/Token
/// /api/Account/RefreshToken
/// /api/analytics/reports/types
/// </summary>

class SwaggerJsonFilter
{
    static void Main(string[] args)
    {
        if (args.Length != 3)
        {
            Console.WriteLine("Usage: SwaggerPathFilter <endpoints.txt> <inputSwagger.json> <outputSwagger.json>");
            return;
        }

        var endpointsFilePath = args[0];
        var inputSwaggerPath = args[1];
        var outputSwaggerPath = args[2];

        if (!File.Exists(endpointsFilePath))
        {
            Console.WriteLine($"Error: Endpoints file not found: {endpointsFilePath}");
            return;
        }

        if (!File.Exists(inputSwaggerPath))
        {
            Console.WriteLine($"Error: Swagger input file not found: {inputSwaggerPath}");
            return;
        }

        try
        {
            var includedPaths = new List<string>(File.ReadAllLines(endpointsFilePath));

            FilterSwaggerPaths(inputSwaggerPath, outputSwaggerPath, includedPaths);
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error: " + ex.Message);
        }
    }

    private static void FilterSwaggerPaths(string inputFilePath, string outputFilePath, List<string> includedPaths)
    {
        var swaggerJson = JObject.Parse(File.ReadAllText(inputFilePath));

        var paths = (JObject)swaggerJson["paths"];
        if (paths == null)
        {
            throw new Exception("The Swagger file does not contain a 'paths' object.");
        }

        var pathsToRemove = new List<string>();
        foreach (var path in paths.Properties())
        {
            if (!includedPaths.Contains(path.Name))
            {
                pathsToRemove.Add(path.Name);
            }
        }

        foreach (var pathToRemove in pathsToRemove)
        {
            paths.Remove(pathToRemove);
        }

        File.WriteAllText(outputFilePath, swaggerJson.ToString());
        Console.WriteLine("Filtered Swagger JSON saved to: " + outputFilePath);
    }
}
