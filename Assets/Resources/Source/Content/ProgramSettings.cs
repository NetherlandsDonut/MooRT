using System.Linq;
using System.Collections.Generic;

using UnityEngine;
using static UnityEngine.KeyCode;

using static Root;

public class ProgramSettings
{
    public ProgramSettings() { }

    //This function generates default values for the settings
    public void FillNulls()
    {
        pixelPerfectVision ??= new Bool(false);
        ratingRanges ??= RatingRange.DefaultRatingRanges();
    }

    //Indicates whether camera rendering is being sharp to keep the pixel ratio
    public Bool pixelPerfectVision;

    //Rating ranges
    public List<RatingRange> ratingRanges;

    //EXTERNAL FILE: Collection of all settings
    public static ProgramSettings settings;
}

public class RatingRange
{
    public static List<RatingRange> DefaultRatingRanges()
    {
        return new()
        {
            new()
            {
                min = new String() { inputType = InputType.Numbers, value = "970" },
                r = 221,
                g = 110,
                b = 000
            },
            new()
            {
                min = new String() { inputType = InputType.Numbers, value = "900" },
                r = 163,
                g = 053,
                b = 238
            },
            new()
            {
                min = new String() { inputType = InputType.Numbers, value = "800" },
                r = 000,
                g = 117,
                b = 226
            },
            new()
            {
                min = new String() { inputType = InputType.Numbers, value = "700" },
                r = 026,
                g = 201,
                b = 000
            },
            new()
            {
                min = new String() { inputType = InputType.Numbers, value = "0" },
                r = 183,
                g = 183,
                b = 183
            },
            new()
            {
                min = new String() { inputType = InputType.Numbers, value = "0" },
                r = 183,
                g = 183,
                b = 183
            },
            new()
            {
                min = new String() { inputType = InputType.Numbers, value = "0" },
                r = 183,
                g = 183,
                b = 183
            },
            new()
            {
                min = new String() { inputType = InputType.Numbers, value = "0" },
                r = 183,
                g = 183,
                b = 183
            },
            new()
            {
                min = new String() { inputType = InputType.Numbers, value = "0" },
                r = 183,
                g = 183,
                b = 183
            },
            new()
            {
                min = new String() { inputType = InputType.Numbers, value = "0" },
                r = 183,
                g = 183,
                b = 183
            },
            new()
            {
                min = new String() { inputType = InputType.Numbers, value = "0" },
                r = 183,
                g = 183,
                b = 183
            },
            new()
            {
                min = new String() { inputType = InputType.Numbers, value = "0" },
                r = 183,
                g = 183,
                b = 183
            },
            new()
            {
                min = new String() { inputType = InputType.Numbers, value = "0" },
                r = 183,
                g = 183,
                b = 183
            },
            new()
            {
                min = new String() { inputType = InputType.Numbers, value = "0" },
                r = 183,
                g = 183,
                b = 183
            },
            new()
            {
                min = new String() { inputType = InputType.Numbers, value = "0" },
                r = 183,
                g = 183,
                b = 183
            },
            new()
            {
                min = new String() { inputType = InputType.Numbers, value = "0" },
                r = 183,
                g = 183,
                b = 183
            },
            new()
            {
                min = new String() { inputType = InputType.Numbers, value = "0" },
                r = 183,
                g = 183,
                b = 183
            },
            new()
            {
                min = new String() { inputType = InputType.Numbers, value = "0" },
                r = 183,
                g = 183,
                b = 183
            }
        };
    }

    public string GetColorCode() => r + ":" + g + ":" + b;

    public void PrintOut()
    {
        AddPaddingRegion(() =>
        {
            AddLine("≥", "Gray");
            AddInputLine(min, GetColorCode());
            AddSmallButton("OtherReverse",
                (h) =>
                {
                    min.Set("0");
                    r = 183;
                    g = 183;
                    b = 183;
                    ProgramSettings.settings.ratingRanges = ProgramSettings.settings.ratingRanges.OrderByDescending(x => int.Parse(x.min.Value())).ToList();
                    CDesktop.RespawnAll();
                }
            );
        });
        AddPaddingRegion(() =>
        {
            AddLine("Red:", "Gray");
            AddLine("" + r, "Gray", "Right");
            AddSmallButton(r < 255 ? "OtherAdd" : "OtherAddOff",
                (h) =>
                {
                    if (r >= 255) return;
                    if (Input.GetKey(LeftShift)) r += 20;
                    else r++;
                    if (r > 255) r = 255;
                    CDesktop.RespawnAll();
                }
            );
            AddSmallButton(r > 0 ? "OtherDetract" : "OtherDetractOff",
                (h) =>
                {
                    if (r <= 0) return;
                    if (Input.GetKey(LeftShift)) r -= 20;
                    else r--;
                    if (r < 0) r = 0;
                    CDesktop.RespawnAll();
                }
            );
        });
        AddPaddingRegion(() =>
        {
            AddLine("Green:", "Gray");
            AddLine("" + g, "Gray", "Right");
            AddSmallButton(g < 255 ? "OtherAdd" : "OtherAddOff",
                (h) =>
                {
                    if (g >= 255) return;
                    if (Input.GetKey(LeftShift)) g += 20;
                    else g++;
                    if (g > 255) g = 255;
                    CDesktop.RespawnAll();
                }
            );
            AddSmallButton(g > 0 ? "OtherDetract" : "OtherDetractOff",
                (h) =>
                {
                    if (g <= 0) return;
                    if (Input.GetKey(LeftShift)) g -= 20;
                    else g--;
                    if (g < 0) g = 0;
                    CDesktop.RespawnAll();
                }
            );
        });
        AddPaddingRegion(() =>
        {
            AddLine("Blue:", "Gray");
            AddLine("" + b, "Gray", "Right");
            AddSmallButton(b < 255 ? "OtherAdd" : "OtherAddOff",
                (h) =>
                {
                    if (b >= 255) return;
                    if (Input.GetKey(LeftShift)) b += 20;
                    else b++;
                    if (b > 255) b = 255;
                    CDesktop.RespawnAll();
                }
            );
            AddSmallButton(g > 0 ? "OtherDetract" : "OtherDetractOff",
                (h) =>
                {
                    if (b <= 0) return;
                    if (Input.GetKey(LeftShift)) b -= 20;
                    else b--;
                    if (b < 0) b = 0;
                    CDesktop.RespawnAll();
                }
            );
        });
    }

    //Rating range
    public String min;

    //Color
    public int r, g, b;
}
