using System.Collections.Generic;
using UnityEngine;

public class Language
{
    public Language(string name, List<MusicRelease> releases)
    {
        this.name = name;
        this.releases = releases;
    }

    public string name;

    public List<MusicRelease> releases;

    public static List<Language> languages;
}
