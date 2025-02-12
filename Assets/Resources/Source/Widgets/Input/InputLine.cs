using UnityEngine;

using static Root;
using static Font;
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
        this.color = Coloring.colors.ContainsKey(color) ? color : "";
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
    
    public int Length() => fonts["Tahoma Bold"].Length(text.text.Value());

    //String which is modified by interacting with the input field
    public static String inputDestination;

    //Window where the current input line resides at
    public static string inputLineWindow;

    public static void ExecuteQuit(String foo)
    {
        if (foo == promptConfirm)
            CloseWindow("ConfirmDeleteCharacter");
    }

    public static void ExecuteChange(String foo)
    {
        if (foo == promptConfirm)
        {
            if (WindowUp("ConfirmDeleteCharacter"))
            {
                //if (foo.Value() == "DELETE")
                //{
                //    saves[GameSettings.settings.selectedRealm].RemoveAll(x => x.player.name == GameSettings.settings.selectedCharacter);
                //    if (saves[GameSettings.settings.selectedRealm].Count > 0)
                //        GameSettings.settings.selectedCharacter = saves[GameSettings.settings.selectedRealm].First().player.name;
                //    else GameSettings.settings.selectedCharacter = "";
                //    CloseWindow("ConfirmDeleteCharacter");
                //    RemoveDesktopBackground();
                //    Respawn("CharacterInfo");
                //    Respawn("CharacterRoster");
                //    Respawn("TitleScreenSingleplayer");
                //    SaveGames();
                //}
                //else
                //    CloseWindow("ConfirmDeleteCharacter");
            }
        }
    }
}
