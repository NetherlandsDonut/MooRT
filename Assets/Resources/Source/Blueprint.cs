using System;
using UnityEngine;
using System.Linq;
using System.Collections.Generic;

using static UnityEngine.KeyCode;

using static Root;
using static Year;
using static Sound;
using static Genre;
using static Decade;
using static Library;
using static Country;
using static Language;
using static Duration;
using static DebutYear;
using static TrackAmount;
using static ReleaseType;
using static MusicRelease;
using static ReleaseRating;

using static Root.Anchor;

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
            var rowAmount = 15;
            var thisWindow = CDesktop.LBWindow();
            thisWindow.SetPaginationSingleStep(() => library.releases.Count, rowAmount);
            CDesktop.quickInputWindow = thisWindow;
            SetAnchor(Center);
            AddRegionGroup();
            SetRegionGroupWidth(37);
            AddButtonRegion(() => AddLine("#", "", "Right"),
                (h) =>
                {
                    library.releases.Reverse();
                }
            );
            for (int i = thisWindow.pagination() == 0 ? 0 : library.releases.Count - thisWindow.pagination() < rowAmount ? library.releases.Count - (thisWindow.pagination() + 1) : 0; i < rowAmount; i++)
            {
                var index = i;
                if (library.releases.Count > index + thisWindow.pagination())
                    AddHeaderRegion(() => AddLine(1 + index + thisWindow.pagination() + "", "", "Right"));
            }
            AddPaddingRegion(() => AddLine(""));
            AddRegionGroup();
            SetRegionGroupWidth(190);
            AddButtonRegion(() => AddLine("Name"),
                (h) =>
                {
                    library.releases = (releasesLastSort == "Name" ? library.releases.OrderByDescending(x => x.name) : library.releases.OrderBy(x => x.name)).ToList();
                    releasesLastSort = releasesLastSort == "Name" ? "" : "Name";
                }
            );
            for (int i = thisWindow.pagination() == 0 ? 0 : library.releases.Count - thisWindow.pagination() < rowAmount ? library.releases.Count - (thisWindow.pagination() + 1) : 0; i < rowAmount; i++)
            {
                var index = i;
                if (library.releases.Count > index + thisWindow.pagination())
                    AddButtonRegion(() =>
                    {
                        var album = library.releases[index + thisWindow.pagination()];
                        AddRegionOverlay(@"RegionReplacements\AlbumNameBar");
                        SetRegionBackgroundAsImage(albumBars[(albumBars.ContainsKey(album.ID + "") ? album.ID : 0) + ""]);
                        AddLine(album.name, "Black");
                    },
                    (h) =>
                    {
                        musicReleaseIndex = index + thisWindow.pagination();
                        musicRelease = library.releases[musicReleaseIndex];
                        SpawnDesktopBlueprint("MusicRelease");
                    },
                    null,
                    (h) => () =>
                    {
                        var album = library.releases[index + thisWindow.pagination()];
                        SetAnchor(BottomRight);
                        AddHeaderGroup();
                        AddHeaderRegion(() =>
                        {
                            AddLine(album.name);
                        });
                    });
            }
            AddPaddingRegion(() => AddLine(library.releases.Count + " out of " + library.originalReleases.Count + " releases", "DarkGray"));
            AddRegionGroup();
            SetRegionGroupWidth(46);
            AddButtonRegion(() => AddLine("Rating"),
                (h) =>
                {
                    library.releases = (releasesLastSort == "Rating" ? library.releases.OrderBy(x => ratings.ContainsKey(x.ID) ? ratings[x.ID].rating : 0) : library.releases.OrderByDescending(x => ratings.ContainsKey(x.ID) ? ratings[x.ID].rating : 0)).ToList();
                    releasesLastSort = releasesLastSort == "Rating" ? "" : "Rating";
                }
            );
            for (int i = thisWindow.pagination() == 0 ? 0 : library.releases.Count - thisWindow.pagination() < rowAmount ? library.releases.Count - (thisWindow.pagination() + 1) : 0; i < rowAmount; i++)
            {
                var index = i;
                if (library.releases.Count > index + thisWindow.pagination())
                {
                    var album = library.releases[index + thisWindow.pagination()];
                    var amount = !ratings.ContainsKey(album.ID) ? 0 : Math.Ceiling(ratings[album.ID].rating / 100.0);
                    AddHeaderRegion(() => AddLine(amount.ToString("000"), amount >= 970 ? "Legendary" : (amount >= 900 ? "Epic" : (amount >= 800 ? "Rare" : (amount >= 700 ? "Uncommon" : "Common")))));
                }
            }
            AddPaddingRegion(() => AddLine(""));
            AddRegionGroup();
            SetRegionGroupWidth(39);
            AddButtonRegion(() => AddLine("Year", "", "Center"),
                (h) =>
                {
                    library.releases = (releasesLastSort == "Year" ? library.releases.OrderBy(x => x.releaseDate) : library.releases.OrderByDescending(x => x.releaseDate)).ToList();
                    releasesLastSort = releasesLastSort == "Year" ? "" : "Year";
                }
            );
            for (int i = thisWindow.pagination() == 0 ? 0 : library.releases.Count - thisWindow.pagination() < rowAmount ? library.releases.Count - (thisWindow.pagination() + 1) : 0; i < rowAmount; i++)
            {
                var index = i;
                if (library.releases.Count > index + thisWindow.pagination())
                    AddButtonRegion(() =>
                    {
                        var album = library.releases[index + thisWindow.pagination()];
                        AddLine(album.releaseDate.Substring(0, 4), "", "Center");
                    },
                    (h) => { },
                    null,
                    (h) => () =>
                    {
                        var album = library.releases[index + thisWindow.pagination()];
                        SetAnchor(BottomRight);
                        AddHeaderGroup();
                        AddHeaderRegion(() =>
                        {
                            AddLine(album.name);
                        });
                    });
            }
            AddPaddingRegion(() => AddLine(""));
            AddRegionGroup();
            SetRegionGroupWidth(58);
            AddButtonRegion(() => AddLine("Duration"),
                (h) =>
                {
                    library.releases = (releasesLastSort == "Duration" ? library.releases.OrderBy(x => x.length) : library.releases.OrderByDescending(x => x.length)).ToList();
                    releasesLastSort = releasesLastSort == "Duration" ? "" : "Duration";
                }
            );
            for (int i = thisWindow.pagination() == 0 ? 0 : library.releases.Count - thisWindow.pagination() < rowAmount ? library.releases.Count - (thisWindow.pagination() + 1) : 0; i < rowAmount; i++)
            {
                var index = i;
                if (library.releases.Count > index + thisWindow.pagination())
                    AddButtonRegion(() =>
                    {
                        var album = library.releases[index + thisWindow.pagination()];
                        AddLine(album.duration + "m", "", "Right");
                    },
                    (h) => { },
                    null,
                    (h) => () =>
                    {
                        var album = library.releases[index + thisWindow.pagination()];
                        SetAnchor(BottomRight);
                        AddHeaderGroup();
                        AddHeaderRegion(() =>
                        {
                            AddLine(album.name);
                        });
                    });
            }
            AddPaddingRegion(() => AddLine(""));
            AddRegionGroup();
            SetRegionGroupWidth(47);
            AddButtonRegion(() => AddLine("Tracks"),
                (h) =>
                {
                    library.releases = (releasesLastSort == "Tracks" ? library.releases.OrderBy(x => x.tracks.Count) : library.releases.OrderByDescending(x => x.tracks.Count)).ToList();
                    releasesLastSort = releasesLastSort == "Tracks" ? "" : "Tracks";
                }
            );
            for (int i = thisWindow.pagination() == 0 ? 0 : library.releases.Count - thisWindow.pagination() < rowAmount ? library.releases.Count - (thisWindow.pagination() + 1) : 0; i < rowAmount; i++)
            {
                var index = i;
                if (library.releases.Count > index + thisWindow.pagination())
                    AddButtonRegion(() =>
                    {
                        var album = library.releases[index + thisWindow.pagination()];
                        AddLine(album.tracks.Count + "", "", "Right");
                    },
                    (h) => { },
                    null,
                    (h) => () =>
                    {
                        var album = library.releases[index + thisWindow.pagination()];
                        SetAnchor(BottomRight);
                        AddHeaderGroup();
                        AddHeaderRegion(() =>
                        {
                            AddLine(album.name);
                        });
                    });
            }
            AddPaddingRegion(() => AddLine(""));
        }),
        new("MusicReleasesScrollbarUp", () => {
            SetAnchor(208, 161);
            AddRegionGroup();
            SetRegionGroupWidth(19);
            AddPaddingRegion(() =>
            {
                var window = CDesktop.windows.Find(x => x.title == "MusicReleases");
                if (window.pagination() > 0)
                    AddSmallButton("OtherPageUp", (h) =>
                    {
                        PlaySound("DesktopChangePage", 0.6f);
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
            SetAnchor(208, 142);
            AddRegionGroup();
            SetRegionGroupWidth(19);
            SetRegionGroupHeight(281);
            AddPaddingRegion(() => AddLine(""));
        }),
        new("MusicReleasesScrollbarDown", () => {
            SetAnchor(208, -143);
            AddRegionGroup();
            SetRegionGroupWidth(19);
            AddPaddingRegion(() =>
            {
                var window = CDesktop.windows.Find(x => x.title == "MusicReleases");
                if (window.pagination() < window.maxPagination())
                    AddSmallButton("OtherPageDown", (h) =>
                    {
                        PlaySound("DesktopChangePage", 0.6f);
                        window.IncrementPagination();
                        CDesktop.RespawnAll();
                        Respawn("MusicReleasesScrollbarUp", true);
                        Respawn("MusicReleasesScrollbar", true);
                        Respawn("MusicReleasesScrollbarDown", true);
                    });
                else AddSmallButton("OtherPageDownOff");
            });
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
                    var upTo = discs.Last() == i ? musicRelease.tracks.Count : discs.First(x => x > i);
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
                                var split = musicRelease.discs.Contains(":") ? musicRelease.discs.Split(":").ToList() : new();
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
                WriteWrap(region, "This release has", "DarkGray");
                WriteWrap(region, musicRelease.tracks.Count + " tracks", "Gray");
                WriteWrap(region, "which", "DarkGray");
                WriteWrap(region, "add up", "DarkGray");
                WriteWrap(region, "to a", "DarkGray");
                WriteWrap(region, "runtime of", "DarkGray");
                WriteWrap(region, musicRelease.duration + "m", "Gray");
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
                        PlaySound("DesktopChangePage", 0.6f);
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
                        PlaySound("DesktopChangePage", 0.6f);
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
                if (CDesktop.title == "AcceptNewAlbum")
                {
                    AddSmallButton("OtherTrash", (h) =>
                    {
                        newArtist = null;
                        newRelease = null;
                        artistFind = null;
                        CloseDesktop(CDesktop.title);
                        CDesktop.RespawnAll();
                    });
                    AddSmallButton("OtherClose", (h) =>
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
                if (CDesktop.title == "AcceptNewAlbum") SetRegionBackgroundAsImage(newCover);
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
            AddButtonRegion(() =>
            {
                AddLine("Modify rating", "", "Center");
            },
            (h) =>
            {

            });
            AddButtonRegion(() =>
            {
                AddLine("Manual placement", "", "Center");
            },
            (h) =>
            {

            });
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
                });
                else AddSmallButton("OtherNextPageOff");
            });
        }),

        //Artists
        new("Artists", () => {
            var rowAmount = 15;
            var thisWindow = CDesktop.LBWindow();
            var list = (showExcludedElements.Value() ? library.artists : library.artists.Where(x => artistFiltering[x.ID].Value())).Where(x => x.name != "Various artists" && (!hideArtistsOfExcludedCountries.Value() || hideArtistsOfExcludedCountries.Value() && countryFiltering[x.country].Value())).ToList();
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
            }
            AddPaddingRegion(() => AddLine(""));
        }),
        new("ArtistsScrollbarUp", () => {
            SetAnchor(173, 142);
            AddRegionGroup();
            SetRegionGroupWidth(19);
            AddPaddingRegion(() =>
            {
                var window = CDesktop.windows.Find(x => x.title == "Artists");
                if (window.pagination() > 0)
                    AddSmallButton("OtherPageUp", (h) =>
                    {
                        PlaySound("DesktopChangePage", 0.6f);
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
            SetAnchor(173, 123);
            AddRegionGroup();
            SetRegionGroupWidth(19);
            SetRegionGroupHeight(281);
            AddPaddingRegion(() => AddLine(""));
        }),
        new("ArtistsScrollbarDown", () => {
            SetAnchor(173, -162);
            AddRegionGroup();
            SetRegionGroupWidth(19);
            AddPaddingRegion(() =>
            {
                var window = CDesktop.windows.Find(x => x.title == "Artists");
                if (window.pagination() < window.maxPagination())
                    AddSmallButton("OtherPageDown", (h) =>
                    {
                        PlaySound("DesktopChangePage", 0.6f);
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
            }
            AddPaddingRegion(() => AddLine(""));
        }),
        new("CountriesScrollbarUp", () => {
            SetAnchor(200, 152);
            AddRegionGroup();
            SetRegionGroupWidth(19);
            AddPaddingRegion(() =>
            {
                var window = CDesktop.windows.Find(x => x.title == "Countries");
                if (window.pagination() > 0)
                    AddSmallButton("OtherPageUp", (h) =>
                    {
                        PlaySound("DesktopChangePage", 0.6f);
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
            SetAnchor(200, 133);
            AddRegionGroup();
            SetRegionGroupWidth(19);
            SetRegionGroupHeight(281);
            AddPaddingRegion(() => AddLine(""));
        }),
        new("CountriesScrollbarDown", () => {
            SetAnchor(200, -152);
            AddRegionGroup();
            SetRegionGroupWidth(19);
            AddPaddingRegion(() =>
            {
                var window = CDesktop.windows.Find(x => x.title == "Countries");
                if (window.pagination() < window.maxPagination())
                    AddSmallButton("OtherPageDown", (h) =>
                    {
                        PlaySound("DesktopChangePage", 0.6f);
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
            }
            AddPaddingRegion(() => AddLine(""));
        }),
        new("GenresScrollbarUp", () => {
            SetAnchor(173, 142);
            AddRegionGroup();
            SetRegionGroupWidth(19);
            AddPaddingRegion(() =>
            {
                var window = CDesktop.windows.Find(x => x.title == "Genres");
                if (window.pagination() > 0)
                    AddSmallButton("OtherPageUp", (h) =>
                    {
                        PlaySound("DesktopChangePage", 0.6f);
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
            SetAnchor(173, 123);
            AddRegionGroup();
            SetRegionGroupWidth(19);
            SetRegionGroupHeight(281);
            AddPaddingRegion(() => AddLine(""));
        }),
        new("GenresScrollbarDown", () => {
            SetAnchor(173, -162);
            AddRegionGroup();
            SetRegionGroupWidth(19);
            AddPaddingRegion(() =>
            {
                var window = CDesktop.windows.Find(x => x.title == "Genres");
                if (window.pagination() < window.maxPagination())
                    AddSmallButton("OtherPageDown", (h) =>
                    {
                        PlaySound("DesktopChangePage", 0.6f);
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
            }
            AddPaddingRegion(() => AddLine(""));
        }),
        new("LanguagesScrollbarUp", () => {
            SetAnchor(173, 142);
            AddRegionGroup();
            SetRegionGroupWidth(19);
            AddPaddingRegion(() =>
            {
                var window = CDesktop.windows.Find(x => x.title == "Languages");
                if (window.pagination() > 0)
                    AddSmallButton("OtherPageUp", (h) =>
                    {
                        PlaySound("DesktopChangePage", 0.6f);
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
            SetAnchor(173, 123);
            AddRegionGroup();
            SetRegionGroupWidth(19);
            SetRegionGroupHeight(281);
            AddPaddingRegion(() => AddLine(""));
        }),
        new("LanguagesScrollbarDown", () => {
            SetAnchor(173, -162);
            AddRegionGroup();
            SetRegionGroupWidth(19);
            AddPaddingRegion(() =>
            {
                var window = CDesktop.windows.Find(x => x.title == "Languages");
                if (window.pagination() < window.maxPagination())
                    AddSmallButton("OtherPageDown", (h) =>
                    {
                        PlaySound("DesktopChangePage", 0.6f);
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
                        PlaySound("DesktopChangePage", 0.6f);
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
                        PlaySound("DesktopChangePage", 0.6f);
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
            var rowAmount = 15;
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
                        PlaySound("DesktopChangePage", 0.6f);
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
                        PlaySound("DesktopChangePage", 0.6f);
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
            var rowAmount = 15;
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
                        PlaySound("DesktopChangePage", 0.6f);
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
                        PlaySound("DesktopChangePage", 0.6f);
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
                        PlaySound("DesktopChangePage", 0.6f);
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
                        PlaySound("DesktopChangePage", 0.6f);
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
                        PlaySound("DesktopChangePage", 0.6f);
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
                        PlaySound("DesktopChangePage", 0.6f);
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
            }
            AddPaddingRegion(() => AddLine(""));
            AddRegionGroup();
            SetRegionGroupWidth(219);
            AddButtonRegion(() => AddLine("Name"),
                (h) =>
                {
                    durations = (lastSort == "Name" ? durations.OrderByDescending(x => x.duration) : durations.OrderBy(x => x.duration)).ToList();
                    lastSort = lastSort == "Name" ? "" : "Name";
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
                    (h) => { },
                    null,
                    (h) => () =>
                    {
                        var duration = list[index + thisWindow.pagination()];
                        SetAnchor(BottomRight);
                        AddHeaderGroup();
                        AddHeaderRegion(() =>
                        {
                            AddLine(duration.duration + "m");
                        });
                    });
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
                        PlaySound("DesktopChangePage", 0.6f);
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
                        PlaySound("DesktopChangePage", 0.6f);
                        window.IncrementPagination();
                        CDesktop.RespawnAll();
                        Respawn("DurationsScrollbarUp", true);
                        Respawn("DurationsScrollbar", true);
                        Respawn("DurationsScrollbarDown", true);
                    });
                else AddSmallButton("OtherPageDownOff");
            });
        }),

        //Menu
        new("MenuBar", () => {
            SetAnchor(Bottom, 0, 10);
            AddRegionGroup();
            if (CDesktop.title == "MusicReleases") AddPaddingRegion(() => AddLine("MusicReleases"));
            else AddButtonRegion(() => AddLine("MusicReleases"), (h) => { var name = CDesktop.title; SpawnDesktopBlueprint("MusicReleases"); CloseDesktop(name); });
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
            AddButtonRegion(() => AddLine("Import new release", "", "Center"), (h) =>
            {
                var failed = -1;
                var data = Serialization.ReadTXT("newRelease");
                var possibleTypes = new List<string> { "Studio album", "Live album", "Extended play", "Compilation album", "Demo recording", "Soundtrack", "Remix album" };
                var artistName = "";
                var artistCountry = "none";
                var newAlbum = new MusicRelease();
                newAlbum.tracks = new();
                for (int i = 0; i < data.Length; i++)
                {
                    if (i == 2)
                        if (data[i].Length > 0) artistName = data[i];
                        else { failed = i; break; };
                    if (i == 8)
                        if (data[i].Length > 0) artistCountry = data[i];
                        else if (!library.originalArtists.Exists(x => x.name == artistName)) { failed = i; break; };
                    if (i == 14)
                        if (data[i].Length > 0) newAlbum.name = data[i];
                        else { failed = i; break; };
                    if (i == 20)
                        if (data[i].Length > 0)
                        {
                            var trim = data[i].Trim();
                            if (trim.Contains("."))
                                newAlbum.releaseDate = data[i];
                            else if (trim.Contains(" "))
                            {
                                var split = trim.Split(" ").ToList();
                                var day = split.Find(x => x.Length <= 2 && x.All(x => char.IsDigit(x)));
                                var month = split.Find(x => x.Length >= 3 && x.All(x => !char.IsDigit(x)));
                                var year = split.Find(x => x.Length == 4 && x.All(x => char.IsDigit(x)));
                                var reverseMonths = monthNames.ToDictionary(x => x.Value, x => x.Key);
                                newAlbum.releaseDate = year + "." + reverseMonths[month].ToString("00") + "." + int.Parse(day).ToString("00");
                            }
                        }
                        else { failed = i; break; };
                    if (i == 26)
                        if (data[i].Length > 0) newCoverURL = data[i];
                        else { failed = i; break; };
                    if (i == 32)
                        if (data[i].Length > 0)
                        {
                            var types = ProcessTypes(data[i]);
                            if (types.All(x => possibleTypes.Contains(x))) newAlbum.types = types;
                            else { failed = 320; break; };
                        }
                        else { failed = i; break; };
                    if (i == 38)
                        if (data[i].Length > 0) newAlbum.genres = ProcessGenres(data[i]);
                        else { failed = i; break; };
                    if (i >= 44)
                        if (data[i].Length > 0)
                        {
                            data[i] = data[i].Replace("\t", " ");
                            if (data[i].Split(" ").Length > 1 && data[i].Split(" ").Last().Contains(":"))
                            {
                                var newTrack = new Track();
                                var lastIndex = data[i].LastIndexOf(" ");
                                newTrack.name = data[i][..lastIndex].Trim();
                                var time = data[i][lastIndex..].Trim();
                                if (time.Split(":").Length != 2) { failed = 441; errorAtLine = i + 1; break; };
                                if (!int.TryParse(time.Split(":")[0], out int minutes)) { failed = 442; errorAtLine = i + 1; break; };
                                if (!int.TryParse(time.Split(":")[1], out int seconds)) { failed = 443; errorAtLine = i + 1; break; };
                                newTrack.length = minutes * 60 + seconds;
                                newTrack.duration = time;
                                newAlbum.tracks.Add(newTrack);
                            }
                            else { failed = 440; errorAtLine = i + 1; break; };
                        }
                        else { failed = i; break; };
                }
                if (failed != -1)
                    SpawnWindowBlueprint("ErrorLoadingAlbum" + failed);
                else
                {
                    newArtist = null;
                    newRelease = newAlbum;
                    artistFind = library.originalArtists.Find(x => x.name == artistName && (x.country == artistCountry || artistCountry == "none"));
                    if (artistFind == null)
                    {
                        artistFind = new Artist()
                        {
                            ID = library.originalArtists.Count + 1,
                            name = artistName,
                            pronoun = "they",
                            country = artistCountry,
                            releases = new()
                        };
                        newArtist = artistFind;
                    }
                    newRelease.ID = library.originalReleases.Count + 1;
                    newRelease.languages = new() { "English" };
                    newRelease.coverDescriptors = new() { };
                    newRelease.format = int.Parse(newRelease.releaseDate[..4]) >= 1990 ? "digital" : "analog";
                    newRelease.artist = artistFind.name;
                    newRelease.artistID = artistFind.ID;
                    newRelease.Initialise(artistFind);
                    musicRelease = newRelease;
                    SpawnDesktopBlueprint("LoadCover");
                }

                List<string> ProcessGenres(string line)
                {
                    var list = line.Split(",").Select(x => ProcessGenre(x.Trim())).ToList();
                    return list;

                    string ProcessGenre(string genre)
                    {
                        var capitalised = string.Join(' ', genre.Split(" ").Select(x => x[..1].ToUpper() + x[1..].ToLower()).ToList());
                        return capitalised;
                    }
                }

                List<string> ProcessTypes(string line)
                {
                    var list = line.Split(",").Select(x => ProcessType(x.Trim())).ToList();
                    return list;

                    string ProcessType(string type)
                    {
                        var capitalised = type[..1].ToUpper() + type[1..].ToLower();
                        return capitalised;
                    }
                }
            });
            AddButtonRegion(() => AddLine("Open new release file", "", "Center"), (h) =>
            {
                Serialization.OpenTXT("newRelease");
            });
            AddButtonRegion(() => AddLine("sex", "", "Center"), (h) =>
            {
                Exporting.ExportSquareChart(library.releases);
            });
            AddButtonRegion(() => AddLine("Exit", "", "Center"), (h) =>
            {
                Application.Quit();
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
        new("ErrorLoadingAlbum44", () => {
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
        new("ErrorLoadingAlbum440", () => {
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
        new("ErrorLoadingAlbum441", () => {
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
        new("ErrorLoadingAlbum442", () => {
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
        new("ErrorLoadingAlbum443", () => {
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
    };

    public static List<Blueprint> desktopBlueprints = new()
    {
        new("LoadingScreen", () => 
        {
            Cursor.cursor.SetCursor(CursorType.None);
            loadingScreenAim = library.originalReleases.Count;
            SetDesktopBackground("Backgrounds/Default");
            SpawnWindowBlueprint("LoadingStatus");
            loadingStatusBar = CDesktop.LBWindow().LBRegionGroup().LBRegion().background.transform;
            UnityEngine.Object.Instantiate(loadingStatusBar, loadingStatusBar.parent);
            loadingStatusBar.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Sprites/Fills/RarityRare");
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
            SpawnWindowBlueprint("MenuBar");
            AddHotkey(Escape, () =>
            {
                CloseDesktop(CDesktop.title);
                SpawnDesktopBlueprint("Menu");
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
                    PlaySound("DesktopChangePage", 0.6f);
                    musicRelease = library.releases[++musicReleaseIndex];
                    CDesktop.RespawnAll();
                    Respawn("MusicReleaseScrollbarUp", true);
                    Respawn("MusicReleaseScrollbar", true);
                    Respawn("MusicReleaseScrollbarDown", true);
                    SpawnAlbumTransition();
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
                        PlaySound("DesktopChangePage", 0.6f);
                        musicRelease = library.releases[musicReleaseIndex];
                        CDesktop.RespawnAll();
                        Respawn("MusicReleaseScrollbarUp", true);
                        Respawn("MusicReleaseScrollbar", true);
                        Respawn("MusicReleaseScrollbarDown", true);
                    }
                }
            }, false);
            AddHotkey(A, () =>
            {
                if (musicReleaseIndex > 0)
                {
                    PlaySound("DesktopChangePage", 0.6f);
                    musicRelease = library.releases[--musicReleaseIndex];
                    CDesktop.RespawnAll();
                    Respawn("MusicReleaseScrollbarUp", true);
                    Respawn("MusicReleaseScrollbar", true);
                    Respawn("MusicReleaseScrollbarDown", true);
                    SpawnAlbumTransition();
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
                        PlaySound("DesktopChangePage", 0.6f);
                        musicRelease = library.releases[musicReleaseIndex];
                        CDesktop.RespawnAll();
                        Respawn("MusicReleaseScrollbarUp", true);
                        Respawn("MusicReleaseScrollbar", true);
                        Respawn("MusicReleaseScrollbarDown", true);
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
            SpawnWindowBlueprint("RatingStatus");
            SpawnWindowBlueprint("RatingStatusScrollbarUp");
            SpawnWindowBlueprint("RatingStatusScrollbar");
            SpawnWindowBlueprint("RatingStatusScrollbarDown");
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
                    PlaySound("DesktopChangePage", 0.6f);
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
                    PlaySound("DesktopChangePage", 0.6f);
                    window.IncrementPagination();
                    CDesktop.RespawnAll();
                    Respawn("MusicReleaseScrollbarUp");
                    Respawn("MusicReleaseScrollbar");
                    Respawn("MusicReleaseScrollbarDown");
                }
            });
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
                PlaySound("DesktopChangePage", 0.6f);
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
                PlaySound("DesktopChangePage", 0.6f);
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
            if (temp != window.pagination())
                PlaySound("DesktopChangePage", 0.6f);
            window.Respawn();
        });
        AddHotkey(D, () => 
        {
            var window = CDesktop.windows.Find(x => x.maxPaginationReq != null);
            if (window == null) return;
            var temp = window.pagination();
            window.IncrementPaginationEuler();
            if (temp != window.pagination())
                PlaySound("DesktopChangePage", 0.6f);
            window.Respawn();
        }, false);
        AddHotkey(A, () => 
        {
            var window = CDesktop.windows.Find(x => x.maxPaginationReq != null);
            if (window == null) return;
            var temp = window.pagination();
            window.DecrementPagination();
            if (temp != window.pagination())
                PlaySound("DesktopChangePage", 0.6f);
            window.Respawn();
        });
        AddHotkey(A, () => 
        {
            var window = CDesktop.windows.Find(x => x.maxPaginationReq != null);
            if (window == null) return;
            var temp = window.pagination();
            window.DecrementPaginationEuler();
            if (temp != window.pagination())
                PlaySound("DesktopChangePage", 0.6f);
            window.Respawn();
        }, false);
    }
}
