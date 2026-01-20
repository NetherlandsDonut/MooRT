using System.Linq;
using System.Collections.Generic;

using UnityEngine;

public class Font
{
    //Initialises a font
    public Font(string name, string charset)
    {
        this.name = name;
        glyphs = new();
        var temp = Resources.LoadAll<Sprite>("Sprites/Fonts/" + name);
        foreach (var glyph in temp)
        {
            var foo = charset[int.Parse(glyph.name.Split("_")[1])];
            if (!glyphs.ContainsKey(foo))
                glyphs.Add(foo, glyph);
        }
        widths = glyphs.ToDictionary(x => x.Key, x => (int)x.Value.rect.width);
        this.charset = charset;
    }

    //Name of the font
    public string name;

    //Fullscreen of all characters provided by the font in the order of the charset variable
    public Dictionary<char, Sprite> glyphs;

    //Widths of the textures, later used in calculating overall text length
    public Dictionary<char, int> widths;

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

    //Finds the first font with the desired character
    public static Font GetFontWithSpecificGlyph(char character)
    {
        var findFont = fonts.ToList().Find(x => x.Value.charset.Contains(character)).Value;
        return findFont ?? fonts["Tahoma Bold"];
    }
}
