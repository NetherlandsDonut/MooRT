using System;
using System.IO;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.Networking;

using static Sound;
using static Blueprint;
using static ProgramSettings;

using static Root.Anchor;
using static Root.RegionBackgroundType;

public static class Root
{
    //Width of the screen in pixels
    public static int screenX = 960;

    //Height of the screen in pixels
    public static int screenY = 540;

    //Checks whether the screen can be unlocked (will happen a frame later)
    public static bool canUnlockScreen;

    //Instance of Random that helps in generating random numbers
    public static System.Random random;

    public static Highlightable mouseOver;

    //Index of the input line marker
    public static int inputLineMarker;

    //Planned tooltip to be shown
    public static Tooltip tooltip;

    //Amount of time left to show the tooltip
    public static float tooltipChanneling;

    public static float lastFunnyEffectTime;
    public static Vector3 lastFunnyEffectPosition;
    public static List<(int, int)> titleScreenFunnyEffect = new();

    public static int keyStack;
    public static float heldKeyTime;
    public static float animationTime;
    public static float animatedSpriteTime;
    public static float quickInputTime;
    public static string quickInput = "";
    public static bool hasFocus = true;

    public static int currentRound;
    public static int perRound = 2;
    public static int tracksPerArtist;

    public static Desktop CDesktop, LBDesktop;

    public static List<Desktop> desktops;

    public static string releasesLastSort, lastSort;

    public static Dictionary<string, Sprite> albumCovers;

    public static Dictionary<string, Sprite> albumBars;

    public static Dictionary<string, Sprite> buildingSprites;

    public static Dictionary<int, Bool> artistFiltering;

    public static Dictionary<string, Bool> countryFiltering;

    public static Dictionary<int, Bool> yearFiltering;

    public static Dictionary<int, Bool> decadeFiltering;

    public static Dictionary<string, Bool> languageFiltering;

    public static Dictionary<string, Bool> genreFiltering;

    public static Dictionary<string, Bool> releaseTypeFiltering;

    public static Dictionary<int, Bool> trackAmountFiltering;

    public static Dictionary<int, Bool> debutYearFiltering;

    public static Dictionary<int, Bool> durationFiltering;

    public static Dictionary<int, Bool> artistBattleParticipants;

    public static int artistBattleTrackAmount;

    public static int loadingScreenProgress;
    public static int loadingScreenAim;
    public static int errorAtLine;
    public static string newCoverURL;
    public static Sprite newCover;
    public static bool startedGettingCover;
    public static bool returnToMenu;

    public static MusicRelease newRelease;
    public static Artist newArtist, artistFind;

    public static Transform loadingStatusBar;

    public static string DayName(string dayOfMonth) => int.Parse(dayOfMonth) + (dayOfMonth[..2] == "11" ? "th" : (dayOfMonth.Last() == '1' ? "st" : (dayOfMonth.Last() == '2' ? "nd" : (dayOfMonth.Last() == '3' ? "rd" : "th"))));

    public static Dictionary<int, string> monthNames = new()
    {
        { 01, "January" },
        { 02, "February" },
        { 03, "March" },
        { 04, "April" },
        { 05, "May" },
        { 06, "June" },
        { 07, "July" },
        { 08, "August" },
        { 09, "September" },
        { 10, "October" },
        { 11, "November" },
        { 12, "December" }
    };

    public static Bool showExcludedElements;
    public static Bool hideArtistsOfExcludedCountries = new(true);
    public static Bool requireAllSelectedGenres = new(false);
    public static Bool requireAllSelectedLanguages = new(false);
    public static Bool rating = new(false);

    #region Desktop

    private static Blueprint FindDesktopBlueprint(string name)
    {
        var find = desktopBlueprints.Find(x => x.title == name);
        return find;
    }

    //Spawns a new desktop and switches automatically by default
    public static void SpawnDesktopBlueprint(string blueprintTitle, bool autoSwitch = true)
    {
        var blueprint = FindDesktopBlueprint(blueprintTitle);
        if (blueprint == null) return;
        var spawnedNew = false;
        if (!desktops.Exists(x => x.title == blueprintTitle))
            { AddDesktop(blueprint.title); spawnedNew = true; }
        if (autoSwitch) SwitchDesktop(blueprintTitle);
        if (spawnedNew) blueprint.actions();
    }

    //Closes a desktop whether it was active or not
    public static bool CloseDesktop(string desktopName)
    {
        var find = desktops.Find(x => x.title == desktopName);
        if (find == null) return false;
        desktops.Remove(find);
        if (find == CDesktop)
            if (desktops.Count > 0) SwitchDesktop(desktops[0].title);
            else CDesktop = null;
        UnityEngine.Object.Destroy(find.gameObject);
        return true;
    }

