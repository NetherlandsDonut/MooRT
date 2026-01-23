using Kawazu;

using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;

using UnityEngine;

using static Root;
using static ReleaseRating;

public class MusicRelease
{
    public void Initialise(Artist artist)
    {
        var sumLength = 0;
        foreach (var track in tracks)
        {
            track.albumID = ID;
            track.duration = track.length / 60 + (track.length % 60 == 0 ? "m" : "m " + track.length % 60 + "s");
            sumLength += track.length;
        }
        duration = sumLength / 60;
        length = sumLength;
        if (artist != null) country = artist.country;
        discs ??= "";
        if (artist != null)
            if (artist.releases.Count > 0)
                debutYear = int.Parse(artist.releases.OrderBy(x => x.releaseDate).ToList()[0].releaseDate.Substring(0, 4));
        if (discs.Length > 0)
            discs = string.Join(':', discs.Split(':').Select(x => int.Parse(x)).Where(x => x != 0 && x < tracks.Count).OrderBy(x => x));
        else if (discs == "0") discs = "";
        AsignFormattedName();
        artist?.AsignFormattedName();
    }

    public static MusicRelease CreatePreviewRelease()
    {
        var newAlbum = new MusicRelease();
        newAlbum.name = String.createNewAlbumReleaseName.Value().Trim();
        if (newAlbum.name == "") newAlbum.name = "Untitled";
        newAlbum.genres = ProcessGenres(String.createNewAlbumGenres.Value().Trim());
        newAlbum.languages = ProcessLanguages(String.createNewAlbumLanguages.Value().Trim());
        newAlbum.types = createNewAlbumReleaseTypeFiltering.ToList().Where(x => x.Value.Value()).Select(x => x.Key).ToList();
        if (newAlbum.types == null || newAlbum.types.Count == 0) newAlbum.types = new() { "Studio album" };
        var dateTrim = String.createNewAlbumReleaseDate.Value().Trim().Replace("-", ".");
        if (dateTrim.Contains(" "))
        {
            var split = dateTrim.Split(" ").ToList();
            var day = split.Find(x => x.Length <= 2 && x.All(x => char.IsDigit(x)));
            var month = split.Find(x => x.Length >= 3 && x.All(x => !char.IsDigit(x)));
            var year = split.Find(x => x.Length == 4 && x.All(x => char.IsDigit(x)));
            var reverseMonths = monthNames.ToDictionary(x => x.Value, x => x.Key);
            newAlbum.releaseDate = year + "." + reverseMonths[month].ToString("00") + (day != null ? "." + int.Parse(day).ToString("00") : "");
        }
        else newAlbum.releaseDate = dateTrim;
        newAlbum.discs = "";
        newCoverURL = String.createNewAlbumCoverURL.Value().Trim();
        newArtist = null;
        newRelease = newAlbum;
        newRelease.tracks = new();
        if (String.createNewAlbumTracklist.Value().Length > 0)
        {
            var data = String.createNewAlbumTracklist.Value().Replace("\\r", "").Replace("\r", "").Replace("\\n", "\n").Split("\n").ToArray();
            for (int i = 0; i < data.Length; i++)
                if (data[i].Length > 0 && i < data.Length - 1) //If this can be track name
                {
                    if ((i + 2 >= data.Length || !(data[i + 2].Length > 0 && data[i + 2].Split(":").Length == 2 && data[i + 2].Split(":")[1].Length == 2 && data[i + 2].All(x => x == ':' || x == '0' || x == '1' || x == '2' || x == '3' || x == '4' || x == '5' || x == '6' || x == '7' || x == '8' || x == '9'))) && data[i + 1].Length > 0 && data[i + 1].Split(":").Length == 2 && data[i + 1].Split(":")[1].Length == 2 && data[i + 1].All(x => x == ':' || x == '0' || x == '1' || x == '2' || x == '3' || x == '4' || x == '5' || x == '6' || x == '7' || x == '8' || x == '9'))
                    {
                        var newTrack = new Track();
                        newTrack.name = data[i].Trim();
                        if (newTrack.name.EndsWith("lyrics"))
                            newTrack.name = newTrack.name[..^"lyrics".Length];
                        var time = data[i + 1].Trim();
                        if (!int.TryParse(time.Split(":")[0], out int minutes)) { }
                        if (!int.TryParse(time.Split(":")[1], out int seconds)) { }
                        newTrack.length = minutes * 60 + seconds;
                        newTrack.duration = time;
                        newRelease.tracks.Add(newTrack);
                        i++;
                    }
                    else continue;
                }
        }
        var artistName = String.createNewAlbumArtistName.Value().Trim();
        var artistCountry = String.createNewAlbumArtistCountry.Value().Trim();
        artistName = artistName == "" || artistName == "-" ? "various artists" : artistName;
        artistCountry = artistName == "various artists" || artistCountry == "" ? "-" : artistCountry;
        artistFind = Library.library.originalArtists.Find(x => x.name == artistName && (x.country == artistCountry || artistCountry == ""));
        if (artistFind == null)
        {
            artistFind = new Artist()
            {
                ID = Library.library.originalArtists.Count + 1,
                name = artistName,
                pronoun = "they",
                country = artistCountry,
                releases = new()
            };
            newArtist = artistFind;
        }
        newRelease.ID = Library.library.originalReleases.Count + 1;
        newRelease.coverDescriptors = new() { };
        if (newRelease.releaseDate.Length < 4) newRelease.releaseDate = "2000";
        newRelease.format = int.Parse(newRelease.releaseDate[..4]) >= 1990 ? "digital" : "analog";
        newRelease.artist = artistFind.name;
        newRelease.artistID = artistFind.ID;
        newRelease.Initialise(artistFind);
        musicRelease = newRelease;
        return newAlbum;

        List<string> ProcessGenres(string line)
        {
            if (line == "") return new();
            var list = line.Split(",").Select(x => ProcessGenre(x.Trim())).ToList();
            return list;

            string ProcessGenre(string genre)
            {
                var capitalised = string.Join(' ', genre.Split(" ").Select(x => x[..1].ToUpper() + x[1..].ToLower()).ToList());
                return capitalised;
            }
        }

        List<string> ProcessLanguages(string line)
        {
            if (line == "") return new();
            var list = line.Split(",").Select(x => ProcessLanguage(x.Trim())).ToList();
            return list;

            string ProcessLanguage(string language)
            {
                var capitalised = string.Join(' ', language.Split(" ").Select(x => x[..1].ToUpper() + x[1..].ToLower()).ToList());
                return capitalised;
            }
        }
    }

