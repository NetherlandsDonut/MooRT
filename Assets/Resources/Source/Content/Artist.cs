using Kawazu;

using System;
using System.Threading.Tasks;
using System.Collections.Generic;

public class Artist
{
    //Asigns the formatted name for the album
    public async void AsignFormattedName() => formattedName = await NameFormatted();

    //ID of the artist
    public int ID;

    //Name of the artist
    public string name;

    //Gets the name of the artist in the formatted format if it's available
    public string GetName() => formattedName != "" ? formattedName : name;

    //Formatted name into better use
    [NonSerialized] public string formattedName = "";

    //Returns a formatted name of the artist
    public async Task<string> NameFormatted()
    {
        if (formattedName != "") return formattedName;
        if (string.IsNullOrWhiteSpace(name)) return name;
        else return await Root.kawazuConverter.Convert(name, To.Romaji, Mode.Spaced, RomajiSystem.Hepburn);
    }

    //Pronoun of this artist
    public string pronoun;

    //Country this artist is from
    public string country;

    [NonSerialized] public List<MusicRelease> releases;
}
