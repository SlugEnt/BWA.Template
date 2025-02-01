using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Sheakley.ICS.VaultClientConnectors;
using Sheakley.ICS.VaultClientConnectors.Configuration;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sheakley.ICS.Template.Blazor.Shared;

/// <summary>
/// Builds the Appsettings Configuration for a Sheakley App.
/// </summary>
public class AppSettingsConfig
{
    private IConfigurationBuilder _configurationBuilder;
    

    private List<string> ConfigFiles { get; set; } = new List<string>();

    /// <summary>
    /// This is a list of custom AppSettings Files to use.  These are full path and file name values, including extensions
    /// </summary>
    private List<string> CustomFiles { get; set; } = new List<string>();

    private string _sensitiveName = string.Empty;


    /// <summary>
    /// Environment variables to add to the configuration
    /// </summary>
    private List<string> EnvironmentVariables { get; set; } = new List<string>();



    /// <summary>
    /// Constructor for the AppSettingsConfig
    /// </summary>
    /// <exception cref="Exception"></exception>
    public AppSettingsConfig(IConfigurationBuilder configurationBuilder)
    {
        _configurationBuilder = configurationBuilder;
        

        DirectoryCurrent = Environment.CurrentDirectory;
        DirectoryParent = Directory.GetParent(DirectoryCurrent).FullName;

        EnvironmentName = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
        if (EnvironmentName == null) EnvironmentName = Environment.GetEnvironmentVariable("DOTNET_ENVIRONMENT");
        if (EnvironmentName == null) Console.WriteLine("EnvironmentName is null. It should be specified in a normal run mode."); }


    /// <summary>
    /// The Current Directory the application is run from.
    /// </summary>
    public string DirectoryCurrent { get; set; } = string.Empty;

    /// <summary>
    /// The Parent of the current directory being run from.  For our server deployed applications we put the permanent non changing appconfig in that directory.
    /// </summary>
    public string DirectoryParent { get; set; } = string.Empty;

    /// <summary>
    /// Name of the current Environment
    /// </summary>
    public string EnvironmentName { get; set; } = string.Empty;

    /// <summary>
    /// If true, then the Sensitive Appsetings file will be added.
    /// </summary>
    public bool UseSensitiveAppSettings { get; set; } = false;
    
    /// <summary>
    /// If true the Parent Folder AppSettings file will be added.
    /// </summary>
    public bool UseParentAppSettings { get; set; } = true;
    
    /// <summary>
    /// f true the appsettings file in the deployed folder will be used.
    /// </summary>
    public bool UseDeployedFolderAppSettings { get; set; } = true;


    /// <summary>
    /// Adds the given environment variable to the list of environment variables to be added to the configuration.
    /// </summary>
    /// <param name="envVarName"></param>
    public void AddEnvironmentVariables(string envVarName)
    {
        EnvironmentVariables.Add(envVarName);
    }


    /// <summary>
    ///  The Sheakley Vault Client Connector to inject vault settings into Configuration object
    /// </summary>
    public VaultClientConnector VaultClientConnector { get; set; }


    /// <summary>
    /// The name of the sensitive Appsetting file name.  Include the full path and extension
    /// </summary>
    public string SensitiveName
    {
        get { return _sensitiveName; }
        set
        {
            _sensitiveName = value;
            UseSensitiveAppSettings = true;
        }
    }


    /// <summary>
    /// Adds the Specified AppSetting File to the list to be included.  They will be included in the order they are added.
    /// </summary>
    /// <param name="fullPathandName">Full path and file name, including extension</param>
    /// <param name="requiredToExist">If true, the app will abort if the file does not exist.</param>
    /// <exception cref="Exception"></exception>
    public void AddSettingFile(string fullPathandName, bool requiredToExist = false) {
        if (requiredToExist)
            if (!System.IO.File.Exists(fullPathandName))
                throw new Exception($"The AppSetting file {fullPathandName} does not exist and is required.");

        CustomFiles.Add(fullPathandName);
    }



    /// <summary>
    /// Builds the Actual Application Configuration based upoon the current environment.
    /// </summary>
    /// <returns></returns>
    public void Build()
    {
        // Add Environment Variables.  These are first so they can be overridden as needed.
        foreach (string envVar in EnvironmentVariables)
            _configurationBuilder.AddEnvironmentVariables(envVar);

        // Add the appropriate Appsettings files.  These are always the first settings files added (Meaning there values can be overridden by other files.
        SetAppSettingsDirAndEnvValue(DirectoryCurrent);
        SetAppSettingsDirAndEnvValue(DirectoryParent);


        // This file is always last added.
        if (UseSensitiveAppSettings)
            AddSettingFile(SensitiveName, true);


        // Add Sheakley Secure Vault
        if (VaultClientConnector != null)
        {
            _configurationBuilder.AddSheakleySecureVault(VaultClientConnector);
            _configurationBuilder.AddJsonStream(new MemoryStream(Encoding.ASCII.GetBytes(VaultClientConnector.BuildSeriLogConfig())));
        }



        // Add Config files
            foreach (string file in ConfigFiles)
            AddJsonRecord(_configurationBuilder, file);

        // Add Custom Files
        foreach (string customFile in CustomFiles)
            AddJsonRecord(_configurationBuilder,customFile);

        //return ConfigurationBuilder.Build();
    }



    /// <summary>
    /// Adds the AppSetting file to the ConfigurationBuilder
    /// </summary>
    /// <param name="configurationBuilder"></param>
    /// <param name="file"></param>
    private void AddJsonRecord(IConfigurationBuilder configurationBuilder, string file)
    {
        // See if it exists
        if (System.IO.File.Exists(file))
        {
            Console.WriteLine("AppSetting File [ {0} ] exists and will be used.",file);
            configurationBuilder.AddJsonFile(file, optional: true, reloadOnChange: true);
        }
        else
        {
            Console.WriteLine("Optional AppSetting File [ {0} ] does not exist and will not be used.", file);
        }
    }



    /// <summary>
    /// Sets an Appsetting file value based upon the path and environment name.  Adds it to tjhe Config List
    /// </summary>
    /// <param name="path"></param>
    private void SetAppSettingsDirAndEnvValue(string path, bool isRequired = false)
    {

        string file = Path.Join(path, $"appsettings.{EnvironmentName}.json");
        if (isRequired)
        {
            if (!System.IO.File.Exists(file))
            {
                Console.WriteLine("The AppSetting file {file} does not exist and is required.", file);
                throw new ApplicationException("The Appsetting file [ " + file + " ] does not exist!  It is required.");
            }
        }

        ConfigFiles.Add(file);
    }
}