    //Asigns the formatted name for the album
    public async void AsignFormattedName() => formattedName = await NameFormatted();

    //ID of this album in the library
    public int ID;

    //Artist performing the album
    public string artist;

    //ID of the artist performing the album
    public int artistID;

    //Name of the album
    public string name;

    //Gets the name of the album in the formatted format if it's available
    public string GetName() => formattedName != "" ? formattedName : name;

    //Formatted name into better use
    [NonSerialized] public string formattedName = "";

    //Returns a formatted name of the album
    public async Task<string> NameFormatted()
    {
        if (string.IsNullOrWhiteSpace(name)) return name;
        else return await kawazuConverter.Convert(name, To.Romaji, Mode.Spaced, RomajiSystem.Hepburn);
    }

    //Type of this music release
    public List<string> types;

    //Genres of the album
    public List<string> genres;

    //Languages in which vocals are being performed
    public List<string> languages;

    //Was the original release of this album done digitally?
    public string format;

    //Release date of the album in the format of "YYYY.MM.DD"
    public string releaseDate;

    //Tracks of the album
    public List<Track> tracks;

    //Information about splits in album's discs
    public string discs;

    //Words describing the album cover
    public List<string> coverDescriptors;

    //Pallete of the album
    [NonSerialized] public List<Color> pallete;

    //Duration of the album based on sum of track lengths
    [NonSerialized] public int debutYear;

    //Duration of the album based on sum of track lengths
    [NonSerialized] public int duration;

    //Length of the album based on sum of track lengths
    [NonSerialized] public int length;

    //Country of the artist that made this album
    [NonSerialized] public string country;

    //Did user clear the rating?
    [NonSerialized] public bool clearedRating;

    public static int musicReleaseIndex;

    public static MusicRelease musicRelease;

    //Generates color pallete for the album
    public void GeneratePallete(Sprite sprite)
    {
        var tex = scaled(sprite.texture, 8, 8);
        var colors = new List<Color>();
        for (int i = 0; i < 4; i++)
            for (int j = 0; j < 4; j++)
                colors.Add(tex.GetPixel(i * 2, j * 2));
        pallete = colors.OrderBy(x => x.grayscale).ToList();
    }

    //Clears track ratings and stores them to backup
    public void RestoreTrackRatings()
    {
        if (!clearedRating) return;
        clearedRating = false;
        if (!ratings.ContainsKey(ID)) return;
        var releaseRating = ratings[ID];
        releaseRating.trackRatings = releaseRating.savedTrackRatings.ToArray();
    }

    //Restore track ratings
    public void ClearTrackRatings()
    {
        if (!ratings.ContainsKey(ID)) return;
        clearedRating = true;
        var releaseRating = ratings[ID];
        releaseRating.trackRatings = new int[tracks.Count];
    }

    //Gets the album rating
    public int GetRating()
    {
        if (!ratings.ContainsKey(ID)) return 0;
        return ratings[ID].rating;
    }
}