    private static void AddDesktop(string title)
    {
        var newObject = new GameObject("Desktop: " + title, typeof(Desktop), typeof(SpriteRenderer));
        newObject.transform.localPosition = new Vector3();
        var newDesktop = newObject.GetComponent<Desktop>();
        LBDesktop = newDesktop;
        newDesktop.Initialise(title);
        desktops.Add(newDesktop);
        newDesktop.screen = new GameObject("Camera", typeof(Camera), typeof(SpriteRenderer)).GetComponent<Camera>();
        newDesktop.screen.transform.parent = newDesktop.transform;
        var screenOffsetter = new GameObject("CameraOffset");
        screenOffsetter.transform.parent = newDesktop.transform;
        screenOffsetter.transform.localPosition = new Vector2(10, -9);
        newDesktop.screen.transform.parent = screenOffsetter.transform;
        newDesktop.GetComponent<SpriteRenderer>().sortingLayerName = "DesktopBackground";
        newDesktop.screen.GetComponent<SpriteRenderer>().sortingLayerName = "DesktopBackground";
        newDesktop.screen.orthographicSize = screenY / 2;
        newDesktop.screen.nearClipPlane = -1024;
        newDesktop.screen.farClipPlane = 4096;
        newDesktop.screen.clearFlags = CameraClearFlags.SolidColor;
        newDesktop.screen.backgroundColor = new Color32(0, 29, 41, 255);
        newDesktop.screen.orthographic = true;
        if (settings.pixelPerfectVision.Value()) newDesktop.screen.gameObject.AddComponent<PixelCamera>();
        var cameraBorder = new GameObject("CameraBorder", typeof(SpriteRenderer));
        cameraBorder.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Sprites/Fullscreen/Camera/CameraBorder");
        cameraBorder.GetComponent<SpriteRenderer>().sortingLayerName = "CameraBorder";
        var cameraShadow = new GameObject("CameraShadow", typeof(SpriteRenderer));
        cameraShadow.transform.parent = cameraBorder.transform.parent = newDesktop.screen.transform;
        cameraShadow.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Sprites/Fullscreen/Camera/CameraShadow");
        cameraShadow.GetComponent<SpriteRenderer>().sortingLayerName = "CameraShadow";
        var cameraGradientTop = new GameObject("CameraGradientTop", typeof(SpriteRenderer));
        cameraGradientTop.transform.parent = cameraBorder.transform.parent = newDesktop.screen.transform;
        newDesktop.gradientTop = cameraGradientTop.GetComponent<SpriteRenderer>();
        newDesktop.gradientTop.sprite = Resources.Load<Sprite>("Sprites/Fullscreen/Backgrounds/GradientTop");
        newDesktop.gradientTop.sortingLayerName = "DesktopBackground";
        var cameraGradientBottom = new GameObject("CameraGradientBottom", typeof(SpriteRenderer));
        cameraGradientBottom.transform.parent = cameraBorder.transform.parent = newDesktop.screen.transform;
        newDesktop.gradientBottom = cameraGradientBottom.GetComponent<SpriteRenderer>();
        newDesktop.gradientBottom.sprite = Resources.Load<Sprite>("Sprites/Fullscreen/Backgrounds/GradientBottom");
        newDesktop.gradientBottom.sortingLayerName = "DesktopBackground";
        newDesktop.screenlock = new GameObject("Screenlock", typeof(BoxCollider2D), typeof(SpriteRenderer));
        newDesktop.screenlock.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Sprites/Camera/CameraScreenlock");
        newDesktop.screenlock.GetComponent<SpriteRenderer>().sortingLayerName = "DesktopBackground";
        newDesktop.screenlock.GetComponent<SpriteRenderer>().sortingOrder = 1;
        newDesktop.screenlock.GetComponent<BoxCollider2D>().size = new Vector2(screenX, screenY);
        newDesktop.screenlock.transform.parent = newDesktop.screen.transform;
        newDesktop.UnlockScreen();
        newObject.SetActive(false);
    }

    public static void SwitchDesktop(string name)
    {
        Cursor.cursor.ResetColor();
        if (CDesktop != null && CDesktop.title == name) return;
        var windows = CDesktop != null ? CDesktop.windows.Select(x => x.title).ToList() : null;
        if (mouseOver != null) mouseOver.OnMouseExit();
        if (CDesktop != null) CDesktop.gameObject.SetActive(false);
        var find = desktops.Find(x => x.title == name);
        if (find != null) CDesktop = find;
        if (CDesktop != null)
        {
            CDesktop.gameObject.SetActive(true);
            desktops.Remove(CDesktop);
            desktops.Insert(0, CDesktop);
            SpawnTransition();
        }
        if (CDesktop.cameraDestination != Vector2.zero)
        {
            Cursor.cursor.transform.position += (Vector3)CDesktop.cameraDestination - CDesktop.screen.transform.localPosition;
            CDesktop.screen.transform.localPosition = (Vector3)CDesktop.cameraDestination;
        }
        if (windows != null)
            foreach (var window in windows)
                Respawn(window, true);
        if (!settings.pixelPerfectVision.Value() && CDesktop.screen.GetComponent<PixelCamera>() != null) UnityEngine.Object.Destroy(CDesktop.screen.GetComponent<PixelCamera>());
        else if (settings.pixelPerfectVision.Value() && CDesktop.screen.GetComponent<PixelCamera>() == null) CDesktop.screen.gameObject.AddComponent<PixelCamera>();
    }

    public static void SpawnTransition(bool single = true, float speed = 2f)
    {
        if (CDesktop.transition != null && single) return;
        var transition = new GameObject("CameraTransition", typeof(SpriteRenderer), typeof(Shatter));
        transition.transform.parent = CDesktop.screen.transform;
        transition.transform.localPosition = new Vector3(0, 0, -0.01f);
        transition.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Sprites/Fullscreen/Camera/CameraTransition");
        transition.GetComponent<SpriteRenderer>().sortingLayerName = "CameraBorder";
        transition.GetComponent<Shatter>().Initiate(speed, 0, transition.GetComponent<SpriteRenderer>());
        CDesktop.transition = transition;
    }

