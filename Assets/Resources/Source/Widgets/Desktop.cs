using System.Linq;
using System.Collections.Generic;

using UnityEngine;

using static Root;
using static Cursor;
using static InputLine;

public class Desktop : MonoBehaviour
{
    //Name of the desktop screen
    public string title;

    //List of all windows active on this desktop
    public List<Window> windows;

    //Camera that is rendering the screen of this desktop
    public Camera screen;

    //Gradient objects for coloring the screen
    public SpriteRenderer gradientTop, gradientBottom;

    //Transition object that shows desktop transition effects
    public GameObject transition;

    //List of hotkeys for this desktop
    public List<Hotkey> hotkeys;

    //Screenlock object that will cover the
    //screen if needed and prevent cursor with interacting with anything
    public GameObject screenlock;

    //Status of the screen lock
    public bool screenLocked;

    //Destination where the camera should be on this desktop
    public Vector2 cameraDestination;

    //Window that handles quick input
    public Window quickInputWindow;

    public void Initialise(string title)
    {
        this.title = title;
        windows = new();
        hotkeys = new();
        cameraDestination = new Vector2();
    }

    public void RespawnAll()
    {
        var temp = screen.gameObject.GetComponent<SpriteRenderer>();
        if (temp != null && temp.sprite != null && temp.sprite.name.Contains("Default"))
            temp.color = new Color32((byte)ProgramSettings.settings.menuBackgroundColor[0], (byte)ProgramSettings.settings.menuBackgroundColor[1], (byte)ProgramSettings.settings.menuBackgroundColor[2], 255);
        for (int i = windows.Count - 1; i >= 0; i--)
            windows[i].Respawn();
    }

    public void RespawnAllScrollbarRelatedWindows()
    {
        var scrollbarRelated = CDesktop.windows.FindAll(x => x.title.ToLower().Contains("scrollbar")).Select(x => x.title);
        foreach (var window in scrollbarRelated) Respawn(window, true);
    }

    //Last built window
    public Window LBWindow() => windows.Last();

    //Is a specific window up
    public bool WindowUp(string title) => windows.Exists(x => x.title == title);

    public void SetTooltip(Tooltip tooltip)
    {
        Root.tooltip = tooltip;
        tooltipChanneling = 0.4f;
    }

    public void LockScreen()
    {
        cursor.SetCursor(CursorType.Await);
        screenLocked = true;
        screenlock.SetActive(true);
    }

    public void UnlockScreen()
    {
        cursor.SetCursor(CursorType.Default);
        canUnlockScreen = false;
        screenLocked = false;
        screenlock.SetActive(false);
    }

    public static bool waitForTexture;

