using System.Collections.Generic;
using System.Linq;

public class Decade
{
    public Decade(int decade, List<Year> years)
    {
        this.decade = decade;
        this.years = years;
        releases = years.SelectMany(x => x.releases).ToList();
    }

    public int decade;

    public List<Year> years;

    public List<MusicRelease> releases;

    public static List<Decade> decades;
}