    public static void SpawnAlbumTransition(float speed = 2f)
    {
        var transition = new GameObject("CameraAlbumTransition", typeof(SpriteRenderer), typeof(Shatter));
        transition.transform.parent = CDesktop.screen.transform;
        transition.transform.localPosition = new Vector3(0, 0, -0.01f);
        transition.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Sprites/Fullscreen/Camera/CameraAlbumTransition" + (rating.Value() ? "Rating" : ""));
        transition.GetComponent<SpriteRenderer>().sortingLayerName = "CameraBorder";
        transition.GetComponent<Shatter>().Initiate(speed, 0, transition.GetComponent<SpriteRenderer>());
        CDesktop.transition = transition;
    }

    public static void RemoveDesktopBackground(bool followCamera = true)
    {
        if (followCamera) CDesktop.screen.GetComponent<SpriteRenderer>().sprite = null;
        else CDesktop.GetComponent<SpriteRenderer>().sprite = null;
    }

    public static void SetDesktopBackground(string texture, bool followCamera = true)
    {
        CDesktop.gradientTop.color = new Color(0, 0, 0, 0);
        CDesktop.gradientBottom.color = new Color(0, 0, 0, 0);
        var sprite = Resources.Load<Sprite>("Sprites/Fullscreen/" + texture);
        var temp = (followCamera ? CDesktop.screen.gameObject : CDesktop.gameObject).GetComponent<SpriteRenderer>();
        if (sprite == null) Debug.Log("ERROR 004: Desktop background not found: \"Sprites/Fullscreen/" + texture + "\"");
        else if (temp.sprite != sprite)
        {
            SpawnTransition();
            temp.sprite = sprite;
        }
    }

    public static void SetDesktopBackgroundAsGradient(List<Color> pallete)
    {
        var temp = CDesktop.screen.gameObject.GetComponent<SpriteRenderer>();
        temp.sprite = null;
        CDesktop.gradientTop.color = pallete[random.Next(0, 8)];
        CDesktop.gradientBottom.color = pallete[random.Next(8, 16)];
    }

    //Hotkeys can be added only on desktop creation!
    public static void AddHotkey(KeyCode key, Action action, bool keyDown = true)
    {
        if (LBDesktop.hotkeys.Exists(x => x.key == key && x.keyDown == keyDown)) return;
        LBDesktop.hotkeys.Add(new Hotkey(key, action, keyDown));
    }

    #endregion

    #region Windows

    public static Blueprint FindWindowBlueprint(string name)
    {
        var find = windowBlueprints.Find(x => x.title == name);
        return find;
    }

    public static Window SpawnWindowBlueprint(string blueprintTitle, bool resetSearch = true)
    {
        return SpawnWindowBlueprint(FindWindowBlueprint(blueprintTitle), resetSearch);
    }

    public static Window SpawnWindowBlueprint(Blueprint blueprint, bool resetSearch = true)
    {
        if (blueprint == null) return null;
        if (WindowUp(blueprint.title)) return null;
        AddWindow(blueprint.title, blueprint.upperUI);
        blueprint.actions();
        if (resetSearch && CDesktop.LBWindow().maxPaginationReq != null) String.search.Set("");
        CDesktop.LBWindow().Rebuild();
        CDesktop.LBWindow().ResetPosition();
        return CDesktop.LBWindow();
    }

    public static bool WindowUp(string title) => CDesktop.windows.Exists(x => x.title == title);

    public static void AddWindow(string title, bool upperUI)
    {
        var newObject = new GameObject("Window: " + title, typeof(Window));
        newObject.transform.parent = CDesktop.transform;
        newObject.GetComponent<Window>().Initialise(CDesktop, title, upperUI);
    }

    public static bool Respawn(string windowName, bool onlyWhenActive = false)
    {
        var window = CDesktop.windows.Find(x => x.title == windowName);
        bool wasThere = window != null;
        if (wasThere) window.Respawn(onlyWhenActive);
        else if (!onlyWhenActive) SpawnWindowBlueprint(windowName, true);
        return wasThere;
    }

    public static bool CloseWindow(string windowName, bool resetPagination = true)
    {
        return CDesktop != null && CloseWindow(CDesktop.windows.Find(x => x.title == windowName), resetPagination);
    }

    public static bool CloseWindow(Window window, bool resetPagination = true)
    {
        if (window == null) return false;
        if (resetPagination && staticPagination.ContainsKey(window.title))
            staticPagination.Remove(window.title);
        CDesktop.windows.Remove(window);
        UnityEngine.Object.Destroy(window.gameObject);
        return true;
    }

    public static void SetAnchor(float x = 0, float y = 0)
    {
        CDesktop.LBWindow().anchor = new WindowAnchor(None, x, y);
    }

    public static void SetAnchor(Anchor anchor, float x = 0, float y = 0)
    {
        CDesktop.LBWindow().anchor = new WindowAnchor(anchor, x, y);
    }

