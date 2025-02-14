using System;
using System.Linq;
using System.Collections.Generic;

using static Library;

public class ReleaseRating
{
    public ReleaseRating() { }
    public ReleaseRating(MusicRelease release)
    {
        ID = release.ID;
        trackRatings = new int[release.tracks.Count];
        UpdateRating();
    }

    //ID of the album that this rating belongs to
    public int ID;

    //Rating points
    public int rating;

    //Points as this release appears in lists
    public int listPoints;

    //Date of last rating
    public DateTime date;

    //Ratings of the tracks
    [NonSerialized] public int[] trackRatings;

    //Backup ratings of the tracks
    public int[] savedTrackRatings;

    //Update the album's rating based on track ratings
    public void UpdateRating()
    {
        if (rating == 0) savedTrackRatings = trackRatings.ToArray();
        if (trackRatings.Any(x => x == 0)) return;
        date = DateTime.Now;
        var doubleRating = 0.0;
        var doublePoints = 0.0;
        for (int i = 0; i < trackRatings.Length; i++)
        {
            var a = 1.0 / (possibleRatings.Length - 1);
            var b = library.originalReleases[ID - 1].tracks[i].length;
            doubleRating += 99999.0 / library.originalReleases[ID - 1].length * a * trackRatings[i] * b;
            if (!library.originalReleases[ID - 1].tracks[i].excluded)
                doublePoints += 99999.0 / library.originalReleases[ID - 1].length * a * trackRatings[i] * b;
        }
        savedTrackRatings = trackRatings.ToArray();
        rating = (int)doubleRating;
        doublePoints /= 400;
        listPoints = (int)(doublePoints * doublePoints * doublePoints / 17000);
    }

    //List of all ratings provided by user
    public static Dictionary<int, ReleaseRating> ratings;

    public static (string, string)[] possibleRatings = new (string, string)[]
    {
        ("?", "DimGray"),
        ("E", "DarkGray"),
        ("E+", "DarkGray"),
        ("D", "Gray"),
        ("D+", "Gray"),
        ("C", "Uncommon"),
        ("C+", "Uncommon"),
        ("B", "Rare"),
        ("B+", "Rare"),
        ("A", "Epic"),
        ("A+", "Epic"),
        ("S", "Legendary"),
        ("S+", "Legendary"),
    };
}
