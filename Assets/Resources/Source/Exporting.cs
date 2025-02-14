using System;
using System.IO;
using System.Linq;
using System.Drawing;
using System.Diagnostics;
using System.Collections.Generic;

using UnityEngine;

using static Root;
using static MusicRelease;

public class Exporting
{
    //This method exports a square chart of XY size
    //that consists of album covers provided on the list
    public static void ExportSquareChart(List<MusicRelease> albums)
    {
        var x = 10;
        var y = 10;
        var offsetWidth = 4;
        var covers = new List<Sprite>();
        for (int i = 0; i < x * y; i++)
            covers.Add(albumCovers[albums[i].ID + ""]);
        var offset = true;
        var amount = 188 + (offset ? offsetWidth : 0);
        var chart = new Texture2D(1, 1);
        chart.SetPixel(0, 0, new UnityEngine.Color(0, 0, 0, 1));
        scale(chart, amount * x + (offset ? offsetWidth : 0), amount * y + (offset ? offsetWidth : 0));
        for (int i = 0; i < x; i++)
            for (int j = 0; j < y && j + i * x < covers.Count; j++)
                Graphics.CopyTexture(covers[j + i * x].texture, 0, 0, 0, 0, 188, 188, chart, 0, 0, j * amount + (offset ? offsetWidth : 0), (x - i - 1) * amount + (offset ? offsetWidth : 0));
        chart.Apply();
        if (!Directory.Exists("MooRT_Export"))
            Directory.CreateDirectory("MooRT_Export");
        File.WriteAllBytes("MooRT_Export/squareChart.png", chart.EncodeToPNG());
    }
}