    public static void DisableGeneralSprites()
    {
        CDesktop.LBWindow().disabledGeneralSprites = true;
    }

    public static void DisableCollisions()
    {
        CDesktop.LBWindow().disabledCollisions = true;
    }

    public static void DisableShadows()
    {
        CDesktop.LBWindow().disabledShadows = true;
    }

    public static void MaskWindow()
    {
        CDesktop.LBWindow().masked = true;
    }

    #endregion

    #region RegionGroups

    public static void AddHeaderGroup(bool paged = false)
    {
        var newObject = new GameObject("HeaderGroup", typeof(RegionGroup));
        newObject.transform.parent = CDesktop.LBWindow().transform;
        newObject.GetComponent<RegionGroup>().Initialise(CDesktop.LBWindow(), true);
    }

    public static void AddRegionGroup()
    {
        var newObject = new GameObject("RegionGroup", typeof(RegionGroup));
        newObject.transform.parent = CDesktop.LBWindow().transform;
        newObject.GetComponent<RegionGroup>().Initialise(CDesktop.LBWindow(), false);
    }

    public static void SetRegionGroupWidth(int width)
    {
        CDesktop.LBWindow().LBRegionGroup().setWidth = width;
    }

    public static void SetRegionGroupHeight(int height)
    {
        CDesktop.LBWindow().LBRegionGroup().setHeight = height;
    }

    #endregion

    #region Regions

    private static void AddRegion(RegionBackgroundType backgroundType, Action draw, Action<Highlightable> pressEvent, Action<Highlightable> rightPressEvent, Func<Highlightable, Action> tooltip, Action<Highlightable> middlePressEvent)
    {
        var region = new GameObject("Region", typeof(Region)).GetComponent<Region>();
        var regionGroup = CDesktop.LBWindow().LBRegionGroup();
        region.transform.parent = regionGroup.transform;
        region.background = new GameObject("Background", typeof(SpriteRenderer), typeof(RegionBackground));
        region.background.transform.parent = region.transform;
        region.Initialise(regionGroup, backgroundType, draw);
        if (pressEvent != null || rightPressEvent != null || middlePressEvent != null || tooltip != null)
            region.background.AddComponent<Highlightable>().Initialise(region, pressEvent, rightPressEvent, tooltip, middlePressEvent);
    }

    public static void HideBottomLine()
    {
        var region = CDesktop.LBWindow().LBRegionGroup().LBRegion();
        region.hiddenBottomLine = true;
    }

    public static void HideTopLine()
    {
        var region = CDesktop.LBWindow().LBRegionGroup().LBRegion();
        region.hiddenTopLine = true;
    }

    public static void AddRegionOverlay(Region onWhat, string overlay, float time = 0)
    {
        var newObject = new GameObject("RegionOverlay", typeof(SpriteRenderer));
        newObject.transform.parent = onWhat.transform;
        newObject.transform.localPosition = onWhat.background.transform.localPosition - new Vector3(0, 0, 0.1f);
        newObject.transform.parent = CDesktop.transform;
        newObject.transform.localScale = onWhat.background.transform.localScale;
        newObject.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Sprites/Fills/" + overlay);
        if (time > 0)
        {
            newObject.AddComponent<Shatter>().render = newObject.GetComponent<SpriteRenderer>();
            newObject.GetComponent<Shatter>().Initiate(time);
        }
    }

    public static void AddRegionOverlay(string overlay)
    {
        var onWhat = CDesktop.LBWindow().LBRegionGroup().LBRegion();
        var newObject = new GameObject("RegionOverlay", typeof(SpriteRenderer));
        newObject.transform.parent = onWhat.transform;
        newObject.transform.localPosition = new Vector3(2, -2, 0.1f);
        var high = onWhat.background.GetComponent<Highlightable>();
        if (high != null) high.additionalRender = newObject.GetComponent<SpriteRenderer>();
        newObject.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Sprites/" + overlay);
    }

    public static void AddButtonRegion(Action draw, Action<Highlightable> pressEvent = null, Action<Highlightable> rightPressEvent = null, Func<Highlightable, Action> tooltip = null, Action<Highlightable> middlePressEvent = null)
    {
        AddRegion(Button, draw, pressEvent, rightPressEvent, tooltip, middlePressEvent);
    }

    public static void AddEmptyRegion()
    {
        AddRegion(Empty, () => { AddLine(""); }, null, null, null, null);
    }

    public static void AddHeaderRegion(Action draw)
    {
        AddRegion(Header, draw, null, null, null, null);
    }

    public static void AddPaddingRegion(Action draw)
    {
        AddRegion(Padding, draw, null, null, null, null);
    }

    public static void AddPaginationLine()
    {
        AddPaddingRegion(() =>
        {
            var thisWindow = CDesktop.LBWindow();
            AddLine("Page: ", "DarkGray");
            AddText(thisWindow.pagination() + 1 + "");
            AddText(" / ", "DarkGray");
            AddText(thisWindow.maxPagination() + 1 + "");
            AddSmallButton("OtherNextPage", (h) =>
            {
                if (thisWindow.pagination() < thisWindow.maxPagination())
                {
                    PlaySound("DesktopChangePage", 0.6f);
                    thisWindow.IncrementPagination();
                }
            });
            AddSmallButton("OtherPreviousPage", (h) =>
            {
                if (thisWindow.pagination() > 0)
                {
                    PlaySound("DesktopChangePage", 0.6f);
                    thisWindow.DecrementPagination();
                }
            });
        });
    }

