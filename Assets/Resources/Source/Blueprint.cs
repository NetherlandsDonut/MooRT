using Kawazu;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Mail;
using System.Net.Security;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using UnityEditor;
using UnityEditor.VersionControl;
using UnityEngine;
using static ArtistBattle;
using static Country;
using static DebutYear;
using static Decade;
using static Duration;
using static Genre;
using static Language;
using static Library;
using static MusicRelease;
using static ProgramSettings;
using static RatingStatus;
using static ReleaseRating;
using static ReleaseType;
using static Root;
using static Root.Anchor;
using static TrackAmount;
using static UnityEngine.KeyCode;
using static Year;

public class Blueprint
{
    public Blueprint(string title, Action actions, bool upperUI = false)
    {
        this.title = title;
        this.actions = actions;
        this.upperUI = upperUI;
    }

    public string title;
    public Action actions;
    public bool upperUI;

    public static List<Blueprint> windowBlueprints = new()
    {
        //Loadings
        new("LoadingStatus", () => {
            SetAnchor(Bottom, 0, 19);
            AddHeaderGroup();
            SetRegionGroupWidth(300);
            AddPaddingRegion(() => AddLine(""));
        }),

        //Music releases
        new("MusicReleases", () => {
            var rowAmount = 20;
            var thisWindow = CDesktop.LBWindow();
            var list = library.releases;
            if (String.searchRelease.Value() != "")
                list = list.Where(x => x.name.ToLower().Contains(String.searchRelease.Value().ToLower())).ToList();
            thisWindow.SetPaginationSingleStep(() => list.Count, rowAmount);
            CDesktop.quickInputWindow = thisWindow;
            SetAnchor(Center, -8, 10);
            AddHeaderGroup();
            SetRegionGroupWidth(445);
            AddPaddingRegion(() => { AddLine("Search:", "DarkGray"); AddInputLine(String.searchRelease); AddSmallButton("OtherReverse", (h) => { String.searchRelease.Set(""); CDesktop.RespawnAll(); }); });
            AddRegionGroup();
            SetRegionGroupWidth(46);
            AddButtonRegion(() => AddLine("#", "", "Right"),
                (h) =>
                {
                    library.releases.Reverse();
                }
            );
            for (int i = thisWindow.pagination() == 0 ? 0 : list.Count - thisWindow.pagination() < rowAmount ? list.Count - (thisWindow.pagination() + 1) : 0; i < rowAmount; i++)
            {
                var index = i;
                if (list.Count > index + thisWindow.pagination())
                    AddHeaderRegion(() => AddLine(1 + index + thisWindow.pagination() + "", "", "Right"));
                else AddPaddingRegion(() => AddLine("", "", "Center"));
            }
            AddPaddingRegion(() => AddLine(""));
            AddRegionGroup();
            SetRegionGroupWidth(190);
            AddButtonRegion(() => AddLine("Name"),
                (h) =>
                {
                    library.releases = (releasesLastSort == "Name" ? list.OrderByDescending(x => x.name) : list.OrderBy(x => x.name)).ToList();
                    releasesLastSort = releasesLastSort == "Name" ? "" : "Name";
                }
            );
            for (int i = thisWindow.pagination() == 0 ? 0 : list.Count - thisWindow.pagination() < rowAmount ? list.Count - (thisWindow.pagination() + 1) : 0; i < rowAmount; i++)
            {
                var index = i;
                if (list.Count > index + thisWindow.pagination())
                    AddButtonRegion(() =>
                    {
                        var album = list[index + thisWindow.pagination()];
                        AddRegionOverlay(@"RegionReplacements\AlbumNameBar");
                        SetRegionBackgroundAsImage(albumBars[(albumBars.ContainsKey(album.ID + "") ? album.ID : 0) + ""]);
                        AddLine(album.name, "Black");
                    },
                    (h) =>
                    {
                        musicReleaseIndex = index + thisWindow.pagination();
                        musicRelease = list[musicReleaseIndex];
                        SpawnDesktopBlueprint("MusicRelease");
                    },
                    null,
                    (h) => () =>
                    {
                        var foo = list[index + thisWindow.pagination()];
                        SetAnchor(-440, 228);
                        AddRegionGroup();
                        SetRegionGroupWidth(190);
                        SetRegionGroupHeight(186);
                        AddPaddingRegion(() =>
                        {
                            if (albumCovers.ContainsKey(foo.ID + ""))
                            {
                                SetRegionBackgroundAsImage(albumCovers[foo.ID + ""]);
                                albumCovers[foo.ID + ""].texture.filterMode = FilterMode.Point;
                            }
                            else SetRegionBackgroundAsImage(albumCovers["0"]);
                            SetRegionAsGroupExtender();
                        });
                    });
                else AddPaddingRegion(() => AddLine("", "", "Center"));
            }
            AddPaddingRegion(() => AddLine(list.Count + " out of " + library.originalReleases.Count + " releases", "DarkGray"));
            AddRegionGroup();
            SetRegionGroupWidth(46);
            AddButtonRegion(() => AddLine("Rating"),
                (h) =>
                {
                    library.releases = (releasesLastSort == "Rating" ? list.OrderBy(x => ratings.ContainsKey(x.ID) ? ratings[x.ID].rating : 0) : list.OrderByDescending(x => ratings.ContainsKey(x.ID) ? ratings[x.ID].rating : 0)).ToList();
                    releasesLastSort = releasesLastSort == "Rating" ? "" : "Rating";
                }
            );
            for (int i = thisWindow.pagination() == 0 ? 0 : list.Count - thisWindow.pagination() < rowAmount ? list.Count - (thisWindow.pagination() + 1) : 0; i < rowAmount; i++)
            {
                var index = i;
                if (list.Count > index + thisWindow.pagination())
                {
                    var album = list[index + thisWindow.pagination()];
                    var amount = !ratings.ContainsKey(album.ID) ? 0 : Math.Ceiling(ratings[album.ID].rating / 100.0);
                    AddHeaderRegion(() => AddLine(amount.ToString("000"), settings.ratingRanges.First(x => int.Parse(x.min.Value()) <= amount).GetColorCode()));
                }
                else AddPaddingRegion(() => AddLine("", "", "Center"));
            }
            AddPaddingRegion(() => AddLine(""));
            AddRegionGroup();
            SetRegionGroupWidth(39);
            AddButtonRegion(() => AddLine("Year", "", "Center"),
                (h) =>
                {
                    library.releases = (releasesLastSort == "Year" ? list.OrderBy(x => x.releaseDate) : list.OrderByDescending(x => x.releaseDate)).ToList();
                    releasesLastSort = releasesLastSort == "Year" ? "" : "Year";
                }
            );
            for (int i = thisWindow.pagination() == 0 ? 0 : list.Count - thisWindow.pagination() < rowAmount ? list.Count - (thisWindow.pagination() + 1) : 0; i < rowAmount; i++)
            {
                var index = i;
                if (list.Count > index + thisWindow.pagination())
                    AddHeaderRegion(() =>
                    {
                        var album = list[index + thisWindow.pagination()];
                        AddLine(album.releaseDate.Substring(0, 4), "", "Center");
                    });
                else AddPaddingRegion(() => AddLine("", "", "Center"));
            }
            AddPaddingRegion(() => AddLine(""));
            AddRegionGroup();
            SetRegionGroupWidth(58);
            AddButtonRegion(() => AddLine("Duration"),
                (h) =>
                {
                    library.releases = (releasesLastSort == "Duration" ? list.OrderBy(x => x.length) : list.OrderByDescending(x => x.length)).ToList();
                    releasesLastSort = releasesLastSort == "Duration" ? "" : "Duration";
                }
            );
            for (int i = thisWindow.pagination() == 0 ? 0 : list.Count - thisWindow.pagination() < rowAmount ? list.Count - (thisWindow.pagination() + 1) : 0; i < rowAmount; i++)
            {
                var index = i;
                if (list.Count > index + thisWindow.pagination())
                    AddHeaderRegion(() =>
                    {
                        var album = list[index + thisWindow.pagination()];
                        AddLine(album.duration + "m", "", "Right");
                    });
                else AddPaddingRegion(() => AddLine("", "", "Center"));
            }
            AddPaddingRegion(() => AddLine(""));
            AddRegionGroup();
            SetRegionGroupWidth(47);
            AddButtonRegion(() => AddLine("Tracks"),
                (h) =>
                {
                    library.releases = (releasesLastSort == "Tracks" ? list.OrderBy(x => x.tracks.Count) : list.OrderByDescending(x => x.tracks.Count)).ToList();
                    releasesLastSort = releasesLastSort == "Tracks" ? "" : "Tracks";
                }
            );
            for (int i = thisWindow.pagination() == 0 ? 0 : list.Count - thisWindow.pagination() < rowAmount ? list.Count - (thisWindow.pagination() + 1) : 0; i < rowAmount; i++)
            {
                var index = i;
                if (list.Count > index + thisWindow.pagination())
                    AddHeaderRegion(() =>
                    {
                        var album = list[index + thisWindow.pagination()];
                        AddLine(album.tracks.Count + "", "", "Right");
                    });
                else AddPaddingRegion(() => AddLine("", "", "Center"));
            }
            AddPaddingRegion(() => AddLine(""));
        }),
        new("MusicReleasesScrollbarUp", () => {
            SetAnchor(195, 209);
            AddRegionGroup();
            SetRegionGroupWidth(19);
            AddPaddingRegion(() =>
            {
                var window = CDesktop.windows.Find(x => x.title == "MusicReleases");
                if (window.pagination() > 0)
                    AddSmallButton("OtherPageUp", (h) =>
                    {
                        window.DecrementPagination();
                        CDesktop.RespawnAll();
                        Respawn("MusicReleasesScrollbarUp", true);
                        Respawn("MusicReleasesScrollbar", true);
                        Respawn("MusicReleasesScrollbarDown", true);
                    });
                else AddSmallButton("OtherPageUpOff");
            });
        }),
        new("MusicReleasesScrollbar", () => {
            SetAnchor(195, 190);
            AddRegionGroup();
            SetRegionGroupWidth(19);
            SetRegionGroupHeight(376);
            AddPaddingRegion(() => AddLine(""));
        }),
        new("MusicReleasesScrollbarDown", () => {
            SetAnchor(195, -190);
            AddRegionGroup();
            SetRegionGroupWidth(19);
            AddPaddingRegion(() =>
            {
                var window = CDesktop.windows.Find(x => x.title == "MusicReleases");
                if (window.pagination() < window.maxPagination())
                    AddSmallButton("OtherPageDown", (h) =>
                    {
                        window.IncrementPagination();
                        CDesktop.RespawnAll();
                        Respawn("MusicReleasesScrollbarUp", true);
                        Respawn("MusicReleasesScrollbar", true);
                        Respawn("MusicReleasesScrollbarDown", true);
                    });
                else AddSmallButton("OtherPageDownOff");
            });
        }),
        new("ResetLibraryFiltering", () => {
            SetAnchor(TopRight, CDesktop.title == "MusicReleases" ? -38 : -19, -19);
            AddRegionGroup();
            AddPaddingRegion(() => AddSmallButton("OtherReverse", (h) =>
            {
                library.ResetLibrary();
                CDesktop.RespawnAll();
            }));
        }),
        new("RollRandomRelease", () => {
            SetAnchor(TopRight, -19, -19);
            AddRegionGroup();
            AddPaddingRegion(() => AddSmallButton(library.releases.Count == 0 ? "OtherRandomOff" : "OtherRandom", (h) =>
            {
                var list = library.releases;
                if (String.searchRelease.Value() != "")
                    list = list.Where(x => x.name.ToLower().Contains(String.searchRelease.Value().ToLower())).ToList();
                var randomIndex = random.Next(list.Count);
                musicReleaseIndex = randomIndex;
                musicRelease = list[randomIndex];
                if (CDesktop.title == "MusicRelease")
                {
                    CDesktop.RespawnAll();
                    Respawn("MusicReleaseScrollbarUp", true);
                    Respawn("MusicReleaseScrollbar", true);
                    Respawn("MusicReleaseScrollbarDown", true);
                    SpawnAlbumTransition();
                    if (albumCovers.ContainsKey(musicRelease.ID + ""))
                    {
                        if (musicRelease.pallete == null)
                            musicRelease.GeneratePallete(albumCovers[musicRelease.ID + ""]);
                        SetDesktopBackgroundAsGradient(musicRelease.pallete);
                    }
                }
                else SpawnDesktopBlueprint("MusicRelease");
            }));
        }),
        new("CreateNewAlbumClose", () => {
            SetAnchor(TopRight, -19, -19);
            AddRegionGroup();
            AddPaddingRegion(() => AddSmallButton("OtherClose", (h) =>
            {
                CloseDesktop(CDesktop.title);
                SpawnDesktopBlueprint("Menu");
            }));
        }),

        //Music release
        new("MusicRelease", () => {
            var rating = Root.rating.Value();
            SetAnchor(rating ? -103 : -58, 209);
            var rowAmount = 14;
            var thisWindow = CDesktop.LBWindow();
            var discs = musicRelease.discs == null || musicRelease.discs.Length == 0 ? new() : musicRelease.discs.Split(":").Select(x => int.Parse(x)).ToList();
            if (discs.Count > 0 && !discs.Contains(0)) discs.Insert(0, 0);
            var discOffset = 0;
            var tracklist = new List<(int, string, string, int)>();
            for (int i = 0; i < musicRelease.tracks.Count; i++)
            {
                if (discs.Contains(i))
                {
                    discOffset++;
                    var sum = 0;
                    var upTo = discs.Last() == i ? musicRelease.tracks.Count : discs.Find(x => x > i);
                    for (int j = i; j < upTo; j++)
                        sum += musicRelease.tracks[j].length;
                    tracklist.Add((-1, musicRelease.format == "digital" ? "Disc " + discOffset : "Side " + (char)(discOffset + 64), sum / 60 + "m" + (sum % 60 > 0 ? " " + sum % 60 + "s" : ""), 0));
                }
                tracklist.Add((i + 1, musicRelease.tracks[i].name, musicRelease.tracks[i].length / 60 + ":" + (musicRelease.tracks[i].length % 60).ToString("00"), i));
            }
            if (tracklist.Count < rowAmount)
                tracklist.Add((-1, "", "", 0));
            thisWindow.SetPaginationSingleStep(() => tracklist.Count, rowAmount);
            AddRegionGroup();
            SetRegionGroupWidth(32);
            SetRegionGroupHeight(281);
            AddHeaderRegion(() => AddLine("#", "", "Right"));
            for (int i = thisWindow.pagination() == 0 ? 0 : tracklist.Count - thisWindow.pagination() < rowAmount ? tracklist.Count - (thisWindow.pagination() + 1) : 0; i < rowAmount; i++)
            {
                var index = i;
                if (tracklist.Count > index + thisWindow.pagination())
                {
                    var track = tracklist[index + thisWindow.pagination()];
                    AddPaddingRegion(() => AddLine(track.Item1 != -1 ? track.Item1 + "" : "", "", "Right"));
                }
            }
            if (rating)
            {
                AddRegionGroup();
                SetRegionGroupWidth(19);
                SetRegionGroupHeight(281);
                AddHeaderRegion(() => AddLine("", "", "Right"));
                for (int i = thisWindow.pagination() == 0 ? 0 : tracklist.Count - thisWindow.pagination() < rowAmount ? tracklist.Count - (thisWindow.pagination() + 1) : 0; i < rowAmount; i++)
                {
                    var index = i;
                    if (tracklist.Count > index + thisWindow.pagination())
                    {
                        var track = tracklist[index + thisWindow.pagination()];
                        if (track.Item1 == -1) AddPaddingRegion(() => AddLine(""));
                        else
                        {
                            var trackRating = !ratings.ContainsKey(musicRelease.ID) ? 0 : ratings[musicRelease.ID].trackRatings[track.Item4];
                            AddPaddingRegion(() => AddSmallButton(trackRating > 0 ? "OtherDetract" : "OtherDetractOff", (h) =>
                            {
                                if (trackRating <= 0) return;
                                if (!ratings.ContainsKey(musicRelease.ID))
                                    ratings.Add(musicRelease.ID, new ReleaseRating(musicRelease));
                                if (Input.GetKey(LeftShift)) ratings[musicRelease.ID].trackRatings[track.Item4] = 1;
                                else ratings[musicRelease.ID].trackRatings[track.Item4]--;
                                ratings[musicRelease.ID].UpdateRating();
                                CDesktop.RespawnAll();
                            }, null, null, (h) =>
                            {
                                if (!Input.GetKey(Backspace)) return;
                                var split = musicRelease.discs.Split(":");
                                if (split.Contains(track.Item4 + "") && track.Item4 != 0)
                                {
                                    split = split.Where(x => x != track.Item4 + "").ToArray();
                                    musicRelease.discs = string.Join(":", split);
                                    CDesktop.RespawnAll();
                                }
                            }));
                        }
                    }
                }
                AddRegionGroup();
                SetRegionGroupWidth(52);
                SetRegionGroupHeight(281);
                AddHeaderRegion(() => AddLine(""));
                for (int i = thisWindow.pagination() == 0 ? 0 : tracklist.Count - thisWindow.pagination() < rowAmount ? tracklist.Count - (thisWindow.pagination() + 1) : 0; i < rowAmount; i++)
                {
                    var index = i;
                    if (tracklist.Count > index + thisWindow.pagination())
                    {
                        var track = tracklist[index + thisWindow.pagination()];
                        if (track.Item1 == -1) AddPaddingRegion(() => AddLine(""));
                        else
                        {
                            var trackRating = !ratings.ContainsKey(musicRelease.ID) ? 0 : ratings[musicRelease.ID].trackRatings[track.Item4];
                            AddPaddingRegion(() => AddLine(possibleRatings[trackRating].Item1, possibleRatings[trackRating].Item2, "Center"));
                        }
                    }
                }
                AddRegionGroup();
                SetRegionGroupWidth(19);
                SetRegionGroupHeight(281);
                AddHeaderRegion(() => AddLine("", "", "Right"));
                for (int i = thisWindow.pagination() == 0 ? 0 : tracklist.Count - thisWindow.pagination() < rowAmount ? tracklist.Count - (thisWindow.pagination() + 1) : 0; i < rowAmount; i++)
                {
                    var index = i;
                    if (tracklist.Count > index + thisWindow.pagination())
                    {
                        var track = tracklist[index + thisWindow.pagination()];
                        if (track.Item1 == -1) AddPaddingRegion(() => AddLine(""));
                        else
                        {
                            var trackRating = !ratings.ContainsKey(musicRelease.ID) ? 0 : ratings[musicRelease.ID].trackRatings[track.Item4];
                            AddPaddingRegion(() => AddSmallButton(trackRating < possibleRatings.Length - 1 ? "OtherAdd" : "OtherAddOff", (h) =>
                            {
                                if (trackRating >= possibleRatings.Length - 1) return;
                                if (!ratings.ContainsKey(musicRelease.ID))
                                    ratings.Add(musicRelease.ID, new ReleaseRating(musicRelease));
                                if (Input.GetKey(LeftShift)) ratings[musicRelease.ID].trackRatings[track.Item4] = possibleRatings.Length - 1;
                                else ratings[musicRelease.ID].trackRatings[track.Item4]++;
                                ratings[musicRelease.ID].UpdateRating();
                                CDesktop.RespawnAll();
                            }, null, null, (h) =>
                            {
                                if (!Input.GetKey(Backspace)) return;
                                var split = musicRelease.discs.Length > 0 ? musicRelease.discs.Split(":").ToList() : new();
                                if (!split.Contains(track.Item4 + "") && track.Item4 != 0)
                                {
                                    split.Add(track.Item4 + "");
                                    musicRelease.discs = string.Join(":", split);
                                    CDesktop.RespawnAll();
                                }
                            }));
                        }
                    }
                }
            }
            AddRegionGroup();
            SetRegionGroupWidth(195);
            SetRegionGroupHeight(281);
            AddHeaderRegion(() => AddLine("Name", "", "Left"));
            for (int i = thisWindow.pagination() == 0 ? 0 : tracklist.Count - thisWindow.pagination() < rowAmount ? tracklist.Count - (thisWindow.pagination() + 1) : 0; i < rowAmount; i++)
            {
                var index = i;
                if (tracklist.Count > index + thisWindow.pagination())
                {
                    var track = tracklist[index + thisWindow.pagination()];
                    AddPaddingRegion(() => AddLine(track.Item2, track.Item1 == -1 ? "DarkGray" : "Gray"));
                }
            }
            AddRegionGroup();
            SetRegionGroupWidth(58);
            SetRegionGroupHeight(281);
            AddHeaderRegion(() => AddLine("Duration", "", "Left"));
            for (int i = thisWindow.pagination() == 0 ? 0 : tracklist.Count - thisWindow.pagination() < rowAmount ? tracklist.Count - (thisWindow.pagination() + 1) : 0; i < rowAmount; i++)
            {
                var index = i;
                if (tracklist.Count > index + thisWindow.pagination())
                {
                    var track = tracklist[index + thisWindow.pagination()];
                    AddPaddingRegion(() => AddLine(track.Item3, track.Item1 != -1 ? "Gray" : "DimGray"));
                }
            }
        }),
        new("MusicReleaseDescription", () => {
            var rating = Root.rating.Value();
            SetAnchor(rating ? -293 : -248, -76);
            AddHeaderGroup();
            SetRegionGroupWidth(rating ? 584 : 494);
            SetRegionGroupHeight(95);
            AddPaddingRegion(() =>
            {
                var region = CDesktop.LBWindow().LBRegionGroup().LBRegion();
                WriteWrap(region, musicRelease.name);
                WriteWrap(region, "uiayoe".Contains(musicRelease.types[0].ToLower()[0]) ? "is an" : "is a", "DarkGray");
                WriteWrap(region, musicRelease.types[0].ToLower(), "DarkGray");
                WriteWrap(region, "by", "DarkGray");
                WriteWrap(region, musicRelease.artist, "Gray");
                if (musicRelease.releaseDate.Length == 10)
                {
                    WriteWrap(region, "released on", "DarkGray");
                    WriteWrap(region, DayName(musicRelease.releaseDate.Substring(8, 2)), "Gray");
                    WriteWrap(region, "of", "DarkGray");
                    WriteWrap(region, monthNames[int.Parse(musicRelease.releaseDate.Substring(5, 2))], "Gray");
                }
                else if (musicRelease.releaseDate.Length == 7)
                {
                    WriteWrap(region, "released in", "DarkGray");
                    WriteWrap(region, monthNames[int.Parse(musicRelease.releaseDate.Substring(5, 2))], "Gray");
                    WriteWrap(region, "of", "DarkGray");
                }
                else if (musicRelease.releaseDate.Length == 4)
                    WriteWrap(region, "released in", "DarkGray");
                WriteWrap(region, musicRelease.releaseDate.Substring(0, 4), "Gray");
                AddText(".", "DarkGray");
                WriteWrap(region, "Music on", "DarkGray");
                WriteWrap(region, "this release", "DarkGray");
                WriteWrap(region, "is", "DarkGray");
                if (musicRelease.genres.Count == 0 || musicRelease.genres == null)
                {
                    WriteWrap(region, "not", "DarkGray");
                    WriteWrap(region, "considered", "DarkGray");
                    WriteWrap(region, "to", "DarkGray");
                    WriteWrap(region, "be", "DarkGray");
                    WriteWrap(region, "of", "DarkGray");
                    WriteWrap(region, "any", "DarkGray");
                    WriteWrap(region, "specific", "DarkGray");
                    WriteWrap(region, "genre.", "DarkGray");
                }
                else
                {
                    WriteWrap(region, "considered", "DarkGray");
                    WriteWrap(region, "to be", "DarkGray");
                    if (musicRelease.genres.Count > 1)
                        WriteWrap(region, "a mix of", "DarkGray");
                    foreach (var genre in musicRelease.genres)
                    {
                        WriteWrap(region, genre, "Gray");
                        if (musicRelease.genres.Count > 1 && musicRelease.genres[^2] == genre) WriteWrap(region, "and", "DarkGray");
                        else if (musicRelease.genres.Last() != genre) AddText(",", "DarkGray");
                        else if (musicRelease.genres.Last() == genre) AddText(".", "DarkGray");
                    }
                }
                WriteWrap(region, "This release has", "DarkGray");
                if (musicRelease.tracks.Count == 0)
                {
                    WriteWrap(region, "no", "DarkGray");
                    WriteWrap(region, "tracks", "DarkGray");
                }
                else
                {
                    WriteWrap(region, musicRelease.tracks.Count + " tracks", "Gray");
                    WriteWrap(region, "which", "DarkGray");
                    WriteWrap(region, "add up", "DarkGray");
                    WriteWrap(region, "to a", "DarkGray");
                    WriteWrap(region, "runtime of", "DarkGray");
                    WriteWrap(region, musicRelease.duration + "m", "Gray");
                }
                AddText(".", "DarkGray");
                SetRegionAsGroupExtender();
            });
        }),
        new("MusicReleaseScrollbarUp", () => {
            SetAnchor(rating.Value() ? 272 : 227, 209);
            AddRegionGroup();
            SetRegionGroupWidth(19);
            AddPaddingRegion(() =>
            {
                var window = CDesktop.windows.Find(x => x.title == "MusicRelease");
                if (window.pagination() > 0)
                    AddSmallButton("OtherPageUp", (h) =>
                    {
                        window.DecrementPagination();
                        CDesktop.RespawnAll();
                        Respawn("MusicReleaseScrollbarUp", true);
                        Respawn("MusicReleaseScrollbar", true);
                        Respawn("MusicReleaseScrollbarDown", true);
                    });
                else AddSmallButton("OtherPageUpOff");
            });
        }),
        new("MusicReleaseScrollbar", () => {
            SetAnchor(rating.Value() ? 272 : 227, 190);
            AddRegionGroup();
            SetRegionGroupWidth(19);
            SetRegionGroupHeight(243);
            AddPaddingRegion(() => AddLine(""));
        }),
        new("MusicReleaseScrollbarDown", () => {
            SetAnchor(rating.Value() ? 272 : 227, -57);
            AddRegionGroup();
            SetRegionGroupWidth(19);
            AddPaddingRegion(() =>
            {
                var window = CDesktop.windows.Find(x => x.title == "MusicRelease");
                if (window.pagination() < window.maxPagination())
                    AddSmallButton("OtherPageDown", (h) =>
                    {
                        window.IncrementPagination();
                        CDesktop.RespawnAll();
                        Respawn("MusicReleaseScrollbarUp", true);
                        Respawn("MusicReleaseScrollbar", true);
                        Respawn("MusicReleaseScrollbarDown", true);
                    });
                else AddSmallButton("OtherPageDownOff");
            });
        }),
        new("MusicReleaseCover", () => {
            var rating = Root.rating.Value();
            SetAnchor(rating ? -293 : -248, 228);
            AddHeaderGroup();
            SetRegionGroupWidth(rating ? 584 : 494);
            AddHeaderRegion(() =>
            {
                AddLine(musicRelease.name);
                AddLine("#" + musicRelease.ID, "DimGray", "Right");
                if (CDesktop.title == "CreateNewAlbumPreview")
                {
                    if (Serialization.libraryExpansion)
                        AddSmallButton("OtherNew", (h) =>
                        {
                            library.originalReleases.Add(newRelease);
                            if (newArtist != null)
                            {
                                newArtist.releases.Add(newRelease);
                                library.originalArtists.Add(newArtist);
                            }
                            else artistFind.releases.Add(newRelease);
                            albumCovers.Add(newRelease.ID + "", newCover);
                            var prefix = "";
                            if (Serialization.useUnityData) prefix = @"C:\Users\ragan\Documents\Projects\Unity\MooRT\";
                            System.IO.File.WriteAllBytes(prefix + "MooRT_Data_3/" + newRelease.ID + ".png", newCover.texture.EncodeToPNG());
                            newArtist = null;
                            newRelease = null;
                            artistFind = null;
                            Starter.SetUpLibrary();
                            CloseDesktop(CDesktop.title);
                            SpawnDesktopBlueprint("Menu");
                        });
                    AddSmallButton("OtherBigger", (h) =>
                    {
                        SpawnDesktopBlueprint("SendingMail");
                    });
                    AddSmallButton("OtherCopy", (h) =>
                    {
                        GUIUtility.systemCopyBuffer = Serialization.StringFromPackage(new(musicRelease, musicRelease.artist, musicRelease.country, newCoverURL));
                        SpawnFallingText(new(0, 8), "Data was copied onto the clipboard!");
                        SpawnFallingText(new(0, -7), "Send it now to your friend :3");
                    });
                }
                else if (CDesktop.title == "AcceptNewAlbum")
                {
                    AddSmallButton("OtherTrash", (h) =>
                    {
                        newArtist = null;
                        newRelease = null;
                        artistFind = null;
                        CloseDesktop(CDesktop.title);
                        CDesktop.RespawnAll();
                    });
                    AddSmallButton("OtherCloseGreen", (h) =>
                    {
                        library.originalReleases.Add(newRelease);
                        if (newArtist != null)
                        {
                            newArtist.releases.Add(newRelease);
                            library.originalArtists.Add(newArtist);
                        }
                        else artistFind.releases.Add(newRelease);
                        albumCovers.Add(newRelease.ID + "", newCover);
                        var prefix = "";
                        if (Serialization.useUnityData) prefix = @"C:\Users\ragan\Documents\Projects\Unity\MooRT\";
                        System.IO.File.WriteAllBytes(prefix + "MooRT_Data_3/" + newRelease.ID + ".png", newCover.texture.EncodeToPNG());
                        newArtist = null;
                        newRelease = null;
                        artistFind = null;
                        Starter.SetUpLibrary();
                        CloseDesktop(CDesktop.title);
                        CDesktop.RespawnAll();
                    });
                }
                else
                    AddSmallButton("OtherClose", (h) =>
                    {
                        CloseDesktop(CDesktop.title);
                        CDesktop.RespawnAll();
                    });

            });
            AddRegionGroup();
            SetRegionGroupWidth(190);
            SetRegionGroupHeight(281);
            AddPaddingRegion(() =>
            {
                if (CDesktop.title == "AcceptNewAlbum" || CDesktop.title == "CreateNewAlbumPreview")
                    SetRegionBackgroundAsImage(newCover);
                else if (albumCovers.ContainsKey(musicRelease.ID + ""))
                {
                    SetRegionBackgroundAsImage(albumCovers[musicRelease.ID + ""]);
                    albumCovers[musicRelease.ID + ""].texture.filterMode = FilterMode.Point;
                }
                else SetRegionBackgroundAsImage(albumCovers["0"]);
                SetRegionAsGroupExtender();
            });
            AddEmptyRegion();
            AddButtonRegion(() =>
            {
                AddLine(Root.rating.Value() ? "Hide track ratings" : "Show track ratings", "", "Center");
            },
            (h) =>
            {
                Root.rating.Invert();
                CDesktop.RespawnAll();
                Respawn("MusicReleaseScrollbarUp", true);
                Respawn("MusicReleaseScrollbar", true);
                Respawn("MusicReleaseScrollbarDown", true);
            });
            if (musicRelease.clearedRating)
                AddButtonRegion(() =>
                {
                    AddLine("Restore rating", "", "Center");
                },
                (h) =>
                {
                    musicRelease.RestoreTrackRatings();
                    CDesktop.RespawnAll();
                    Respawn("MusicReleaseScrollbarUp", true);
                    Respawn("MusicReleaseScrollbar", true);
                    Respawn("MusicReleaseScrollbarDown", true);
                });
            else
                AddButtonRegion(() =>
                {
                    AddLine("Clear rating", "", "Center");
                },
                (h) =>
                {
                    musicRelease.ClearTrackRatings();
                    CDesktop.RespawnAll();
                    Respawn("MusicReleaseScrollbarUp", true);
                    Respawn("MusicReleaseScrollbar", true);
                    Respawn("MusicReleaseScrollbarDown", true);
                });
            AddPaddingRegion(() =>
            {
                if (ratings.ContainsKey(musicRelease.ID) && ratings[musicRelease.ID].rating > 0)
                {
                    AddLine("#" + (library.originalReleases.OrderByDescending(x => ratings.ContainsKey(x.ID) ? ratings[x.ID].rating : 0).ToList().IndexOf(musicRelease) + 1));
                    AddText(" overall", "DarkGray");
                }
                else AddLine("", "", "Center");
            });
            AddPaddingRegion(() =>
            {
                if (ratings.ContainsKey(musicRelease.ID) && ratings[musicRelease.ID].rating > 0)
                {
                    AddLine("#" + (years.Find(x => x.year == int.Parse(musicRelease.releaseDate[..4])).releases.OrderByDescending(x => ratings.ContainsKey(x.ID) ? ratings[x.ID].rating : 0).ToList().IndexOf(musicRelease) + 1));
                    AddText(" for " + musicRelease.releaseDate[..4] + ", ", "DarkGray");
                    AddText("#" + (decades.Find(x => x.decade == int.Parse(musicRelease.releaseDate[..3] + "0")).releases.OrderByDescending(x => ratings.ContainsKey(x.ID) ? ratings[x.ID].rating : 0).ToList().IndexOf(musicRelease) + 1), "Gray");
                    AddText(" for " + musicRelease.releaseDate[..3] + "0s", "DarkGray");
                }
                else AddLine("", "", "Center");
            });
        }),
        new("MusicReleaseBottomLine", () => {
            var rating = Root.rating.Value();
            SetAnchor(rating ? -293 : -248, 19);
            AddRegionGroup();
            AddPaddingRegion(() =>
            {
                if (musicReleaseIndex > 0) AddSmallButton("OtherPreviousPage", (h) =>
                {
                    musicRelease = library.releases[--musicReleaseIndex];
                    CDesktop.RespawnAll();
                    Respawn("MusicReleaseScrollbarUp", true);
                    Respawn("MusicReleaseScrollbar", true);
                    Respawn("MusicReleaseScrollbarDown", true);
                    SpawnAlbumTransition();
                    if (albumCovers.ContainsKey(musicRelease.ID + ""))
                    {
                        if (musicRelease.pallete == null)
                            musicRelease.GeneratePallete(albumCovers[musicRelease.ID + ""]);
                        SetDesktopBackgroundAsGradient(musicRelease.pallete);
                    }
                });
                else AddSmallButton("OtherPreviousPageOff");
            });
            AddRegionGroup();
            SetRegionGroupWidth(152);
            AddPaddingRegion(() => AddLine(musicReleaseIndex + 1 + " / " + library.releases.Count, "DarkGray", "Center"));
            AddRegionGroup();
            AddPaddingRegion(() =>
            {
                if (musicReleaseIndex < library.releases.Count - 1) AddSmallButton("OtherNextPage", (h) =>
                {
                    musicRelease = library.releases[++musicReleaseIndex];
                    CDesktop.RespawnAll();
                    Respawn("MusicReleaseScrollbarUp", true);
                    Respawn("MusicReleaseScrollbar", true);
                    Respawn("MusicReleaseScrollbarDown", true);
                    SpawnAlbumTransition();
                    if (albumCovers.ContainsKey(musicRelease.ID + ""))
                    {
                        if (musicRelease.pallete == null)
                            musicRelease.GeneratePallete(albumCovers[musicRelease.ID + ""]);
                        SetDesktopBackgroundAsGradient(musicRelease.pallete);
                    }
                });
                else AddSmallButton("OtherNextPageOff");
            });
        }),

        //Artists
        new("Artists", () => {
            var rowAmount = 15;
            var thisWindow = CDesktop.LBWindow();
            var list = (showExcludedElements.Value() ? library.artists : library.artists.Where(x => artistFiltering[x.ID].Value())).Where(x => x.name != "Various artists" && (!hideArtistsOfExcludedCountries.Value() || hideArtistsOfExcludedCountries.Value() && countryFiltering[x.country].Value())).ToList();
            if (String.searchArtist.Value() != "")
                list = list.Where(x => x.name.ToLower().Contains(String.searchArtist.Value().ToLower())).ToList();
            CDesktop.quickInputWindow = thisWindow;
            thisWindow.SetPaginationSingleStep(() => list.Count, rowAmount);
            SetAnchor(Center);
            AddHeaderGroup();
            SetRegionGroupWidth(385);
            AddPaddingRegion(() =>
            {
                AddCheckbox(showExcludedElements);
                AddLine("Show excluded elements");
            });
            AddPaddingRegion(() =>
            {
                AddCheckbox(hideArtistsOfExcludedCountries);
                AddLine("Hide artists of excluded countries");
            });
            AddPaddingRegion(() => { AddLine("Search:", "DarkGray"); AddInputLine(String.searchArtist); AddSmallButton("OtherReverse", (h) => { String.searchArtist.Set(""); CDesktop.RespawnAll(); }); });
            AddRegionGroup();
            SetRegionGroupWidth(37);
            AddButtonRegion(() => AddLine("#", "", "Right"),
                (h) =>
                {
                    library.artists.Reverse();
                }
            );
            for (int i = thisWindow.pagination() == 0 ? 0 : list.Count - thisWindow.pagination() < rowAmount ? list.Count - (thisWindow.pagination() + 1) : 0; i < rowAmount; i++)
            {
                var index = i;
                if (list.Count > index + thisWindow.pagination())
                    AddHeaderRegion(() => AddLine(1 + index + thisWindow.pagination() + "", "", "Right"));
                else
                    AddPaddingRegion(() => { AddLine(""); });
            }
            AddPaddingRegion(() => AddLine(""));
            AddRegionGroup();
            SetRegionGroupWidth(219);
            AddButtonRegion(() => AddLine("Name"),
                (h) =>
                {
                    library.artists = (lastSort == "Name" ? library.artists.OrderByDescending(x => x.name) : library.artists.OrderBy(x => x.name)).ToList();
                    lastSort = lastSort == "Name" ? "" : "Name";
                }
            );
            for (int i = thisWindow.pagination() == 0 ? 0 : list.Count - thisWindow.pagination() < rowAmount ? list.Count - (thisWindow.pagination() + 1) : 0; i < rowAmount; i++)
            {
                var index = i;
                if (list.Count > index + thisWindow.pagination())
                    AddButtonRegion(() =>
                    {
                        var artist = list[index + thisWindow.pagination()];
                        AddLine(artist.name);
                        AddCheckbox(artistFiltering[artist.ID], artistFiltering.Select(x => x.Value).ToList());
                    },
                    (h) => { });
                else
                    AddPaddingRegion(() => { AddLine(""); });
            }
            AddPaddingRegion(() => AddLine(library.artists.Count + " out of " + library.originalArtists.Count + " artists", "DarkGray"));
            AddRegionGroup();
            SetRegionGroupWidth(55);
            AddButtonRegion(() => AddLine("Country"),
                (h) =>
                {
                    library.artists = (lastSort == "Country" ? library.artists.OrderByDescending(x => x.country) : library.artists.OrderBy(x => x.country)).ToList();
                    lastSort = lastSort == "Country" ? "" : "Country";
                }
            );
            for (int i = thisWindow.pagination() == 0 ? 0 : list.Count - thisWindow.pagination() < rowAmount ? list.Count - (thisWindow.pagination() + 1) : 0; i < rowAmount; i++)
            {
                var index = i;
                if (list.Count > index + thisWindow.pagination())
                    AddButtonRegion(() =>
                    {
                        var artist = list[index + thisWindow.pagination()];
                        AddLine(countryCodes.ContainsKey(artist.country) ? Country.countryCodes[artist.country] : "???", "", "Right");
                    },
                    (h) => { },
                    null,
                    (h) => () =>
                    {
                        var artist = list[index + thisWindow.pagination()];
                        SetAnchor(BottomRight);
                        AddHeaderGroup();
                        AddHeaderRegion(() =>
                        {
                            AddLine(artist.name);
                        });
                    });
                else
                    AddPaddingRegion(() => { AddLine(""); });
            }
            AddPaddingRegion(() => AddLine(""));
            AddRegionGroup();
            SetRegionGroupWidth(55);
            AddButtonRegion(() => AddLine("Points"),
                (h) =>
                {
                    library.artists = (lastSort == "Points" ? library.artists.OrderByDescending(y => y.releases.Sum(x => ratings.ContainsKey(x.ID) ? ratings[x.ID].listPoints : 0)) : library.artists.OrderBy(y => y.releases.Sum(x => ratings.ContainsKey(x.ID) ? ratings[x.ID].listPoints : 0))).ToList();
                    lastSort = lastSort == "Points" ? "" : "Points";
                }
            );
            for (int i = thisWindow.pagination() == 0 ? 0 : list.Count - thisWindow.pagination() < rowAmount ? list.Count - (thisWindow.pagination() + 1) : 0; i < rowAmount; i++)
            {
                var index = i;
                if (list.Count > index + thisWindow.pagination())
                    AddButtonRegion(() =>
                    {
                        var artist = list[index + thisWindow.pagination()];
                        AddLine(artist.releases.Sum(x => ratings.ContainsKey(x.ID) ? ratings[x.ID].listPoints : 0) + "", "", "Right");
                    },
                    (h) => { },
                    null,
                    (h) => () =>
                    {
                        var artist = list[index + thisWindow.pagination()];
                        SetAnchor(BottomRight);
                        AddHeaderGroup();
                        AddHeaderRegion(() =>
                        {
                            AddLine(artist.name);
                        });
                    });
                else
                    AddPaddingRegion(() => { AddLine(""); });
            }
            AddPaddingRegion(() => AddLine(""));
        }),
        new("ArtistsScrollbarUp", () => {
            SetAnchor(173, 133);
            AddRegionGroup();
            SetRegionGroupWidth(19);
            AddPaddingRegion(() =>
            {
                var window = CDesktop.windows.Find(x => x.title == "Artists");
                if (window.pagination() > 0)
                    AddSmallButton("OtherPageUp", (h) =>
                    {
                        window.DecrementPagination();
                        CDesktop.RespawnAll();
                        Respawn("ArtistsScrollbarUp", true);
                        Respawn("ArtistsScrollbar", true);
                        Respawn("ArtistsScrollbarDown", true);
                    });
                else AddSmallButton("OtherPageUpOff");
            });
        }),
        new("ArtistsScrollbar", () => {
            SetAnchor(173, 114);
            AddRegionGroup();
            SetRegionGroupWidth(19);
            SetRegionGroupHeight(281);
            AddPaddingRegion(() => AddLine(""));
        }),
        new("ArtistsScrollbarDown", () => {
            SetAnchor(173, -171);
            AddRegionGroup();
            SetRegionGroupWidth(19);
            AddPaddingRegion(() =>
            {
                var window = CDesktop.windows.Find(x => x.title == "Artists");
                if (window.pagination() < window.maxPagination())
                    AddSmallButton("OtherPageDown", (h) =>
                    {
                        window.IncrementPagination();
                        CDesktop.RespawnAll();
                        Respawn("ArtistsScrollbarUp", true);
                        Respawn("ArtistsScrollbar", true);
                        Respawn("ArtistsScrollbarDown", true);
                    });
                else AddSmallButton("OtherPageDownOff");
            });
        }),

        //Countries
        new("Countries", () => {
            var rowAmount = 15;
            var thisWindow = CDesktop.LBWindow();
            var list = (showExcludedElements.Value() ? countries : countries.Where(x => countryFiltering[x.name].Value())).Where(x => x.name != "-").ToList();
            if (String.searchCountry.Value() != "")
                list = list.Where(x => x.name.ToLower().Contains(String.searchCountry.Value().ToLower())).ToList();
            CDesktop.quickInputWindow = thisWindow;
            thisWindow.SetPaginationSingleStep(() => list.Count, rowAmount);
            SetAnchor(Center);
            AddHeaderGroup();
            SetRegionGroupWidth(440);
            AddPaddingRegion(() =>
            {
                AddCheckbox(showExcludedElements);
                AddLine("Show excluded elements");
            });
            AddPaddingRegion(() => { AddLine("Search:", "DarkGray"); AddInputLine(String.searchCountry); AddSmallButton("OtherReverse", (h) => { String.searchCountry.Set(""); CDesktop.RespawnAll(); }); });
            AddRegionGroup();
            SetRegionGroupWidth(37);
            AddButtonRegion(() => AddLine("#", "", "Right"),
                (h) =>
                {
                    countries.Reverse();
                }
            );
            for (int i = thisWindow.pagination() == 0 ? 0 : list.Count - thisWindow.pagination() < rowAmount ? list.Count - (thisWindow.pagination() + 1) : 0; i < rowAmount; i++)
            {
                var index = i;
                if (list.Count > index + thisWindow.pagination())
                    AddHeaderRegion(() => AddLine(1 + index + thisWindow.pagination() + "", "", "Right"));
                else
                    AddPaddingRegion(() => { AddLine(""); });
            }
            AddPaddingRegion(() => AddLine(""));
            AddRegionGroup();
            SetRegionGroupWidth(219);
            AddButtonRegion(() => AddLine("Name"),
                (h) =>
                {
                    countries = (lastSort == "Name" ? countries.OrderByDescending(x => x.name) : countries.OrderBy(x => x.name)).ToList();
                    lastSort = lastSort == "Name" ? "" : "Name";
                }
            );
            for (int i = thisWindow.pagination() == 0 ? 0 : list.Count - thisWindow.pagination() < rowAmount ? list.Count - (thisWindow.pagination() + 1) : 0; i < rowAmount; i++)
            {
                var index = i;
                if (list.Count > index + thisWindow.pagination())
                    AddButtonRegion(() =>
                    {
                        var country = list[index + thisWindow.pagination()];
                        AddLine(country.name);
                        AddCheckbox(countryFiltering[country.name], countryFiltering.Select(x => x.Value).ToList());
                    },
                    (h) => { });
                else
                    AddPaddingRegion(() => { AddLine(""); });
            }
            AddPaginationLine();
            AddRegionGroup();
            SetRegionGroupWidth(55);
            AddButtonRegion(() => AddLine("Short"),
                (h) =>
                {
                    countries = (lastSort == "Name" ? countries.OrderByDescending(x => x.name) : countries.OrderBy(x => x.name)).ToList();
                    lastSort = lastSort == "Name" ? "" : "Name";
                }
            );
            for (int i = thisWindow.pagination() == 0 ? 0 : list.Count - thisWindow.pagination() < rowAmount ? list.Count - (thisWindow.pagination() + 1) : 0; i < rowAmount; i++)
            {
                var index = i;
                if (list.Count > index + thisWindow.pagination())
                    AddButtonRegion(() =>
                    {
                        var country = list[index + thisWindow.pagination()];
                        AddLine(countryCodes.ContainsKey(country.name) ? countryCodes[country.name] : "???", "", "Right");
                    },
                    (h) => { },
                    null,
                    (h) => () =>
                    {
                        var country = list[index + thisWindow.pagination()];
                        SetAnchor(BottomRight);
                        AddHeaderGroup();
                        AddHeaderRegion(() =>
                        {
                            AddLine(country.name);
                        });
                    });
                else
                    AddPaddingRegion(() => { AddLine(""); });
            }
            AddPaddingRegion(() => AddLine(""));
            AddRegionGroup();
            SetRegionGroupWidth(55);
            AddButtonRegion(() => AddLine("Artists"),
                (h) =>
                {
                    countries = (lastSort == "Artists" ? countries.OrderBy(x => x.artists.Count) : countries.OrderByDescending(x => x.artists.Count)).ToList();
                    lastSort = lastSort == "Artists" ? "" : "Artists";
                }
            );
            for (int i = thisWindow.pagination() == 0 ? 0 : list.Count - thisWindow.pagination() < rowAmount ? list.Count - (thisWindow.pagination() + 1) : 0; i < rowAmount; i++)
            {
                var index = i;
                if (list.Count > index + thisWindow.pagination())
                    AddButtonRegion(() =>
                    {
                        var country = list[index + thisWindow.pagination()];
                        AddLine(country.artists.Count + "", "", "Right");
                    },
                    (h) => { },
                    null,
                    (h) => () =>
                    {
                        var country = list[index + thisWindow.pagination()];
                        SetAnchor(BottomRight);
                        AddHeaderGroup();
                        AddHeaderRegion(() =>
                        {
                            AddLine(country.name);
                        });
                    });
                else
                    AddPaddingRegion(() => { AddLine(""); });
            }
            AddPaddingRegion(() => AddLine(""));
            AddRegionGroup();
            SetRegionGroupWidth(55);
            AddButtonRegion(() => AddLine("Points"),
                (h) =>
                {
                    countries = (lastSort == "Points" ? countries.OrderByDescending(y => y.artists.Sum(z => z.releases.Sum(x => ratings.ContainsKey(x.ID) ? ratings[x.ID].listPoints : 0))) : countries.OrderBy(y => y.artists.Sum(z => z.releases.Sum(x => ratings.ContainsKey(x.ID) ? ratings[x.ID].listPoints : 0)))).ToList();
                    lastSort = lastSort == "Points" ? "" : "Points";
                }
            );
            for (int i = thisWindow.pagination() == 0 ? 0 : list.Count - thisWindow.pagination() < rowAmount ? list.Count - (thisWindow.pagination() + 1) : 0; i < rowAmount; i++)
            {
                var index = i;
                if (list.Count > index + thisWindow.pagination())
                    AddButtonRegion(() =>
                    {
                        var country = list[index + thisWindow.pagination()];
                        AddLine(country.artists.Sum(y => y.releases.Sum(x => ratings.ContainsKey(x.ID) ? ratings[x.ID].listPoints : 0)) + "", "", "Right");
                    },
                    (h) => { });
                else
                    AddPaddingRegion(() => { AddLine(""); });
            }
            AddPaddingRegion(() => AddLine(""));
        }),
        new("CountriesScrollbarUp", () => {
            SetAnchor(200, 142);
            AddRegionGroup();
            SetRegionGroupWidth(19);
            AddPaddingRegion(() =>
            {
                var window = CDesktop.windows.Find(x => x.title == "Countries");
                if (window.pagination() > 0)
                    AddSmallButton("OtherPageUp", (h) =>
                    {
                        window.DecrementPagination();
                        CDesktop.RespawnAll();
                        Respawn("CountriesScrollbarUp", true);
                        Respawn("CountriesScrollbar", true);
                        Respawn("CountriesScrollbarDown", true);
                    });
                else AddSmallButton("OtherPageUpOff");
            });
        }),
        new("CountriesScrollbar", () => {
            SetAnchor(200, 123);
            AddRegionGroup();
            SetRegionGroupWidth(19);
            SetRegionGroupHeight(281);
            AddPaddingRegion(() => AddLine(""));
        }),
        new("CountriesScrollbarDown", () => {
            SetAnchor(200, -162);
            AddRegionGroup();
            SetRegionGroupWidth(19);
            AddPaddingRegion(() =>
            {
                var window = CDesktop.windows.Find(x => x.title == "Countries");
                if (window.pagination() < window.maxPagination())
                    AddSmallButton("OtherPageDown", (h) =>
                    {
                        window.IncrementPagination();
                        CDesktop.RespawnAll();
                        Respawn("CountriesScrollbarUp", true);
                        Respawn("CountriesScrollbar", true);
                        Respawn("CountriesScrollbarDown", true);
                    });
                else AddSmallButton("OtherPageDownOff");
            });
        }),

        //Genres
        new("Genres", () => {
            var rowAmount = 15;
            var thisWindow = CDesktop.LBWindow();
            var list = (showExcludedElements.Value() ? genres : genres.Where(x => genreFiltering[x.name].Value())).Where(x => x.name != "-").ToList();
            if (String.searchGenre.Value() != "")
                list = list.Where(x => x.name.ToLower().Contains(String.searchGenre.Value().ToLower())).ToList();
            thisWindow.SetPaginationSingleStep(() => list.Count, rowAmount);
            SetAnchor(Center);
            AddHeaderGroup();
            SetRegionGroupWidth(385);
            AddPaddingRegion(() =>
            {
                AddCheckbox(showExcludedElements);
                AddLine("Show excluded elements");
            });
            AddPaddingRegion(() =>
            {
                AddCheckbox(requireAllSelectedGenres);
                AddLine("Require all selected genres");
            });
            AddPaddingRegion(() => { AddLine("Search:", "DarkGray"); AddInputLine(String.searchGenre); AddSmallButton("OtherReverse", (h) => { String.searchGenre.Set(""); CDesktop.RespawnAll(); }); });
            AddRegionGroup();
            SetRegionGroupWidth(37);
            AddButtonRegion(() => AddLine("#", "", "Right"),
                (h) =>
                {
                    genres.Reverse();
                }
            );
            for (int i = thisWindow.pagination() == 0 ? 0 : list.Count - thisWindow.pagination() < rowAmount ? list.Count - (thisWindow.pagination() + 1) : 0; i < rowAmount; i++)
            {
                var index = i;
                if (list.Count > index + thisWindow.pagination())
                    AddHeaderRegion(() => AddLine(1 + index + thisWindow.pagination() + "", "", "Right"));
                else
                    AddPaddingRegion(() => { AddLine(""); });
            }
            AddPaddingRegion(() => AddLine(""));
            AddRegionGroup();
            SetRegionGroupWidth(219);
            AddButtonRegion(() => AddLine("Name"),
                (h) =>
                {
                    genres = (lastSort == "Name" ? genres.OrderByDescending(x => x.name) : genres.OrderBy(x => x.name)).ToList();
                    lastSort = lastSort == "Name" ? "" : "Name";
                }
            );
            for (int i = thisWindow.pagination() == 0 ? 0 : list.Count - thisWindow.pagination() < rowAmount ? list.Count - (thisWindow.pagination() + 1) : 0; i < rowAmount; i++)
            {
                var index = i;
                if (list.Count > index + thisWindow.pagination())
                    AddButtonRegion(() =>
                    {
                        var genre = list[index + thisWindow.pagination()];
                        AddLine(genre.name);
                        AddCheckbox(genreFiltering[genre.name], genreFiltering.Select(x => x.Value).ToList());
                    },
                    (h) => { });
                else
                    AddPaddingRegion(() => { AddLine(""); });
            }
            AddPaginationLine();
            AddRegionGroup();
            SetRegionGroupWidth(55);
            AddButtonRegion(() => AddLine("Albums"),
                (h) =>
                {
                    genres = (lastSort == "Albums" ? genres.OrderBy(x => x.releases.Count) : genres.OrderByDescending(x => x.releases.Count)).ToList();
                    lastSort = lastSort == "Albums" ? "" : "Albums";
                }
            );
            for (int i = thisWindow.pagination() == 0 ? 0 : list.Count - thisWindow.pagination() < rowAmount ? list.Count - (thisWindow.pagination() + 1) : 0; i < rowAmount; i++)
            {
                var index = i;
                if (list.Count > index + thisWindow.pagination())
                    AddButtonRegion(() =>
                    {
                        var genre = list[index + thisWindow.pagination()];
                        AddLine(genre.releases.Count + "", "", "Right");
                    },
                    (h) => { },
                    null,
                    (h) => () =>
                    {
                        var genre = list[index + thisWindow.pagination()];
                        SetAnchor(BottomRight);
                        AddHeaderGroup();
                        AddHeaderRegion(() =>
                        {
                            AddLine(genre.name);
                        });
                    });
                else
                    AddPaddingRegion(() => { AddLine(""); });
            }
            AddPaddingRegion(() => AddLine(""));
            AddRegionGroup();
            SetRegionGroupWidth(55);
            AddButtonRegion(() => AddLine("Points"),
                (h) =>
                {
                    genres = (lastSort == "Points" ? genres.OrderBy(y => y.releases.Sum(x => ratings.ContainsKey(x.ID) ? ratings[x.ID].listPoints : 0)) : genres.OrderByDescending(y => y.releases.Sum(x => ratings.ContainsKey(x.ID) ? ratings[x.ID].listPoints : 0))).ToList();
                    lastSort = lastSort == "Points" ? "" : "Points";
                }
            );
            for (int i = thisWindow.pagination() == 0 ? 0 : list.Count - thisWindow.pagination() < rowAmount ? list.Count - (thisWindow.pagination() + 1) : 0; i < rowAmount; i++)
            {
                var index = i;
                if (list.Count > index + thisWindow.pagination())
                    AddButtonRegion(() =>
                    {
                        var genre = list[index + thisWindow.pagination()];
                        AddLine(genre.releases.Sum(x => ratings.ContainsKey(x.ID) ? ratings[x.ID].listPoints : 0) + "", "", "Right");
                    },
                    (h) => { });
                else
                    AddPaddingRegion(() => { AddLine(""); });
            }
            AddPaddingRegion(() => AddLine(""));
        }),
        new("GenresScrollbarUp", () => {
            SetAnchor(173, 133);
            AddRegionGroup();
            SetRegionGroupWidth(19);
            AddPaddingRegion(() =>
            {
                var window = CDesktop.windows.Find(x => x.title == "Genres");
                if (window.pagination() > 0)
                    AddSmallButton("OtherPageUp", (h) =>
                    {
                        window.DecrementPagination();
                        CDesktop.RespawnAll();
                        Respawn("GenresScrollbarUp", true);
                        Respawn("GenresScrollbar", true);
                        Respawn("GenresScrollbarDown", true);
                    });
                else AddSmallButton("OtherPageUpOff");
            });
        }),
        new("GenresScrollbar", () => {
            SetAnchor(173, 114);
            AddRegionGroup();
            SetRegionGroupWidth(19);
            SetRegionGroupHeight(281);
            AddPaddingRegion(() => AddLine(""));
        }),
        new("GenresScrollbarDown", () => {
            SetAnchor(173, -171);
            AddRegionGroup();
            SetRegionGroupWidth(19);
            AddPaddingRegion(() =>
            {
                var window = CDesktop.windows.Find(x => x.title == "Genres");
                if (window.pagination() < window.maxPagination())
                    AddSmallButton("OtherPageDown", (h) =>
                    {
                        window.IncrementPagination();
                        CDesktop.RespawnAll();
                        Respawn("GenresScrollbarUp", true);
                        Respawn("GenresScrollbar", true);
                        Respawn("GenresScrollbarDown", true);
                    });
                else AddSmallButton("OtherPageDownOff");
            });
        }),

        //Languages
        new("Languages", () => {
            var rowAmount = 15;
            var thisWindow = CDesktop.LBWindow();
            var list = (showExcludedElements.Value() ? languages : languages.Where(x => languageFiltering[x.name].Value())).Where(x => x.name != "-").ToList();
            if (String.searchLanguage.Value() != "")
                list = list.Where(x => x.name.ToLower().Contains(String.searchLanguage.Value().ToLower())).ToList();
            thisWindow.SetPaginationSingleStep(() => list.Count, rowAmount);
            SetAnchor(Center);
            AddHeaderGroup();
            SetRegionGroupWidth(385);
            AddPaddingRegion(() =>
            {
                AddCheckbox(showExcludedElements);
                AddLine("Show excluded elements");
            });
            AddPaddingRegion(() =>
            {
                AddCheckbox(requireAllSelectedLanguages);
                AddLine("Require all selected languages");
            });
            AddPaddingRegion(() => { AddLine("Search:", "DarkGray"); AddInputLine(String.searchLanguage); AddSmallButton("OtherReverse", (h) => { String.searchLanguage.Set(""); CDesktop.RespawnAll(); }); });
            AddRegionGroup();
            SetRegionGroupWidth(37);
            AddButtonRegion(() => AddLine("#", "", "Right"),
                (h) =>
                {
                    languages.Reverse();
                }
            );
            for (int i = thisWindow.pagination() == 0 ? 0 : list.Count - thisWindow.pagination() < rowAmount ? list.Count - (thisWindow.pagination() + 1) : 0; i < rowAmount; i++)
            {
                var index = i;
                if (list.Count > index + thisWindow.pagination())
                    AddHeaderRegion(() => AddLine(1 + index + thisWindow.pagination() + "", "", "Right"));
                else
                    AddPaddingRegion(() => { AddLine(""); });
            }
            AddPaddingRegion(() => AddLine(""));
            AddRegionGroup();
            SetRegionGroupWidth(219);
            AddButtonRegion(() => AddLine("Language"),
                (h) =>
                {
                    languages = (lastSort == "Language" ? languages.OrderByDescending(x => x.name) : languages.OrderBy(x => x.name)).ToList();
                    lastSort = lastSort == "Language" ? "" : "Language";
                }
            );
            for (int i = thisWindow.pagination() == 0 ? 0 : list.Count - thisWindow.pagination() < rowAmount ? list.Count - (thisWindow.pagination() + 1) : 0; i < rowAmount; i++)
            {
                var index = i;
                if (list.Count > index + thisWindow.pagination())
                    AddButtonRegion(() =>
                    {
                        var language = list[index + thisWindow.pagination()];
                        AddLine(language.name);
                        AddCheckbox(languageFiltering[language.name], languageFiltering.Select(x => x.Value).ToList());
                    },
                    (h) => { });
                else
                    AddPaddingRegion(() => { AddLine(""); });
            }
            AddPaginationLine();
            AddRegionGroup();
            SetRegionGroupWidth(55);
            AddButtonRegion(() => AddLine("Albums"),
                (h) =>
                {
                    languages = (lastSort == "Artists" ? languages.OrderBy(x => x.releases.Count) : languages.OrderByDescending(x => x.releases.Count)).ToList();
                    lastSort = lastSort == "Artists" ? "" : "Artists";
                }
            );
            for (int i = thisWindow.pagination() == 0 ? 0 : list.Count - thisWindow.pagination() < rowAmount ? list.Count - (thisWindow.pagination() + 1) : 0; i < rowAmount; i++)
            {
                var index = i;
                if (list.Count > index + thisWindow.pagination())
                    AddButtonRegion(() =>
                    {
                        var language = list[index + thisWindow.pagination()];
                        AddLine(language.releases.Count + "", "", "Right");
                    },
                    (h) => { },
                    null,
                    (h) => () =>
                    {
                        var language = list[index + thisWindow.pagination()];
                        SetAnchor(BottomRight);
                        AddHeaderGroup();
                        AddHeaderRegion(() =>
                        {
                            AddLine(language.name);
                        });
                    });
                else
                    AddPaddingRegion(() => { AddLine(""); });
            }
            AddPaddingRegion(() => AddLine(""));
            AddRegionGroup();
            SetRegionGroupWidth(55);
            AddButtonRegion(() => AddLine("Points"),
                (h) =>
                {
                    languages = (lastSort == "Points" ? languages.OrderBy(y => y.releases.Sum(x => ratings.ContainsKey(x.ID) ? ratings[x.ID].listPoints : 0)) : languages.OrderByDescending(y => y.releases.Sum(x => ratings.ContainsKey(x.ID) ? ratings[x.ID].listPoints : 0))).ToList();
                    lastSort = lastSort == "Points" ? "" : "Points";
                }
            );
            for (int i = thisWindow.pagination() == 0 ? 0 : list.Count - thisWindow.pagination() < rowAmount ? list.Count - (thisWindow.pagination() + 1) : 0; i < rowAmount; i++)
            {
                var index = i;
                if (list.Count > index + thisWindow.pagination())
                    AddButtonRegion(() =>
                    {
                        var language = list[index + thisWindow.pagination()];
                        AddLine(language.releases.Sum(x => ratings.ContainsKey(x.ID) ? ratings[x.ID].listPoints : 0) + "", "", "Right");
                    },
                    (h) => { },
                    null,
                    (h) => () =>
                    {
                        var language = list[index + thisWindow.pagination()];
                        SetAnchor(BottomRight);
                        AddHeaderGroup();
                        AddHeaderRegion(() =>
                        {
                            AddLine(language.name);
                        });
                    });
                else
                    AddPaddingRegion(() => { AddLine(""); });
            }
            AddPaddingRegion(() => AddLine(""));
        }),
        new("LanguagesScrollbarUp", () => {
            SetAnchor(173, 133);
            AddRegionGroup();
            SetRegionGroupWidth(19);
            AddPaddingRegion(() =>
            {
                var window = CDesktop.windows.Find(x => x.title == "Languages");
                if (window.pagination() > 0)
                    AddSmallButton("OtherPageUp", (h) =>
                    {
                        window.DecrementPagination();
                        CDesktop.RespawnAll();
                        Respawn("LanguagesScrollbarUp", true);
                        Respawn("LanguagesScrollbar", true);
                        Respawn("LanguagesScrollbarDown", true);
                    });
                else AddSmallButton("OtherPageUpOff");
            });
        }),
        new("LanguagesScrollbar", () => {
            SetAnchor(173, 114);
            AddRegionGroup();
            SetRegionGroupWidth(19);
            SetRegionGroupHeight(281);
            AddPaddingRegion(() => AddLine(""));
        }),
        new("LanguagesScrollbarDown", () => {
            SetAnchor(173, -171);
            AddRegionGroup();
            SetRegionGroupWidth(19);
            AddPaddingRegion(() =>
            {
                var window = CDesktop.windows.Find(x => x.title == "Languages");
                if (window.pagination() < window.maxPagination())
                    AddSmallButton("OtherPageDown", (h) =>
                    {
                        window.IncrementPagination();
                        CDesktop.RespawnAll();
                        Respawn("LanguagesScrollbarUp", true);
                        Respawn("LanguagesScrollbar", true);
                        Respawn("LanguagesScrollbarDown", true);
                    });
                else AddSmallButton("OtherPageDownOff");
            });
        }),

        //Years
        new("Years", () => {
            var rowAmount = 15;
            var thisWindow = CDesktop.LBWindow();
            var list = (showExcludedElements.Value() ? years : years.Where(x => yearFiltering[x.year].Value())).ToList();
            thisWindow.SetPaginationSingleStep(() => list.Count, rowAmount);
            SetAnchor(Center);
            AddHeaderGroup();
            SetRegionGroupWidth(385);
            AddPaddingRegion(() =>
            {
                AddCheckbox(showExcludedElements);
                AddLine("Show excluded elements");
            });
            AddRegionGroup();
            SetRegionGroupWidth(37);
            AddButtonRegion(() => AddLine("#", "", "Right"),
                (h) =>
                {
                    years.Reverse();
                }
            );
            for (int i = thisWindow.pagination() == 0 ? 0 : list.Count - thisWindow.pagination() < rowAmount ? list.Count - (thisWindow.pagination() + 1) : 0; i < rowAmount; i++)
            {
                var index = i;
                if (list.Count > index + thisWindow.pagination())
                    AddHeaderRegion(() => AddLine(1 + index + thisWindow.pagination() + "", "", "Right"));
                else
                    AddPaddingRegion(() => { AddLine(""); });
            }
            AddPaddingRegion(() => AddLine(""));
            AddRegionGroup();
            SetRegionGroupWidth(219);
            AddButtonRegion(() => AddLine("Year"),
                (h) =>
                {
                    years = (lastSort == "Year" ? years.OrderByDescending(x => x.year) : years.OrderBy(x => x.year)).ToList();
                    lastSort = lastSort == "Year" ? "" : "Year";
                }
            );
            for (int i = thisWindow.pagination() == 0 ? 0 : list.Count - thisWindow.pagination() < rowAmount ? list.Count - (thisWindow.pagination() + 1) : 0; i < rowAmount; i++)
            {
                var index = i;
                if (list.Count > index + thisWindow.pagination())
                    AddButtonRegion(() =>
                    {
                        var year = list[index + thisWindow.pagination()];
                        AddLine(year.year + "");
                        AddCheckbox(yearFiltering[year.year], yearFiltering.Select(x => x.Value).ToList());
                    },
                    (h) => { });
                else
                    AddPaddingRegion(() => { AddLine(""); });
            }
            AddPaginationLine();
            AddRegionGroup();
            SetRegionGroupWidth(55);
            AddButtonRegion(() => AddLine("Albums"),
                (h) =>
                {
                    years = (lastSort == "Artists" ? years.OrderBy(x => x.releases.Count) : years.OrderByDescending(x => x.releases.Count)).ToList();
                    lastSort = lastSort == "Artists" ? "" : "Artists";
                }
            );
            for (int i = thisWindow.pagination() == 0 ? 0 : list.Count - thisWindow.pagination() < rowAmount ? list.Count - (thisWindow.pagination() + 1) : 0; i < rowAmount; i++)
            {
                var index = i;
                if (list.Count > index + thisWindow.pagination())
                    AddButtonRegion(() =>
                    {
                        var year = list[index + thisWindow.pagination()];
                        AddLine(year.releases.Count + "", "", "Right");
                    },
                    (h) => { },
                    null,
                    (h) => () =>
                    {
                        var year = list[index + thisWindow.pagination()];
                        SetAnchor(BottomRight);
                        AddHeaderGroup();
                        AddHeaderRegion(() =>
                        {
                            AddLine(year.year + "");
                        });
                    });
                else
                    AddPaddingRegion(() => { AddLine(""); });
            }
            AddPaddingRegion(() => AddLine(""));
            AddRegionGroup();
            SetRegionGroupWidth(55);
            AddButtonRegion(() => AddLine("Points"),
                (h) =>
                {
                    years = (lastSort == "Points" ? years.OrderBy(y => y.releases.Sum(x => ratings.ContainsKey(x.ID) ? ratings[x.ID].listPoints : 0)) : years.OrderByDescending(y => y.releases.Sum(x => ratings.ContainsKey(x.ID) ? ratings[x.ID].listPoints : 0))).ToList();
                    lastSort = lastSort == "Points" ? "" : "Points";
                }
            );
            for (int i = thisWindow.pagination() == 0 ? 0 : list.Count - thisWindow.pagination() < rowAmount ? list.Count - (thisWindow.pagination() + 1) : 0; i < rowAmount; i++)
            {
                var index = i;
                if (list.Count > index + thisWindow.pagination())
                    AddButtonRegion(() =>
                    {
                        var year = list[index + thisWindow.pagination()];
                        AddLine(year.releases.Sum(x => ratings.ContainsKey(x.ID) ? ratings[x.ID].listPoints : 0) + "", "", "Right");
                    },
                    (h) => { });
                else
                    AddPaddingRegion(() => { AddLine(""); });
            }
            AddPaddingRegion(() => AddLine(""));
        }),
        new("YearsScrollbarUp", () => {
            SetAnchor(173, 152);
            AddRegionGroup();
            SetRegionGroupWidth(19);
            AddPaddingRegion(() =>
            {
                var window = CDesktop.windows.Find(x => x.title == "Years");
                if (window.pagination() > 0)
                    AddSmallButton("OtherPageUp", (h) =>
                    {
                        window.DecrementPagination();
                        CDesktop.RespawnAll();
                        Respawn("YearsScrollbarUp", true);
                        Respawn("YearsScrollbar", true);
                        Respawn("YearsScrollbarDown", true);
                    });
                else AddSmallButton("OtherPageUpOff");
            });
        }),
        new("YearsScrollbar", () => {
            SetAnchor(173, 133);
            AddRegionGroup();
            SetRegionGroupWidth(19);
            SetRegionGroupHeight(281);
            AddPaddingRegion(() => AddLine(""));
        }),
        new("YearsScrollbarDown", () => {
            SetAnchor(173, -152);
            AddRegionGroup();
            SetRegionGroupWidth(19);
            AddPaddingRegion(() =>
            {
                var window = CDesktop.windows.Find(x => x.title == "Years");
                if (window.pagination() < window.maxPagination())
                    AddSmallButton("OtherPageDown", (h) =>
                    {
                        window.IncrementPagination();
                        CDesktop.RespawnAll();
                        Respawn("YearsScrollbarUp", true);
                        Respawn("YearsScrollbar", true);
                        Respawn("YearsScrollbarDown", true);
                    });
                else AddSmallButton("OtherPageDownOff");
            });
        }),

        //Decades
        new("Decades", () => {
            var rowAmount = 8;
            var thisWindow = CDesktop.LBWindow();
            var list = (showExcludedElements.Value() ? decades : decades.Where(x => decadeFiltering[x.decade].Value())).ToList();
            thisWindow.SetPaginationSingleStep(() => list.Count, rowAmount);
            SetAnchor(Center);
            AddHeaderGroup();
            SetRegionGroupWidth(385);
            AddPaddingRegion(() =>
            {
                AddCheckbox(showExcludedElements);
                AddLine("Show excluded elements");
            });
            AddRegionGroup();
            SetRegionGroupWidth(37);
            AddButtonRegion(() => AddLine("#", "", "Right"),
                (h) =>
                {
                    decades.Reverse();
                }
            );
            for (int i = thisWindow.pagination() == 0 ? 0 : list.Count - thisWindow.pagination() < rowAmount ? list.Count - (thisWindow.pagination() + 1) : 0; i < rowAmount; i++)
            {
                var index = i;
                if (list.Count > index + thisWindow.pagination())
                    AddHeaderRegion(() => AddLine(1 + index + thisWindow.pagination() + "", "", "Right"));
                else
                    AddPaddingRegion(() => { AddLine(""); });
            }
            AddPaddingRegion(() => AddLine(""));
            AddRegionGroup();
            SetRegionGroupWidth(219);
            AddButtonRegion(() => AddLine("Decade"),
                (h) =>
                {
                    decades = (lastSort == "Decade" ? decades.OrderByDescending(x => x.decade) : decades.OrderBy(x => x.decade)).ToList();
                    lastSort = lastSort == "Decade" ? "" : "Decade";
                }
            );
            for (int i = thisWindow.pagination() == 0 ? 0 : list.Count - thisWindow.pagination() < rowAmount ? list.Count - (thisWindow.pagination() + 1) : 0; i < rowAmount; i++)
            {
                var index = i;
                if (list.Count > index + thisWindow.pagination())
                    AddButtonRegion(() =>
                    {
                        var decade = list[index + thisWindow.pagination()];
                        AddLine(decade.decade + "");
                        AddCheckbox(decadeFiltering[decade.decade], decadeFiltering.Select(x => x.Value).ToList());
                    },
                    (h) => { });
                else
                    AddPaddingRegion(() => { AddLine(""); });
            }
            AddPaginationLine();
            AddRegionGroup();
            SetRegionGroupWidth(55);
            AddButtonRegion(() => AddLine("Albums"),
                (h) =>
                {
                    decades = (lastSort == "Artists" ? decades.OrderBy(x => x.releases.Count) : decades.OrderByDescending(x => x.releases.Count)).ToList();
                    lastSort = lastSort == "Artists" ? "" : "Artists";
                }
            );
            for (int i = thisWindow.pagination() == 0 ? 0 : list.Count - thisWindow.pagination() < rowAmount ? list.Count - (thisWindow.pagination() + 1) : 0; i < rowAmount; i++)
            {
                var index = i;
                if (list.Count > index + thisWindow.pagination())
                    AddButtonRegion(() =>
                    {
                        var decade = list[index + thisWindow.pagination()];
                        AddLine(decade.releases.Count + "", "", "Right");
                    },
                    (h) => { },
                    null,
                    (h) => () =>
                    {
                        var decade = list[index + thisWindow.pagination()];
                        SetAnchor(BottomRight);
                        AddHeaderGroup();
                        AddHeaderRegion(() =>
                        {
                            AddLine(decade.decade + "");
                        });
                    });
                else
                    AddPaddingRegion(() => { AddLine(""); });
            }
            AddPaddingRegion(() => AddLine(""));
            AddRegionGroup();
            SetRegionGroupWidth(55);
            AddButtonRegion(() => AddLine("Points"),
                (h) =>
                {
                    decades = (lastSort == "Points" ? decades.OrderBy(y => y.releases.Sum(x => ratings.ContainsKey(x.ID) ? ratings[x.ID].listPoints : 0)) : decades.OrderByDescending(y => y.releases.Sum(x => ratings.ContainsKey(x.ID) ? ratings[x.ID].listPoints : 0))).ToList();
                    lastSort = lastSort == "Points" ? "" : "Points";
                }
            );
            for (int i = thisWindow.pagination() == 0 ? 0 : list.Count - thisWindow.pagination() < rowAmount ? list.Count - (thisWindow.pagination() + 1) : 0; i < rowAmount; i++)
            {
                var index = i;
                if (list.Count > index + thisWindow.pagination())
                    AddButtonRegion(() =>
                    {
                        var decade = list[index + thisWindow.pagination()];
                        AddLine(decade.releases.Sum(x => ratings.ContainsKey(x.ID) ? ratings[x.ID].listPoints : 0) + "", "", "Right");
                    },
                    (h) => { });
                else
                    AddPaddingRegion(() => { AddLine(""); });
            }
            AddPaddingRegion(() => AddLine(""));
        }),
        new("DecadesScrollbarUp", () => {
            SetAnchor(173, 85);
            AddRegionGroup();
            SetRegionGroupWidth(19);
            AddPaddingRegion(() =>
            {
                var window = CDesktop.windows.Find(x => x.title == "Decades");
                if (window.pagination() > 0)
                    AddSmallButton("OtherPageUp", (h) =>
                    {
                        window.DecrementPagination();
                        CDesktop.RespawnAll();
                        Respawn("DecadesScrollbarUp", true);
                        Respawn("DecadesScrollbar", true);
                        Respawn("DecadesScrollbarDown", true);
                    });
                else AddSmallButton("OtherPageUpOff");
            });
        }),
        new("DecadesScrollbar", () => {
            SetAnchor(173, 66);
            AddRegionGroup();
            SetRegionGroupWidth(19);
            SetRegionGroupHeight(148);
            AddPaddingRegion(() => AddLine(""));
        }),
        new("DecadesScrollbarDown", () => {
            SetAnchor(173, -86);
            AddRegionGroup();
            SetRegionGroupWidth(19);
            AddPaddingRegion(() =>
            {
                var window = CDesktop.windows.Find(x => x.title == "Decades");
                if (window.pagination() < window.maxPagination())
                    AddSmallButton("OtherPageDown", (h) =>
                    {
                        window.IncrementPagination();
                        CDesktop.RespawnAll();
                        Respawn("DecadesScrollbarUp", true);
                        Respawn("DecadesScrollbar", true);
                        Respawn("DecadesScrollbarDown", true);
                    });
                else AddSmallButton("OtherPageDownOff");
            });
        }),

        //Release types
        new("ReleaseTypes", () => {
            var rowAmount = 7;
            var thisWindow = CDesktop.LBWindow();
            var list = (showExcludedElements.Value() ? releaseTypes : releaseTypes.Where(x => releaseTypeFiltering[x.name].Value())).Where(x => x.name != "-").ToList();
            thisWindow.SetPaginationSingleStep(() => list.Count, rowAmount);
            SetAnchor(Center);
            AddHeaderGroup();
            SetRegionGroupWidth(385);
            AddPaddingRegion(() =>
            {
                AddCheckbox(showExcludedElements);
                AddLine("Show excluded elements");
            });
            AddRegionGroup();
            SetRegionGroupWidth(37);
            AddButtonRegion(() => AddLine("#", "", "Right"),
                (h) =>
                {
                    releaseTypes.Reverse();
                }
            );
            for (int i = thisWindow.pagination() == 0 ? 0 : list.Count - thisWindow.pagination() < rowAmount ? list.Count - (thisWindow.pagination() + 1) : 0; i < rowAmount; i++)
            {
                var index = i;
                if (list.Count > index + thisWindow.pagination())
                    AddHeaderRegion(() => AddLine(1 + index + thisWindow.pagination() + "", "", "Right"));
                else
                    AddPaddingRegion(() => { AddLine(""); });
            }
            AddPaddingRegion(() => AddLine(""));
            AddRegionGroup();
            SetRegionGroupWidth(219);
            AddButtonRegion(() => AddLine("Name"),
                (h) =>
                {
                    releaseTypes = (lastSort == "Name" ? releaseTypes.OrderByDescending(x => x.name) : releaseTypes.OrderBy(x => x.name)).ToList();
                    lastSort = lastSort == "Name" ? "" : "Name";
                }
            );
            for (int i = thisWindow.pagination() == 0 ? 0 : list.Count - thisWindow.pagination() < rowAmount ? list.Count - (thisWindow.pagination() + 1) : 0; i < rowAmount; i++)
            {
                var index = i;
                if (list.Count > index + thisWindow.pagination())
                    AddButtonRegion(() =>
                    {
                        var releaseType = list[index + thisWindow.pagination()];
                        AddLine(releaseType.name);
                        AddCheckbox(releaseTypeFiltering[releaseType.name], releaseTypeFiltering.Select(x => x.Value).ToList());
                    },
                    (h) => { });
                else
                    AddPaddingRegion(() => { AddLine(""); });
            }
            AddPaginationLine();
            AddRegionGroup();
            SetRegionGroupWidth(55);
            AddButtonRegion(() => AddLine("Albums"),
                (h) =>
                {
                    releaseTypes = (lastSort == "Albums" ? releaseTypes.OrderBy(x => x.releases.Count) : releaseTypes.OrderByDescending(x => x.releases.Count)).ToList();
                    lastSort = lastSort == "Albums" ? "" : "Albums";
                }
            );
            for (int i = thisWindow.pagination() == 0 ? 0 : list.Count - thisWindow.pagination() < rowAmount ? list.Count - (thisWindow.pagination() + 1) : 0; i < rowAmount; i++)
            {
                var index = i;
                if (list.Count > index + thisWindow.pagination())
                    AddButtonRegion(() =>
                    {
                        var releaseType = list[index + thisWindow.pagination()];
                        AddLine(releaseType.releases.Count + "", "", "Right");
                    },
                    (h) => { },
                    null,
                    (h) => () =>
                    {
                        var releaseType = list[index + thisWindow.pagination()];
                        SetAnchor(BottomRight);
                        AddHeaderGroup();
                        AddHeaderRegion(() =>
                        {
                            AddLine(releaseType.name);
                        });
                    });
                else
                    AddPaddingRegion(() => { AddLine(""); });
            }
            AddPaddingRegion(() => AddLine(""));
            AddRegionGroup();
            SetRegionGroupWidth(55);
            AddButtonRegion(() => AddLine("Points"),
                (h) =>
                {
                    releaseTypes = (lastSort == "Points" ? releaseTypes.OrderBy(y => y.releases.Sum(x => ratings.ContainsKey(x.ID) ? ratings[x.ID].listPoints : 0)) : releaseTypes.OrderByDescending(y => y.releases.Sum(x => ratings.ContainsKey(x.ID) ? ratings[x.ID].listPoints : 0))).ToList();
                    lastSort = lastSort == "Points" ? "" : "Points";
                }
            );
            for (int i = thisWindow.pagination() == 0 ? 0 : list.Count - thisWindow.pagination() < rowAmount ? list.Count - (thisWindow.pagination() + 1) : 0; i < rowAmount; i++)
            {
                var index = i;
                if (list.Count > index + thisWindow.pagination())
                    AddButtonRegion(() =>
                    {
                        var releaseType = list[index + thisWindow.pagination()];
                        AddLine(releaseType.releases.Sum(x => ratings.ContainsKey(x.ID) ? ratings[x.ID].listPoints : 0) + "", "", "Right");
                    },
                    (h) => { });
                else
                    AddPaddingRegion(() => { AddLine(""); });
            }
            AddPaddingRegion(() => AddLine(""));
        }),
        new("ReleaseTypesScrollbarUp", () => {
            SetAnchor(173, 76);
            AddRegionGroup();
            SetRegionGroupWidth(19);
            AddPaddingRegion(() =>
            {
                var window = CDesktop.windows.Find(x => x.title == "ReleaseTypes");
                if (window.pagination() > 0)
                    AddSmallButton("OtherPageUp", (h) =>
                    {
                        window.DecrementPagination();
                        CDesktop.RespawnAll();
                        Respawn("ReleaseTypesScrollbarUp", true);
                        Respawn("ReleaseTypesScrollbar", true);
                        Respawn("ReleaseTypesScrollbarDown", true);
                    });
                else AddSmallButton("OtherPageUpOff");
            });
        }),
        new("ReleaseTypesScrollbar", () => {
            SetAnchor(173, 57);
            AddRegionGroup();
            SetRegionGroupWidth(19);
            SetRegionGroupHeight(129);
            AddPaddingRegion(() => AddLine(""));
        }),
        new("ReleaseTypesScrollbarDown", () => {
            SetAnchor(173, -76);
            AddRegionGroup();
            SetRegionGroupWidth(19);
            AddPaddingRegion(() =>
            {
                var window = CDesktop.windows.Find(x => x.title == "ReleaseTypes");
                if (window.pagination() < window.maxPagination())
                    AddSmallButton("OtherPageDown", (h) =>
                    {
                        window.IncrementPagination();
                        CDesktop.RespawnAll();
                        Respawn("ReleaseTypesScrollbarUp", true);
                        Respawn("ReleaseTypesScrollbar", true);
                        Respawn("ReleaseTypesScrollbarDown", true);
                    });
                else AddSmallButton("OtherPageDownOff");
            });
        }),

        //Track amounts
        new("TrackAmounts", () => {
            var rowAmount = 15;
            var thisWindow = CDesktop.LBWindow();
            var list = (showExcludedElements.Value() ? trackAmounts : trackAmounts.Where(x => trackAmountFiltering[x.amount].Value())).ToList();
            thisWindow.SetPaginationSingleStep(() => list.Count, rowAmount);
            SetAnchor(Center);
            AddHeaderGroup();
            SetRegionGroupWidth(385);
            AddPaddingRegion(() =>
            {
                AddCheckbox(showExcludedElements);
                AddLine("Show excluded elements");
            });
            AddRegionGroup();
            SetRegionGroupWidth(37);
            AddButtonRegion(() => AddLine("#", "", "Right"),
                (h) =>
                {
                    trackAmounts.Reverse();
                }
            );
            for (int i = thisWindow.pagination() == 0 ? 0 : list.Count - thisWindow.pagination() < rowAmount ? list.Count - (thisWindow.pagination() + 1) : 0; i < rowAmount; i++)
            {
                var index = i;
                if (list.Count > index + thisWindow.pagination())
                    AddHeaderRegion(() => AddLine(1 + index + thisWindow.pagination() + "", "", "Right"));
                else
                    AddPaddingRegion(() => { AddLine(""); });
            }
            AddPaddingRegion(() => AddLine(""));
            AddRegionGroup();
            SetRegionGroupWidth(219);
            AddButtonRegion(() => AddLine("Track amount"),
                (h) =>
                {
                    trackAmounts = (lastSort == "TrackAmount" ? trackAmounts.OrderBy(x => x.amount) : trackAmounts.OrderByDescending(x => x.amount)).ToList();
                    lastSort = lastSort == "TrackAmount" ? "" : "TrackAmount";
                }
            );
            for (int i = thisWindow.pagination() == 0 ? 0 : list.Count - thisWindow.pagination() < rowAmount ? list.Count - (thisWindow.pagination() + 1) : 0; i < rowAmount; i++)
            {
                var index = i;
                if (list.Count > index + thisWindow.pagination())
                    AddButtonRegion(() =>
                    {
                        var trackAmount = list[index + thisWindow.pagination()];
                        AddLine(trackAmount.amount + "");
                        AddCheckbox(trackAmountFiltering[trackAmount.amount], trackAmountFiltering.Select(x => x.Value).ToList());
                    },
                    (h) => { });
                else
                    AddPaddingRegion(() => { AddLine(""); });
            }
            AddPaginationLine();
            AddRegionGroup();
            SetRegionGroupWidth(55);
            AddButtonRegion(() => AddLine("Albums"),
                (h) =>
                {
                    trackAmounts = (lastSort == "Albums" ? trackAmounts.OrderBy(x => x.releases.Count) : trackAmounts.OrderByDescending(x => x.releases.Count)).ToList();
                    lastSort = lastSort == "Albums" ? "" : "Albums";
                }
            );
            for (int i = thisWindow.pagination() == 0 ? 0 : list.Count - thisWindow.pagination() < rowAmount ? list.Count - (thisWindow.pagination() + 1) : 0; i < rowAmount; i++)
            {
                var index = i;
                if (list.Count > index + thisWindow.pagination())
                    AddButtonRegion(() =>
                    {
                        var trackAmount = list[index + thisWindow.pagination()];
                        AddLine(trackAmount.releases.Count + "", "", "Right");
                    },
                    (h) => { },
                    null,
                    (h) => () =>
                    {
                        var trackAmount = list[index + thisWindow.pagination()];
                        SetAnchor(BottomRight);
                        AddHeaderGroup();
                        AddHeaderRegion(() =>
                        {
                            AddLine(trackAmount.amount + "");
                        });
                    });
                else
                    AddPaddingRegion(() => { AddLine(""); });
            }
            AddPaddingRegion(() => AddLine(""));
            AddRegionGroup();
            SetRegionGroupWidth(55);
            AddButtonRegion(() => AddLine("Points"),
                (h) =>
                {
                    durations = (lastSort == "Points" ? durations.OrderBy(y => y.releases.Sum(x => ratings.ContainsKey(x.ID) ? ratings[x.ID].listPoints : 0)) : durations.OrderByDescending(y => y.releases.Sum(x => ratings.ContainsKey(x.ID) ? ratings[x.ID].listPoints : 0))).ToList();
                    lastSort = lastSort == "Points" ? "" : "Points";
                }
            );
            for (int i = thisWindow.pagination() == 0 ? 0 : list.Count - thisWindow.pagination() < rowAmount ? list.Count - (thisWindow.pagination() + 1) : 0; i < rowAmount; i++)
            {
                var index = i;
                if (list.Count > index + thisWindow.pagination())
                    AddButtonRegion(() =>
                    {
                        var duration = list[index + thisWindow.pagination()];
                        AddLine(duration.releases.Sum(x => ratings.ContainsKey(x.ID) ? ratings[x.ID].listPoints : 0) + "", "", "Right");
                    },
                    (h) => { });
                else
                    AddPaddingRegion(() => { AddLine(""); });
            }
            AddPaddingRegion(() => AddLine(""));
        }),
        new("TrackAmountsScrollbarUp", () => {
            SetAnchor(173, 152);
            AddRegionGroup();
            SetRegionGroupWidth(19);
            AddPaddingRegion(() =>
            {
                var window = CDesktop.windows.Find(x => x.title == "TrackAmounts");
                if (window.pagination() > 0)
                    AddSmallButton("OtherPageUp", (h) =>
                    {
                        window.DecrementPagination();
                        CDesktop.RespawnAll();
                        Respawn("TrackAmountsScrollbarUp", true);
                        Respawn("TrackAmountsScrollbar", true);
                        Respawn("TrackAmountsScrollbarDown", true);
                    });
                else AddSmallButton("OtherPageUpOff");
            });
        }),
        new("TrackAmountsScrollbar", () => {
            SetAnchor(173, 133);
            AddRegionGroup();
            SetRegionGroupWidth(19);
            SetRegionGroupHeight(281);
            AddPaddingRegion(() => AddLine(""));
        }),
        new("TrackAmountsScrollbarDown", () => {
            SetAnchor(173, -152);
            AddRegionGroup();
            SetRegionGroupWidth(19);
            AddPaddingRegion(() =>
            {
                var window = CDesktop.windows.Find(x => x.title == "TrackAmounts");
                if (window.pagination() < window.maxPagination())
                    AddSmallButton("OtherPageDown", (h) =>
                    {
                        window.IncrementPagination();
                        CDesktop.RespawnAll();
                        Respawn("TrackAmountsScrollbarUp", true);
                        Respawn("TrackAmountsScrollbar", true);
                        Respawn("TrackAmountsScrollbarDown", true);
                    });
                else AddSmallButton("OtherPageDownOff");
            });
        }),

        //Debut years
        new("DebutYears", () => {
            var rowAmount = 15;
            var thisWindow = CDesktop.LBWindow();
            var list = (showExcludedElements.Value() ? debutYears : debutYears.Where(x => debutYearFiltering[x.year].Value())).ToList();
            thisWindow.SetPaginationSingleStep(() => list.Count, rowAmount);
            SetAnchor(Center);
            AddHeaderGroup();
            SetRegionGroupWidth(385);
            AddPaddingRegion(() =>
            {
                AddCheckbox(showExcludedElements);
                AddLine("Show excluded elements");
            });
            AddRegionGroup();
            SetRegionGroupWidth(37);
            AddButtonRegion(() => AddLine("#", "", "Right"),
                (h) =>
                {
                    debutYears.Reverse();
                }
            );
            for (int i = thisWindow.pagination() == 0 ? 0 : list.Count - thisWindow.pagination() < rowAmount ? list.Count - (thisWindow.pagination() + 1) : 0; i < rowAmount; i++)
            {
                var index = i;
                if (list.Count > index + thisWindow.pagination())
                    AddHeaderRegion(() => AddLine(1 + index + thisWindow.pagination() + "", "", "Right"));
                else
                    AddPaddingRegion(() => { AddLine(""); });
            }
            AddPaddingRegion(() => AddLine(""));
            AddRegionGroup();
            SetRegionGroupWidth(219);
            AddButtonRegion(() => AddLine("Year"),
                (h) =>
                {
                    debutYears = (lastSort == "Year" ? debutYears.OrderBy(x => x.year) : debutYears.OrderByDescending(x => x.year)).ToList();
                    lastSort = lastSort == "Year" ? "" : "Year";
                }
            );
            for (int i = thisWindow.pagination() == 0 ? 0 : list.Count - thisWindow.pagination() < rowAmount ? list.Count - (thisWindow.pagination() + 1) : 0; i < rowAmount; i++)
            {
                var index = i;
                if (list.Count > index + thisWindow.pagination())
                    AddButtonRegion(() =>
                    {
                        var debutYear = list[index + thisWindow.pagination()];
                        AddLine(debutYear.year + "");
                        AddCheckbox(debutYearFiltering[debutYear.year], debutYearFiltering.Select(x => x.Value).ToList());
                    },
                    (h) => { });
                else
                    AddPaddingRegion(() => { AddLine(""); });
            }
            AddPaginationLine();
            AddRegionGroup();
            SetRegionGroupWidth(55);
            AddButtonRegion(() => AddLine("Debuts"),
                (h) =>
                {
                    debutYears = (lastSort == "Debuts" ? debutYears.OrderBy(x => x.releases.Count) : debutYears.OrderByDescending(x => x.releases.Count)).ToList();
                    lastSort = lastSort == "Debuts" ? "" : "Debuts";
                }
            );
            for (int i = thisWindow.pagination() == 0 ? 0 : list.Count - thisWindow.pagination() < rowAmount ? list.Count - (thisWindow.pagination() + 1) : 0; i < rowAmount; i++)
            {
                var index = i;
                if (list.Count > index + thisWindow.pagination())
                    AddButtonRegion(() =>
                    {
                        var debutYear = list[index + thisWindow.pagination()];
                        AddLine(debutYear.releases.Count + "", "", "Right");
                    },
                    (h) => { },
                    null,
                    (h) => () =>
                    {
                        var debutYear = list[index + thisWindow.pagination()];
                        SetAnchor(BottomRight);
                        AddHeaderGroup();
                        AddHeaderRegion(() =>
                        {
                            AddLine(debutYear.year + "");
                        });
                    });
                else
                    AddPaddingRegion(() => { AddLine(""); });
            }
            AddPaddingRegion(() => AddLine(""));
            AddRegionGroup();
            SetRegionGroupWidth(55);
            AddButtonRegion(() => AddLine("Points"),
                (h) =>
                {
                    debutYears = (lastSort == "Points" ? debutYears.OrderBy(y => y.releases.Sum(x => ratings.ContainsKey(x.ID) ? ratings[x.ID].listPoints : 0)) : debutYears.OrderByDescending(y => y.releases.Sum(x => ratings.ContainsKey(x.ID) ? ratings[x.ID].listPoints : 0))).ToList();
                    lastSort = lastSort == "Points" ? "" : "Points";
                }
            );
            for (int i = thisWindow.pagination() == 0 ? 0 : list.Count - thisWindow.pagination() < rowAmount ? list.Count - (thisWindow.pagination() + 1) : 0; i < rowAmount; i++)
            {
                var index = i;
                if (list.Count > index + thisWindow.pagination())
                    AddButtonRegion(() =>
                    {
                        var debutYear = list[index + thisWindow.pagination()];
                        AddLine(debutYear.releases.Sum(x => ratings.ContainsKey(x.ID) ? ratings[x.ID].listPoints : 0) + "", "", "Right");
                    },
                    (h) => { });
                else
                    AddPaddingRegion(() => { AddLine(""); });
            }
            AddPaddingRegion(() => AddLine(""));
        }),
        new("DebutYearsScrollbarUp", () => {
            SetAnchor(173, 152);
            AddRegionGroup();
            SetRegionGroupWidth(19);
            AddPaddingRegion(() =>
            {
                var window = CDesktop.windows.Find(x => x.title == "DebutYears");
                if (window.pagination() > 0)
                    AddSmallButton("OtherPageUp", (h) =>
                    {
                        window.DecrementPagination();
                        CDesktop.RespawnAll();
                        Respawn("DebutYearsScrollbarUp", true);
                        Respawn("DebutYearsScrollbar", true);
                        Respawn("DebutYearsScrollbarDown", true);
                    });
                else AddSmallButton("OtherPageUpOff");
            });
        }),
        new("DebutYearsScrollbar", () => {
            SetAnchor(173, 133);
            AddRegionGroup();
            SetRegionGroupWidth(19);
            SetRegionGroupHeight(281);
            AddPaddingRegion(() => AddLine(""));
        }),
        new("DebutYearsScrollbarDown", () => {
            SetAnchor(173, -152);
            AddRegionGroup();
            SetRegionGroupWidth(19);
            AddPaddingRegion(() =>
            {
                var window = CDesktop.windows.Find(x => x.title == "DebutYears");
                if (window.pagination() < window.maxPagination())
                    AddSmallButton("OtherPageDown", (h) =>
                    {
                        window.IncrementPagination();
                        CDesktop.RespawnAll();
                        Respawn("DebutYearsScrollbarUp", true);
                        Respawn("DebutYearsScrollbar", true);
                        Respawn("DebutYearsScrollbarDown", true);
                    });
                else AddSmallButton("OtherPageDownOff");
            });
        }),

        //Durations
        new("Durations", () => {
            var rowAmount = 15;
            var thisWindow = CDesktop.LBWindow();
            var list = (showExcludedElements.Value() ? durations : durations.Where(x => durationFiltering[x.duration].Value())).ToList();
            thisWindow.SetPaginationSingleStep(() => list.Count, rowAmount);
            SetAnchor(Center);
            AddHeaderGroup();
            SetRegionGroupWidth(385);
            AddPaddingRegion(() =>
            {
                AddCheckbox(showExcludedElements);
                AddLine("Show excluded elements");
            });
            AddRegionGroup();
            SetRegionGroupWidth(37);
            AddButtonRegion(() => AddLine("#", "", "Right"),
                (h) =>
                {
                    durations.Reverse();
                }
            );
            for (int i = thisWindow.pagination() == 0 ? 0 : list.Count - thisWindow.pagination() < rowAmount ? list.Count - (thisWindow.pagination() + 1) : 0; i < rowAmount; i++)
            {
                var index = i;
                if (list.Count > index + thisWindow.pagination())
                    AddHeaderRegion(() => AddLine(1 + index + thisWindow.pagination() + "", "", "Right"));
                else
                    AddPaddingRegion(() => { AddLine(""); });
            }
            AddPaddingRegion(() => AddLine(""));
            AddRegionGroup();
            SetRegionGroupWidth(219);
            AddButtonRegion(() => AddLine("Duration"),
                (h) =>
                {
                    durations = (lastSort == "Duration" ? durations.OrderByDescending(x => x.duration) : durations.OrderBy(x => x.duration)).ToList();
                    lastSort = lastSort == "Duration" ? "" : "Duration";
                }
            );
            for (int i = thisWindow.pagination() == 0 ? 0 : list.Count - thisWindow.pagination() < rowAmount ? list.Count - (thisWindow.pagination() + 1) : 0; i < rowAmount; i++)
            {
                var index = i;
                if (list.Count > index + thisWindow.pagination())
                    AddButtonRegion(() =>
                    {
                        var duration = list[index + thisWindow.pagination()];
                        AddLine(duration.duration + "m");
                        AddCheckbox(durationFiltering[duration.duration], durationFiltering.Select(x => x.Value).ToList());
                    },
                    (h) => { });
                else
                    AddPaddingRegion(() => { AddLine(""); });
            }
            AddPaginationLine();
            AddRegionGroup();
            SetRegionGroupWidth(55);
            AddButtonRegion(() => AddLine("Albums"),
                (h) =>
                {
                    durations = (lastSort == "Albums" ? durations.OrderBy(x => x.releases.Count) : durations.OrderByDescending(x => x.releases.Count)).ToList();
                    lastSort = lastSort == "Albums" ? "" : "Albums";
                }
            );
            for (int i = thisWindow.pagination() == 0 ? 0 : list.Count - thisWindow.pagination() < rowAmount ? list.Count - (thisWindow.pagination() + 1) : 0; i < rowAmount; i++)
            {
                var index = i;
                if (list.Count > index + thisWindow.pagination())
                    AddButtonRegion(() =>
                    {
                        var duration = list[index + thisWindow.pagination()];
                        AddLine(duration.releases.Count + "", "", "Right");
                    },
                    (h) => { },
                    null,
                    (h) => () =>
                    {
                        var duration = list[index + thisWindow.pagination()];
                        SetAnchor(BottomRight);
                        AddHeaderGroup();
                        AddHeaderRegion(() =>
                        {
                            AddLine(duration.duration + "");
                        });
                    });
                else
                    AddPaddingRegion(() => { AddLine(""); });
            }
            AddPaddingRegion(() => AddLine(""));
            AddRegionGroup();
            SetRegionGroupWidth(55);
            AddButtonRegion(() => AddLine("Points"),
                (h) =>
                {
                    durations = (lastSort == "Points" ? durations.OrderBy(y => y.releases.Sum(x => ratings.ContainsKey(x.ID) ? ratings[x.ID].listPoints : 0)) : durations.OrderByDescending(y => y.releases.Sum(x => ratings.ContainsKey(x.ID) ? ratings[x.ID].listPoints : 0))).ToList();
                    lastSort = lastSort == "Points" ? "" : "Points";
                }
            );
            for (int i = thisWindow.pagination() == 0 ? 0 : list.Count - thisWindow.pagination() < rowAmount ? list.Count - (thisWindow.pagination() + 1) : 0; i < rowAmount; i++)
            {
                var index = i;
                if (list.Count > index + thisWindow.pagination())
                    AddButtonRegion(() =>
                    {
                        var duration = list[index + thisWindow.pagination()];
                        AddLine(duration.releases.Sum(x => ratings.ContainsKey(x.ID) ? ratings[x.ID].listPoints : 0) + "", "", "Right");
                    },
                    (h) => { });
                else
                    AddPaddingRegion(() => { AddLine(""); });
            }
            AddPaddingRegion(() => AddLine(""));
        }),
        new("DurationsScrollbarUp", () => {
            SetAnchor(173, 152);
            AddRegionGroup();
            SetRegionGroupWidth(19);
            AddPaddingRegion(() =>
            {
                var window = CDesktop.windows.Find(x => x.title == "Durations");
                if (window.pagination() > 0)
                    AddSmallButton("OtherPageUp", (h) =>
                    {
                        window.DecrementPagination();
                        CDesktop.RespawnAll();
                        Respawn("DurationsScrollbarUp", true);
                        Respawn("DurationsScrollbar", true);
                        Respawn("DurationsScrollbarDown", true);
                    });
                else AddSmallButton("OtherPageUpOff");
            });
        }),
        new("DurationsScrollbar", () => {
            SetAnchor(173, 133);
            AddRegionGroup();
            SetRegionGroupWidth(19);
            SetRegionGroupHeight(281);
            AddPaddingRegion(() => AddLine(""));
        }),
        new("DurationsScrollbarDown", () => {
            SetAnchor(173, -152);
            AddRegionGroup();
            SetRegionGroupWidth(19);
            AddPaddingRegion(() =>
            {
                var window = CDesktop.windows.Find(x => x.title == "Durations");
                if (window.pagination() < window.maxPagination())
                    AddSmallButton("OtherPageDown", (h) =>
                    {
                        window.IncrementPagination();
                        CDesktop.RespawnAll();
                        Respawn("DurationsScrollbarUp", true);
                        Respawn("DurationsScrollbar", true);
                        Respawn("DurationsScrollbarDown", true);
                    });
                else AddSmallButton("OtherPageDownOff");
            });
        }),

        //Rating statuses
        new("RatingStatuses", () => {
            var rowAmount = 3;
            var thisWindow = CDesktop.LBWindow();
            var list = (showExcludedElements.Value() ? ratingStatuses : ratingStatuses.Where(x => ratingStatusFiltering[x.status].Value())).Where(x => x.status != "-").ToList();
            thisWindow.SetPaginationSingleStep(() => list.Count, rowAmount);
            SetAnchor(Center);
            AddHeaderGroup();
            SetRegionGroupWidth(330);
            AddPaddingRegion(() =>
            {
                AddCheckbox(showExcludedElements);
                AddLine("Show excluded elements");
            });
            AddRegionGroup();
            SetRegionGroupWidth(37);
            AddButtonRegion(() => AddLine("#", "", "Right"),
                (h) =>
                {
                    ratingStatuses.Reverse();
                }
            );
            for (int i = thisWindow.pagination() == 0 ? 0 : list.Count - thisWindow.pagination() < rowAmount ? list.Count - (thisWindow.pagination() + 1) : 0; i < rowAmount; i++)
            {
                var index = i;
                if (list.Count > index + thisWindow.pagination())
                    AddHeaderRegion(() => AddLine(1 + index + thisWindow.pagination() + "", "", "Right"));
                else
                    AddPaddingRegion(() => { AddLine(""); });
            }
            AddPaddingRegion(() => AddLine(""));
            AddRegionGroup();
            SetRegionGroupWidth(219);
            AddButtonRegion(() => AddLine("Status"),
                (h) =>
                {
                    ratingStatuses = (lastSort == "Status" ? ratingStatuses.OrderByDescending(x => x.status == "Rated" ? 2 : (x.status == "Partially rated" ? 1 : 0)) : ratingStatuses.OrderBy(x => x.status == "Rated" ? 2 : (x.status == "Partially rated" ? 1 : 0))).ToList();
                    lastSort = lastSort == "Status" ? "" : "Status";
                }
            );
            for (int i = thisWindow.pagination() == 0 ? 0 : list.Count - thisWindow.pagination() < rowAmount ? list.Count - (thisWindow.pagination() + 1) : 0; i < rowAmount; i++)
            {
                var index = i;
                if (list.Count > index + thisWindow.pagination())
                    AddButtonRegion(() =>
                    {
                        var ratingStatus = list[index + thisWindow.pagination()];
                        AddLine(ratingStatus.status);
                        AddCheckbox(ratingStatusFiltering[ratingStatus.status], ratingStatusFiltering.Select(x => x.Value).ToList());
                    },
                    (h) => { });
                else
                    AddPaddingRegion(() => { AddLine(""); });
            }
            AddPaginationLine();
            AddRegionGroup();
            SetRegionGroupWidth(55);
            AddButtonRegion(() => AddLine("Albums"),
                (h) =>
                {
                    ratingStatuses = (lastSort == "Albums" ? ratingStatuses.OrderBy(x => library.originalReleases.Count(y => (y.GetRating() > 0 ? "Rated" : (ratings.ContainsKey(y.ID) && ratings[y.ID].trackRatings.Any(y => y != 0) ? "Partially rated" : "Unrated")) == x.status)) : ratingStatuses.OrderByDescending(x => library.originalReleases.Count(y => (y.GetRating() > 0 ? "Rated" : (ratings.ContainsKey(y.ID) && ratings[y.ID].trackRatings.Any(y => y != 0) ? "Partially rated" : "Unrated")) == x.status))).ToList();
                    lastSort = lastSort == "Albums" ? "" : "Albums";
                }
            );
            for (int i = thisWindow.pagination() == 0 ? 0 : list.Count - thisWindow.pagination() < rowAmount ? list.Count - (thisWindow.pagination() + 1) : 0; i < rowAmount; i++)
            {
                var index = i;
                if (list.Count > index + thisWindow.pagination())
                    AddButtonRegion(() =>
                    {
                        var ratingStatus = list[index + thisWindow.pagination()];
                        AddLine(library.originalReleases.Count(x => (x.GetRating() > 0 ? "Rated" : (ratings.ContainsKey(x.ID) && ratings[x.ID].trackRatings.Any(x => x != 0) ? "Partially rated" : "Unrated")) == ratingStatus.status) + "", "", "Right");
                    },
                    (h) => { });
                else
                    AddPaddingRegion(() => { AddLine(""); });
            }
            AddPaddingRegion(() => AddLine(""));
        }),
        new("RatingStatusesScrollbarUp", () => {
            SetAnchor(145, 38);
            AddRegionGroup();
            SetRegionGroupWidth(19);
            AddPaddingRegion(() =>
            {
                var window = CDesktop.windows.Find(x => x.title == "RatingStatuses");
                if (window.pagination() > 0)
                    AddSmallButton("OtherPageUp", (h) =>
                    {
                        window.DecrementPagination();
                        CDesktop.RespawnAll();
                        Respawn("RatingStatusesScrollbarUp", true);
                        Respawn("RatingStatusesScrollbar", true);
                        Respawn("RatingStatusesScrollbarDown", true);
                    });
                else AddSmallButton("OtherPageUpOff");
            });
        }),
        new("RatingStatusesScrollbar", () => {
            SetAnchor(145, 19);
            AddRegionGroup();
            SetRegionGroupWidth(19);
            SetRegionGroupHeight(53);
            AddPaddingRegion(() => AddLine(""));
        }),
        new("RatingStatusesScrollbarDown", () => {
            SetAnchor(145, -38);
            AddRegionGroup();
            SetRegionGroupWidth(19);
            AddPaddingRegion(() =>
            {
                var window = CDesktop.windows.Find(x => x.title == "RatingStatuses");
                if (window.pagination() < window.maxPagination())
                    AddSmallButton("OtherPageDown", (h) =>
                    {
                        window.IncrementPagination();
                        CDesktop.RespawnAll();
                        Respawn("RatingStatusesScrollbarUp", true);
                        Respawn("RatingStatusesScrollbar", true);
                        Respawn("RatingStatusesScrollbarDown", true);
                    });
                else AddSmallButton("OtherPageDownOff");
            });
        }),

        //Menu
        new("MenuBar", () => {
            SetAnchor(Bottom, 0, 10);
            AddRegionGroup();
            if (CDesktop.title == "MusicReleases") AddPaddingRegion(() => AddLine("Music Releases"));
            else AddButtonRegion(() => AddLine("Music Releases"), (h) => { var name = CDesktop.title; SpawnDesktopBlueprint("MusicReleases"); CloseDesktop(name); });
            AddRegionGroup();
            if (CDesktop.title == "Artists") AddPaddingRegion(() => AddLine("Artists"));
            else AddButtonRegion(() => AddLine("Artists"), (h) => { var name = CDesktop.title; SpawnDesktopBlueprint("Artists"); CloseDesktop(name); });
            AddRegionGroup();
            if (CDesktop.title == "Countries") AddPaddingRegion(() => AddLine("Countries"));
            else AddButtonRegion(() => AddLine("Countries"),(h) => { var name = CDesktop.title; SpawnDesktopBlueprint("Countries"); CloseDesktop(name); });
            AddRegionGroup();
            if (CDesktop.title == "Genres") AddPaddingRegion(() => AddLine("Genres"));
            else AddButtonRegion(() => AddLine("Genres"),(h) => { var name = CDesktop.title; SpawnDesktopBlueprint("Genres"); CloseDesktop(name); });
            AddRegionGroup();
            if (CDesktop.title == "ReleaseTypes") AddPaddingRegion(() => AddLine("Release Types"));
            else AddButtonRegion(() => AddLine("Release Types"),(h) => { var name = CDesktop.title; SpawnDesktopBlueprint("ReleaseTypes"); CloseDesktop(name); });
            AddRegionGroup();
            if (CDesktop.title == "Years") AddPaddingRegion(() => AddLine("Years"));
            else AddButtonRegion(() => AddLine("Years"),(h) => { var name = CDesktop.title; SpawnDesktopBlueprint("Years"); CloseDesktop(name); });
            AddRegionGroup();
            if (CDesktop.title == "Decades") AddPaddingRegion(() => AddLine("Decades"));
            else AddButtonRegion(() => AddLine("Decades"),(h) => { var name = CDesktop.title; SpawnDesktopBlueprint("Decades"); CloseDesktop(name); });
            AddRegionGroup();
            if (CDesktop.title == "Durations") AddPaddingRegion(() => AddLine("Durations"));
            else AddButtonRegion(() => AddLine("Durations"),(h) => { var name = CDesktop.title; SpawnDesktopBlueprint("Durations"); CloseDesktop(name); });
            AddRegionGroup();
            if (CDesktop.title == "TrackAmounts") AddPaddingRegion(() => AddLine("Track Amounts"));
            else AddButtonRegion(() => AddLine("Track Amounts"),(h) => { var name = CDesktop.title; SpawnDesktopBlueprint("TrackAmounts"); CloseDesktop(name); });
            AddRegionGroup();
            if (CDesktop.title == "Languages") AddPaddingRegion(() => AddLine("Languages"));
            else AddButtonRegion(() => AddLine("Languages"),(h) => { var name = CDesktop.title; SpawnDesktopBlueprint("Languages"); CloseDesktop(name); });
            AddRegionGroup();
            if (CDesktop.title == "DebutYears") AddPaddingRegion(() => AddLine("Debut Years"));
            else AddButtonRegion(() => AddLine("Debut Years"),(h) => { var name = CDesktop.title; SpawnDesktopBlueprint("DebutYears"); CloseDesktop(name); });
            AddRegionGroup();
            if (CDesktop.title == "Anniversaries") AddPaddingRegion(() => AddLine("Anniversaries"));
            else AddButtonRegion(() => AddLine("Anniversaries"),(h) => { var name = CDesktop.title; SpawnDesktopBlueprint("Anniversaries"); CloseDesktop(name); });
            AddRegionGroup();
            if (CDesktop.title == "RatingStatus") AddPaddingRegion(() => AddLine("Rating Status"));
            else AddButtonRegion(() => AddLine("Rating Status"),(h) => { var name = CDesktop.title; SpawnDesktopBlueprint("RatingStatus"); CloseDesktop(name); });
        }),
        new("Menu", () => {
            SetAnchor(Center);
            AddRegionGroup();
            SetRegionGroupWidth(180);
            AddHeaderRegion(() => AddLine("Library management:"));
            AddButtonRegion(() => AddLine("Paste new release"), (h) =>
            {
                //Setup
                newCover = null;
                newRelease = null;
                String.searchNewAlbumCountry.Set("");
                String.createNewAlbumReleaseName.Set("");
                String.createNewAlbumReleaseDate.Set("");
                String.createNewAlbumGenres.Set("");
                String.createNewAlbumLanguages.Set("");
                String.createNewAlbumTracklist.Set("");
                String.createNewAlbumArtistName.Set("");
                String.createNewAlbumArtistCountry.Set("-");
                String.createNewAlbumCoverURL.Set("");
                createNewAlbumReleaseTypeFiltering = releaseTypes.ToDictionary(x => x.name, x => new Bool(false));
                //Load
                var package = Serialization.PackageFromString(GUIUtility.systemCopyBuffer);
                if (package == null) return;
                newRelease = musicRelease = package.musicRelease;
                newCoverURL = package.coverURL;
                String.createNewAlbumArtistName.Set(package.artistName);
                String.createNewAlbumArtistCountry.Set(package.artistCountry);
                String.createNewAlbumCoverURL.Set(package.coverURL);
                if (newRelease.genres.Count > 0) String.createNewAlbumGenres.Set(string.Join(", ", newRelease.genres));
                else String.createNewAlbumLanguages.Set("");
                if (newRelease.languages.Count > 0) String.createNewAlbumLanguages.Set(string.Join(", ", newRelease.languages));
                else String.createNewAlbumLanguages.Set("");
                String.createNewAlbumReleaseDate.Set(newRelease.releaseDate);
                String.createNewAlbumReleaseName.Set(newRelease.name);
                if (newRelease.tracks.Count > 0) String.createNewAlbumTracklist.Set(string.Join("\\n", newRelease.tracks.Select(x => x.name + "\\n" + (x.length / 60) + ":" + (x.length % 60).ToString("00"))));
                else String.createNewAlbumTracklist.Set("");
                foreach (var type in createNewAlbumReleaseTypeFiltering)
                    createNewAlbumReleaseTypeFiltering[type.Key].Set(newRelease.types.Contains(type.Key));
                CreatePreviewRelease();
                CloseDesktop(CDesktop.title);
                SpawnDesktopBlueprint("CreateNewAlbumPreviewLoadCover");
            });
            AddButtonRegion(() => AddLine("Create new release"), (h) =>
            {
                //Setup
                newCover = null;
                newRelease = null;
                String.searchNewAlbumCountry.Set("");
                String.createNewAlbumReleaseName.Set("");
                String.createNewAlbumReleaseDate.Set("");
                String.createNewAlbumGenres.Set("");
                String.createNewAlbumLanguages.Set("");
                String.createNewAlbumTracklist.Set("");
                String.createNewAlbumArtistName.Set("");
                String.createNewAlbumArtistCountry.Set("-");
                String.createNewAlbumCoverURL.Set("");
                createNewAlbumReleaseTypeFiltering = releaseTypes.ToDictionary(x => x.name, x => new Bool(false));
                //Load
                SpawnDesktopBlueprint("CreateNewAlbumReleaseName");
            });
            if (!Serialization.libraryExpansion)
            {
                AddButtonRegion(() => AddLine("Refetch online library"), (h) =>
                {
                    Starter.goToMenuOnFailedWWW = true;
                    Starter.enteredSecondStage = false;
                    Starter.enteredThirdStage = false;
                    Starter.enteredFourthStage = false;
                    Starter.enteredFifthStage = false;
                    if (Serialization.useUnityData) Serialization.urlContent = "x";
                    else
                    {
                        Serialization.urlContent = "";
                        MonoBehaviour.FindAnyObjectByType<Starter>().StartCoroutine(GetJSON("https://raw.githubusercontent.com/NetherlandsDonut/MooRT/refs/heads/main/MooRT_Data_2/library.json"));
                    }
                    Cursor.cursor.SetCursor(CursorType.Await);
                    Starter.enteredSecondStage = true;
                    CloseDesktop("Menu");
                });
            }
            AddEmptyRegion();
            AddHeaderRegion(() => AddLine("Exporting:"));
            AddButtonRegion(() => AddLine("Quick #100 Studio albums"), (h) =>
            {
                Exporting.ExportSquareChart(library.originalReleases.Where(x => x.GetRating() > 0 && x.types.Contains("Studio album")).OrderByDescending(x => x.GetRating()).ToList());
            });
            AddButtonRegion(() => AddLine("Quick chart of current"), (h) =>
            {
                var rel = library.releases.Where(x => x.GetRating() > 0).OrderByDescending(x => x.GetRating()).ToList();
                var squareN = 1;
                for (int i = 2; ; i++)
                    if (i * i > rel.Count) break;
                    else squareN = i;
                Exporting.ExportSquareChart(rel, squareN, squareN);
            });
            AddButtonRegion(() => AddLine("Export album chart"), (h) =>
            {
                SpawnDesktopBlueprint("SquareChart");
            });
            AddButtonRegion(() => AddLine("Export scaled album chart"), (h) =>
            {
                var list = library.releases.Where(x => x.GetRating() > 0).ToList();
                Exporting.GenerateScaledChartBlueprint(list.Count, scaledChartPerfectFit, scaledChartFirstRowAmount, scaledChartRowAmount);
                SpawnDesktopBlueprint("ScaledChart");
            });
            AddButtonRegion(() => AddLine("Export sequence album chart"), (h) =>
            {
                SpawnDesktopBlueprint("SequenceChart");
            });
            AddEmptyRegion();
            AddHeaderRegion(() => AddLine("Tools:"));
            AddButtonRegion(() => AddLine("Artist battle"), (h) =>
            {
                tracksPerArtist = 1;
                SpawnDesktopBlueprint("PrepareArtistBattle");
            });
            AddEmptyRegion();
            AddHeaderRegion(() =>
            {
                AddLine("Settings:", "Gray");
                //AddSmallButton("OtherClose", (h) =>
                //{
                //    CloseWindow(h.window);
                //    Respawn("Menu");
                //});
            });
            AddButtonRegion(() => AddLine("Rating color ranges"), (h) =>
            {
                SpawnDesktopBlueprint("RatingColorRange");
            });
            AddButtonRegion(() => AddLine("Menu background color"), (h) =>
            {
                SpawnDesktopBlueprint("MenuBackgroundColor");
            });
            AddEmptyRegion();
            AddHeaderRegion(() => AddLine("Menu:"));
            //AddButtonRegion(() => AddLine("Settings"), (h) =>
            //{
            //    CloseWindow(h.window);
            //    SpawnWindowBlueprint("MenuSettings");
            //});
            AddButtonRegion(() => AddLine("Exit"), (h) =>
            {
                Serialization.Serialize(settings, "settings");
                if (library != null && library.originalReleases.Count > 0 && library.originalArtists.Count > 0)
                    Serialization.Serialize(library, "library");
                if (ratings != null && ratings.Count > 0)
                    Serialization.Serialize(ratings, "ratings");
                Application.Quit();
            });
        }),
        new("MenuSettings", () => {
            SetAnchor(Center);
            AddRegionGroup();
            SetRegionGroupWidth(190);
            //AddButtonRegion(() =>
            //{
            //    AddCheckbox(settings.pixelPerfectVision);
            //    AddLine("Pixel perfect vision");
            //},
            //(h) =>
            //{
            //    settings.pixelPerfectVision.Invert();
            //    CDesktop.RespawnAll();
            //});
        }),
        new("RatingColorRange1", () => {
            SetAnchor(Center, -322);
            AddRegionGroup();
            SetRegionGroupWidth(110);
            for (int j = 0; j < 3; j++)
            {
                var index = 0 + j * 6;
                var range = settings.ratingRanges[index];
                range.PrintOut();
                if (j < 2) AddEmptyRegion();
            }
        }),
        new("RatingColorRange2", () => {
            SetAnchor(Center, -194);
            AddRegionGroup();
            SetRegionGroupWidth(110);
            for (int j = 0; j < 3; j++)
            {
                var index = 1 + j * 6;
                var range = settings.ratingRanges[index];
                range.PrintOut();
                if (j < 2) AddEmptyRegion();
            }
        }),
        new("RatingColorRange3", () => {
            SetAnchor(Center, -64);
            AddRegionGroup();
            SetRegionGroupWidth(110);
            for (int j = 0; j < 3; j++)
            {
                var index = 2 + j * 6;
                var range = settings.ratingRanges[index];
                range.PrintOut();
                if (j < 2) AddEmptyRegion();
            }
        }),
        new("RatingColorRange4", () => {
            SetAnchor(Center, 64);
            AddRegionGroup();
            SetRegionGroupWidth(110);
            for (int j = 0; j < 3; j++)
            {
                var index = 3 + j * 6;
                var range = settings.ratingRanges[index];
                range.PrintOut();
                if (j < 2) AddEmptyRegion();
            }
        }),
        new("RatingColorRange5", () => {
            SetAnchor(Center, 194);
            AddRegionGroup();
            SetRegionGroupWidth(110);
            for (int j = 0; j < 3; j++)
            {
                var index = 4 + j * 6;
                var range = settings.ratingRanges[index];
                range.PrintOut();
                if (j < 2) AddEmptyRegion();
            }
        }),
        new("RatingColorRange6", () => {
            SetAnchor(Center, 322);
            AddRegionGroup();
            SetRegionGroupWidth(110);
            for (int j = 0; j < 3; j++)
            {
                var index = 5 + j * 6;
                var range = settings.ratingRanges[index];
                range.PrintOut();
                if (j < 2) AddEmptyRegion();
            }
        }),
        new("RatingColorRangeMenuBar", () => {
            SetAnchor(Bottom, 0, 10);
            AddRegionGroup();
            AddButtonRegion(() => AddLine("Clear All Ranges"), (h) =>
            {
                foreach (var foo in settings.ratingRanges)
                {
                    foo.min.Set("0");
                    foo.r = 183;
                    foo.g = 183;
                    foo.b = 183;
                }
                CDesktop.RespawnAll();
            });
            AddRegionGroup();
            AddButtonRegion(() => AddLine("Reset To Default Values"), (h) =>
            {
                settings.ratingRanges = RatingRange.DefaultRatingRanges();
                CDesktop.RespawnAll();
            });
        }),
        new("MenuBackgroundColor", () => {
            SetAnchor(Center);
            AddRegionGroup();
            SetRegionGroupWidth(110);
            AddPaddingRegion(() =>
            {
                AddLine("Red:", "Gray");
                AddLine("" + settings.menuBackgroundColor[0], "Gray", "Right");
                AddSmallButton(settings.menuBackgroundColor[0] < 255 ? "OtherAdd" : "OtherAddOff",
                    (h) =>
                    {
                        if (settings.menuBackgroundColor[0] >= 255) return;
                        if (Input.GetKey(LeftShift)) settings.menuBackgroundColor[0] += 20;
                        else settings.menuBackgroundColor[0]++;
                        if (settings.menuBackgroundColor[0] > 255) settings.menuBackgroundColor[0] = 255;
                        CDesktop.RespawnAll();
                    }
                );
                AddSmallButton(settings.menuBackgroundColor[0] > 0 ? "OtherDetract" : "OtherDetractOff",
                    (h) =>
                    {
                        if (settings.menuBackgroundColor[0] <= 0) return;
                        if (Input.GetKey(LeftShift)) settings.menuBackgroundColor[0] -= 20;
                        else settings.menuBackgroundColor[0]--;
                        if (settings.menuBackgroundColor[0] < 0) settings.menuBackgroundColor[0] = 0;
                        CDesktop.RespawnAll();
                    }
                );
            });
            AddPaddingRegion(() =>
            {
                AddLine("Green:", "Gray");
                AddLine("" + settings.menuBackgroundColor[1], "Gray", "Right");
                AddSmallButton(settings.menuBackgroundColor[1] < 255 ? "OtherAdd" : "OtherAddOff",
                    (h) =>
                    {
                        if (settings.menuBackgroundColor[1] >= 255) return;
                        if (Input.GetKey(LeftShift)) settings.menuBackgroundColor[1] += 20;
                        else settings.menuBackgroundColor[1]++;
                        if (settings.menuBackgroundColor[1] > 255) settings.menuBackgroundColor[1] = 255;
                        CDesktop.RespawnAll();
                    }
                );
                AddSmallButton(settings.menuBackgroundColor[1] > 0 ? "OtherDetract" : "OtherDetractOff",
                    (h) =>
                    {
                        if (settings.menuBackgroundColor[1] <= 0) return;
                        if (Input.GetKey(LeftShift)) settings.menuBackgroundColor[1] -= 20;
                        else settings.menuBackgroundColor[1]--;
                        if (settings.menuBackgroundColor[1] < 0) settings.menuBackgroundColor[1] = 0;
                        CDesktop.RespawnAll();
                    }
                );
            });
            AddPaddingRegion(() =>
            {
                AddLine("Blue:", "Gray");
                AddLine("" + settings.menuBackgroundColor[2], "Gray", "Right");
                AddSmallButton(settings.menuBackgroundColor[2] < 255 ? "OtherAdd" : "OtherAddOff",
                    (h) =>
                    {
                        if (settings.menuBackgroundColor[2] >= 255) return;
                        if (Input.GetKey(LeftShift)) settings.menuBackgroundColor[2] += 20;
                        else settings.menuBackgroundColor[2]++;
                        if (settings.menuBackgroundColor[2] > 255) settings.menuBackgroundColor[2] = 255;
                        CDesktop.RespawnAll();
                    }
                );
                AddSmallButton(settings.menuBackgroundColor[2] > 0 ? "OtherDetract" : "OtherDetractOff",
                    (h) =>
                    {
                        if (settings.menuBackgroundColor[2] <= 0) return;
                        if (Input.GetKey(LeftShift)) settings.menuBackgroundColor[2] -= 20;
                        else settings.menuBackgroundColor[2]--;
                        if (settings.menuBackgroundColor[2] < 0) settings.menuBackgroundColor[2] = 0;
                        CDesktop.RespawnAll();
                    }
                );
            });
        }),
        new("MenuBackgroundColorMenuBar", () => {
            SetAnchor(Bottom, 0, 10);
            AddRegionGroup();
            AddButtonRegion(() => AddLine("Reset To Default Value"), (h) =>
            {
                settings.menuBackgroundColor = new[] { 96, 79, 124 };
                CDesktop.RespawnAll();
            });
        }),

        //Errors
        new("ErrorLoadingAlbum2", () => {
            SetAnchor(Top, 0, -19);
            AddRegionGroup();
            SetRegionGroupWidth(300);
            AddHeaderRegion(() =>
            {
                AddLine("Error loading album");
                AddSmallButton("OtherClose", (h) => CloseWindow(h.window));
            });
            AddPaddingRegion(() =>
            {
                AddLine("Error at line 3, no artist name provided");
            });
        }),
        new("ErrorLoadingAlbum8", () => {
            SetAnchor(Top, 0, -19);
            AddRegionGroup();
            SetRegionGroupWidth(300);
            AddHeaderRegion(() =>
            {
                AddLine("Error loading album");
                AddSmallButton("OtherClose", (h) => CloseWindow(h.window));
            });
            AddPaddingRegion(() =>
            {
                AddLine("Error at line 9, no artist country provided");
            });
        }),
        new("ErrorLoadingAlbum14", () => {
            SetAnchor(Top, 0, -19);
            AddRegionGroup();
            SetRegionGroupWidth(300);
            AddHeaderRegion(() =>
            {
                AddLine("Error loading album");
                AddSmallButton("OtherClose", (h) => CloseWindow(h.window));
            });
            AddPaddingRegion(() =>
            {
                AddLine("Error at line 15, no album name provided");
            });
        }),
        new("ErrorLoadingAlbum20", () => {
            SetAnchor(Top, 0, -19);
            AddRegionGroup();
            SetRegionGroupWidth(300);
            AddHeaderRegion(() =>
            {
                AddLine("Error loading album");
                AddSmallButton("OtherClose", (h) => CloseWindow(h.window));
            });
            AddPaddingRegion(() =>
            {
                AddLine("Error at line 21, release date in wrong fromat");
            });
        }),
        new("ErrorLoadingAlbum26", () => {
            SetAnchor(Top, 0, -19);
            AddRegionGroup();
            SetRegionGroupWidth(300);
            AddHeaderRegion(() =>
            {
                AddLine("Error loading album");
                AddSmallButton("OtherClose", (h) => CloseWindow(h.window));
            });
            AddPaddingRegion(() =>
            {
                AddLine("Error at line 27, error loading image");
            });
        }),
        new("ErrorLoadingAlbum32", () => {
            SetAnchor(Top, 0, -19);
            AddRegionGroup();
            SetRegionGroupWidth(300);
            AddHeaderRegion(() =>
            {
                AddLine("Error loading album");
                AddSmallButton("OtherClose", (h) => CloseWindow(h.window));
            });
            AddPaddingRegion(() =>
            {
                AddLine("Error at line 33, no album type provided");
            });
        }),
        new("ErrorLoadingAlbum320", () => {
            SetAnchor(Top, 0, -19);
            AddRegionGroup();
            SetRegionGroupWidth(300);
            AddHeaderRegion(() =>
            {
                AddLine("Error loading album");
                AddSmallButton("OtherClose", (h) => CloseWindow(h.window));
            });
            AddPaddingRegion(() =>
            {
                AddLine("Error at line 33, provided album type isn't recognised");
            });
        }),
        new("ErrorLoadingAlbum38", () => {
            SetAnchor(Top, 0, -19);
            AddRegionGroup();
            SetRegionGroupWidth(300);
            AddHeaderRegion(() =>
            {
                AddLine("Error loading album");
                AddSmallButton("OtherClose", (h) => CloseWindow(h.window));
            });
            AddPaddingRegion(() =>
            {
                AddLine("Error at line 39, no album genres provided");
            });
        }),
        new("ErrorLoadingAlbum50", () => {
            SetAnchor(Top, 0, -19);
            AddRegionGroup();
            SetRegionGroupWidth(300);
            AddHeaderRegion(() =>
            {
                AddLine("Error loading album");
                AddSmallButton("OtherClose", (h) => CloseWindow(h.window));
            });
            AddPaddingRegion(() =>
            {
                AddLine("Error at line " + errorAtLine + ", no tracks were provided");
            });
        }),
        new("ErrorLoadingAlbum500", () => {
            SetAnchor(Top, 0, -19);
            AddRegionGroup();
            SetRegionGroupWidth(300);
            AddHeaderRegion(() =>
            {
                AddLine("Error loading album");
                AddSmallButton("OtherClose", (h) => CloseWindow(h.window));
            });
            AddPaddingRegion(() =>
            {
                AddLine("Error at line " + errorAtLine + ", track provided with wrong format");
            });
        }),
        new("ErrorLoadingAlbum501", () => {
            SetAnchor(Top, 0, -19);
            AddRegionGroup();
            SetRegionGroupWidth(300);
            AddHeaderRegion(() =>
            {
                AddLine("Error loading album");
                AddSmallButton("OtherClose", (h) => CloseWindow(h.window));
            });
            AddPaddingRegion(() =>
            {
                AddLine("Error at line " + errorAtLine + ", duration provided in wrong format");
            });
        }),
        new("ErrorLoadingAlbum502", () => {
            SetAnchor(Top, 0, -19);
            AddRegionGroup();
            SetRegionGroupWidth(300);
            AddHeaderRegion(() =>
            {
                AddLine("Error loading album");
                AddSmallButton("OtherClose", (h) => CloseWindow(h.window));
            });
            AddPaddingRegion(() =>
            {
                AddLine("Error at line " + errorAtLine + ", minutes aren't a number");
            });
        }),
        new("ErrorLoadingAlbum503", () => {
            SetAnchor(Top, 0, -19);
            AddRegionGroup();
            SetRegionGroupWidth(300);
            AddHeaderRegion(() =>
            {
                AddLine("Error loading album");
                AddSmallButton("OtherClose", (h) => CloseWindow(h.window));
            });
            AddPaddingRegion(() =>
            {
                AddLine("Error at line " + errorAtLine + ", seconds aren't a number");
            });
        }),

        //Artist battle
        new("ArtistBattle", () => {
            SetAnchor(Center);
            for (int i = 0; i < artistBattle.perRound; i++)
            {
                var index = i;
                var candidate = artistBattle.rounds[currentRound].candidates[index];
                var track = candidate.track;
                var album = library.originalReleases.Find(x => x.ID == candidate.releaseID);
                var artist = library.originalArtists.Find(x => x.ID == candidate.artistID);
                AddRegionGroup();
                SetRegionGroupWidth(190);
                SetRegionGroupHeight(243);
                AddButtonRegion(() =>
                {
                    if (albumCovers.ContainsKey(album.ID + ""))
                        SetRegionBackgroundAsImage(albumCovers[album.ID + ""]);
                    else SetRegionBackgroundAsImage(albumCovers["0"]);
                    SetRegionAsGroupExtender();
                },
                (h) =>
                {
                    artistBattle.rounds[currentRound++].choice = candidate.artistID;
                    if (currentRound == artistBattle.rounds.Count)
                    {
                        CloseDesktop("ArtistBattle");
                        Exporting.ExportArtistBattleResults(artistBattle);
                    }
                    else CDesktop.RespawnAll();
                });
                AddHeaderRegion(() =>
                {
                    AddLine(track.duration);
                    AddText(" / ", "DarkGray");
                    AddText(track.name, "Gray");
                });
                AddPaddingRegion(() =>
                {
                    AddLine(artist.name);
                });
                AddPaddingRegion(() =>
                {
                    AddLine(album.name);
                });
            }
        }),
        new("ArtistBattleHeader", () => {
            SetAnchor(Center, 0, 133);
            AddRegionGroup();
            SetRegionGroupWidth(190 * artistBattle.perRound);
            SetRegionGroupHeight(19);
            AddHeaderRegion(() =>
            {
                AddLine("Round " + (currentRound + 1) + " / " + artistBattle.roundAmount, "", "Center");
            });
        }),
        new("ArtistBattlePerRound", () => {
            SetAnchor(-221, 225);
            AddHeaderGroup();
            SetRegionGroupWidth(440);
            SetRegionGroupHeight(19);
            AddHeaderRegion(() => AddLine("Tracks per round", "", "Center"));
            AddRegionGroup();
            SetRegionGroupWidth(146);
            SetRegionGroupHeight(19);
            if (perRound == 2) AddHeaderRegion(() => AddLine("2", "", "Center"));
            else AddButtonRegion(() =>
            {
                AddLine("2", "", "Center");
            },
            (h) =>
            {
                perRound = 2;
                CDesktop.RespawnAll();
            });
            AddRegionGroup();
            SetRegionGroupWidth(147);
            SetRegionGroupHeight(19);
            if (perRound == 3) AddHeaderRegion(() => AddLine("3", "", "Center"));
            else AddButtonRegion(() =>
            {
                AddLine("3", "", "Center");
            },
            (h) =>
            {
                perRound = 3;
                CDesktop.RespawnAll();
            });
            AddRegionGroup();
            SetRegionGroupWidth(146);
            SetRegionGroupHeight(19);
            if (perRound == 4) AddHeaderRegion(() => AddLine("4", "", "Center"));
            else AddButtonRegion(() => AddLine("4", "", "Center"),
            (h) =>
            {
                perRound = 4;
                CDesktop.RespawnAll();
            });
        }),
        new("ArtistBattlePerArtist", () => {
            SetAnchor(-221, 187);
            AddHeaderGroup();
            SetRegionGroupWidth(440);
            SetRegionGroupHeight(19);
            AddHeaderRegion(() => AddLine("Tracks per artist", "", "Center"));
            AddRegionGroup();
            SetRegionGroupWidth(19);
            SetRegionGroupHeight(19);
            AddPaddingRegion(() => AddSmallButton(tracksPerArtist > 1 ? "OtherDetract" : "OtherDetractOff", (h) =>
            {
                if (tracksPerArtist <= 1) return;
                if (Input.GetKey(LeftShift)) tracksPerArtist = 1;
                else tracksPerArtist--;
                CDesktop.RespawnAll();
            }));
            AddRegionGroup();
            SetRegionGroupWidth(402);
            SetRegionGroupHeight(19);
            AddPaddingRegion(() => AddLine(tracksPerArtist + "", "", "Center"));
            AddRegionGroup();
            SetRegionGroupWidth(19);
            SetRegionGroupHeight(19);
            var arts = library.originalArtists.Where(x => artistBattleParticipants[x.ID].Value());
            var max = arts.Count() > 0 ? arts.Min(x => x.releases.Where(y => ratings.ContainsKey(y.ID)).Sum(y => y.tracks.Count(z => !z.excluded))) : 1;
            if (tracksPerArtist > max) tracksPerArtist = max;
            AddPaddingRegion(() => AddSmallButton(tracksPerArtist < max ? "OtherAdd" : "OtherAddOff", (h) =>
            {
                if (tracksPerArtist >= max) return;
                if (Input.GetKey(LeftShift)) tracksPerArtist = max;
                else tracksPerArtist++;
                CDesktop.RespawnAll();
            }));
        }),
        new("ArtistBattleFinish", () => {
            SetAnchor(-221, -193);
            AddHeaderGroup();
            SetRegionGroupWidth(440);
            SetRegionGroupHeight(19);
            AddButtonRegion(() =>
            {
                AddLine("Generate", "", "Center");
            },
            (h) =>
            {
                var arts = library.originalArtists.Where(x => artistBattleParticipants[x.ID].Value());
                if (arts.Count() < perRound || arts.Count() * tracksPerArtist % perRound != 0) return;
                currentRound = 0;
                artistBattle = new ArtistBattle(library.originalArtists.Where(x => artistBattleParticipants[x.ID].Value()).ToList());
                SpawnDesktopBlueprint("ArtistBattle");
                CloseDesktop("PrepareArtistBattle");
            });
        }),
        new("ArtistBattleArtists", () => {
            var rowAmount = 15;
            var thisWindow = CDesktop.LBWindow();
            var list = library.artists.Where(x => x.name != "Various artists" && (!hideArtistsOfExcludedCountries.Value() || hideArtistsOfExcludedCountries.Value() && countryFiltering[x.country].Value())).ToList();
            CDesktop.quickInputWindow = thisWindow;
            thisWindow.SetPaginationSingleStep(() => list.Count, rowAmount);
            SetAnchor(-221, 149);
            AddHeaderGroup();
            SetRegionGroupWidth(440);
            AddPaddingRegion(() =>
            {
                AddCheckbox(hideArtistsOfExcludedCountries);
                AddLine("Hide artists of excluded countries");
            });
            AddRegionGroup();
            SetRegionGroupWidth(37);
            AddButtonRegion(() => AddLine("#", "", "Right"),
                (h) =>
                {
                    library.artists.Reverse();
                }
            );
            for (int i = thisWindow.pagination() == 0 ? 0 : list.Count - thisWindow.pagination() < rowAmount ? list.Count - (thisWindow.pagination() + 1) : 0; i < rowAmount; i++)
            {
                var index = i;
                if (list.Count > index + thisWindow.pagination())
                    AddHeaderRegion(() => AddLine(1 + index + thisWindow.pagination() + "", "", "Right"));
                else
                    AddPaddingRegion(() => { AddLine(""); });
            }
            AddPaddingRegion(() => AddLine(""));
            AddRegionGroup();
            SetRegionGroupWidth(219);
            AddButtonRegion(() => AddLine("Name"),
                (h) =>
                {
                    library.artists = (lastSort == "Name" ? library.artists.OrderByDescending(x => x.name) : library.artists.OrderBy(x => x.name)).ToList();
                    lastSort = lastSort == "Name" ? "" : "Name";
                }
            );
            for (int i = thisWindow.pagination() == 0 ? 0 : list.Count - thisWindow.pagination() < rowAmount ? list.Count - (thisWindow.pagination() + 1) : 0; i < rowAmount; i++)
            {
                var index = i;
                if (list.Count > index + thisWindow.pagination())
                    AddButtonRegion(() =>
                    {
                        var artist = list[index + thisWindow.pagination()];
                        AddLine(artist.name);
                        AddCheckbox(artistBattleParticipants[artist.ID], artistBattleParticipants.Select(x => x.Value).ToList());
                    },
                    (h) => { });
                else
                    AddPaddingRegion(() => { AddLine(""); });
            }
            AddPaddingRegion(() => AddLine(library.artists.Count + " out of " + library.originalArtists.Count + " artists", "DarkGray"));
            AddRegionGroup();
            SetRegionGroupWidth(55);
            AddButtonRegion(() => AddLine("Country"),
                (h) =>
                {
                    library.artists = (lastSort == "Country" ? library.artists.OrderByDescending(x => x.country) : library.artists.OrderBy(x => x.country)).ToList();
                    lastSort = lastSort == "Country" ? "" : "Country";
                }
            );
            for (int i = thisWindow.pagination() == 0 ? 0 : list.Count - thisWindow.pagination() < rowAmount ? list.Count - (thisWindow.pagination() + 1) : 0; i < rowAmount; i++)
            {
                var index = i;
                if (list.Count > index + thisWindow.pagination())
                    AddButtonRegion(() =>
                    {
                        var artist = list[index + thisWindow.pagination()];
                        AddLine(countryCodes.ContainsKey(artist.country) ? Country.countryCodes[artist.country] : "???", "", "Right");
                    },
                    (h) => { });
                else
                    AddPaddingRegion(() => { AddLine(""); });
            }
            AddPaddingRegion(() => AddLine(""));
            AddRegionGroup();
            SetRegionGroupWidth(55);
            AddButtonRegion(() => AddLine("Points"),
                (h) =>
                {
                    library.artists = (lastSort == "Points" ? library.artists.OrderByDescending(y => y.releases.Sum(x => ratings.ContainsKey(x.ID) ? ratings[x.ID].listPoints : 0)) : library.artists.OrderBy(y => y.releases.Sum(x => ratings.ContainsKey(x.ID) ? ratings[x.ID].listPoints : 0))).ToList();
                    lastSort = lastSort == "Points" ? "" : "Points";
                }
            );
            for (int i = thisWindow.pagination() == 0 ? 0 : list.Count - thisWindow.pagination() < rowAmount ? list.Count - (thisWindow.pagination() + 1) : 0; i < rowAmount; i++)
            {
                var index = i;
                if (list.Count > index + thisWindow.pagination())
                    AddButtonRegion(() =>
                    {
                        var artist = list[index + thisWindow.pagination()];
                        AddLine(artist.releases.Sum(x => ratings.ContainsKey(x.ID) ? ratings[x.ID].listPoints : 0) + "", "", "Right");
                    },
                    (h) => { });
                else
                    AddPaddingRegion(() => { AddLine(""); });
            }
            AddPaddingRegion(() => AddLine(""));
            AddRegionGroup();
            SetRegionGroupWidth(55);
            AddButtonRegion(() => AddLine("RTracks"),
                (h) =>
                {
                    library.artists = (lastSort == "Rated tracks" ? library.artists.OrderByDescending(y => y.releases.Sum(x => ratings.ContainsKey(x.ID) ? x.tracks.Count(z => !z.excluded) : 0)) : library.artists.OrderBy(y => y.releases.Sum(x => ratings.ContainsKey(x.ID) ? x.tracks.Count(z => !z.excluded) : 0))).ToList();
                    lastSort = lastSort == "Rated tracks" ? "" : "Rated tracks";
                }
            );
            for (int i = thisWindow.pagination() == 0 ? 0 : list.Count - thisWindow.pagination() < rowAmount ? list.Count - (thisWindow.pagination() + 1) : 0; i < rowAmount; i++)
            {
                var index = i;
                if (list.Count > index + thisWindow.pagination())
                    AddButtonRegion(() =>
                    {
                        var artist = list[index + thisWindow.pagination()];
                        AddLine(artist.releases.Sum(x => ratings.ContainsKey(x.ID) ? x.tracks.Count(z => !z.excluded) : 0) + "", "", "Right");
                    },
                    (h) => { });
                else
                    AddPaddingRegion(() => { AddLine(""); });
            }
            AddPaddingRegion(() => AddLine(""));
        }),
        new("ArtistBattleArtistsScrollbarUp", () => {
            SetAnchor(200, 130);
            AddRegionGroup();
            SetRegionGroupWidth(19);
            AddPaddingRegion(() =>
            {
                var window = CDesktop.windows.Find(x => x.title == "ArtistBattleArtists");
                if (window.pagination() > 0)
                    AddSmallButton("OtherPageUp", (h) =>
                    {
                        window.DecrementPagination();
                        CDesktop.RespawnAll();
                        Respawn("ArtistsScrollbarUp", true);
                        Respawn("ArtistsScrollbar", true);
                        Respawn("ArtistsScrollbarDown", true);
                    });
                else AddSmallButton("OtherPageUpOff");
            });
        }),
        new("ArtistBattleArtistsScrollbar", () => {
            SetAnchor(200, 111);
            AddRegionGroup();
            SetRegionGroupWidth(19);
            SetRegionGroupHeight(281);
            AddPaddingRegion(() => AddLine(""));
        }),
        new("ArtistBattleArtistsScrollbarDown", () => {
            SetAnchor(200, -174);
            AddRegionGroup();
            SetRegionGroupWidth(19);
            AddPaddingRegion(() =>
            {
                var window = CDesktop.windows.Find(x => x.title == "ArtistBattleArtists");
                if (window.pagination() < window.maxPagination())
                    AddSmallButton("OtherPageDown", (h) =>
                    {
                        window.IncrementPagination();
                        CDesktop.RespawnAll();
                        Respawn("ArtistsScrollbarUp", true);
                        Respawn("ArtistsScrollbar", true);
                        Respawn("ArtistsScrollbarDown", true);
                    });
                else AddSmallButton("OtherPageDownOff");
            });
        }),

        //Export album chart
        new("SquareChartOffset", () => {
            SetAnchor(-145, 95);
            AddHeaderGroup();
            SetRegionGroupWidth(294);
            SetRegionGroupHeight(19);
            AddHeaderRegion(() => AddLine("Offset between each album art", "", "Center"));
            AddRegionGroup();
            SetRegionGroupWidth(146);
            SetRegionGroupHeight(19);
            if (!squareChartOffset) AddHeaderRegion(() => AddLine("No", "", "Center"));
            else AddButtonRegion(() =>
            {
                AddLine("No", "", "Center");
            },
            (h) =>
            {
                squareChartOffset = false;
                CDesktop.RespawnAll();
            });
            AddRegionGroup();
            SetRegionGroupWidth(147);
            SetRegionGroupHeight(19);
            if (squareChartOffset) AddHeaderRegion(() => AddLine("Yes", "", "Center"));
            else AddButtonRegion(() =>
            {
                AddLine("Yes", "", "Center");
            },
            (h) =>
            {
                squareChartOffset = true;
                CDesktop.RespawnAll();
            });
        }),
        new("SquareChartWidth", () => {
            SetAnchor(-145, 57);
            AddHeaderGroup();
            SetRegionGroupWidth(292);
            SetRegionGroupHeight(19);
            AddHeaderRegion(() => AddLine("Width of the chart", "", "Center"));
            AddRegionGroup();
            SetRegionGroupWidth(19);
            SetRegionGroupHeight(19);
            AddPaddingRegion(() => AddSmallButton(squareChartXSize > 1 ? "OtherDetract" : "OtherDetractOff", (h) =>
            {
                if (squareChartXSize <= 1) return;
                if (Input.GetKey(LeftShift)) squareChartXSize = 1;
                else squareChartXSize--;
                CDesktop.RespawnAll();
            }));
            AddRegionGroup();
            SetRegionGroupWidth(254);
            SetRegionGroupHeight(19);
            AddPaddingRegion(() => AddLine(squareChartXSize + "", "", "Center"));
            AddRegionGroup();
            SetRegionGroupWidth(19);
            SetRegionGroupHeight(19);
            AddPaddingRegion(() => AddSmallButton(squareChartXSize < 200 ? "OtherAdd" : "OtherAddOff", (h) =>
            {
                if (Input.GetKey(LeftShift)) squareChartXSize += 10;
                else squareChartXSize++;
                if (squareChartXSize > 200) squareChartXSize = 200;
                CDesktop.RespawnAll();
            }));
        }),
        new("SquareChartHeight", () => {
            SetAnchor(-145, 19);
            AddHeaderGroup();
            SetRegionGroupWidth(292);
            SetRegionGroupHeight(19);
            AddHeaderRegion(() => AddLine("Height of the chart", "", "Center"));
            AddRegionGroup();
            SetRegionGroupWidth(19);
            SetRegionGroupHeight(19);
            AddPaddingRegion(() => AddSmallButton(squareChartYSize > 1 ? "OtherDetract" : "OtherDetractOff", (h) =>
            {
                if (squareChartYSize <= 1) return;
                if (Input.GetKey(LeftShift)) squareChartYSize = 1;
                else squareChartYSize--;
                CDesktop.RespawnAll();
            }));
            AddRegionGroup();
            SetRegionGroupWidth(254);
            SetRegionGroupHeight(19);
            AddPaddingRegion(() => AddLine(squareChartYSize + "", "", "Center"));
            AddRegionGroup();
            SetRegionGroupWidth(19);
            SetRegionGroupHeight(19);
            AddPaddingRegion(() => AddSmallButton(squareChartYSize < 200 ? "OtherAdd" : "OtherAddOff", (h) =>
            {
                if (Input.GetKey(LeftShift)) squareChartYSize += 10;
                else squareChartYSize++;
                if (squareChartYSize > 200) squareChartYSize = 200;
                CDesktop.RespawnAll();
            }));
        }),
        new("SquareChartFinish", () => {
            SetAnchor(-145, -19);
            AddHeaderGroup();
            SetRegionGroupWidth(292);
            SetRegionGroupHeight(19);
            AddHeaderRegion(() => AddLine("Amount of albums:", "", "Center"));
            AddPaddingRegion(() => AddLine(squareChartXSize + " × " + squareChartYSize + " = " + (squareChartXSize * squareChartYSize), "", "Center"));
            AddButtonRegion(() =>
            {
                AddLine("Generate", "", "Center");
            },
            (h) =>
            {
                Exporting.ExportSquareChart(library.releases.Where(x => x.GetRating() > 0).ToList(), squareChartXSize, squareChartYSize, squareChartOffset);
            });
        }),

        //Export scaled album chart
        new("ScaledChartPerfectFit", () => {
            SetAnchor(-145, 95);
            AddHeaderGroup();
            SetRegionGroupWidth(294);
            SetRegionGroupHeight(19);
            AddHeaderRegion(() => AddLine("Perfect fit on each row", "", "Center"));
            AddRegionGroup();
            SetRegionGroupWidth(146);
            SetRegionGroupHeight(19);
            if (!scaledChartPerfectFit) AddHeaderRegion(() => AddLine("No", "", "Center"));
            else AddButtonRegion(() =>
            {
                AddLine("No", "", "Center");
            },
            (h) =>
            {
                scaledChartPerfectFit = false;
                var list = library.releases.Where(x => x.GetRating() > 0).ToList();
                Exporting.GenerateScaledChartBlueprint(list.Count, scaledChartPerfectFit, scaledChartFirstRowAmount, scaledChartRowAmount);
                CDesktop.RespawnAll();
            });
            AddRegionGroup();
            SetRegionGroupWidth(147);
            SetRegionGroupHeight(19);
            if (scaledChartPerfectFit) AddHeaderRegion(() => AddLine("Yes", "", "Center"));
            else AddButtonRegion(() =>
            {
                AddLine("Yes", "", "Center");
            },
            (h) =>
            {
                scaledChartPerfectFit = true;
                var list = library.releases.Where(x => x.GetRating() > 0).ToList();
                Exporting.GenerateScaledChartBlueprint(list.Count, scaledChartPerfectFit, scaledChartFirstRowAmount, scaledChartRowAmount);
                CDesktop.RespawnAll();
            });
        }),
        new("ScaledChartFirstRowSize", () => {
            SetAnchor(-145, 57);
            AddHeaderGroup();
            SetRegionGroupWidth(292);
            SetRegionGroupHeight(19);
            AddHeaderRegion(() => AddLine("First row cover amount", "", "Center"));
            AddRegionGroup();
            SetRegionGroupWidth(19);
            SetRegionGroupHeight(19);
            AddPaddingRegion(() => AddSmallButton(scaledChartFirstRowAmount > 1 ? "OtherDetract" : "OtherDetractOff", (h) =>
            {
                if (scaledChartFirstRowAmount <= 1) return;
                if (Input.GetKey(LeftShift)) scaledChartFirstRowAmount = 1;
                else scaledChartFirstRowAmount--;
                var list = library.releases.Where(x => x.GetRating() > 0).ToList();
                Exporting.GenerateScaledChartBlueprint(list.Count, scaledChartPerfectFit, scaledChartFirstRowAmount, scaledChartRowAmount);
                CDesktop.RespawnAll();
            }));
            AddRegionGroup();
            SetRegionGroupWidth(254);
            SetRegionGroupHeight(19);
            AddPaddingRegion(() => AddLine(scaledChartFirstRowAmount + "", "", "Center"));
            AddRegionGroup();
            SetRegionGroupWidth(19);
            SetRegionGroupHeight(19);
            AddPaddingRegion(() => AddSmallButton(scaledChartFirstRowAmount < 200 ? "OtherAdd" : "OtherAddOff", (h) =>
            {
                if (Input.GetKey(LeftShift)) scaledChartFirstRowAmount += 10;
                else scaledChartFirstRowAmount++;
                if (scaledChartFirstRowAmount > 200) scaledChartFirstRowAmount = 200;
                var list = library.releases.Where(x => x.GetRating() > 0).ToList();
                Exporting.GenerateScaledChartBlueprint(list.Count, scaledChartPerfectFit, scaledChartFirstRowAmount, scaledChartRowAmount);
                CDesktop.RespawnAll();
            }));
        }),
        new("ScaledChartRowAmount", () => {
            SetAnchor(-145, 19);
            AddHeaderGroup();
            SetRegionGroupWidth(292);
            SetRegionGroupHeight(19);
            AddHeaderRegion(() => AddLine("Row amount of the chart", "", "Center"));
            AddRegionGroup();
            SetRegionGroupWidth(19);
            SetRegionGroupHeight(19);
            AddPaddingRegion(() => AddSmallButton(scaledChartRowAmount > 1 ? "OtherDetract" : "OtherDetractOff", (h) =>
            {
                if (scaledChartRowAmount <= 1) return;
                if (Input.GetKey(LeftShift)) scaledChartRowAmount = 1;
                else scaledChartRowAmount--;
                var list = library.releases.Where(x => x.GetRating() > 0).ToList();
                Exporting.GenerateScaledChartBlueprint(list.Count, scaledChartPerfectFit, scaledChartFirstRowAmount, scaledChartRowAmount);
                CDesktop.RespawnAll();
            }));
            AddRegionGroup();
            SetRegionGroupWidth(254);
            SetRegionGroupHeight(19);
            AddPaddingRegion(() => AddLine(scaledChartRowAmount + "", "", "Center"));
            AddRegionGroup();
            SetRegionGroupWidth(19);
            SetRegionGroupHeight(19);
            AddPaddingRegion(() => AddSmallButton(scaledChartRowAmount < 200 ? "OtherAdd" : "OtherAddOff", (h) =>
            {
                if (Input.GetKey(LeftShift)) scaledChartRowAmount += 10;
                else scaledChartRowAmount++;
                if (scaledChartRowAmount > 200) scaledChartRowAmount = 200;
                var list = library.releases.Where(x => x.GetRating() > 0).ToList();
                Exporting.GenerateScaledChartBlueprint(list.Count, scaledChartPerfectFit, scaledChartFirstRowAmount, scaledChartRowAmount);
                CDesktop.RespawnAll();
            }));
        }),
        new("ScaledChartFinish", () => {
            SetAnchor(-145, -19);
            AddHeaderGroup();
            SetRegionGroupWidth(292);
            SetRegionGroupHeight(19);
            AddHeaderRegion(() => AddLine("Amount of albums:", "", "Center"));
            AddPaddingRegion(() => AddLine((Exporting.scaledChartBlueprint == null ? 0 : Exporting.scaledChartBlueprint.Sum(x => x.Item2)) + "", "", "Center"));
            AddButtonRegion(() =>
            {
                AddLine("Generate", "", "Center");
            },
            (h) =>
            {
                Exporting.ExportScaledChart(library.releases.Where(x => x.GetRating() > 0).ToList());
            });
        }),
        new("SequenceChartSplitOnYears", () => {
            SetAnchor(-145, 57);
            AddHeaderGroup();
            SetRegionGroupWidth(294);
            SetRegionGroupHeight(19);
            AddHeaderRegion(() => AddLine("Split chart on years", "", "Center"));
            AddRegionGroup();
            SetRegionGroupWidth(146);
            SetRegionGroupHeight(19);
            if (!sequenceChartSplitOnYears) AddHeaderRegion(() => AddLine("No", "", "Center"));
            else AddButtonRegion(() =>
            {
                AddLine("No", "", "Center");
            },
            (h) =>
            {
                sequenceChartSplitOnYears = false;
                CDesktop.RespawnAll();
            });
            AddRegionGroup();
            SetRegionGroupWidth(147);
            SetRegionGroupHeight(19);
            if (sequenceChartSplitOnYears) AddHeaderRegion(() => AddLine("Yes", "", "Center"));
            else AddButtonRegion(() =>
            {
                AddLine("Yes", "", "Center");
            },
            (h) =>
            {
                sequenceChartSplitOnYears = true;
                CDesktop.RespawnAll();
            });
        }),
        new("SequenceChartSplitOnDecades", () => {
            SetAnchor(-145, 19);
            AddHeaderGroup();
            SetRegionGroupWidth(294);
            SetRegionGroupHeight(19);
            AddHeaderRegion(() => AddLine("Split chart on decades", "", "Center"));
            AddRegionGroup();
            SetRegionGroupWidth(146);
            SetRegionGroupHeight(19);
            if (!sequenceChartSplitOnDecades) AddHeaderRegion(() => AddLine("No", "", "Center"));
            else AddButtonRegion(() =>
            {
                AddLine("No", "", "Center");
            },
            (h) =>
            {
                sequenceChartSplitOnDecades = false;
                CDesktop.RespawnAll();
            });
            AddRegionGroup();
            SetRegionGroupWidth(147);
            SetRegionGroupHeight(19);
            if (sequenceChartSplitOnDecades) AddHeaderRegion(() => AddLine("Yes", "", "Center"));
            else AddButtonRegion(() =>
            {
                AddLine("Yes", "", "Center");
            },
            (h) =>
            {
                sequenceChartSplitOnDecades = true;
                CDesktop.RespawnAll();
            });
        }),
        new("SequenceChartFinish", () => {
            SetAnchor(-145, -19);
            AddHeaderGroup();
            SetRegionGroupWidth(292);
            SetRegionGroupHeight(19);
            AddHeaderRegion(() => AddLine("Amount of albums:", "", "Center"));
            var all = library.releases.Where(x => x.GetRating() > 0).Count();
            AddPaddingRegion(() => AddLine(all > 70 ? "70 (" + all + ")" : all + "", "", "Center"));
            AddButtonRegion(() =>
            {
                AddLine("Generate", "", "Center");
            },
            (h) =>
            {
                Exporting.ExportSequenceChart(library.releases.Where(x => x.GetRating() > 0).Take(70).ToList(), sequenceChartSplitOnYears, sequenceChartSplitOnDecades);
            });
        }),

        //Refetch library
        new("LibraryRefetchFailure", () => {
            SetAnchor(Center);
            AddHeaderGroup();
            SetRegionGroupWidth(400);
            AddHeaderRegion(() => AddLine("Failed to fetch the online library.", "", "Center"));
            AddButtonRegion(() =>
            {
                AddLine("Okay", "", "Center");
            },
            (h) =>
            {
                CloseDesktop(CDesktop.title);
                SpawnDesktopBlueprint("MusicReleases");
            });
        }),
        new("LibraryRefetchSuccess", () => {
            SetAnchor(Center);
            AddHeaderGroup();
            SetRegionGroupWidth(400);
            AddHeaderRegion(() => AddLine("Successfully fetched the online library.", "", "Center"));
            AddPaddingRegion(() =>
            {
                if (refetchLibraryArtistCount == library.originalArtists.Count)
                    AddLine("No difference in artist count found.", "", "Center");
                else if (refetchLibraryArtistCount > library.originalArtists.Count)
                    AddLine("The newly downloaded library has " + (refetchLibraryArtistCount - library.originalArtists.Count) + " artists less.", "", "Center");
                else if (refetchLibraryArtistCount < library.originalArtists.Count)
                    AddLine("The newly downloaded library has " + (library.originalArtists.Count - refetchLibraryArtistCount) + " artists more.", "", "Center");
                if (refetchLibraryReleasesCount == library.originalReleases.Count)
                    AddLine("No difference in release count found.", "", "Center");
                else if (refetchLibraryReleasesCount > library.originalReleases.Count)
                    AddLine("The newly downloaded library has " + (refetchLibraryReleasesCount - library.originalReleases.Count) + " releases less.", "", "Center");
                else if (refetchLibraryReleasesCount < library.originalReleases.Count)
                    AddLine("The newly downloaded library has " + (library.originalReleases.Count - refetchLibraryReleasesCount) + " releases more.", "", "Center");
            });
            AddButtonRegion(() =>
            {
                AddLine("Okay", "", "Center");
            },
            (h) =>
            {
                CloseDesktop(CDesktop.title);
                SpawnDesktopBlueprint("MusicReleases");
            });
        }),

        //Create new album
        new("CreateNewAlbumReleaseName", () => {
            SetAnchor(Center);
            AddHeaderGroup();
            SetRegionGroupWidth(385);
            AddHeaderRegion(() =>
            {
                AddLine("Release name:", "DarkGray");
                AddInputLine(String.createNewAlbumReleaseName);
            });
            AddPaddingRegion(() =>
            {
                AddLine("Type the release name.", "DarkGray");
            });
            AddPaddingRegion(() =>
            {
                AddLine("There can be multiple albums with the same name in the library.", "DarkGray");
                AddLine("Avoid duplicating albums of the same artist.", "DarkGray");
            });
            var sorted = library.originalReleases.OrderBy(x => x.GetName().Length).ToList();
            var fitting = String.createNewAlbumReleaseName.Value() == "" ? new() : sorted.FindAll(x => x.GetName().ToLower().Contains(String.createNewAlbumReleaseName.Value().ToLower()));
            fitting = fitting.OrderBy(x => x.GetName().Length).ToList();
            AddEmptyRegion();
            for (int i = 0; i < 5; i++)
            {
                var index = i;
                if (fitting.Count > i)
                {
                    AddHeaderRegion(() =>
                    {
                        AddLine(fitting[index].name);
                    });
                    AddPaddingRegion(() =>
                    {
                        AddLine("By: " + fitting[index].artist, "DarkGray");
                    });
                    AddEmptyRegion();
                }
            }
            if (String.createNewAlbumReleaseName.Value() != "" && fitting.Count == 0)
            {
                AddPaddingRegion(() =>
                {
                    AddLine("Found no existing albums of similiar name.", "DarkGray");
                });
                AddEmptyRegion();
            }
            AddButtonRegion(() =>
            {
                AddLine("Next Step", "", "Center");
            },
            (h) =>
            {
                CloseDesktop(CDesktop.title);
                SpawnDesktopBlueprint("CreateNewAlbumReleaseDate");
            });
        }),
        new("CreateNewAlbumReleaseDate", () => {
            SetAnchor(Center);
            AddHeaderGroup();
            SetRegionGroupWidth(385);
            AddHeaderRegion(() =>
            {
                AddLine("Release date:", "DarkGray");
                AddInputLine(String.createNewAlbumReleaseDate);
            });
            AddPaddingRegion(() =>
            {
                AddLine("Type in the release date of the album.", "DarkGray");
            });
            AddEmptyRegion();
            AddPaddingRegion(() =>
            {
                AddLine("Example #1: ", "DarkGray");
                AddText("25.11.2025", "Gray");
            });
            AddPaddingRegion(() =>
            {
                AddLine("Example #2: ", "DarkGray");
                AddText("11.2025", "Gray");
            });
            AddPaddingRegion(() =>
            {
                AddLine("Example #3: ", "DarkGray");
                AddText("25 November 2025", "Gray");
            });
            AddPaddingRegion(() =>
            {
                AddLine("Example #4: ", "DarkGray");
                AddText("November 2025", "Gray");
            });
            AddPaddingRegion(() =>
            {
                AddLine("Example #5: ", "DarkGray");
                AddText("2025", "Gray");
            });
            AddEmptyRegion();
            AddButtonRegion(() =>
            {
                AddLine("Next Step", "", "Center");
            },
            (h) =>
            {
                CloseDesktop(CDesktop.title);
                SpawnDesktopBlueprint("CreateNewAlbumReleaseType");
            });
        }),
        new("CreateNewAlbumReleaseType", () => {
            SetAnchor(Center);
            AddHeaderGroup();
            SetRegionGroupWidth(385);
            AddHeaderRegion(() =>
            {
                AddLine("Select all valid release types.", "DarkGray");
            });
            foreach (var releaseType in releaseTypes)
                AddPaddingRegion(() =>
                {
                    AddLine(releaseType.name);
                    AddCheckbox(createNewAlbumReleaseTypeFiltering[releaseType.name], createNewAlbumReleaseTypeFiltering.Select(x => x.Value).ToList());
                });
            AddEmptyRegion();
            AddButtonRegion(() =>
            {
                AddLine("Next Step", "", "Center");
            },
            (h) =>
            {
                CloseDesktop(CDesktop.title);
                SpawnDesktopBlueprint("CreateNewAlbumReleaseGenres");
            });
        }),
        new("CreateNewAlbumReleaseGenres", () => {
            SetAnchor(Center);
            AddHeaderGroup();
            SetRegionGroupWidth(385);
            AddHeaderRegion(() =>
            {
                AddLine("Release genres:", "DarkGray");
                AddInputLine(String.createNewAlbumGenres);
            });
            AddPaddingRegion(() =>
            {
                AddLine("Type genres of this album.", "DarkGray");
            });
            AddEmptyRegion();
            AddPaddingRegion(() =>
            {
                AddLine("Example #1: ", "DarkGray");
                AddText("Progressive Rock, Pop", "Gray");
            });
            AddPaddingRegion(() =>
            {
                AddLine("Example #2: ", "DarkGray");
                AddText("Ambient", "Gray");
            });
            AddEmptyRegion();
            AddButtonRegion(() =>
            {
                AddLine("Next Step", "", "Center");
            },
            (h) =>
            {
                CloseDesktop(CDesktop.title);
                SpawnDesktopBlueprint("CreateNewAlbumReleaseLanguages");
            });
        }),
        new("CreateNewAlbumReleaseLanguages", () => {
            SetAnchor(Center);
            AddHeaderGroup();
            SetRegionGroupWidth(385);
            AddHeaderRegion(() =>
            {
                AddLine("Release languages:", "DarkGray");
                AddInputLine(String.createNewAlbumLanguages);
            });
            AddPaddingRegion(() =>
            {
                AddLine("Type the languages in which the vocals are performed.", "DarkGray");
            });
            AddPaddingRegion(() =>
            {
                AddLine("Leave the input empty in case of lack of vocals.", "DarkGray");
            });
            AddEmptyRegion();
            AddPaddingRegion(() =>
            {
                AddLine("Example #1: ", "DarkGray");
                AddText("French, Spanish", "Gray");
            });
            AddPaddingRegion(() =>
            {
                AddLine("Example #2: ", "DarkGray");
                AddText("English", "Gray");
            });
            AddEmptyRegion();
            AddButtonRegion(() =>
            {
                AddLine("Next Step", "", "Center");
            },
            (h) =>
            {
                CloseDesktop(CDesktop.title);
                SpawnDesktopBlueprint("CreateNewAlbumReleaseTracklist");
            });
        }),
        new("CreateNewAlbumReleaseTracklist", () => {
            SetAnchor(Center);
            AddHeaderGroup();
            SetRegionGroupWidth(385);
            AddHeaderRegion(() =>
            {
                AddLine("Release tracklist:", "DarkGray");
                AddInputLine(String.createNewAlbumTracklist);
            });
            AddPaddingRegion(() =>
            {
                AddLine("Paste in the tracklist from RYM's release page.", "DarkGray");
            });
            AddPaddingRegion(() =>
            {
                AddLine("Make sure the pasted tracklist contains track lengths.", "DarkGray");
                AddLine("otherwise the process will fail.", "DarkGray");
            });
            AddEmptyRegion();
            AddPaddingRegion(() =>
            {
                AddLine("Example #1: ", "DarkGray");
                AddText("A1\\r\\nTime Was\\r\\n9:42\\r\\nA2\\r\\nSometime World\\r\\n6:55\\r\\nA3\\r\\nBlowin' Free...", "Gray");
            });
            AddPaddingRegion(() =>
            {
                AddLine("Example #2: ", "DarkGray");
                AddText("\\r\\nLeft\\r\\n1.1\\r\\nSomewhat Damagedlyrics\\r\\n4:31\\r\\n1.2\\r\\nThe Day the World...", "Gray");
            });
            AddEmptyRegion();
            AddButtonRegion(() =>
            {
                AddLine("Next Step", "", "Center");
            },
            (h) =>
            {
                CloseDesktop(CDesktop.title);
                SpawnDesktopBlueprint("CreateNewAlbumArtistName");
            });
        }),
        new("CreateNewAlbumArtistName", () => {
            SetAnchor(Center);
            AddHeaderGroup();
            SetRegionGroupWidth(385);
            AddHeaderRegion(() =>
            {
                AddLine("Artist name:", "DarkGray");
                AddInputLine(String.createNewAlbumArtistName);
            });
            var sorted = library.originalArtists.OrderBy(x => x.GetName().Length).ToList();
            var fitting = String.createNewAlbumArtistName.Value() == "" ? new() : sorted.FindAll(x => x.GetName().ToLower().Contains(String.createNewAlbumArtistName.Value().ToLower()));
            fitting = fitting.OrderBy(x => x.GetName().Length).ToList();
            if (fitting.Count == 0)
                AddPaddingRegion(() =>
                {
                    AddLine("Type the artist name.", "DarkGray");
                });
            else
                AddPaddingRegion(() =>
                {
                    AddLine("Type the artist name or choose one of the artists below.", "DarkGray");
                });
            AddPaddingRegion(() =>
            {
                AddLine("Artist's name left empty will count as various artists.", "DarkGray");
                AddLine("Country choice will not be taken into consideration then.", "DarkGray");
            });
            AddEmptyRegion();
            for (int i = 0; i < 5; i++)
            {
                var index = i;
                if (fitting.Count > i)
                {
                    AddButtonRegion(() =>
                    {
                        AddLine(fitting[index].name);
                    },
                    (h) =>
                    {
                        String.createNewAlbumArtistName.Set(fitting[index].name);
                        String.createNewAlbumArtistCountry.Set(fitting[index].country);
                    });
                    AddPaddingRegion(() =>
                    {
                        AddLine("From: " + fitting[index].country, "DarkGray");
                    });
                    AddEmptyRegion();
                }
            }
            AddButtonRegion(() =>
            {
                AddLine("Next Step", "", "Center");
            },
            (h) =>
            {
                CloseDesktop(CDesktop.title);
                countryCodes = countryCodes.OrderByDescending(x => library.originalArtists.Count(y => y.country == x.Key)).ToDictionary(x => x.Key, x => x.Value);
                SpawnDesktopBlueprint("CreateNewAlbumArtistCountry");
            });
        }),
        new("CreateNewAlbumArtistCountryNextStep", () => {
            SetAnchor(-193, -190);
            AddHeaderGroup();
            SetRegionGroupWidth(385);
            AddButtonRegion(() =>
            {
                AddLine("Next Step", "", "Center");
            },
            (h) =>
            {
                CloseDesktop(CDesktop.title);
                SpawnDesktopBlueprint("CreateNewAlbumReleaseCoverURL");
            });
        }),
        new("CreateNewAlbumArtistCountry", () => {
            var rowAmount = 15;
            var thisWindow = CDesktop.LBWindow();
            var list = countryCodes.Select(x => x.Key).ToList();
            if (String.searchNewAlbumCountry.Value() != "")
                list = list.Where(x => x.ToLower().Contains(String.searchNewAlbumCountry.Value().ToLower())).ToList();
            CDesktop.quickInputWindow = thisWindow;
            thisWindow.SetPaginationSingleStep(() => list.Count, rowAmount);
            SetAnchor(Center, 0, 19);
            AddHeaderGroup();
            SetRegionGroupWidth(385);
            AddHeaderRegion(() =>
            {
                AddLine("Artist country: ", "DarkGray");
                AddText(String.createNewAlbumArtistCountry.Value(), "Gray");
                AddSmallButton("OtherReverse", (h) => { String.createNewAlbumArtistCountry.Set("-"); CDesktop.RespawnAll(); });
            });
            AddEmptyRegion();
            AddPaddingRegion(() => { AddLine("Search:", "DarkGray"); AddInputLine(String.searchNewAlbumCountry); AddSmallButton("OtherReverse", (h) => { String.searchNewAlbumCountry.Set(""); CDesktop.RespawnAll(); }); });
            AddRegionGroup();
            SetRegionGroupWidth(37);
            AddButtonRegion(() => AddLine("#", "", "Right"),
                (h) =>
                {
                    countryCodes.Reverse();
                }
            );
            for (int i = thisWindow.pagination() == 0 ? 0 : list.Count - thisWindow.pagination() < rowAmount ? list.Count - (thisWindow.pagination() + 1) : 0; i < rowAmount; i++)
            {
                var index = i;
                if (list.Count > index + thisWindow.pagination())
                    AddHeaderRegion(() => AddLine(1 + index + thisWindow.pagination() + "", "", "Right"));
                else
                    AddPaddingRegion(() => { AddLine(""); });
            }
            AddPaddingRegion(() => AddLine(""));
            AddRegionGroup();
            SetRegionGroupWidth(219);
            AddButtonRegion(() => AddLine("Name"),
                (h) =>
                {
                    countryCodes = (lastSort == "Name" ? countryCodes.OrderByDescending(x => x.Key) : countryCodes.OrderBy(x => x.Key)).ToDictionary(x => x.Key, x => x.Value);
                    lastSort = lastSort == "Name" ? "" : "Name";
                }
            );
            for (int i = thisWindow.pagination() == 0 ? 0 : list.Count - thisWindow.pagination() < rowAmount ? list.Count - (thisWindow.pagination() + 1) : 0; i < rowAmount; i++)
            {
                var index = i;
                if (list.Count > index + thisWindow.pagination())
                    AddButtonRegion(() =>
                    {
                        var country = list[index + thisWindow.pagination()];
                        AddLine(country);
                    },
                    (h) =>
                    {
                        var country = list[index + thisWindow.pagination()];
                        String.createNewAlbumArtistCountry.Set(country);
                    });
                else
                    AddPaddingRegion(() => { AddLine(""); });
            }
            AddPaginationLine();
            AddRegionGroup();
            SetRegionGroupWidth(55);
            AddButtonRegion(() => AddLine("Short"),
                (h) =>
                {
                    countryCodes = (lastSort == "Name" ? countryCodes.OrderByDescending(x => x.Value) : countryCodes.OrderBy(x => x.Value)).ToDictionary(x => x.Key, x => x.Value);
                    lastSort = lastSort == "Name" ? "" : "Name";
                }
            );
            for (int i = thisWindow.pagination() == 0 ? 0 : list.Count - thisWindow.pagination() < rowAmount ? list.Count - (thisWindow.pagination() + 1) : 0; i < rowAmount; i++)
            {
                var index = i;
                if (list.Count > index + thisWindow.pagination())
                    AddButtonRegion(() =>
                    {
                        var country = list[index + thisWindow.pagination()];
                        AddLine(countryCodes[country], "", "Right");
                    });
                else
                    AddPaddingRegion(() => { AddLine(""); });
            }
            AddPaddingRegion(() => AddLine(""));
            AddRegionGroup();
            SetRegionGroupWidth(55);
            AddButtonRegion(() => AddLine("Artists"),
                (h) =>
                {
                    countryCodes = (lastSort == "Artists" ? countryCodes.OrderBy(x => library.originalArtists.Count(y => y.country == x.Key)) : countryCodes.OrderByDescending(x => library.originalArtists.Count(y => y.country == x.Key))).ToDictionary(x => x.Key, x => x.Value);
                    lastSort = lastSort == "Artists" ? "" : "Artists";
                }
            );
            for (int i = thisWindow.pagination() == 0 ? 0 : list.Count - thisWindow.pagination() < rowAmount ? list.Count - (thisWindow.pagination() + 1) : 0; i < rowAmount; i++)
            {
                var index = i;
                if (list.Count > index + thisWindow.pagination())
                    AddButtonRegion(() =>
                    {
                        var country = list[index + thisWindow.pagination()];
                        AddLine(library.originalArtists.Count(x => x.country == country) + "", "", "Right");
                    },
                    (h) => { });
                else
                    AddPaddingRegion(() => { AddLine(""); });
            }
            AddPaddingRegion(() => AddLine(""));
        }),
        new("CreateNewAlbumArtistCountryScrollbarUp", () => {
            SetAnchor(173, 152);
            AddRegionGroup();
            SetRegionGroupWidth(19);
            AddPaddingRegion(() =>
            {
                var window = CDesktop.windows.Find(x => x.title == "CreateNewAlbumArtistCountry");
                if (window.pagination() > 0)
                    AddSmallButton("OtherPageUp", (h) =>
                    {
                        window.DecrementPagination();
                        CDesktop.RespawnAll();
                        Respawn("CreateNewAlbumArtistCountryScrollbarUp", true);
                        Respawn("CreateNewAlbumArtistCountryScrollbar", true);
                        Respawn("CreateNewAlbumArtistCountryScrollbarDown", true);
                    });
                else AddSmallButton("OtherPageUpOff");
            });
        }),
        new("CreateNewAlbumArtistCountryScrollbar", () => {
            SetAnchor(173, 133);
            AddRegionGroup();
            SetRegionGroupWidth(19);
            SetRegionGroupHeight(281);
            AddPaddingRegion(() => AddLine(""));
        }),
        new("CreateNewAlbumArtistCountryScrollbarDown", () => {
            SetAnchor(173, -152);
            AddRegionGroup();
            SetRegionGroupWidth(19);
            AddPaddingRegion(() =>
            {
                var window = CDesktop.windows.Find(x => x.title == "CreateNewAlbumArtistCountry");
                if (window.pagination() < window.maxPagination())
                    AddSmallButton("OtherPageDown", (h) =>
                    {
                        window.IncrementPagination();
                        CDesktop.RespawnAll();
                        Respawn("CreateNewAlbumArtistCountryScrollbarUp", true);
                        Respawn("CreateNewAlbumArtistCountryScrollbar", true);
                        Respawn("CreateNewAlbumArtistCountryScrollbarDown", true);
                    });
                else AddSmallButton("OtherPageDownOff");
            });
        }),
        new("CreateNewAlbumReleaseCoverURL", () => {
            SetAnchor(Center);
            AddHeaderGroup();
            SetRegionGroupWidth(385);
            AddHeaderRegion(() =>
            {
                AddLine("Album cover URL:", "DarkGray");
                AddInputLine(String.createNewAlbumCoverURL);
            });
            AddPaddingRegion(() =>
            {
                AddLine("Paste the album cover URL.", "DarkGray");
                AddLine("Don't copy links from RYM's release page as they are protected.", "DarkGray");
            });
            AddEmptyRegion();
            AddButtonRegion(() =>
            {
                AddLine("Finalize", "", "Center");
            },
            (h) =>
            {
                CreatePreviewRelease();
                CloseDesktop(CDesktop.title);
                SpawnDesktopBlueprint("CreateNewAlbumPreviewLoadCover");
            });
        }),
        new("CreateNewAlbumMenuBar", () => {
            SetAnchor(Bottom, 0, 10);
            AddRegionGroup();
            if (CDesktop.title == "CreateNewAlbumReleaseName") AddPaddingRegion(() => AddLine("Release Name"));
            else AddButtonRegion(() => AddLine("Release Name"), (h) => { var name = CDesktop.title; SpawnDesktopBlueprint("CreateNewAlbumReleaseName"); CloseDesktop(name); });
            AddRegionGroup();
            if (CDesktop.title == "CreateNewAlbumReleaseDate") AddPaddingRegion(() => AddLine("Release Date"));
            else AddButtonRegion(() => AddLine("Release Date"), (h) => { var name = CDesktop.title; SpawnDesktopBlueprint("CreateNewAlbumReleaseDate"); CloseDesktop(name); });
            AddRegionGroup();
            if (CDesktop.title == "CreateNewAlbumReleaseType") AddPaddingRegion(() => AddLine("Release Type"));
            else AddButtonRegion(() => AddLine("Release Type"),(h) => { var name = CDesktop.title; SpawnDesktopBlueprint("CreateNewAlbumReleaseType"); CloseDesktop(name); });
            AddRegionGroup();
            if (CDesktop.title == "CreateNewAlbumReleaseGenres") AddPaddingRegion(() => AddLine("Release Genres"));
            else AddButtonRegion(() => AddLine("Release Genres"),(h) => { var name = CDesktop.title; SpawnDesktopBlueprint("CreateNewAlbumReleaseGenres"); CloseDesktop(name); });
            AddRegionGroup();
            if (CDesktop.title == "CreateNewAlbumReleaseLanguages") AddPaddingRegion(() => AddLine("Release Languages"));
            else AddButtonRegion(() => AddLine("Release Languages"),(h) => { var name = CDesktop.title; SpawnDesktopBlueprint("CreateNewAlbumReleaseLanguages"); CloseDesktop(name); });
            AddRegionGroup();
            if (CDesktop.title == "CreateNewAlbumReleaseTracklist") AddPaddingRegion(() => AddLine("Release Tracklist"));
            else AddButtonRegion(() => AddLine("Release Tracklist"),(h) => { var name = CDesktop.title; SpawnDesktopBlueprint("CreateNewAlbumReleaseTracklist"); CloseDesktop(name); });
            AddRegionGroup();
            if (CDesktop.title == "CreateNewAlbumArtistName") AddPaddingRegion(() => AddLine("Artist Name"));
            else AddButtonRegion(() => AddLine("Artist Name"),(h) => { var name = CDesktop.title; SpawnDesktopBlueprint("CreateNewAlbumArtistName"); CloseDesktop(name); });
            AddRegionGroup();
            if (CDesktop.title == "CreateNewAlbumArtistCountry") AddPaddingRegion(() => AddLine("Artist Country"));
            else AddButtonRegion(() => AddLine("Artist Country"),(h) =>
            {
                var name = CDesktop.title;
                countryCodes = countryCodes.OrderByDescending(x => library.originalArtists.Count(y => y.country == x.Key)).ToDictionary(x => x.Key, x => x.Value);
                SpawnDesktopBlueprint("CreateNewAlbumArtistCountry");
                CloseDesktop(name);
            });
            AddRegionGroup();
            if (CDesktop.title == "CreateNewAlbumReleaseCoverURL") AddPaddingRegion(() => AddLine("Release Cover"));
            else AddButtonRegion(() => AddLine("Release Cover"),(h) => { var name = CDesktop.title; SpawnDesktopBlueprint("CreateNewAlbumReleaseCoverURL"); CloseDesktop(name); });
            AddRegionGroup();
            if (CDesktop.title == "CreateNewAlbumPreview") AddPaddingRegion(() => AddLine("Preview"));
            else AddButtonRegion(() => AddLine("Preview"),(h) =>
            {
                CreatePreviewRelease();
                CloseDesktop(CDesktop.title);
                SpawnDesktopBlueprint("CreateNewAlbumPreviewLoadCover");
            });
        }),
        new("SendingMail", () => {
            SetAnchor(Center);
            AddHeaderGroup();
            SetRegionGroupWidth(200);
            AddHeaderRegion(() =>
            {
                AddLine("Sending mail..", "", "Center");
            });
        }),
    };

