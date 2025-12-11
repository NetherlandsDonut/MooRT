using System.Linq;

using UnityEditor;
using UnityEngine;

using static Root;
using static Font;
using static Cursor;
using static Library;
using static Defines;
using static ReleaseRating;
using static Serialization;
using static ProgramSettings;

public class Starter : MonoBehaviour
{
    void Start()
    {
        //Sets the initial values for the base variables
        //of the program such as list of desktops or cursor handle
        #region Initial Variables

        //This variable stores random number generator for
        //things such as damage / heal rolls or chance for effects to happen
        random = new System.Random();

        //Initialise storage for pagination
        staticPagination = new();

        //This is the font that will be used
        //by the game's UI system and is the basis of the program
        fonts = new()
        {
            { "Tahoma Bold", new Font("Tahoma Bold", "!\"#$%&\'()*+,-./0123456789:;<=>?@ABCDEFGHIJKLMNOPQRSTUVWXYZ[\\]^_`abcdefghijklmnopqrstuvwxyz{|}~¡¢£¤¥¦§¨©ª«¬®¯°±²³´µ¶·¸¹º»¼½¾¿ÀÁÂÃÄÅÆÇÈÉÊËÌÍÎÏÐÑÒÓÔÕÖ×ØÙÚÛÜÝÞßàáâãäåæçèéêëìíîïðñòóôõö÷ø ùúûüýþÿĀāĂăĄąĆćĈĉĊċČčĎďĐđĒēĔĕĖėĘęĚěĜĝĞğĠġĢģĤĥĦħĨĩĪīĬĭĮįİıĲĳĴĵĶķĸĹĺĻļĽľĿŀŁłŃńŅņŇňŉŊŋŌōŎŏŐőŒœŔŕŖŗŘřŚśŜŝŞşŠšŢţŤťŦŧŨũŪūŬŭŮůŰűŲųŴŵŶŷŸŹźŻżŽžſƁƂƃƄƅƆƇƈƊƋƌƍƎƏƐƑƒƓƔƕƖƗƘƙƚƛƜƝƞƟƠơƢƣƤƥƦƧƨƩƪƫƬƭƮƯưƱƲƳƴƵƶƷƸƹƺƻƼƽƾƿǀǁǂǃǄǅǆǇǈǉǊǋǌǍǎǏǑǒǓǔǕǖǗǘǙǚǛǜǝǞǟǠǡǢǣǤǥǦǧǨǩǪǫǬǭǮǯǰǱǲǳǴǵǶǷǸǹǺǻǼǽǾǿȀȁȂȃȄȅȆȇȈȉȊȋȒȓȔȕȖȗȘșȚțȜȝȞȟȠȡȢȣȤȥȦȧȨȩȪȫȬȭȮȯȰȱȲȳȴȵȶȷȸȹȺȻȼȽȾȿɀɁɂɃɄɅȌȍȎȏȐȑɆɇɈɉɊɋɌɍɎɏɐɑɒɓɔɕɖɗɘəɚɛɜɝɞɟɠɡɢɣɤɥɦɧɨɩɪɫɬɭɮɯɰɱɲɳɴɵɶɷɸɹɺɻɼɽɾʀʁʂʃʄʅʆʇʈʉʊʋʌʍʎʏʐʑʒʓʔʕʖʗʘʙʚʛʜʝʞʟʠʡʢʬʭʮʯʰˆˇˉ˘˙˚˛˜˝;΄΅Ά·ΈΉΊΌΎΏΐΑΒΓΔΕΖΗΘΙΚΛΜΝΞΟΠΡΣΤΥΦΧΨΩΪΫάέήίΰαβγδεζηθικλμνξοπρςστυφχψωϊϋόύώЀЁЂЃЄЅІЇЈЉЊЋЌЍЎЏАБВГДЕЖЗИЙКЛМНОПРСТУФХЦЧШЩЪЫЬЭЮЯабвгдежзийклмнопрстуфхцчшщъыьэюяѐёђѓєѕіїјљњћќѝўџҐґҒғҖҗҚқҜҝҢңҮүҰұҲҳҸҹҺһӘә׃אבגדהוזחטיךכלםמןנסעףפץצקרשתװױײ׳״´῾–—―‗‘‚‛“„†‡•…‰′″‹›‼‾⁄ⁿ₣₤₥₦₧₨₩₪₫€₭₮₯℅ℓ№™Ω℮⅛⅜⅝⅞∂∆∏∑−∕∙√∞∫≈≠≤≥□▪▫◊●◦ﬁﬂ’”ƀǐɿ") }
        };

        //List of active desktops
        //Thanks to this list the user can switch between screens while 
        //retaining their data as the desktops are disabled only temporarily
        //unless you call CloseDesktop() which will remove it from the list and wipe it's data
        desktops = new();

        //List of in-game settings that describe visual style of the game,
        //control how the audio works and other things for personalisation and user info storage
        settings = new ProgramSettings();

        //This is the player cursor that follows the hidden system cursor
        cursor = FindAnyObjectByType<Cursor>();

        //In case of Unity debugging set data directory
        //to that of the build so we don't have to store game data in two places
        #if (UNITY_EDITOR)
        prefix = "D:/Programs/MooRT/";
        #endif

        buildingSprites = Resources.LoadAll<Sprite>("Sprites/Other/Second").ToDictionary(x => x.name, x => x);
        buildingSprites.Add("RegionBorder", Resources.Load<Sprite>("Sprites/PremadeBorders/RegionBorder"));
        buildingSprites.Add("RegionBorderCorner", Resources.Load<Sprite>("Sprites/PremadeBorders/RegionBorderCorner"));
        buildingSprites.Add("RegionBorderCornerTabbing", Resources.Load<Sprite>("Sprites/PremadeBorders/RegionBorderCornerTabbing"));

        #endregion

        //Gets the user characters and settings into the game
        #region User Data Deserialization

        //Get all the library going on..
        Deserialize(ref ratings, "ratings", false, prefix);
        ratings ??= new();
        foreach (var rating in ratings)
            if (rating.Value.savedTrackRatings != null)
                rating.Value.trackRatings = rating.Value.savedTrackRatings.ToArray();
        ratings = ratings.Where(x => x.Value.savedTrackRatings != null).ToDictionary(x => x.Key, x => x.Value);
        if (useUnityData) urlContent = "x";
        else StartCoroutine(GetJSON("https://raw.githubusercontent.com/NetherlandsDonut/MooRT/refs/heads/main/MooRT_Data_2/library.json"));
        enteredSecondStage = true;
    }

