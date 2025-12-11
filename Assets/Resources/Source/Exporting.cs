using System;
using System.IO;
using System.Linq;
using System.Diagnostics;
using System.Collections.Generic;

using UnityEngine;

using static Root;

public class Exporting
{
    //This method exports a square chart of XY size
    //that consists of album covers provided on the list
    public static void ExportSquareChart(List<MusicRelease> albums, int x = 10, int y = 10, bool offset = true)
    {
        var offsetWidth = offset ? 3 : 0;
        var covers = new List<Sprite>();
        for (int i = 0; i < x * y && i < albums.Count; i++)
            covers.Add(albumCovers[albums[i].ID + ""]);
        var amount = 188 + offsetWidth;
        var chart = new Texture2D(1, 1);
        chart.SetPixel(0, 0, new Color(0, 0, 0, 1));
        scale(chart, amount * x + offsetWidth, amount * y + offsetWidth);
        for (int j = 0; j < y; j++)
            for (int i = 0; i < x && i + j * x < covers.Count; i++)
                Graphics.CopyTexture(covers[i + j * x].texture, 0, 0, 0, 0, 188, 188, chart, 0, 0, i * amount + offsetWidth, (y - j - 1) * amount + offsetWidth);
        chart.Apply();
        if (!Directory.Exists("MooRT_Export")) Directory.CreateDirectory("MooRT_Export");
        File.WriteAllBytes("MooRT_Export/squareChart.png", chart.EncodeToPNG());
        Process.Start(Environment.CurrentDirectory + "\\MooRT_Export\\squareChart.png");
    }

    //Variable to store prepared resources to build a scaled chart
    public static List<(int, int)> scaledChartBlueprint;

    //This method prepares the blueprint for the scaled chart
    public static void GenerateScaledChartBlueprint(int count, bool perfectFit, int firstRowAlbumCount, int rowAmount)
    {
        var temp = new List<(int, int)>();
        if (firstRowAlbumCount != 0)
            for (int i = firstRowAlbumCount; temp.Count < rowAmount && temp.Sum(x => x.Item2) + i <= count; i++)
                if (i == 0 || firstRowAlbumCount * 188 < i || i > 1 && firstRowAlbumCount * 188 / (i - 1) == 1) break;
                else if (!perfectFit && firstRowAlbumCount * 188 % i <= scaledChartDecrease || perfectFit && firstRowAlbumCount * 188 % i == 0)
                    temp.Add((firstRowAlbumCount * 188 / i, i));
        scaledChartRowAmount = temp.Count;
        scaledChartBlueprint = temp;
    }

    //This method exports a scaled chart of prepared size
    //that consists of album covers provided on the list
    public static void ExportScaledChart(List<MusicRelease> albums)
    {
        int y = 0, z = 0;
        var covers = new List<Sprite>();
        for (int i = 0; i < scaledChartBlueprint.Sum(x => x.Item2); i++)
            covers.Add(albumCovers[albums[i].ID + ""]);
        var chart = new Texture2D(1, 1);
        chart.SetPixel(0, 0, new Color(0, 0, 0, 1));
        scale(chart, scaledChartBlueprint[0].Item1 * scaledChartBlueprint[0].Item2, scaledChartBlueprint.Sum(x => x.Item1));
        y = chart.height;
        for (int i = 0; i < scaledChartBlueprint.Count; i++)
        {
            y -= scaledChartBlueprint[i].Item1;
            var offset = (chart.width - scaledChartBlueprint[i].Item2 * scaledChartBlueprint[i].Item1) / 2;
            for (int j = 0; j < scaledChartBlueprint[i].Item2; j++)
            {
                var cover = scaled(covers[z++].texture, scaledChartBlueprint[i].Item1, scaledChartBlueprint[i].Item1);
                cover.Apply();
                Graphics.CopyTexture(cover, 0, 0, 0, 0, scaledChartBlueprint[i].Item1, scaledChartBlueprint[i].Item1, chart, 0, 0, offset + j * scaledChartBlueprint[i].Item1, y);
            }
        }
        chart.Apply();
        if (!Directory.Exists("MooRT_Export")) Directory.CreateDirectory("MooRT_Export");
        File.WriteAllBytes("MooRT_Export/scaledChart.png", chart.EncodeToPNG());
        Process.Start(Environment.CurrentDirectory + "\\MooRT_Export\\scaledChart.png");
    }

