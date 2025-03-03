using UnityEngine;
using System.Collections.Generic;

public static class Coloring
{
    //Color pallete of the UI
    public static Dictionary<string, Color32> colors = new()
    {
        //Basic UI colors
        { "White",          new Color32(234, 234, 234, 255) },
        { "LightGray",      new Color32(202, 202, 202, 255) },
        { "Gray",           new Color32(183, 183, 183, 255) },
        { "DarkGray",       new Color32(114, 114, 114, 255) },
        { "DimGray",        new Color32(045, 045, 045, 255) },
        { "Black",          new Color32(031, 031, 031, 255) },

        //Currency colors
        { "Copper",         new Color32(184, 080, 041, 255) },
        { "Silver",         new Color32(170, 188, 210, 255) },
        { "Gold",           new Color32(255, 210, 011, 255) },

        //Rating colors
        { "Poor",           new Color32(114, 114, 114, 255) },
        { "Common",         new Color32(183, 183, 183, 255) },
        { "Uncommon",       new Color32(026, 201, 000, 255) },
        { "Rare",           new Color32(000, 117, 226, 255) },
        { "Epic",           new Color32(163, 053, 238, 255) },
        { "Legendary",      new Color32(221, 110, 000, 255) },
    };
}
