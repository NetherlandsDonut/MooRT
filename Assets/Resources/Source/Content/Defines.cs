public class Defines
{
    public Defines()
    {
        FillNulls();
    }

    //This function generates default values for the game settings
    public void FillNulls()
    {
        if (textPaddingLeft < 0)
            textPaddingLeft = 4;
        if (textPaddingRight < 0)
            textPaddingRight = 4;
        if (shadowSystem < 0)
            shadowSystem = 1;
        if (markerCharacter == null || markerCharacter == "")
            markerCharacter ??= "_";
        if (textWrapEnding == null || textWrapEnding == "")
            textWrapEnding ??= "...";
    }

    //Amount of padding on the left side of text lines
    public int textPaddingLeft;
    
    //Amount of padding on the right side of text lines
    public int textPaddingRight;

    //Shadow system used by the program
    public int shadowSystem;

    //Text displayed when an input field is active, this 
    public string markerCharacter;

    //Indicates whether the program will use the specail text wrap ending indicator at the end of text lines that don't fit all the text
    public string textWrapEnding;

    //Indicates whether the full rectangular borders of the window are drawn
    public bool windowBorders;

    //EXTERNAL FILE: Collection of constant values or alternate ways of handling engine
    public static Defines defines;
}
