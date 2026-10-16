using System.IO;
using Microsoft.Xna.Framework;

namespace Raveyard;

// STRUCTS
public struct LEVELS_metadata
{
    public string fileName;
    public string displayName;
    public LEVELS_skinparams levelParameters = new LEVELS_skinparams();

    public LEVELS_metadata() {}
}

public struct LEVELS_skinparams
{
    public CustomerVariantData[] customerVariants;

    public LEVELS_skinparams() // defaults
    {
        customerVariants =
        [
            new CustomerVariantData(){ asset_name = "customer_sprsheet", texture_region = new Vector2(551, 581)}
        ];
    }
}
//

public static class LEVELS
{
    public readonly static LEVELS_metadata[] List = [

        new LEVELS_metadata() {
            fileName = "tutorial",
            displayName = "Tutorial",

            levelParameters = 
            {
                customerVariants = [
                    new CustomerVariantData(){ asset_name = "customer_sprsheet_thewind", texture_region = new Vector2(1, 1)},
                ]
            }
        },

        new LEVELS_metadata() {
            fileName = "main",
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