    //This method exports a timeline/sequence chart
    //that consists of album covers provided on the list
    public static void ExportSequenceChart(List<MusicRelease> albums, bool splitOnYears, bool splitOnDecades)
    {
        //int minimumRating = int.Parse(inputFields["SequenceChartMinimumRating"]);
        var covers = new List<(Sprite, MusicRelease)>();
        for (int i = 0; i < albums.Count; i++)
            covers.Add((albumCovers[albums[i].ID + ""], albums[i]));
        var groups = covers.GroupBy(x => splitOnYears ? x.Item2.releaseDate[..4] : (splitOnDecades ? x.Item2.releaseDate[..3] : "")).ToList();
        var chart = new Texture2D(188 * covers.Count + groups.Count - 1, 1187);
        //chart.Set(0, 0, new Color(1, 1, 1, 1));
        int currentX = 0;
        for (int i = 0; i < groups.Count; i++)
            for (int j = 0; j < groups[i].Count(); j++)
            {
                var pair = groups[i].ToList()[j];
                var y = pair.Item2.GetRating() / 100;
                Graphics.CopyTexture(pair.Item1.texture, 0, 0, 0, 0, 188, 188, chart, 0, 0, currentX, y);
                currentX += 188;
                if (j == groups[i].Count() - 1 && i != groups.Count - 1)
                {
                    for (int z = 0; z < chart.height; z++)
                        chart.SetPixel(currentX, z, new Color(0, 0, 0, 1));
                    currentX++;
                }
            }
        chart.Apply();
        if (!Directory.Exists("MooRT_Export")) Directory.CreateDirectory("MooRT_Export");
        File.WriteAllBytes("MooRT_Export/scaledChart.png", chart.EncodeToPNG());
        Process.Start(Environment.CurrentDirectory + "\\MooRT_Export\\scaledChart.png");
    }

    public static void ExportArtistBattleResults(ArtistBattle artistBattle)
    {
        var file = new List<string>
        {
            "<meta http-equiv=\"Content-Type\" content=\"text/html;charset=utf-8\"><style type=\"text/css\">.winsWin{background-color:#ffd966;font-weight:bold;color:#000000;font-size:14pt;}.winsLos{background-color:#cccccc;font-weight:bold;color:#000000;font-size:14pt;}.los{background-color:#ffffff;color:#000000;}.win{background-color:#000000;}.bracketLos{background-color:#434343;min-width:235px;width:9999;font-size:24pt;}.bracketWin{background-color:#7f6000;min-width:235px;width:9999;font-size:24pt;}table{box-shadow: 5px 5px 5px #888888;padding:0px;background-color:#D9D9D9;text-align:center;font-family:'docs-Roboto',Arial;color:#ffffff;font-size:12pt;white-space:nowrap;padding:2px 2px 2px 2px;vertical-align:middle;}</style><table><tbody>"
        };
        var scores = artistBattle.rounds.GroupBy(x => x.choice).Select(x => (artistBattle.artists.Find(y => y.ID == x.First().choice), x.Count())).OrderByDescending(x => x.Item2).ToList();
        var newLine = "<tr>";
        for (int i = 0; i < scores.Count(); i++)
            newLine += "<td class=\"" + (i == 0 ? "bracketWin" : "bracketLos") + "\">" + scores[i].Item1.name + "</td>";
        file.Add(newLine + "</tr>");
        newLine = "<tr>";
        for (int i = 0; i < scores.Count(); i++)
            newLine += "<td class=\"" + (i == 0 ? "winsWin" : "winsLos") + "\">" + scores[i].Item2 + "</td>";
        file.Add(newLine + "</tr>");
        foreach (var round in artistBattle.rounds)
        {
            round.candidates = round.candidates.OrderBy(x => scores.FindIndex(y => y.Item1.ID == x.artistID)).ToList();
            newLine = "<tr>";
            for (int i = 0; i < scores.Count(); i++)
            {
                var find = round.candidates.Find(x => x.artistID == scores[i].Item1.ID);
                newLine += find == null ? "<td class=\"los\"></td>" : "<td class=\"" + (find.artistID == round.choice ? "win" : "los") + "\">" + find.track.name + "</td>";
            }
            file.Add(newLine + "</tr>");
        }
        file.Add("</tbody></table>");
        if (!Directory.Exists("MooRT_Export"))
            Directory.CreateDirectory("MooRT_Export");
        if (!Directory.Exists("MooRT_Export/Battles"))
            Directory.CreateDirectory("MooRT_Export/Battles");
        var fileName = "MooRT_Export/Battles/" + DateTime.Now.ToShortDateString() + " " + DateTime.Now.Hour + "h " + DateTime.Now.Minute + "m" + ".html";
        File.WriteAllLines(fileName.Replace("/", "\\"), file);
        Process.Start(fileName.Replace("/", "\\"));
    }
}
