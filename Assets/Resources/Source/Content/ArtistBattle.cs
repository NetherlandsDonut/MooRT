using System.Collections.Generic;
using System.Linq;

using static ReleaseRating;

public class ArtistBattle
{
    public ArtistBattle(List<Artist> artists)
    {
        perRound = Root.perRound;
        tracksPerArtist = Root.tracksPerArtist;
        roundAmount = artists.Count * tracksPerArtist / perRound;
        this.artists = artists.ToList();
        this.artists.Shuffle();
        var tracks = new List<(Artist, List<Track>)>();
        foreach (var artist in artists.ToDictionary(x => x, x => x.releases.Where(y => ratings.ContainsKey(y.ID)).ToList()))
        {
            var artistTracks = artist.Value.SelectMany(x => x.tracks.Select(y => (y, ratings[x.ID].savedTrackRatings[x.tracks.IndexOf(y)] + Root.random.Next(-1, 2)))).Where(x => !x.y.excluded).OrderByDescending(x => x.Item2).ToList();
            tracks.Add((artist.Key, artistTracks.Take(tracksPerArtist).Select(x => x.y).ToList()));
        }
        rounds = new();
        for (int i = 0; i < roundAmount; i++)
        {
            tracks.Shuffle();
            tracks = tracks.OrderByDescending(x => x.Item2.Count).ToList();
            var round = new ArtistBattleRound { candidates = new() };
            for (int j = 0; j < perRound; j++)
            {
                var candidate = new ArtistBattleCandidate
                {
                    artistID = tracks[j].Item1.ID,
                    releaseID = tracks[j].Item2[0].albumID,
                    track = tracks[j].Item2[0]
                };
                tracks[j].Item2.RemoveAt(0);
                round.candidates.Add(candidate);
            }
            round.candidates.Shuffle();
            rounds.Add(round);
        }
    }

    //List of all artists
    public List<Artist> artists;

    //All of the rounds of the battle
    public List<ArtistBattleRound> rounds;

    //Amount of rounds
    public int roundAmount;

    //Amount of artists battling per round [2 - 4]
    public int perRound;

    //Amount of artists battling per round [2 - 4]
    public int tracksPerArtist;

    //Current artist battle
    public static ArtistBattle artistBattle;
}

public class ArtistBattleRound
{
    //All candidates in the round
    public List<ArtistBattleCandidate> candidates;

    //Which track has been chosen as the winner of the round
    public int choice;
}

public class ArtistBattleCandidate
{
    //Track of choice
    public Track track;

    //ID of the artist
    public int artistID;
    
    //ID of the release this track is on
    public int releaseID;
}
