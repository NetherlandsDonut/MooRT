using System.Collections.Generic;

public class Year
{
    public Year(int year, List<MusicRelease> releases)
    {
        this.year = year;
        this.releases = releases;
    }

    public int year;

    public List<MusicRelease> releases;

    public static List<Year> years;
}
