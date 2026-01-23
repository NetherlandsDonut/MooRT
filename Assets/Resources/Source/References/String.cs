using System;
using System.Linq;
using System.Collections.Generic;

using UnityEngine;

using static Root;
using static Root.InputType;

public class String
{
    public string value = "";
    public InputType inputType = Everything;
    [NonSerialized] private string backupValue = "";

    public string Value() => value;
    public string Insert(int i, char x) => value = value.Length <= i ? value += x : value.Insert(i, x + "");
    public string Add(char x) => value += x;
    public string RemovePreviousOne(int i) => value = value.Remove(i - 1, 1);
    public string RemoveNextOne(int i) => value = value.Remove(i, 1);

    public void Clear() => value = "";
    public void Paste() => value = GUIUtility.systemCopyBuffer.Replace("\b", "").Replace("\r", "\\r").Replace("\n", "\\n").Trim();
    public void Set(string value) => backupValue = this.value = value;
    public void Confirm() => backupValue = value;
    public void Reset() => value = backupValue;

    public bool CheckInput(char letter) => inputType switch
    {
        Letters => char.IsLetter(letter) || letter == ' ' && value.Length != 0 && value.Last() != ' ' || letter == '\'' && value.Length != 0 && value.Last() != '\'',
        StrictLetters => char.IsLetter(letter) || letter == '\'' && value.Length != 0 && value.Last() != '\'',
        Capitals => char.IsLetter(letter) || letter == ' ' && value.Length != 0 && value.Last() != ' ',
        Numbers => char.IsDigit(letter),
        InputType.Decimal => char.IsDigit(letter) || letter == ',' && !Value().Contains(','),
        _ => true,
    };

    public static String promptConfirm = new() { inputType = Capitals };
    public static String searchRelease = new() { inputType = Everything };
    public static String searchArtist = new() { inputType = Everything };
    public static String searchGenre = new() { inputType = Everything };
    public static String searchCountry = new() { inputType = Everything };
    public static String searchLanguage = new() { inputType = Everything };
    public static String searchNewAlbumCountry = new() { inputType = Everything };
    public static String createNewAlbumReleaseName = new() { inputType = Everything };
    public static String createNewAlbumReleaseDate = new() { inputType = Everything };
    public static String createNewAlbumGenres = new() { inputType = Everything };
    public static String createNewAlbumLanguages = new() { inputType = Everything };
    public static String createNewAlbumTracklist = new() { inputType = Everything };
    public static String createNewAlbumArtistName = new() { inputType = Everything };
    public static String createNewAlbumArtistCountry = new() { inputType = Everything };
    public static String createNewAlbumCoverURL = new() { inputType = Everything };
}
