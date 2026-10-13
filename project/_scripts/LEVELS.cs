using System.IO;

namespace Raveyard;

public static class LEVELS
{
    public readonly static LEVELS_metadata[] List = [

        new LEVELS_metadata() {
            fileName = "prototype",
            displayName = "prototype",
        },

        new LEVELS_metadata() {
            fileName = "peakuniku",
            displayName = "prototype 2",
        },

        new LEVELS_metadata() {
            fileName = "custom",
            displayName = "custom",
        },

    ];

    public static string GetLevelPathByName(string fileName)
    {
        return Path.Combine(Directory.GetCurrentDirectory(), @"_charts\", fileName);
    }
}

public struct LEVELS_metadata
{
    public string fileName;
    public string displayName;
}