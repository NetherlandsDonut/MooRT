using System.Linq;
using System.Collections.Generic;

using UnityEngine;

using static Coloring;
using static Defines;
using static Font;

public class InputText : MonoBehaviour
{
    public InputLine inputLine;
    public List<GameObject> characters;
    public String text;

    public void Initialise(InputLine inputLine, String text)
    {
        this.text = text;
        characters = new();
        this.inputLine = inputLine;
        inputLine.text = this;
    }

    public void Erase()
    {
        while (characters.Count > 0)
        {
            Destroy(characters[0]);
            characters.RemoveAt(0);
        }
    }

    public int SpawnCharacter(char character, int offset, string color = "")
    {
        var newCharacter = new GameObject("Character", typeof(SpriteRenderer));
        newCharacter.transform.parent = transform;
        newCharacter.transform.localPosition = new Vector3(offset, 0, 0.2f);
        var glyph = GetFontWithSpecificGlyph(character).GetGlyph(character);
        newCharacter.GetComponent<SpriteRenderer>().sortingLayerName = inputLine.region.regionGroup.window.layer;
        newCharacter.GetComponent<SpriteRenderer>().sprite = glyph;
        if (character + "" == defines.markerCharacter)
            newCharacter.GetComponent<SpriteRenderer>().color = colors["Gray"];
        else if (color != null && color.Count(x => x == ':') == 2)
        {
            var split = color.Split(':').Select(x => int.Parse(x)).ToArray();
            newCharacter.GetComponent<SpriteRenderer>().color = new Color32((byte)split[0], (byte)split[1], (byte)split[2], 255);
        }
        else
            newCharacter.GetComponent<SpriteRenderer>().color = colors[color != "" ? color : "LightGray"];
        if (character + "" == defines.markerCharacter) newCharacter.AddComponent<Blinking>();
        else
        {
            newCharacter.AddComponent<Highlightable>().Initialise(inputLine.region, null, null, null, null);
            newCharacter.AddComponent<InputCharacter>().Initialise(this);
            newCharacter.AddComponent<BoxCollider2D>().size += new Vector2(1f, 0);
        }
        characters.Add(newCharacter);
        return offset + (int)glyph.rect.width + 1;
    }
}
