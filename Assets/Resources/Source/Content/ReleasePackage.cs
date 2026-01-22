
public class ReleasePackage
{
    public ReleasePackage(MusicRelease musicRelease, string artistName, string artistCountry, string coverURL)
    {
        this.musicRelease = musicRelease;
        this.artistName = artistName;
        this.artistCountry = artistCountry;
        this.coverURL = coverURL;
    }

    public MusicRelease musicRelease;

    public string artistName;

    public string artistCountry;

    public string coverURL;
}
