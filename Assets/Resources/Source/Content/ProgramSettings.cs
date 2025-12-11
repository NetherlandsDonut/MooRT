public class ProgramSettings
{
    public ProgramSettings()
    {
        FillNulls();
    }

    //This function generates default values for the settings
    public void FillNulls()
    {
        pixelPerfectVision ??= new Bool(false);
    }

    //Indicates whether camera rendering is being sharp to keep the pixel ratio
    public Bool pixelPerfectVision;

    //EXTERNAL FILE: Collection of all settings
    public static ProgramSettings settings;
}
