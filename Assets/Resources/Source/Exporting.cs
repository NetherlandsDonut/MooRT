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
        var offsetWidth = offset ? 4 : 0;
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
        if (!Directory.Exists("MooRT_Export"))
            Directory.CreateDirectory("MooRT_Export");
        File.WriteAllBytes("MooRT_Export/squareChart.png", chart.EncodeToPNG());
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
