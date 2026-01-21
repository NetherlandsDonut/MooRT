using System.Linq;

using UnityEngine;

using static Font;
using static Root;
using static Cursor;
using static String;

public class InputLine : MonoBehaviour
{
    public Region region;
    public InputText text;
    public string color;
    public string align;

    public void Initialise(Region region, String refText, string color, string align)
    {
        this.region = region;
        this.color = color;
        this.align = align;
        text = new GameObject("InputText", typeof(InputText)).GetComponent<InputText>();
        text.transform.parent = transform;
        text.Initialise(this, refText);

        this.region.inputLine = this;
    }

    public void Activate(int marker = 0)
    {
        cursor.SetCursor(CursorType.None);
        UnityEngine.Cursor.lockState = CursorLockMode.Locked;
        inputLineWindow = region.regionGroup.window.title;
        inputDestination = text.inputLine.text.text;
        inputLineMarker = marker == 0 ? text.inputLine.text.text.value.Length : marker;
        region.regionGroup.window.Respawn();
    }
    
    public int Length() => text.text.Value().Sum(x => 1 + GetFontWithSpecificGlyph(x).Length(x)) - 1;

    //String which is modified by interacting with the input field
    public static String inputDestination;

    //Window where the current input line resides at
    public static string inputLineWindow;

    public static void ExecuteQuit(String foo)
    {
        if (foo == promptConfirm)
            CloseWindow("ConfirmDeleteCharacter");
        CDesktop.RespawnAll();
    }

    public static void ExecuteChange(String foo)
    {
        if (foo.inputType == InputType.Numbers)
           foo.Set("" + int.Parse(foo.Value()));
        if (WindowUp("RatingColorRange1"))
        {
            if (int.Parse(foo.Value()) > 999) foo.Set("999");
            else if (int.Parse(foo.Value()) < 0) foo.Set("0");
            ProgramSettings.settings.ratingRanges = ProgramSettings.settings.ratingRanges.OrderByDescending(x => int.Parse(x.min.Value())).ToList();
        }
        CDesktop.RespawnAll();
    }
}
