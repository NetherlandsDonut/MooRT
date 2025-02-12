using System;

public class Track
{
    //Name of the track
    public string name;

    //Length of the track in seconds
    public int length;

    //Is this track excluded from point calculation for lists
    public bool excluded;

    //Duration of the track in the format of "MM:SS"
    [NonSerialized] public string duration;

    //ID of the album that this track comes from
    [NonSerialized] public int albumID;
}
