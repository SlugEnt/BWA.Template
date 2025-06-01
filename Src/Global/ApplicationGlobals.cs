namespace Global;


    public static class ApplicationGlobals
    {


    // TODO Adjust this to correct name for this project
    public const string SensitiveFileName = "SlugEnt.BWA.Template_AppSettingsSensitive";
    public const string SensitiveSettings = "SENSITIVE_SETTINGS";


    /// <summary>
    /// Returns the full path and file name to the Sensitive AppSettings file.  If the Environment Variable is not found then it returns an empty string.
    /// </summary>
    /// <param name="suffix">A suffix to put afterthe actual filename part of the settings file.  Useful to designate a UnitTest setting file, or Dev or some other way to distinguish it.</param>
    /// <returns></returns>
    public static string GetSensitiveAppSettingFullPath(string suffix = "")
    {
        try
        {
            string  fullPath      = "";
            string? sensitivePath = Environment.GetEnvironmentVariable(ApplicationGlobals.SensitiveSettings);
            if (!string.IsNullOrWhiteSpace(sensitivePath))
            {

                string fileName = suffix != string.Empty
                                      ? ApplicationGlobals.SensitiveFileName + "_" + suffix + ".json"
                                      : ApplicationGlobals.SensitiveFileName + ".json";
                fullPath = Path.Join(sensitivePath, fileName);
            }

            return fullPath;
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error in GetSensitiveAppSettingFullPath: " + ex.Message);
            return "";
        }
    }
}


