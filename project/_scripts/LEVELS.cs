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
            // skeleton
            new CustomerVariantData()
            { 
                asset_name = "customer_sprsheet", texture_region = new Vector2(551, 581),
                frames_talk = [1, 0], frames_satisfied = [2],
            },

            // drac
            new CustomerVariantData()
            { 
                asset_name = "customer_sprsheet_dracula", texture_region = new Vector2(596, 563),
                frames_talk = [0, 1], frames_satisfied = [2], frames_extra = [2, 3, 4], frames_idle = [1]
            },
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