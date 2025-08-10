using DesignProjectStructure.Configuration;
using DesignProjectStructure.Helpers;
using DesignProjectStructure.Models;
using System.Text.Json;

namespace DesignProjectStructure.FileTypes;

/// <summary>
/// JSON output generator with advanced statistics
/// </summary>
public class OutputJsonGenerator : IOutputGenerator
{
    public string Generate(StructureItens structureItens, string rootPath)
    {
        var config = ConfigurationManager.Instance.Config;
        var rootStructure = GenerateJsonStructureFlat(rootPath);

        // Calculate statistics by type
        var statistics = CalculateDetailedStatistics(rootStructure);

        var output = new
        {
            projectName = Path.GetFileName(rootPath),
            path = rootPath,
            generatedAt = DateTime.Now,
            configuration = new
            {
                includeHiddenFiles = config.General.IncludeHiddenFiles,
                maxDepth = config.General.MaxDepth,
                formats = config.Output.Formats
            },
            statistics = new
            {
                totalFolders = structureItens.FolderCounter,
                totalFiles = structureItens.FileCounter,
                totalItems = structureItens.ProcessedItems,
                fileTypes = statistics.FileTypes,
                largestFiles = statistics.LargestFiles,
                deepestPath = statistics.DeepestPath
            },
            structure = rootStructure
        };

        return JsonSerializer.Serialize(output, new JsonSerializerOptions
        {
            WriteIndented = true
        });
    }

    public string GetFileExtension() => "json";

    public string GetFormatName() => "JSON";

    public bool SupportsFormat(string format) =>
        format.Equals("json", StringComparison.OrdinalIgnoreCase);

    #region Métodos privados de geração JSON

    private List<object> GenerateJsonStructureFlat(string rootPath)
    {
        var items = new List<object>();
        GenerateJsonStructureFlatRecursive(rootPath, "", items, 0);
        return items;
    }

    private void GenerateJsonStructureFlatRecursive(string path, string relativePath, List<object> items, int depth)
    {
        try
        {
            var name = Path.GetFileName(path);
            if (string.IsNullOrEmpty(name))
                name = path;

            var isDirectory = Directory.Exists(path);
            var currentRelativePath = string.IsNullOrEmpty(relativePath)
                ? name
                : $"{relativePath}/{name}";

            var item = new Dictionary<string, object>
            {
                ["name"] = name,
                ["type"] = isDirectory ? "directory" : "file",
                ["relativePath"] = currentRelativePath,
                ["depth"] = depth
            };

            // Add extension to files
            if (!isDirectory)
            {
                var extension = Path.GetExtension(path);
                if (!string.IsNullOrEmpty(extension))
                {
                    item["extension"] = extension.ToLower();
                }

                // Add file size if configured
                var config = ConfigurationManager.Instance.Config;
                if (config.Statistics.CalculateFileSize)
                {
                    try
                    {
                        var fileInfo = new FileInfo(path);
                        item["size"] = fileInfo.Length;
                        item["sizeFormatted"] = FormatBytes(fileInfo.Length);
                    }
                    catch
                    {
                        // If unable to get the size, do not add the properties
                    }
                }
            }

            items.Add(item);

            // If it is a directory, process children
            if (isDirectory)
            {
                try
                {
                    var childItems = Directory.GetFileSystemEntries(path);
                    Array.Sort(childItems, (x, y) =>
                    {
                        bool xIsDir = Directory.Exists(x);
                        bool yIsDir = Directory.Exists(y);

                        if (xIsDir && !yIsDir) return -1;
                        if (!xIsDir && yIsDir) return 1;

                        return string.Compare(Path.GetFileName(x), Path.GetFileName(y), StringComparison.OrdinalIgnoreCase);
                    });

                    foreach (var childPath in childItems)
                    {
                        if (!IgnoreFilter.MustIgnore(Path.GetFileName(childPath)))
                        {
                            GenerateJsonStructureFlatRecursive(childPath, currentRelativePath, items, depth + 1);
                        }
                    }
                }
                catch (UnauthorizedAccessException)
                {
                    items.Add(new Dictionary<string, object>
                    {
                        ["name"] = "[Access Denied]",
                        ["type"] = "error",
                        ["relativePath"] = $"{currentRelativePath}/[Access Denied]",
                        ["depth"] = depth + 1,
                        ["error"] = "UnauthorizedAccess"
                    });
                }
                catch (Exception ex)
                {
                    items.Add(new Dictionary<string, object>
                    {
                        ["name"] = "[Error]",
                        ["type"] = "error",
                        ["relativePath"] = $"{currentRelativePath}/[Error]",
                        ["depth"] = depth + 1,
                        ["error"] = ex.Message
                    });
                }
            }
        }
        catch (Exception ex)
        {
            items.Add(new Dictionary<string, object>
            {
                ["name"] = Path.GetFileName(path),
                ["type"] = "error",
                ["relativePath"] = relativePath,
                ["depth"] = depth,
                ["error"] = ex.Message
            });
        }
    }

    private static string FormatBytes(long bytes)
    {
        if (bytes == 0) return "0 B";

        string[] suffixes = { "B", "KB", "MB", "GB", "TB" };
        int counter = 0;
        decimal number = bytes;

        while (Math.Round(number / 1024) >= 1 && counter < suffixes.Length - 1)
        {
            number /= 1024;
            counter++;
        }

        return string.Format("{0:n1} {1}", number, suffixes[counter]);
    }

    private dynamic CalculateDetailedStatistics(List<object> structure)
    {
        var fileTypes = new Dictionary<string, int>();
        var largestFiles = new List<object>();
        var deepestPath = 0;

        foreach (var itemObj in structure)
        {
            if (itemObj is Dictionary<string, object> item)
            {
                // Depth statistics
                if (item.ContainsKey("depth") && item["depth"] is int depth)
                {
                    deepestPath = Math.Max(deepestPath, depth);
                }

                // File type statistics
                if (item.ContainsKey("extension") && item["extension"] is string ext)
                {
                    fileTypes[ext] = fileTypes.ContainsKey(ext) ? fileTypes[ext] + 1 : 1;
                }

                // Larger files
                if (item.ContainsKey("size") && item["size"] is long size && size > 0)
                {
                    largestFiles.Add(new
                    {
                        name = item["name"],
                        relativePath = item["relativePath"],
                        size = size,
                        sizeFormatted = item.ContainsKey("sizeFormatted") ? item["sizeFormatted"] : FormatBytes(size)
                    });
                }
            }
        }

        // Sort the largest files by size
        largestFiles = largestFiles.OrderByDescending(f => ((dynamic)f).size).Take(10).ToList();

        return new
        {
            FileTypes = fileTypes.OrderByDescending(kvp => kvp.Value).ToDictionary(kvp => kvp.Key, kvp => kvp.Value),
            LargestFiles = largestFiles,
            DeepestPath = deepestPath
        };
    }

    #endregion
}