    public void FixedUpdate()
    {
        if (CDesktop.title == "LoadingScreen")
        {
            if (Starter.enteredFifthStage)

                //If we still have to load more covers..
                if (loadingScreenProgress < loadingScreenAim)
                {
                    var atStart = loadingScreenProgress;

                    //If we already waiting for a cover to download and it appears to be downloaded..
                    if (waitForTexture && newCover != null)
                    {
                        var prefix = "";
                        if (Serialization.useUnityData) prefix = @"C:\Users\ragan\Documents\Projects\Unity\MooRT\";

                        //Save the newly downloaded album cover
                        var bytes = newCover.texture.EncodeToPNG();
                        System.IO.File.WriteAllBytes(prefix + "MooRT_Data_3/" + newCoverID + ".png", bytes);
                        Starter.coverBytes[newCoverID] = bytes;

                        //Reset the variables for downloading a cover
                        newCoverID = 0;
                        waitForTexture = false;
                        newCover = null;
                        returnToMenu = false;
                    }
                    else for (int i = atStart; i < loadingScreenAim; i++)
                        {
                            if (i - 10 >= atStart) break;
                            if (waitForTexture && returnToMenu)
                            {
                                waitForTexture = false;
                                newCover = null;
                                returnToMenu = false;
                                continue;
                            }
                            else if (!waitForTexture)
                            {
                                var rawBar = LoadBar(i + "");
                                if (rawBar != null && !albumBars.ContainsKey(i + ""))
                                    albumBars.Add(i + "", Sprite.Create(rawBar, new Rect(0, 0, 188, 17), new Vector2(0, 1), 1));

                                var localFilePresent = Starter.coverBytes[i] != null;
                                if (!localFilePresent && !Serialization.useUnityData)
                                {
                                    waitForTexture = true;
                                    newCover = null;
                                    returnToMenu = false;
                                    newCoverID = i;
                                    UnityEngine.Debug.Log("Downloading: " + "https://raw.githubusercontent.com/NetherlandsDonut/MooRT/refs/heads/main/MooRT_Data_3/" + i + ".png");
                                    StartCoroutine(GetTexture("https://raw.githubusercontent.com/NetherlandsDonut/MooRT/refs/heads/main/MooRT_Data_3/" + i + ".png"));
                                    break;
                                }
                                if (!localFilePresent) continue;
                                if (!albumCovers.ContainsKey(i + ""))
                                {
                                    loadingScreenProgress++;
                                    var localCover = new Texture2D(1, 1, TextureFormat.RGB24, false);
                                    ImageConversion.LoadImage(localCover, Starter.coverBytes[i]);
                                    albumCovers.Add(i + "", Sprite.Create(localCover, new Rect(0, 0, 188, 188), new Vector2(0, 1), 1));
                                    if (!albumBars.ContainsKey(i + ""))
                                    {
                                        Texture2D bar = new(188, 17, TextureFormat.ARGB32, false);
                                        bar.CopyPixels(localCover, 0, 0, 0, 93, 188, 17, 0, 0, 0);
                                        bar.Apply();
                                        albumBars.Add(i + "", Sprite.Create(bar, new Rect(0, 0, 188, 17), new Vector2(0, 1), 1));
                                        var prefix = "";
                                        if (Serialization.useUnityData) prefix = @"C:\Users\ragan\Documents\Projects\Unity\MooRT\";
                                        System.IO.File.WriteAllBytes(prefix + "MooRT_Data_4/" + i + ".png", albumBars.Last().Value.texture.EncodeToPNG());
                                    }
                                }
                            }
                        }
                    loadingStatusBar.transform.localScale = new Vector2(Mathf.Round((float)loadingScreenProgress / loadingScreenAim * 298.0f), 1);
                }
                else if (loadingScreenProgress >= loadingScreenAim)
                {
                    Starter.coverBytes = null;
                    cursor.SetCursor(CursorType.Default);
                    if (Starter.stopwatch != null)
                    {
                        UnityEngine.Debug.Log("Load time: " + Starter.stopwatch.ElapsedTicks);
                        Starter.stopwatch.Stop();
                        Starter.stopwatch = null;
                    }
                    UnityEngine.Cursor.visible = false;
                    if (Starter.goToMenuOnFailedWWW) SpawnDesktopBlueprint("LibraryRefetchSuccess");
                    else SpawnDesktopBlueprint("MusicReleases");
                    CloseDesktop("LoadingScreen");
                }
        }
        if (CDesktop.title != "LoadingScreen")
        {
            if (animatedSpriteTime >= 0)
            {
                animatedSpriteTime -= Time.deltaTime;
                if (animatedSpriteTime <= 0)
                {
                    animatedSpriteTime = 0.1f;
                    if (++AnimatedSprite.globalIndex == 24)
                        AnimatedSprite.globalIndex = 0;
                }
            }
        }
    }