    public static List<Blueprint> desktopBlueprints = new()
    {
        new("LoadingScreen", () =>
        {
            Cursor.cursor.SetCursor(CursorType.None);
            loadingScreenAim = library.originalReleases.Count + 1;
            SetDesktopBackground("Backgrounds/Default");
            SpawnWindowBlueprint("LoadingStatus");
            loadingStatusBar = CDesktop.LBWindow().LBRegionGroup().LBRegion().background.transform;
            UnityEngine.Object.Instantiate(loadingStatusBar, loadingStatusBar.parent);
            loadingStatusBar.localScale = new Vector3(0, 0, 0);
            loadingStatusBar.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Sprites/Fills/LoadingBar");
            loadingStatusBar.GetComponent<SpriteRenderer>().sortingOrder = 1;
        }),
        new("MusicReleases", () =>
        {
            library.ApplyFiltering();
            SetDesktopBackground("Backgrounds/Default");
            SpawnWindowBlueprint("MusicReleases");
            SpawnWindowBlueprint("MusicReleasesScrollbarUp");
            SpawnWindowBlueprint("MusicReleasesScrollbar");
            SpawnWindowBlueprint("MusicReleasesScrollbarDown");
            SpawnWindowBlueprint("ResetLibraryFiltering");
            SpawnWindowBlueprint("RollRandomRelease");
            SpawnWindowBlueprint("MenuBar");
            AddHotkey(Escape, () =>
            {
                CloseDesktop(CDesktop.title);
                SpawnDesktopBlueprint("Menu");
            });
            AddHotkey(Home, () =>
            {
                var window = CDesktop.windows.Find(x => x.title == "MusicReleases");
                window.PreparePagination();
                staticPagination["MusicReleases"] = 0;
                window.CorrectPagination();
                CDesktop.RespawnAll();
            });
            AddHotkey(End, () =>
            {
                var window = CDesktop.windows.Find(x => x.title == "MusicReleases");
                window.PreparePagination();
                staticPagination["MusicReleases"] = int.MaxValue;
                window.CorrectPagination();
                CDesktop.RespawnAll();
            });
            AddMousePaginationHotkeys("MusicReleases");
        }),
        new("MusicRelease", () =>
        {
            if (albumCovers.ContainsKey(musicRelease.ID + ""))
            {
                if (musicRelease.pallete == null)
                    musicRelease.GeneratePallete(albumCovers[musicRelease.ID + ""]);
                SetDesktopBackgroundAsGradient(musicRelease.pallete);
            }
            else SetDesktopBackground("Backgrounds/Default");
            SpawnWindowBlueprint("MusicRelease");
            SpawnWindowBlueprint("MusicReleaseCover");
            SpawnWindowBlueprint("MusicReleaseDescription");
            SpawnWindowBlueprint("MusicReleaseBottomLine");
            SpawnWindowBlueprint("MusicReleaseScrollbarUp");
            SpawnWindowBlueprint("MusicReleaseScrollbar");
            SpawnWindowBlueprint("MusicReleaseScrollbarDown");
            SpawnWindowBlueprint("RollRandomRelease");
            SpawnWindowBlueprint("CloseMusicRelease");
            SpawnWindowBlueprint("MenuBar");
            AddHotkey(Escape, () =>
            {
                CloseDesktop(CDesktop.title);
                CDesktop.RespawnAll();
            });
            AddHotkey(D, () =>
            {
                if (musicReleaseIndex < library.releases.Count - 1)
                {
                    musicRelease = library.releases[++musicReleaseIndex];
                    CDesktop.RespawnAll();
                    Respawn("MusicReleaseScrollbarUp", true);
                    Respawn("MusicReleaseScrollbar", true);
                    Respawn("MusicReleaseScrollbarDown", true);
                    SpawnAlbumTransition();
                    if (albumCovers.ContainsKey(musicRelease.ID + ""))
                    {
                        if (musicRelease.pallete == null)
                            musicRelease.GeneratePallete(albumCovers[musicRelease.ID + ""]);
                        SetDesktopBackgroundAsGradient(musicRelease.pallete);
                    }
                }
            });
            AddHotkey(D, () =>
            {
                if (musicReleaseIndex < library.releases.Count - 1)
                {
                    var temp = musicReleaseIndex;
                    musicReleaseIndex += (int)Math.Round(EuelerGrowth()) / 2;
                    if (musicReleaseIndex != temp)
                    {
                        musicRelease = library.releases[musicReleaseIndex];
                        CDesktop.RespawnAll();
                        Respawn("MusicReleaseScrollbarUp", true);
                        Respawn("MusicReleaseScrollbar", true);
                        Respawn("MusicReleaseScrollbarDown", true);
                        if (albumCovers.ContainsKey(musicRelease.ID + ""))
                        {
                            if (musicRelease.pallete == null)
                                musicRelease.GeneratePallete(albumCovers[musicRelease.ID + ""]);
                            SetDesktopBackgroundAsGradient(musicRelease.pallete);
                        }
                    }
                }
            }, false);
            AddHotkey(A, () =>
            {
                if (musicReleaseIndex > 0)
                {
                    musicRelease = library.releases[--musicReleaseIndex];
                    CDesktop.RespawnAll();
                    Respawn("MusicReleaseScrollbarUp", true);
                    Respawn("MusicReleaseScrollbar", true);
                    Respawn("MusicReleaseScrollbarDown", true);
                    SpawnAlbumTransition();
                    if (albumCovers.ContainsKey(musicRelease.ID + ""))
                    {
                        if (musicRelease.pallete == null)
                            musicRelease.GeneratePallete(albumCovers[musicRelease.ID + ""]);
                        SetDesktopBackgroundAsGradient(musicRelease.pallete);
                    }
                }
            });
            AddHotkey(A, () =>
            {
                if (musicReleaseIndex > 0)
                {
                    var temp = musicReleaseIndex;
                    musicReleaseIndex -= (int)Math.Round(EuelerGrowth()) / 2;
                    if (musicReleaseIndex != temp)
                    {
                        musicRelease = library.releases[musicReleaseIndex];
                        CDesktop.RespawnAll();
                        Respawn("MusicReleaseScrollbarUp", true);
                        Respawn("MusicReleaseScrollbar", true);
                        Respawn("MusicReleaseScrollbarDown", true);
                        if (albumCovers.ContainsKey(musicRelease.ID + ""))
                        {
                            if (musicRelease.pallete == null)
                                musicRelease.GeneratePallete(albumCovers[musicRelease.ID + ""]);
                            SetDesktopBackgroundAsGradient(musicRelease.pallete);
                        }
                    }
                }
            }, false);
            AddMousePaginationHotkeys("MusicRelease");
        }),
        new("Artists", () =>
        {
            showExcludedElements = new Bool(true);
            SetDesktopBackground("Backgrounds/Default");
            SpawnWindowBlueprint("Artists");
            SpawnWindowBlueprint("ArtistsScrollbarUp");
            SpawnWindowBlueprint("ArtistsScrollbar");
            SpawnWindowBlueprint("ArtistsScrollbarDown");
            SpawnWindowBlueprint("ResetLibraryFiltering");
            SpawnWindowBlueprint("MenuBar");
            AddHotkey(Escape, () =>
            {
                CloseDesktop(CDesktop.title);
                SpawnDesktopBlueprint("Menu");
            });
            AddMousePaginationHotkeys("Artists");
        }),
        new("Countries", () =>
        {
            showExcludedElements = new Bool(true);
            SetDesktopBackground("Backgrounds/Default");
            SpawnWindowBlueprint("Countries");
            SpawnWindowBlueprint("CountriesScrollbarUp");
            SpawnWindowBlueprint("CountriesScrollbar");
            SpawnWindowBlueprint("CountriesScrollbarDown");
            SpawnWindowBlueprint("ResetLibraryFiltering");
            SpawnWindowBlueprint("MenuBar");
            AddHotkey(Escape, () =>
            {
                CloseDesktop(CDesktop.title);
                SpawnDesktopBlueprint("Menu");
            });
            AddMousePaginationHotkeys("Countries");
        }),
        new("Genres", () =>
        {
            showExcludedElements = new Bool(true);
            SetDesktopBackground("Backgrounds/Default");
            SpawnWindowBlueprint("Genres");
            SpawnWindowBlueprint("GenresScrollbarUp");
            SpawnWindowBlueprint("GenresScrollbar");
            SpawnWindowBlueprint("GenresScrollbarDown");
            SpawnWindowBlueprint("ResetLibraryFiltering");
            SpawnWindowBlueprint("MenuBar");
            AddHotkey(Escape, () =>
            {
                CloseDesktop(CDesktop.title);
                SpawnDesktopBlueprint("Menu");
            });
            AddMousePaginationHotkeys("Genres");
        }),
        new("ReleaseTypes", () =>
        {
            showExcludedElements = new Bool(true);
            SetDesktopBackground("Backgrounds/Default");
            SpawnWindowBlueprint("ReleaseTypes");
            SpawnWindowBlueprint("ReleaseTypesScrollbarUp");
            SpawnWindowBlueprint("ReleaseTypesScrollbar");
            SpawnWindowBlueprint("ReleaseTypesScrollbarDown");
            SpawnWindowBlueprint("ResetLibraryFiltering");
            SpawnWindowBlueprint("MenuBar");
            AddHotkey(Escape, () =>
            {
                CloseDesktop(CDesktop.title);
                SpawnDesktopBlueprint("Menu");
            });
            AddMousePaginationHotkeys("ReleaseTypes");
        }),
        new("Years", () =>
        {
            showExcludedElements = new Bool(true);
            SetDesktopBackground("Backgrounds/Default");
            SpawnWindowBlueprint("Years");
            SpawnWindowBlueprint("YearsScrollbarUp");
            SpawnWindowBlueprint("YearsScrollbar");
            SpawnWindowBlueprint("YearsScrollbarDown");
            SpawnWindowBlueprint("ResetLibraryFiltering");
            SpawnWindowBlueprint("MenuBar");
            AddHotkey(Escape, () =>
            {
                CloseDesktop(CDesktop.title);
                SpawnDesktopBlueprint("Menu");
            });
            AddMousePaginationHotkeys("Years");
        }),
        new("Decades", () =>
        {
            showExcludedElements = new Bool(true);
            SetDesktopBackground("Backgrounds/Default");
            SpawnWindowBlueprint("Decades");
            SpawnWindowBlueprint("DecadesScrollbarUp");
            SpawnWindowBlueprint("DecadesScrollbar");
            SpawnWindowBlueprint("DecadesScrollbarDown");
            SpawnWindowBlueprint("ResetLibraryFiltering");
            SpawnWindowBlueprint("MenuBar");
            AddHotkey(Escape, () =>
            {
                CloseDesktop(CDesktop.title);
                SpawnDesktopBlueprint("Menu");
            });
            AddMousePaginationHotkeys("Decades");
        }),
        new("Durations", () =>
        {
            showExcludedElements = new Bool(true);
            SetDesktopBackground("Backgrounds/Default");
            SpawnWindowBlueprint("Durations");
            SpawnWindowBlueprint("DurationsScrollbarUp");
            SpawnWindowBlueprint("DurationsScrollbar");
            SpawnWindowBlueprint("DurationsScrollbarDown");
            SpawnWindowBlueprint("ResetLibraryFiltering");
            SpawnWindowBlueprint("MenuBar");
            AddHotkey(Escape, () =>
            {
                CloseDesktop(CDesktop.title);
                SpawnDesktopBlueprint("Menu");
            });
            AddMousePaginationHotkeys("Durations");
        }),
        new("TrackAmounts", () =>
        {
            showExcludedElements = new Bool(true);
            SetDesktopBackground("Backgrounds/Default");
            SpawnWindowBlueprint("TrackAmounts");
            SpawnWindowBlueprint("TrackAmountsScrollbarUp");
            SpawnWindowBlueprint("TrackAmountsScrollbar");
            SpawnWindowBlueprint("TrackAmountsScrollbarDown");
            SpawnWindowBlueprint("ResetLibraryFiltering");
            SpawnWindowBlueprint("MenuBar");
            AddHotkey(Escape, () =>
            {
                CloseDesktop(CDesktop.title);
                SpawnDesktopBlueprint("Menu");
            });
            AddMousePaginationHotkeys("TrackAmounts");
        }),
        new("Languages", () =>
        {
            showExcludedElements = new Bool(true);
            SetDesktopBackground("Backgrounds/Default");
            SpawnWindowBlueprint("Languages");
            SpawnWindowBlueprint("LanguagesScrollbarUp");
            SpawnWindowBlueprint("LanguagesScrollbar");
            SpawnWindowBlueprint("LanguagesScrollbarDown");
            SpawnWindowBlueprint("ResetLibraryFiltering");
            SpawnWindowBlueprint("MenuBar");
            AddHotkey(Escape, () =>
            {
                CloseDesktop(CDesktop.title);
                SpawnDesktopBlueprint("Menu");
            });
            AddMousePaginationHotkeys("Languages");
        }),
        new("DebutYears", () =>
        {
            showExcludedElements = new Bool(true);
            SetDesktopBackground("Backgrounds/Default");
            SpawnWindowBlueprint("DebutYears");
            SpawnWindowBlueprint("DebutYearsScrollbarUp");
            SpawnWindowBlueprint("DebutYearsScrollbar");
            SpawnWindowBlueprint("DebutYearsScrollbarDown");
            SpawnWindowBlueprint("ResetLibraryFiltering");
            SpawnWindowBlueprint("MenuBar");
            AddHotkey(Escape, () =>
            {
                CloseDesktop(CDesktop.title);
                SpawnDesktopBlueprint("Menu");
            });
            AddMousePaginationHotkeys("DebutYears");
        }),
        new("Anniversaries", () =>
        {
            showExcludedElements = new Bool(true);
            SetDesktopBackground("Backgrounds/Default");
            SpawnWindowBlueprint("Anniversaries");
            SpawnWindowBlueprint("AnniversariesScrollbarUp");
            SpawnWindowBlueprint("AnniversariesScrollbar");
            SpawnWindowBlueprint("AnniversariesScrollbarDown");
            SpawnWindowBlueprint("ResetLibraryFiltering");
            SpawnWindowBlueprint("MenuBar");
            AddHotkey(Escape, () =>
            {
                CloseDesktop(CDesktop.title);
                SpawnDesktopBlueprint("Menu");
            });
            AddMousePaginationHotkeys("Anniversaries");
        }),
        new("RatingStatus", () =>
        {
            showExcludedElements = new Bool(true);
            SetDesktopBackground("Backgrounds/Default");
            SpawnWindowBlueprint("RatingStatuses");
            SpawnWindowBlueprint("RatingStatusesScrollbarUp");
            SpawnWindowBlueprint("RatingStatusesScrollbar");
            SpawnWindowBlueprint("RatingStatusesScrollbarDown");
            SpawnWindowBlueprint("ResetLibraryFiltering");
            SpawnWindowBlueprint("MenuBar");
            AddHotkey(Escape, () =>
            {
                CloseDesktop(CDesktop.title);
                SpawnDesktopBlueprint("Menu");
            });
            AddMousePaginationHotkeys("RatingStatus");
        }),
        new("Menu", () =>
        {
            SetDesktopBackground("Backgrounds/Default");
            SpawnWindowBlueprint("Menu");
            SpawnWindowBlueprint("MenuBar");
            AddPaginationHotkeys();
            AddHotkey(KeypadMultiply, () =>
            {
                ratings.ToList().ForEach(x => x.Value.UpdateRating());
            });
        }),
        new("LoadCover", () =>
        {
            newCover = null;
            startedGettingCover = false;
            SetDesktopBackground("Backgrounds/Default");
        }),
        new("PrepareArtistBattle", () =>
        {
            SetDesktopBackground("Backgrounds/Default");
            SpawnWindowBlueprint("ArtistBattlePerRound");
            SpawnWindowBlueprint("ArtistBattlePerArtist");
            SpawnWindowBlueprint("ArtistBattleFinish");
            SpawnWindowBlueprint("ArtistBattleArtists");
            SpawnWindowBlueprint("ArtistBattleArtistsScrollbarUp");
            SpawnWindowBlueprint("ArtistBattleArtistsScrollbar");
            SpawnWindowBlueprint("ArtistBattleArtistsScrollbarDown");
            AddHotkey(Escape, () =>
            {
                CloseDesktop(CDesktop.title);
                CDesktop.RespawnAll();
            });
            AddMousePaginationHotkeys("ArtistBattleArtists");
        }),
        new("ArtistBattle", () =>
        {
            SetDesktopBackground("Backgrounds/Default");
            SpawnWindowBlueprint("ArtistBattle");
            SpawnWindowBlueprint("ArtistBattleHeader");
            AddHotkey(Escape, () =>
            {
                CloseDesktop(CDesktop.title);
                CDesktop.RespawnAll();
            });
        }),
        new("AcceptNewAlbum", () =>
        {
            if (newRelease.pallete == null)
                newRelease.GeneratePallete(newCover);
            SetDesktopBackgroundAsGradient(newRelease.pallete);
            SpawnWindowBlueprint("MusicRelease");
            SpawnWindowBlueprint("MusicReleaseCover");
            SpawnWindowBlueprint("MusicReleaseDescription");
            SpawnWindowBlueprint("MusicReleaseBottomLine");
            SpawnWindowBlueprint("MusicReleaseScrollbarUp");
            SpawnWindowBlueprint("MusicReleaseScrollbar");
            SpawnWindowBlueprint("MusicReleaseScrollbarDown");
            SpawnWindowBlueprint("CloseMusicRelease");
            SpawnWindowBlueprint("MenuBar");
            AddHotkey(Escape, () =>
            {
                CloseDesktop(CDesktop.title);
                CDesktop.RespawnAll();
            });
            AddHotkey(PageUp, () =>
            {
                var window = CDesktop.windows.Find(x => x.title == "MusicRelease");
                if (window.pagination() > 0)
                {
                    window.DecrementPagination();
                    CDesktop.RespawnAll();
                    Respawn("MusicReleaseScrollbarUp");
                    Respawn("MusicReleaseScrollbar");
                    Respawn("MusicReleaseScrollbarDown");
                }
            });
            AddHotkey(PageDown, () =>
            {
                var window = CDesktop.windows.Find(x => x.title == "MusicRelease");
                if (window.pagination() < window.maxPagination())
                {
                    window.IncrementPagination();
                    CDesktop.RespawnAll();
                    Respawn("MusicReleaseScrollbarUp");
                    Respawn("MusicReleaseScrollbar");
                    Respawn("MusicReleaseScrollbarDown");
                }
            });
        }),
        new("SquareChart", () =>
        {
            SetDesktopBackground("Backgrounds/Default");
            SpawnWindowBlueprint("SquareChartOffset");
            SpawnWindowBlueprint("SquareChartWidth");
            SpawnWindowBlueprint("SquareChartHeight");
            SpawnWindowBlueprint("SquareChartFinish");
            AddHotkey(Escape, () =>
            {
                CloseDesktop(CDesktop.title);
                CDesktop.RespawnAll();
            });
        }),
        new("ScaledChart", () =>
        {
            SetDesktopBackground("Backgrounds/Default");
            SpawnWindowBlueprint("ScaledChartPerfectFit");
            SpawnWindowBlueprint("ScaledChartFirstRowSize");
            SpawnWindowBlueprint("ScaledChartRowAmount");
            SpawnWindowBlueprint("ScaledChartFinish");
            AddHotkey(Escape, () =>
            {
                CloseDesktop(CDesktop.title);
                CDesktop.RespawnAll();
            });
        }),
        new("SequenceChart", () =>
        {
            SetDesktopBackground("Backgrounds/Default");
            SpawnWindowBlueprint("SequenceChartSplitOnYears");
            SpawnWindowBlueprint("SequenceChartSplitOnDecades");
            SpawnWindowBlueprint("SequenceChartFinish");
            AddHotkey(Escape, () =>
            {
                CloseDesktop(CDesktop.title);
                CDesktop.RespawnAll();
            });
        }),
        new("RatingColorRange", () =>
        {
            SetDesktopBackground("Backgrounds/Default");
            SpawnWindowBlueprint("RatingColorRange1");
            SpawnWindowBlueprint("RatingColorRange2");
            SpawnWindowBlueprint("RatingColorRange3");
            SpawnWindowBlueprint("RatingColorRange4");
            SpawnWindowBlueprint("RatingColorRange5");
            SpawnWindowBlueprint("RatingColorRange6");
            SpawnWindowBlueprint("RatingColorRangeMenuBar");
            AddHotkey(Escape, () =>
            {
                Serialization.Serialize(settings, "settings");
                CloseDesktop(CDesktop.title);
                CDesktop.RespawnAll();
            });
        }),
        new("MenuBackgroundColor", () =>
        {
            SetDesktopBackground("Backgrounds/Default");
            SpawnWindowBlueprint("MenuBackgroundColor");
            SpawnWindowBlueprint("MenuBackgroundColorMenuBar");
            AddHotkey(Escape, () =>
            {
                Serialization.Serialize(settings, "settings");
                CloseDesktop(CDesktop.title);
                CDesktop.RespawnAll();
            });
        }),
        new("LibraryRefetchSuccess", () =>
        {
            SetDesktopBackground("Backgrounds/Default");
            SpawnWindowBlueprint("LibraryRefetchSuccess");
            AddHotkey(Escape, () =>
            {
                CloseDesktop(CDesktop.title);
                SpawnDesktopBlueprint("MusicReleases");
            });
        }),
        new("LibraryRefetchFailure", () =>
        {
            SetDesktopBackground("Backgrounds/Default");
            SpawnWindowBlueprint("LibraryRefetchFailure");
            AddHotkey(Escape, () =>
            {
                CloseDesktop(CDesktop.title);
                SpawnDesktopBlueprint("MusicReleases");
            });
        }),
        new("CreateNewAlbumReleaseName", () =>
        {
            SetDesktopBackground("Backgrounds/Default");
            SpawnWindowBlueprint("CreateNewAlbumReleaseName");
            SpawnWindowBlueprint("CreateNewAlbumClose");
            SpawnWindowBlueprint("CreateNewAlbumMenuBar");
        }),
        new("CreateNewAlbumReleaseDate", () =>
        {
            SetDesktopBackground("Backgrounds/Default");
            SpawnWindowBlueprint("CreateNewAlbumReleaseDate");
            SpawnWindowBlueprint("CreateNewAlbumClose");
            SpawnWindowBlueprint("CreateNewAlbumMenuBar");
            AddHotkey(Escape, () =>
            {
                CloseDesktop(CDesktop.title);
                SpawnDesktopBlueprint("CreateNewAlbumReleaseName");
            });
        }),
        new("CreateNewAlbumReleaseType", () =>
        {
            SetDesktopBackground("Backgrounds/Default");
            SpawnWindowBlueprint("CreateNewAlbumReleaseType");
            SpawnWindowBlueprint("CreateNewAlbumClose");
            SpawnWindowBlueprint("CreateNewAlbumMenuBar");
            AddHotkey(Escape, () =>
            {
                CloseDesktop(CDesktop.title);
                SpawnDesktopBlueprint("CreateNewAlbumReleaseDate");
            });
        }),
        new("CreateNewAlbumReleaseGenres", () =>
        {
            SetDesktopBackground("Backgrounds/Default");
            SpawnWindowBlueprint("CreateNewAlbumReleaseGenres");
            SpawnWindowBlueprint("CreateNewAlbumClose");
            SpawnWindowBlueprint("CreateNewAlbumMenuBar");
            AddHotkey(Escape, () =>
            {
                CloseDesktop(CDesktop.title);
                SpawnDesktopBlueprint("CreateNewAlbumReleaseType");
            });
        }),
        new("CreateNewAlbumReleaseLanguages", () =>
        {
            SetDesktopBackground("Backgrounds/Default");
            SpawnWindowBlueprint("CreateNewAlbumReleaseLanguages");
            SpawnWindowBlueprint("CreateNewAlbumClose");
            SpawnWindowBlueprint("CreateNewAlbumMenuBar");
            AddHotkey(Escape, () =>
            {
                CloseDesktop(CDesktop.title);
                SpawnDesktopBlueprint("CreateNewAlbumReleaseGenres");
            });
        }),
        new("CreateNewAlbumReleaseTracklist", () =>
        {
            SetDesktopBackground("Backgrounds/Default");
            SpawnWindowBlueprint("CreateNewAlbumReleaseTracklist");
            SpawnWindowBlueprint("CreateNewAlbumClose");
            SpawnWindowBlueprint("CreateNewAlbumMenuBar");
            AddHotkey(Escape, () =>
            {
                CloseDesktop(CDesktop.title);
                SpawnDesktopBlueprint("CreateNewAlbumReleaseLanguages");
            });
        }),
        new("CreateNewAlbumArtistName", () =>
        {
            SetDesktopBackground("Backgrounds/Default");
            SpawnWindowBlueprint("CreateNewAlbumArtistName");
            SpawnWindowBlueprint("CreateNewAlbumClose");
            SpawnWindowBlueprint("CreateNewAlbumMenuBar");
            AddHotkey(Escape, () =>
            {
                CloseDesktop(CDesktop.title);
                SpawnDesktopBlueprint("CreateNewAlbumReleaseTracklist");
            });
        }),
        new("CreateNewAlbumArtistCountry", () =>
        {
            SetDesktopBackground("Backgrounds/Default");
            SpawnWindowBlueprint("CreateNewAlbumArtistCountry");
            SpawnWindowBlueprint("CreateNewAlbumArtistCountryNextStep");
            SpawnWindowBlueprint("CreateNewAlbumArtistCountryScrollbarUp");
            SpawnWindowBlueprint("CreateNewAlbumArtistCountryScrollbar");
            SpawnWindowBlueprint("CreateNewAlbumArtistCountryScrollbarDown");
            SpawnWindowBlueprint("CreateNewAlbumClose");
            SpawnWindowBlueprint("CreateNewAlbumMenuBar");
            AddHotkey(Escape, () =>
            {
                CloseDesktop(CDesktop.title);
                SpawnDesktopBlueprint("CreateNewAlbumArtistName");
            });
            AddMousePaginationHotkeys("CreateNewAlbumArtistCountry");
        }),
        new("CreateNewAlbumReleaseCoverURL", () =>
        {
            SetDesktopBackground("Backgrounds/Default");
            SpawnWindowBlueprint("CreateNewAlbumReleaseCoverURL");
            SpawnWindowBlueprint("CreateNewAlbumClose");
            SpawnWindowBlueprint("CreateNewAlbumMenuBar");
            AddHotkey(Escape, () =>
            {
                CloseDesktop(CDesktop.title);
                countryCodes = countryCodes.OrderByDescending(x => library.originalArtists.Count(y => y.country == x.Key)).ToDictionary(x => x.Key, x => x.Value);
                SpawnDesktopBlueprint("CreateNewAlbumArtistCountry");
            });
        }),
        new("CreateNewAlbumPreviewLoadCover", () =>
        {
            newCover = null;
            startedGettingCover = false;
            SetDesktopBackground("Backgrounds/Default");
            AddHotkey(Escape, () =>
            {
                CloseDesktop(CDesktop.title);
                SpawnDesktopBlueprint("CreateNewAlbumReleaseCoverURL");
            });
        }),
        new("CreateNewAlbumPreview", () =>
        {
            if (musicRelease.pallete == null)
                musicRelease.GeneratePallete(newCover);
            SetDesktopBackgroundAsGradient(musicRelease.pallete);
            SpawnWindowBlueprint("MusicRelease");
            SpawnWindowBlueprint("MusicReleaseCover");
            SpawnWindowBlueprint("MusicReleaseDescription");
            SpawnWindowBlueprint("MusicReleaseBottomLine");
            SpawnWindowBlueprint("MusicReleaseScrollbarUp");
            SpawnWindowBlueprint("MusicReleaseScrollbar");
            SpawnWindowBlueprint("MusicReleaseScrollbarDown");
            SpawnWindowBlueprint("CloseMusicRelease");
            SpawnWindowBlueprint("CreateNewAlbumMenuBar");
            SpawnWindowBlueprint("CreateNewAlbumClose");
            AddHotkey(Escape, () =>
            {
                CloseDesktop(CDesktop.title);
                CDesktop.RespawnAll();
            });
            AddHotkey(PageUp, () =>
            {
                var window = CDesktop.windows.Find(x => x.title == "MusicRelease");
                if (window.pagination() > 0)
                {
                    window.DecrementPagination();
                    CDesktop.RespawnAll();
                    Respawn("MusicReleaseScrollbarUp");
                    Respawn("MusicReleaseScrollbar");
                    Respawn("MusicReleaseScrollbarDown");
                }
            });
            AddHotkey(PageDown, () =>
            {
                var window = CDesktop.windows.Find(x => x.title == "MusicRelease");
                if (window.pagination() < window.maxPagination())
                {
                    window.IncrementPagination();
                    CDesktop.RespawnAll();
                    Respawn("MusicReleaseScrollbarUp");
                    Respawn("MusicReleaseScrollbar");
                    Respawn("MusicReleaseScrollbarDown");
                }
            });
        }),
        new("SendingMail", () =>
        {
            if (musicRelease.pallete == null)
                musicRelease.GeneratePallete(newCover);
            SetDesktopBackgroundAsGradient(musicRelease.pallete);
            SpawnWindowBlueprint("SendingMail");
            Serialization.SendMail();
        }),
    };

