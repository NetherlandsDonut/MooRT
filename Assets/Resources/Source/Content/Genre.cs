using System.Collections.Generic;

public class Genre
{
    public Genre(string name, List<MusicRelease> releases)
    {
        this.name = name;
        this.releases = releases;
    }

    public string name;

    public List<MusicRelease> releases;

    public static List<Genre> genres;
}
