using System.Collections.Generic;

public class Duration
{
    public Duration(int duration, List<MusicRelease> releases)
    {
        this.duration = duration;
        this.releases = releases;
    }

    public int duration;

    public List<MusicRelease> releases;

    public static List<Duration> durations;
}