    public static void AddMousePaginationHotkeys(string windowName)
    {
        AddHotkey(PageUp, () =>
        {
            var moved = false;
            var window = CDesktop.windows.Find(x => x.title == windowName);
            for (int i = Input.GetKey(LeftShift) ? window.perPage - 1 : 0; i >= 0; i--)
                if (window.pagination() > 0)
                {
                    moved = true;
                    window.DecrementPagination();
                }
                else break;
            if (moved)
            {
                CDesktop.RespawnAll();
                Respawn(windowName + "ScrollbarUp");
                Respawn(windowName + "Scrollbar");
                Respawn(windowName + "ScrollbarDown");
            }
        });
        AddHotkey(PageDown, () =>
        {
            var moved = false;
            var window = CDesktop.windows.Find(x => x.title == windowName);
            for (int i = Input.GetKey(LeftShift) ? window.perPage - 1 : 0; i >= 0; i--)
                if (window.pagination() < window.maxPagination())
                {
                    moved = true;
                    window.IncrementPagination();
                }
                else break;
            if (moved)
            {
                CDesktop.RespawnAll();
                Respawn(windowName + "ScrollbarUp");
                Respawn(windowName + "Scrollbar");
                Respawn(windowName + "ScrollbarDown");
            }
        });
    }

    public static void AddPaginationHotkeys()
    {
        AddHotkey(D, () =>
        {
            var window = CDesktop.windows.Find(x => x.maxPaginationReq != null);
            if (window == null) return;
            var temp = window.pagination();
            window.IncrementPagination();
            window.Respawn();
        });
        AddHotkey(D, () =>
        {
            var window = CDesktop.windows.Find(x => x.maxPaginationReq != null);
            if (window == null) return;
            var temp = window.pagination();
            window.IncrementPaginationEuler();
            window.Respawn();
        }, false);
        AddHotkey(A, () =>
        {
            var window = CDesktop.windows.Find(x => x.maxPaginationReq != null);
            if (window == null) return;
            var temp = window.pagination();
            window.DecrementPagination();
            window.Respawn();
        });
        AddHotkey(A, () =>
        {
            var window = CDesktop.windows.Find(x => x.maxPaginationReq != null);
            if (window == null) return;
            var temp = window.pagination();
            window.DecrementPaginationEuler();
            window.Respawn();
        }, false);
    }
}
