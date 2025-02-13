using System;
using System.Linq;
using System.Collections.Generic;

using static Root;
using static Year;
using static Genre;
using static Decade;
using static Country;
using static Language;
using static Duration;
using static DebutYear;
using static ReleaseType;
using static TrackAmount;
using static ReleaseRating;

public class Library
{
    public Library()
    {
        originalArtists ??= new();
        originalReleases ??= new();
    }
    
    public void ResetLibrary(bool resetFilters = true)
    {
        artists = originalArtists.OrderBy(x => x.name).OrderByDescending(y => y.releases.Sum(x => ratings.ContainsKey(x.ID) ? ratings[x.ID].rating : 0)).ToList();
        releases = originalReleases.OrderBy(x => x.name).OrderByDescending(x => ratings.ContainsKey(x.ID) ? ratings[x.ID].rating : 0).ToList();
        countries = countries.OrderBy(x => x.name).OrderByDescending(z => z.artists.Sum(y => y.releases.Sum(x => ratings.ContainsKey(x.ID) ? ratings[x.ID].rating : 0))).ToList();
        years = years.OrderBy(x => x.year).OrderByDescending(y => y.releases.Sum(x => ratings.ContainsKey(x.ID) ? ratings[x.ID].rating : 0)).ToList();
        decades = decades.OrderBy(x => x.decade).OrderByDescending(y => y.releases.Sum(x => ratings.ContainsKey(x.ID) ? ratings[x.ID].rating : 0)).ToList();
        languages = languages.OrderBy(x => x.name).OrderByDescending(y => y.releases.Sum(x => ratings.ContainsKey(x.ID) ? ratings[x.ID].rating : 0)).ToList();
        genres = genres.OrderBy(x => x.name).OrderByDescending(y => y.releases.Sum(x => ratings.ContainsKey(x.ID) ? ratings[x.ID].rating : 0)).ToList();
        trackAmounts = trackAmounts.OrderBy(x => x.amount).OrderByDescending(y => y.releases.Sum(x => ratings.ContainsKey(x.ID) ? ratings[x.ID].rating : 0)).ToList();
        releaseTypes = releaseTypes.OrderBy(x => x.name).OrderByDescending(y => y.releases.Sum(x => ratings.ContainsKey(x.ID) ? ratings[x.ID].rating : 0)).ToList();
        debutYears = debutYears.OrderBy(x => x.year).OrderByDescending(y => y.releases.Sum(x => ratings.ContainsKey(x.ID) ? ratings[x.ID].rating : 0)).ToList();
        durations = durations.OrderBy(x => x.duration).OrderByDescending(y => y.releases.Sum(x => ratings.ContainsKey(x.ID) ? ratings[x.ID].rating : 0)).ToList();
        if (!resetFilters) return;
        artistFiltering = originalArtists.ToDictionary(x => x.ID, x => new Bool(true));
        countryFiltering = countries.ToDictionary(x => x.name, x => new Bool(true));
        yearFiltering = years.ToDictionary(x => x.year, x => new Bool(true));
        decadeFiltering = decades.ToDictionary(x => x.decade, x => new Bool(true));
        languageFiltering = languages.ToDictionary(x => x.name, x => new Bool(true));
        genreFiltering = genres.ToDictionary(x => x.name, x => new Bool(true));
        trackAmountFiltering = trackAmounts.ToDictionary(x => x.amount, x => new Bool(true));
        releaseTypeFiltering =  releaseTypes.ToDictionary(x => x.name, x => new Bool(true));
        debutYearFiltering = debutYears.ToDictionary(x => x.year, x => new Bool(true));
        durationFiltering = durations.ToDictionary(x => x.duration, x => new Bool(true));
    }

    public void ApplyFiltering()
    {
        ResetLibrary(false);
        releases = releases.Where(x => countryFiltering[x.country].Value()).ToList();
        releases = releases.Where(x => artistFiltering[x.artistID].Value()).ToList();
        releases = releases.Where(x => yearFiltering[int.Parse(x.releaseDate.Substring(0, 4))].Value()).ToList();
        releases = releases.Where(x => decadeFiltering[int.Parse(x.releaseDate.Substring(0, 3) + "0")].Value()).ToList();
        releases = releases.Where(x => durationFiltering[x.duration].Value()).ToList();
        releases = releases.Where(x => x.types.Any(y => releaseTypeFiltering[y].Value())).ToList();
        releases = releases.Where(x => (requireAllSelectedGenres.Value() && genreFiltering.Where(x => x.Value.Value()).All(y => x.genres.Contains(y.Key))) || (!requireAllSelectedGenres.Value() && (x.genres.Count == 0 || x.genres.Any(y => genreFiltering[y].Value())))).ToList();
        releases = releases.Where(x => (requireAllSelectedLanguages.Value() && languageFiltering.Where(x => x.Value.Value()).All(y => x.languages.Contains(y.Key))) || (!requireAllSelectedLanguages.Value() && (x.languages.Count == 0 || x.languages.Any(y => languageFiltering[y].Value())))).ToList();
        releases = releases.Where(x => trackAmountFiltering[x.tracks.Count].Value()).ToList();
        releases = releases.Where(x => debutYearFiltering[x.debutYear].Value()).ToList();
    }

    //List of all artists in the library
    public List<Artist> originalArtists;
    [NonSerialized] public List<Artist> artists;

    //List of all music releases in the library
    public List<MusicRelease> originalReleases;
    [NonSerialized] public List<MusicRelease> releases;

    //Application's library of music
    public static Library library;
}
