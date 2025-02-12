using System.Collections.Generic;

public class TrackAmount
{
    public TrackAmount(int amount, List<MusicRelease> releases)
    {
        this.amount = amount;
        this.releases = releases;
    }

    public int amount;

    public List<MusicRelease> releases;

    public static List<TrackAmount> trackAmounts;
}
