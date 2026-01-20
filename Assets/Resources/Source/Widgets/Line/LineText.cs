using UnityEngine;

using System.Linq;
using System.Collections.Generic;

using static Font;
using static Coloring;

public class LineText : MonoBehaviour
{
    //Parent
    public Window window;

    //Children
    public List<GameObject> characters;

    //Fields
    public string text, color, layer;

    public void Initialise(Line line, string text, string color, string layer = "")
    {
        this.color = color;
        this.text = text;
        this.layer = layer;
        window = line.region.regionGroup.window;
        characters = new();
        line.texts.Add(this);
    }

    public void Initialise(Window window, string text, string color, string layer = "")
    {
        this.color = color;
        this.text = text;
        this.layer = layer;
        this.window = window;
        characters = new();
    }

    public void Erase()
    {
        while (characters.Count > 0)
        {
            Destroy(characters[0]);
            characters.RemoveAt(0);
        }
    }

    public int SpawnCharacter(char character, int offset)
    {
        var newCharacter = new GameObject("Character", typeof(SpriteRenderer));
        newCharacter.transform.parent = transform;
        newCharacter.transform.localPosition = new Vector3(offset, 0, -0.05f);
        var glyph = GetFontWithSpecificGlyph(character).GetGlyph(character);
        var r = newCharacter.GetComponent<SpriteRenderer>();
        r.sprite = glyph;
        if (color != null && color.Count(x => x == ':') == 2)
        {
            var split = color.Split(':').Select(x => int.Parse(x)).ToArray();
            r.color = new Color32((byte)split[0], (byte)split[1], (byte)split[2], 255);
        }
        else
        {
            if (color == null) { Debug.Log("ERROR 009: Color was not set"); color = "Gray"; }
            else if (!colors.ContainsKey(color)) { Debug.Log("ERROR 008: Color not found: \"" + color + "\""); color = "Gray"; }
            r.color = colors[color];
        }
        r.sortingLayerName = layer == "" ? window.layer : layer;
        characters.Add(newCharacter);
        return offset + (int)glyph.rect.width + 1;
    }
}
