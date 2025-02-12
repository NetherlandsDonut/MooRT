using System.Collections.Generic;
using System.Linq;

using UnityEngine;

public class Font
{
    //Initialises a font
    public Font(string name, string charset)
    {
        this.name = name;
        glyphs = Resources.LoadAll<Sprite>("Sprites/Fonts/" + name).ToDictionary(x => charset[int.Parse(x.name.Split("_")[1])], x => x);
        widths = glyphs.ToDictionary(x => x.Key, x => (int)x.Value.rect.width);
        this.charset = charset;
    }

    //Name of the font
    public string name;

    //Fullscreen of all characters provided by the font in the order of the charset variable
    public Dictionary<char, Sprite> glyphs;

    //Widths of the textures, later used in calculating overall text length
    public Dictionary<char, int> widths;

    //Provides information on how many pixels does specific text take up to be printed.
    //This is the basic way to calculate the width of regions and overally of UI
    public int Length(string text)
    {
        var sum = 0;
        foreach (var character in text)
            sum += 1 + Length(character);
        return sum - 1;
    }

    public int Length(char character) => widths.ContainsKey(character) ? widths[character] : 0;

    //Set of all characters available to print in UI
    public string charset;

    //Returns a texture corresponding to the given character
    //based on the order of the characters in the charset variable
    public Sprite GetGlyph(char character)
    {
        if (!fonts[name].glyphs.ContainsKey(character)) { Debug.LogWarning("This character was not found in the font glyph set: " + character); return fonts[name].glyphs['?']; }
        return fonts[name].glyphs[character];
    }

    //Current font loaded into memory
    public static Dictionary<string, Font> fonts;
}
