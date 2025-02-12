using System;
using System.Linq;
using System.Collections.Generic;

using UnityEngine;

public class RegionGroup : MonoBehaviour
{
    //Window this region group belongs to
    public Window window;

    //All the regions this region group has
    public List<Region> regions;

    public Region stretchRegion;
    public int setWidth, setHeight, currentHeight;

    public void Initialise(Window window, bool header)
    {
        regions = new();
        this.window = window;
        if (header) { window.headerGroup = this; window.regionGroups.Insert(0, this); }
        else window.regionGroups.Add(this);
    }

    public int PlannedHeight()
    {
        var regionSum = regions.Sum(x => x.PlannedHeight());
        return regionSum;
    }

    public Region LBRegion() => regions.Last();

    public int AutoWidth()
    {
        var regionMax = regions.Count == 0 ? 0 : regions.Max(x => x.AutoWidth());
        return setWidth != 0 ? setWidth : regionMax;
    }
}