    public static void ReverseButtons()
    {
        CDesktop.LBWindow().LBRegionGroup().LBRegion().reverseButtons ^= true;
    }

    public static void SetRegionBackgroundToGrayscale()
    {
        var region = CDesktop.LBWindow().LBRegionGroup().LBRegion();
        region.background.GetComponent<SpriteRenderer>().material = Resources.Load<Material>("Shaders/Grayscale");
    }

    //When other region groups are lenghier than the
    //one this region is in then the unique extender will
    //be extended to match length of the other group regions
    public static void SetRegionAsGroupExtender()
    {
        var temp = CDesktop.LBWindow().LBRegionGroup();
        temp.stretchRegion = temp.LBRegion();
    }

    public static void SetRegionBackground(RegionBackgroundType backgroundType)
    {
        CDesktop.LBWindow().LBRegionGroup().LBRegion().backgroundType = backgroundType;
    }

    public static void SetRegionBackgroundAsImage(string replacement)
    {
        SetRegionBackground(Image);
        CDesktop.LBWindow().LBRegionGroup().LBRegion().backgroundImage = Resources.Load<Sprite>("Sprites/RegionReplacements/" + replacement);
    }

    public static void SetRegionBackgroundAsImage(Sprite replacement)
    {
        SetRegionBackground(Image);
        CDesktop.LBWindow().LBRegionGroup().LBRegion().backgroundImage = replacement;
    }

    public static void WriteWrap(Region region, string what, string color = "", string align = "Left")
    {
        if (region.lines.Count == 0 || region.LBLine().Length() + Font.fonts["Tahoma Bold"].Length(what) + 40 > region.regionGroup.setWidth) AddLine(what, color, align);
        else AddText(" " + what, color);
    }

    #endregion

    #region Lines

    public static void AddLine(string text = "", string color = "", string align = "Left")
    {
        var region = CDesktop.LBWindow().LBRegionGroup().LBRegion();
        if (region.lines.Count > 0 && region.smallButtons.Count > 0) return;
        var newObject = new GameObject("Line", typeof(Line));
        newObject.transform.parent = region.transform;
        newObject.GetComponent<Line>().Initialise(region, align);
        AddText(text, color == "" ? DefaultTextColorForRegion(region.backgroundType) : color);
    }

    public static string DefaultTextColorForRegion(RegionBackgroundType type)
    {
        if (type == Header) return "Gray";
        if (type == Padding) return "Gray";
        if (type == Button) return "Black";
        if (type == ButtonRed) return "Black";
        else return "Gray";
    }

    #endregion

    #region SmallButtons

    public static void SetSmallButtonToRed()
    {
        var region = CDesktop.LBWindow().LBRegionGroup().LBRegion();
        var button = region.LBSmallButton().gameObject;
        button.GetComponent<SpriteRenderer>().material = Resources.Load<Material>("Shaders/Red");
    }

    public static void SetSmallButtonToGrayscale()
    {
        var region = CDesktop.LBWindow().LBRegionGroup().LBRegion();
        var button = region.LBSmallButton().gameObject;
        button.GetComponent<SpriteRenderer>().material = Resources.Load<Material>("Shaders/Grayscale");
    }

    public static void AddSmallButtonOverlay(string overlay, float time = 0, int sortingOrder = 0)
    {
        var region = CDesktop.LBWindow().LBRegionGroup().LBRegion();
        var button = region.LBSmallButton().gameObject;
        AddSmallButtonOverlay(button, overlay, time, sortingOrder);
    }

    public static void SmallButtonFlipX()
    {
        var region = CDesktop.LBWindow().LBRegionGroup().LBRegion();
        var button = region.LBSmallButton().gameObject;
        button.GetComponent<SpriteRenderer>().flipX ^= true;
    }

    public static void SmallButtonFlipY()
    {
        var region = CDesktop.LBWindow().LBRegionGroup().LBRegion();
        var button = region.LBSmallButton().gameObject;
        button.GetComponent<SpriteRenderer>().flipY ^= true;
    }

