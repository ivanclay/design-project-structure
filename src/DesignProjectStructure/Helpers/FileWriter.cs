using System.Text;

namespace DesignProjectStructure.Helpers;

public static class FileWriter
{
    public static void SaveToFile(string outputFile, string completeText)
    {
        File.WriteAllText(outputFile, completeText.ToString(), Encoding.UTF8);
    }
}

