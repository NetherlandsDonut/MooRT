using System;
using System.Linq;
using System.Collections.Generic;

using UnityEngine;

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
        country = artist.country;
        discs ??= "";
        if (artist.releases.Count > 0)
            debutYear = int.Parse(artist.releases.OrderBy(x => x.releaseDate).ToList()[0].releaseDate.Substring(0, 4));
        if (discs.Length > 0)
            discs = string.Join(':', discs.Split(':').Select(x => int.Parse(x)).Where(x => x != 0 && x < tracks.Count).OrderBy(x => x));
        else if (discs == "0") discs = "";
    }

    //ID of this album in the library
    public int ID;

    //Artist performing the album
    public string artist;

    //Artist performing the album
    public int artistID;

    //Name of the album
    public string name;

    //Type of this music release
    public string type;

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

    public static int musicReleaseIndex;

    public static MusicRelease musicRelease;

    //Generates color pallete for the album
    public void GeneratePallete(Sprite sprite)
    {
        var tex = Root.scaled(sprite.texture, 8, 8);
        var colors = new List<Color>();
        for (int i = 0; i < 4; i++)
            for (int j = 0; j < 4; j++)
                colors.Add(tex.GetPixel(i * 2, j * 2));
        pallete = colors.OrderBy(x => x.grayscale).ToList();
    }

    //Clears track ratings and stores them to backup
    public void ClearTrackRatings()
    {
        var releaseRating = ratings[ID];
        releaseRating.trackRatings = new int[tracks.Count];
    }
}
