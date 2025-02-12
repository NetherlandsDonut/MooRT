using System;
using System.Collections.Generic;

public class Artist
{
    //ID of the artist
    public int ID;

    //Name of the artist
    public string name;

    //Pronoun of this artist
    public string pronoun;

    //Country this artist is from
    public string country;

    [NonSerialized] public List<MusicRelease> releases;
}