    public static void AddSmallButtonOverlay(GameObject onWhat, string overlay, float time = 0, int sortingOrder = 0)
    {
        var newObject = new GameObject("SmallButtonOverlay", typeof(SpriteRenderer));
        newObject.transform.parent = onWhat.transform;
        newObject.transform.localPosition = new Vector3(overlay == "PlayerLocationFromBelow" || overlay == "PlayerLocationSmall" || overlay == "AvailableQuest" ? 1 : (overlay == "YellowGlowBig" ? 0.5f : 0), overlay == "AvailableQuest" ? -8.5f : (overlay == "PlayerLocationSmall" || overlay == "PlayerLocationFromBelow" ? -6f : (overlay == "YellowGlowBig" ? -0.5f : 0)), -0.01f);
        if (overlay == "Cooldown") newObject.AddComponent<AnimatedSprite>().Initiate("Sprites/Other/Cooldown", true);
        else if (overlay == "YellowGlow") newObject.AddComponent<AnimatedSprite>().Initiate("Sprites/Other/YellowGlow", true);
        else if (overlay == "YellowGlowBig") newObject.AddComponent<AnimatedSprite>().Initiate("Sprites/Other/YellowGlowBig", false, 0.07f);
        else if (overlay == "AutoCast") newObject.AddComponent<AnimatedSprite>().Initiate("Sprites/Other/AutoCastFull", true);
        else if (overlay == "PlayerLocationFromBelow") newObject.AddComponent<AnimatedSprite>().Initiate("Sprites/Other/PlayerLocationFromBelow", false, 0.07f);
        else if (overlay == "PlayerLocationSmall") newObject.AddComponent<AnimatedSprite>().Initiate("Sprites/Other/PlayerLocationSmall", false, 0.07f);
        else if (overlay == "AvailableQuest") newObject.AddComponent<AnimatedSprite>().Initiate("Sprites/Other/AvailableQuest", false, 0.07f);
        else newObject.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Sprites/Buttons/" + overlay);
        newObject.GetComponent<SpriteRenderer>().sortingOrder = sortingOrder;
        newObject.GetComponent<SpriteRenderer>().sortingLayerName = CDesktop.LBWindow().layer;
        if (time > 0)
        {
            newObject.AddComponent<Shatter>().render = newObject.GetComponent<SpriteRenderer>();
            newObject.GetComponent<Shatter>().Initiate(time);
        }
    }

    public static void AddSmallButton(string type, Action<Highlightable> pressEvent = null, Action<Highlightable> rightPressEvent = null, Func<Highlightable, Action> tooltip = null, Action<Highlightable> middlePressEvent = null)
    {
        var region = CDesktop.LBWindow().LBRegionGroup().LBRegion();
        var newObject = new GameObject("SmallButton: " + type, typeof(LineSmallButton), typeof(SpriteRenderer));
        newObject.transform.parent = region.transform;
        newObject.GetComponent<LineSmallButton>().Initialise(region, type);
        if (pressEvent != null || rightPressEvent != null || tooltip != null)
            newObject.AddComponent<Highlightable>().Initialise(region, pressEvent, rightPressEvent, tooltip, middlePressEvent);
    }

    #endregion

    #region BigButtons

    public static void SetBigButtonToRed()
    {
        var region = CDesktop.LBWindow().LBRegionGroup().LBRegion();
        var button = region.LBBigButton().gameObject;
        button.GetComponent<SpriteRenderer>().material = Resources.Load<Material>("Shaders/Red");
    }

    public static void SetBigButtonToGrayscale()
    {
        var region = CDesktop.LBWindow().LBRegionGroup().LBRegion();
        var button = region.LBBigButton().gameObject;
        button.GetComponent<SpriteRenderer>().material = Resources.Load<Material>("Shaders/Grayscale");
    }

    public static GameObject AddBigButtonOverlay(string overlay, float time = 0, int sortingOrder = 0)
    {
        var region = CDesktop.LBWindow().LBRegionGroup().LBRegion();
        var button = region.LBBigButton().gameObject;
        return AddBigButtonOverlay(button, "Sprites/ButtonsBig/" + overlay, time, sortingOrder);
    }

    public static void BigButtonFlipX()
    {
        var region = CDesktop.LBWindow().LBRegionGroup().LBRegion();
        var button = region.LBBigButton().gameObject;
        button.GetComponent<SpriteRenderer>().flipX ^= true;
    }

    public static void BigButtonFlipY()
    {
        var region = CDesktop.LBWindow().LBRegionGroup().LBRegion();
        var button = region.LBBigButton().gameObject;
        button.GetComponent<SpriteRenderer>().flipY ^= true;
    }

    public static GameObject AddBigButtonCooldownOverlay(double percentage)
    {
        var region = CDesktop.LBWindow().LBRegionGroup().LBRegion();
        var button = region.LBBigButton().gameObject;
        var newObject = new GameObject("BigButtonGrid", typeof(SpriteRenderer));
        newObject.transform.parent = button.transform;
        newObject.transform.localPosition = new Vector3(0, 0, -0.01f);
        var sprites = Resources.LoadAll<Sprite>("Sprites/Other/CooldownBig");
        newObject.GetComponent<SpriteRenderer>().sortingLayerName = CDesktop.LBWindow().layer;
        var value = 1.0 / sprites.Length;
        var first = 0;
        for (int i = 0; i < sprites.Length - 1; i++)
            if (percentage > value)
            {
                percentage -= value;
                first++;
            }
        newObject.GetComponent<SpriteRenderer>().color = new Color32(255, 255, 255, 164);
        newObject.GetComponent<SpriteRenderer>().sprite = sprites[Math.Abs(sprites.Length - 1 - first)];
        return newObject;
    }

    public static GameObject AddBigButtonOverlay(GameObject onWhat, string overlay, float time = 0, int sortingOrder = 0)
    {
        var newObject = new GameObject("BigButtonGrid", typeof(SpriteRenderer));
        newObject.transform.parent = onWhat.transform;
        newObject.transform.localPosition = new Vector3(0, 0, -0.01f);
        newObject.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>(overlay);
        newObject.GetComponent<SpriteRenderer>().sortingOrder = sortingOrder;
        newObject.GetComponent<SpriteRenderer>().sortingLayerName = CDesktop.LBWindow().layer;
        if (time > 0)
        {
            newObject.AddComponent<Shatter>().render = newObject.GetComponent<SpriteRenderer>();
            newObject.GetComponent<Shatter>().Initiate(time);
        }
        return newObject;
    }