    public static bool enteredSecondStage = false;
    public static bool enteredThirdStage = false;

    public void Update()
    {
        if (enteredSecondStage && urlContent != "" && !enteredThirdStage)
        {
            enteredThirdStage = true;

            if (urlContent != "x") DeserializeFromURL(ref library, false);
            if (library == null) Deserialize(ref library, "library", false, prefix);
            else Serialize(library, "library");

            library ??= new();

            SetUpLibrary();

            //Get user settings..
            Deserialize(ref settings, "settings", false, prefix);
            settings ??= new();
            settings.FillNulls();

            #endregion

            //Loads the game content data from the game directory
            LoadData();

            //Spawn the initial desktop so the user can perform all actions from there
            SpawnDesktopBlueprint("LoadingScreen");

            ////Destroy this object as it's only used for program initialization
            //Destroy(gameObject);
        }
    }

    public static void SetUpLibrary()
    {
        library.originalArtists.ForEach(x => x.releases = library.originalReleases.Where(y => y.artistID == x.ID).ToList());
        library.originalArtists.ForEach(x => x.releases.ForEach(y => y.Initialise(x)));
        Country.countries = library.originalArtists.GroupBy(x => x.country).Select(x => new Country(x.Key, x.ToList())).ToList();
        Year.years = library.originalReleases.GroupBy(x => x.releaseDate.Substring(0, 4)).Select(x => new Year(int.Parse(x.Key), x.ToList())).ToList();
        Decade.decades = Year.years.GroupBy(x => x.year.ToString().Substring(0, 3)).Select(x => new Decade(int.Parse(x.Key + "0"), x.ToList())).ToList();
        var allLanguages = library.originalReleases.SelectMany(x => x.languages).Distinct();
        Language.languages = allLanguages.Select(x => new Language(x, library.originalReleases.Where(y => y.languages.Contains(x)).ToList())).ToList();
        var allGenres = library.originalReleases.SelectMany(x => x.genres).Distinct();
        Genre.genres = allGenres.Select(x => new Genre(x, library.originalReleases.Where(y => y.genres.Contains(x)).ToList())).ToList();
        TrackAmount.trackAmounts = library.originalReleases.GroupBy(x => x.tracks.Count).Select(x => new TrackAmount(x.Key, x.ToList())).ToList();
        var allReleaseTypes = library.originalReleases.SelectMany(x => x.types).Distinct();
        ReleaseType.releaseTypes = allReleaseTypes.Select(x => new ReleaseType(x, library.originalReleases.Where(y => y.types.Contains(x)).ToList())).ToList();
        var debuts = library.originalArtists.Where(x => x.releases.Count > 0).Select(x => x.releases.OrderBy(x => x.releaseDate).First());
        DebutYear.debutYears = debuts.GroupBy(x => x.releaseDate.Substring(0, 4)).Select(x => new DebutYear(int.Parse(x.Key), x.ToList())).ToList();
        Duration.durations = library.originalReleases.GroupBy(x => x.length / 60).Select(x => new Duration(x.Key, x.ToList())).ToList();
        library.ResetLibrary();
    }

    private void OnApplicationQuit()
    {
        OnQuit();
    }

    private void OnApplicationFocus(bool hasFocus)
    {
        Root.hasFocus = hasFocus;
        if (!hasFocus)
        {
            if (library != null && library.originalReleases.Count > 0 && library.originalArtists.Count > 0)
                Serialize(library, "library");
            if (ratings != null && ratings.Count > 0)
                Serialize(ratings, "ratings");
        }
    }

    private void OnQuit()
    {
        if (library != null && library.originalReleases.Count > 0 && library.originalArtists.Count > 0)
            Serialize(library, "library", true);
        if (ratings != null && ratings.Count > 0)
            Serialize(ratings, "ratings", true);
    }

    public static void LoadData()
    {
        //This region is responsible for deserializing the game content
        //into the game. By game content I mean specs, abilities, instances etc
        #region Data Deserialization

        Deserialize(ref defines, "defines", false, prefix);
        defines ??= new();

        albumCovers = new();
        albumBars = new();

        #endregion
    }
}
