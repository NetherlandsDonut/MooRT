using System.Collections.Generic;

public class ReleaseType
{
    public ReleaseType(string name, List<MusicRelease> releases)
    {
        this.name = name;
        this.releases = releases;
    }

    public string name;

    public List<MusicRelease> releases;

    public static List<ReleaseType> releaseTypes;
}