    public static GameObject AddBigButtonOverlay(Vector2 position, string overlay, float time = 0, int sortingOrder = 0)
    {
        var newObject = new GameObject("BigButtonGrid", typeof(SpriteRenderer));
        newObject.transform.parent = CDesktop.transform;
        newObject.transform.localPosition = new Vector3(position.x, position.y, -0.01f);
        newObject.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Sprites/ButtonsBig/" + overlay);
        newObject.GetComponent<SpriteRenderer>().sortingOrder = sortingOrder;
        newObject.GetComponent<SpriteRenderer>().sortingLayerName = CDesktop.LBWindow().layer;
        if (time > 0)
        {
            newObject.AddComponent<Shatter>().render = newObject.GetComponent<SpriteRenderer>();
            newObject.GetComponent<Shatter>().Initiate(time);
        }
        return newObject;
    }

    public static void AddBigButton(string type, Action<Highlightable> pressEvent = null, Action<Highlightable> rightPressEvent = null, Func<Highlightable, Action> tooltip = null, Action<Highlightable> middlePressEvent = null)
    {
        var region = CDesktop.LBWindow().LBRegionGroup().LBRegion();
        var newObject = new GameObject("BigButton: " + (type == null ? "Empty" : type), typeof(LineBigButton), typeof(SpriteRenderer));
        newObject.transform.parent = region.transform;
        newObject.GetComponent<LineBigButton>().Initialise(region, type);
        if (pressEvent != null || rightPressEvent != null || tooltip != null)
            newObject.AddComponent<Highlightable>().Initialise(region, pressEvent, rightPressEvent, tooltip, middlePressEvent);
    }

    #endregion

    #region Checkboxes

    public static void AddCheckbox(Bool value, List<Bool> referenceList = null)
    {
        var region = CDesktop.LBWindow().LBRegionGroup().LBRegion();
        if (region.checkbox != null) return;
        var newObject = new GameObject("Checkbox", typeof(LineCheckbox), typeof(SpriteRenderer));
        newObject.transform.parent = region.transform;
        newObject.GetComponent<LineCheckbox>().Initialise(region, value, referenceList);
    }

    #endregion

    #region Text

    public static void AddText(string text = "", string color = "")
    {
        text ??= "";
        var newObject = new GameObject("Text", typeof(LineText));
        var line = CDesktop.LBWindow().LBRegionGroup().LBRegion().LBLine();
        newObject.transform.parent = line.transform;
        newObject.GetComponent<LineText>().Initialise(line, text, color == "" ? line.LBText().color : color);
    }

    public static void SpawnFloatingText(Vector2 position, string text = "", string color = "", string align = "Center")
    {
        text ??= "";
        var newObject = new GameObject("FloatingText", typeof(FloatingText));
        newObject.transform.parent = CDesktop.LBWindow().LBRegionGroup().LBRegion().transform;
        newObject.transform.localPosition = position;
        var temp = newObject.GetComponent<FloatingText>();
        temp.Initialise(text, color == "" ? "Gray" : color, align, false);
    }

    public static void SpawnFallingText(Vector2 position, string text = "", string color = "", string align = "Center")
    {
        text ??= "";
        var newObject = new GameObject("FallingText", typeof(FloatingText));
        newObject.transform.parent = CDesktop.transform;
        newObject.transform.localPosition = position;
        newObject.AddComponent<Rigidbody2D>().gravityScale = 2.0f;
        newObject.AddComponent<Shatter>().Initiate(7);
        var temp = newObject.GetComponent<FloatingText>();
        temp.Initialise(text, color == "" ? "Gray" : color, align);
    }

    #endregion

    #region InputLines

    public static void AddInputLine(String refText, string color = "", string align = "Left")
    {
        var region = CDesktop.LBWindow().LBRegionGroup().LBRegion();
        if (region.lines.Count > 0 && region.checkbox != null) return;
        var newObject = new GameObject("InputLine", typeof(InputLine));
        newObject.transform.parent = region.transform;
        newObject.GetComponent<InputLine>().Initialise(region, refText, color, align);
    }

    #endregion

    #region Static Pagination

    //Saved static pagination
    public static Dictionary<string, int> staticPagination;

    public static void PreparePagination(this Window w)
    {
        if (!staticPagination.ContainsKey(w.title))
            staticPagination.Add(w.title, 0);
    }

    public static void CorrectPagination(this Window w)
    {
        var pg = w.pagination();
        var mpg = w.maxPagination();
        if (pg > mpg) staticPagination[w.title] = mpg;
        else if (pg < 0) staticPagination[w.title] = 0;
    }

    public static void SetPagination(this Window w, int to)
    {
        w.PreparePagination();
        staticPagination[w.title] = to;
        w.CorrectPagination();
    }

    public static void IncrementPagination(this Window w)
    {
        w.PreparePagination();
        staticPagination[w.title]++;
        w.CorrectPagination();
    }

    public static void IncrementPaginationEuler(this Window w)
    {
        w.PreparePagination();
        staticPagination[w.title] += (int)Math.Round(EuelerGrowth()) / 2;
        w.CorrectPagination();
    }

