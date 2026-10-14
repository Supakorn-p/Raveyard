using System.IO;

namespace Raveyard;

public struct LEVELS_metadata
{
    public string fileName;
    public string displayName;
}

public struct LEVELS_skinparams
{
    
}

public static class LEVELS
{
    public readonly static LEVELS_metadata[] List = [

        new LEVELS_metadata() {
            fileName = "tutorial",
            displayName = "Tutorial",
        },

        new LEVELS_metadata() {
            fileName = "custom",
            displayName = "-----",
        },

        new LEVELS_metadata() {
            fileName = "peakuniku",
            displayName = "Pikuniku OST",
        },

        new LEVELS_metadata() {
            fileName = "prototype",
            displayName = "test",
        },

    ];

    public static string GetLevelPathByName(string fileName)
    {
        return Path.Combine(Directory.GetCurrentDirectory(), @"_charts\", fileName);
    }
}