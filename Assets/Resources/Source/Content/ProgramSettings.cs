public class ProgramSettings
{
    public ProgramSettings()
    {
        FillNulls();
    }

    //This function generates default values for the settings
    public void FillNulls()
    {
        soundEffects ??= new Bool(true);
        pixelPerfectVision ??= new Bool(false);
    }

    //Indicates whether program plays sound effects
    public Bool soundEffects;

    //Indicates whether camera rendering is being sharp to keep the pixel ratio
    public Bool pixelPerfectVision;

    //EXTERNAL FILE: Collection of all settings
    public static ProgramSettings settings;
}