    public static void DecrementPagination(this Window w)
    {
        w.PreparePagination();
        staticPagination[w.title]--;
        w.CorrectPagination();
    }

    public static void DecrementPaginationEuler(this Window w)
    {
        w.PreparePagination();
        staticPagination[w.title] -= (int)Math.Round(EuelerGrowth()) / 2;
        w.CorrectPagination();
    }

    #endregion

    #region General

    //Set what highlightible object mouse is currently hovering over
    public static void SetMouseOver(Highlightable highlightable) => mouseOver = highlightable;

    //Euler function
    public static float EuelerGrowth()
    {
        return (float)Math.Pow(keyStack / 150.0 + 1.0, Math.E);
    }

    //Converts a number into the roman notation
    public static string ToRoman(int number)
    {
        if (number < 0 || number > 3999) return "";
        if (number < 1) return string.Empty;
        if (number >= 1000) return "M" + ToRoman(number - 1000);
        if (number >= 900) return "CM" + ToRoman(number - 900);
        if (number >= 500) return "D" + ToRoman(number - 500);
        if (number >= 400) return "CD" + ToRoman(number - 400);
        if (number >= 100) return "C" + ToRoman(number - 100);
        if (number >= 90) return "XC" + ToRoman(number - 90);
        if (number >= 50) return "L" + ToRoman(number - 50);
        if (number >= 40) return "XL" + ToRoman(number - 40);
        if (number >= 10) return "X" + ToRoman(number - 10);
        if (number >= 9) return "IX" + ToRoman(number - 9);
        if (number >= 5) return "V" + ToRoman(number - 5);
        if (number >= 4) return "IV" + ToRoman(number - 4);
        if (number >= 1) return "I" + ToRoman(number - 1);
        return "";
    }

    public static Texture2D LoadImage(string file, bool encoded = false, string prefix = "")
    {
        if (Serialization.useUnityData) prefix = @"C:\Users\ragan\Documents\Projects\Unity\MooRT\";
        if (!Directory.Exists(prefix + "MooRT_Data_3"))
            Directory.CreateDirectory(prefix + "MooRT_Data_3");
        var imagePath = prefix + @"MooRT_Data_3\" + file + (encoded ? "" : ".png");
        if (!File.Exists(imagePath)) return null;
        byte[] byteArray = File.ReadAllBytes(imagePath);
        Texture2D tex = new(1, 1);
        ImageConversion.LoadImage(tex, byteArray);
        tex.filterMode = FilterMode.Point;
        tex.Apply();
        return tex;
    }

    public static IEnumerator GetTexture(string link)
    {
        UnityWebRequest www = UnityWebRequestTexture.GetTexture(link);
        yield return www.SendWebRequest();

        if (www == null || www.result != UnityWebRequest.Result.Success) returnToMenu = true;
        else
        {
            Texture2D tex = DownloadHandlerTexture.GetContent(www);
            scale(tex, 188, 188, FilterMode.Trilinear);
            newCover = Sprite.Create(tex, new Rect(0, 0, 188, 188), new Vector2(0, 1), 1);
            newCover.texture.filterMode = FilterMode.Point;
        }
    }

    public static Texture2D scaled(Texture2D src, int width, int height, FilterMode mode = FilterMode.Bilinear)
    {
        Rect texR = new(0, 0, width, height);
        _gpu_scale(src, width, height, mode);
        Texture2D result = new(width, height, TextureFormat.ARGB32, true);
        result.Reinitialize(width, height);
        result.ReadPixels(texR, 0, 0, true);
        return result;
    }

    public static void scale(Texture2D tex, int width, int height, FilterMode mode = FilterMode.Bilinear)
    {
        Rect texR = new(0, 0, width, height);
        _gpu_scale(tex, width, height, mode);
        tex.Reinitialize(width, height);
        tex.ReadPixels(texR, 0, 0, true);
        tex.Apply(true);
    }

    // Internal unility that renders the source texture into the RTT - the scaling method itself.
    static void _gpu_scale(Texture2D src, int width, int height, FilterMode fmode)
    {
        src.filterMode = fmode;
        src.Apply(true);
        RenderTexture rtt = new(width, height, 32);
        Graphics.SetRenderTarget(rtt);
        GL.LoadPixelMatrix(0, 1, 1, 0);
        GL.Clear(true, true, new Color(0, 0, 0, 0));
        Graphics.DrawTexture(new Rect(0, 0, 1, 1), src);
    }

    #endregion

    #region Enumerations

    public enum InputType
    {
        Everything,
        Letters,
        StrictLetters,
        Capitals,
        Numbers,
        Decimal
    }

    public enum RegionBackgroundType
    {
        Empty,
        Image,
        Padding,
        Header,
        Button,
        ButtonRed,
        Experience,
        ExperienceNone,
        ExperienceRested,
        ProgressDone,
        ProgressEmpty,
        ChartBackground
    }

    public enum Anchor
    {
        None,
        Center,
        Bottom,
        BottomRight,
        BottomLeft,
        Top,
        TopRight,
        TopLeft,
        LeftBottom,
        Left,
        LeftTop,
        RightBottom,
        Right,
        RightTop
    }

    public enum CursorType
    {
        None,
        Default,
        Click,
        Grab,
        Await,
        Write
    }

    #endregion
}