    public void Update()
    {
        if (!Starter.enteredFifthStage) return;
        if (title == "LoadCover")
        {
            if (!startedGettingCover)
            {
                returnToMenu = false;
                startedGettingCover = true;
                StartCoroutine(GetTexture(newCoverURL));
            }
            else if (returnToMenu) CloseDesktop("LoadCover");
            else if (newCover != null)
            {
                CloseDesktop("LoadCover");
                rating.Set(false);
                SpawnDesktopBlueprint("AcceptNewAlbum");
            }
        }
        if (title == "CreateNewAlbumPreviewLoadCover")
        {
            if (!startedGettingCover)
            {
                returnToMenu = false;
                startedGettingCover = true;
                StartCoroutine(GetTexture(newCoverURL));
            }
            else if (returnToMenu)
            {
                CloseDesktop("CreateNewAlbumPreviewLoadCover");
                rating.Set(false);
                SpawnDesktopBlueprint("CreateNewAlbumPreview");
            }
            else if (newCover != null)
            {
                CloseDesktop("CreateNewAlbumPreviewLoadCover");
                rating.Set(false);
                SpawnDesktopBlueprint("CreateNewAlbumPreview");
            }
        }
        if (CDesktop.title == "LoadingScreen") return;
        if (hasFocus)
        {
            if (quickInputWindow != null && inputLineWindow == null && Scrollbar.scrollbarUsed == null)
            {
                if (quickInputTime <= 1) quickInputTime += Time.deltaTime;
                if (quickInputTime > 1) quickInput = "";
                if (Input.inputString.Length > 0)
                {
                    var before = quickInput.Length;
                    foreach (char c in Input.inputString)
                        if (c != '\b' && c != '\n' && c != '\r')
                        {
                            quickInput += c;
                            quickInputTime = 0;
                        }
                    if (before != quickInput.Length)
                    {
                        if (quickInputWindow.title == "MusicReleases")
                        {
                            var sorted = Library.library.releases.OrderBy(x => x.name.Length).ToList();
                            var first = sorted.FindIndex(x => x.name.ToLower().Contains(quickInput.ToLower()));
                            quickInputWindow.SetPagination(first >= 0 ? Library.library.releases.IndexOf(sorted[first]) : 0);
                        }
                        else if (quickInputWindow.title == "Artists")
                        {
                            var sorted = Library.library.artists.OrderBy(x => x.name.Length).ToList();
                            var first = sorted.FindIndex(x => x.name.ToLower().Contains(quickInput.ToLower()));
                            quickInputWindow.SetPagination(first >= 0 ? Library.library.artists.IndexOf(sorted[first]) : 0);
                        }
                        else if (quickInputWindow.title == "Countries")
                        {
                            var sorted = Country.countries.OrderBy(x => x.name.Length).ToList();
                            var first = sorted.FindIndex(x => x.name.ToLower().Contains(quickInput.ToLower()));
                            quickInputWindow.SetPagination(first >= 0 ? Country.countries.IndexOf(sorted[first]) : 0);
                        }
                        CDesktop.RespawnAll();
                    }
                }
            }
            if (Input.GetKeyDown(KeyCode.LeftShift) || Input.GetKeyUp(KeyCode.LeftShift) || Input.GetKeyDown(KeyCode.LeftControl) || Input.GetKeyUp(KeyCode.LeftControl)) CloseWindow("Tooltip");
            if (Input.GetMouseButtonUp(0) && Scrollbar.scrollbarUsed != null)
            {
                Scrollbar.scrollbarUsed.GetComponent<Highlightable>().pressedState = "None";
                Scrollbar.scrollbarUsed = null;
                cursor.SetCursor(CursorType.Default);
            }
            if (mouseOver != null)
            {
                if (mouseOver.pressedState == "None")
                {
                    if (Input.GetMouseButtonDown(0))
                        mouseOver.MouseDown("Left");
                    if (Input.GetMouseButtonDown(1))
                        mouseOver.MouseDown("Right");
                    if (Input.GetMouseButtonDown(2))
                        mouseOver.MouseDown("Middle");
                }
                else if (Input.GetMouseButtonUp(0))
                    mouseOver.MouseUp("Left");
                else if (Input.GetMouseButtonUp(1))
                    mouseOver.MouseUp("Right");
                else if (Input.GetMouseButtonUp(2))
                    mouseOver.MouseUp("Middle");
            }
            if (!screenLocked)
            {
                if (tooltip != null && !WindowUp("Tooltip"))
                {
                    tooltipChanneling -= Time.deltaTime;
                    if (tooltipChanneling <= 0 && tooltip.caller != null && tooltip.caller() != null)
                        tooltip.SpawnTooltip();
                }
                if (heldKeyTime > 0) heldKeyTime -= Time.deltaTime;
                if (inputLineWindow != null)
                {
                    var didSomething = false;
                    var length = inputDestination.Value().Length;
                    if (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.Return))
                    {
                        inputLineWindow = null;
                        UnityEngine.Cursor.lockState = CursorLockMode.None;
                        cursor.SetCursor(CursorType.Default);
                        if (Input.GetKeyDown(KeyCode.Return))
                        {
                            inputDestination.Confirm();
                            ExecuteChange(inputDestination);
                            didSomething = true;
                        }
                        else
                        {
                            inputDestination.Reset();
                            ExecuteQuit(inputDestination);
                            didSomething = true;
                        }
                    }
                    else if (Input.GetKeyDown(KeyCode.Delete) && inputLineMarker < length)
                    {
                        heldKeyTime = 0.4f;
                        inputDestination.RemoveNextOne(inputLineMarker);
                        didSomething = true;
                    }
                    else if (Input.GetKey(KeyCode.Delete) && inputLineMarker < length && heldKeyTime <= 0)
                    {
                        heldKeyTime = 0.0245f;
                        inputDestination.RemoveNextOne(inputLineMarker);
                        didSomething = true;
                    }
                    else if (Input.GetKeyDown(KeyCode.LeftArrow) && inputLineMarker > 0)
                    {
                        heldKeyTime = 0.4f;
                        inputLineMarker--;
                        didSomething = true;
                    }
                    else if (Input.GetKey(KeyCode.LeftArrow) && inputLineMarker > 0 && heldKeyTime <= 0)
                    {
                        heldKeyTime = 0.0245f;
                        inputLineMarker--;
                        didSomething = true;
                    }
                    else if (Input.GetKeyDown(KeyCode.RightArrow) && inputLineMarker < length)
                    {
                        heldKeyTime = 0.4f;
                        inputLineMarker++;
                        didSomething = true;
                    }
                    else if (Input.GetKey(KeyCode.RightArrow) && inputLineMarker < length && heldKeyTime <= 0)
                    {
                        heldKeyTime = 0.0245f;
                        inputLineMarker++;
                        didSomething = true;
                    }
                    else if (Input.GetKey(KeyCode.A) && Input.GetKey(KeyCode.LeftControl))
                    {
                        inputDestination.Clear();
                        inputLineMarker = 0;
                        didSomething = true;
                    }
                    else if (Input.GetKey(KeyCode.V) && Input.GetKey(KeyCode.LeftControl))
                    {
                        inputDestination.Paste();
                        inputLineMarker = inputDestination.Value().Length;
                        didSomething = true;
                    }
                    else foreach (char c in Input.inputString)
                        {
                            var a = inputLineMarker;
                            if (c == '\b')
                            {
                                if (inputLineMarker > 0 && length >= inputLineMarker)
                                    inputDestination.RemovePreviousOne(inputLineMarker--);
                            }
                            else if (c != '\n' && c != '\r' && inputDestination.CheckInput(c))
                            {
                                inputDestination.Insert(inputLineMarker, inputDestination.inputType == InputType.StrictLetters ? (inputDestination.Value().Length == 0 ? char.ToUpper(c) : char.ToLower(c)) : (inputDestination.inputType == InputType.Capitals ? char.ToUpper(c) : c));
                                inputLineMarker++;
                            }
                            if (length == inputDestination.Value().Length)
                                inputLineMarker = a;
                            didSomething = true;
                        }
                    if (didSomething)
                    {
                        Respawn(inputLineWindow);
                        var scrollbarRelated = CDesktop.windows.FindAll(x => x.title.ToLower().Contains("scrollbar")).Select(x => x.title);
                        foreach (var window in scrollbarRelated) Respawn(window, true);
                    }
                }
                else if (heldKeyTime <= 0)
                {
                    int helds = 0;
                    foreach (var hotkey in hotkeys.OrderByDescending(x => x.keyDown))
                        if (Input.GetKeyDown(hotkey.key) && hotkey.keyDown || Input.GetKey(hotkey.key) && !hotkey.keyDown)
                        {
                            CloseWindow("Tooltip");
                            tooltip = null;
                            if (Input.GetKeyDown(hotkey.key)) keyStack = 0;
                            else
                            {
                                heldKeyTime = 0.02f;
                                helds++;
                            }
                            hotkey.action();
                        }
                    if (helds > 0 && keyStack < 100) keyStack++;
                }
                var pageUp = hotkeys.Find(x => x.key == KeyCode.PageUp);
                if (pageUp != null && Input.mouseScrollDelta.y > 0)
                {
                    CloseWindow("Tooltip");
                    tooltip = null;
                    pageUp.action();
                }
                var pageDown = hotkeys.Find(x => x.key == KeyCode.PageDown);
                if (pageDown != null && Input.mouseScrollDelta.y < 0)
                {
                    CloseWindow("Tooltip");
                    tooltip = null;
                    pageDown.action();
                }
            }
        }
    }
}
