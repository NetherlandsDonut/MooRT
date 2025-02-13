using System;
using System.Collections.Generic;

using static Library;

public class Release
{
    public string artist, name, type, releaseDate;
    public bool isSoundtrack, exclusiveTrackLaws;
    public List<string> musicDescriptors, genres, languages;
    public string format, discs;
    public List<string> coverDescriptors;
    public List<Track> tracks, splitTracks;

    [NonSerialized] public int id, decade, year, runningTime, minutes, points, order;
    [NonSerialized] public string duration;
}
