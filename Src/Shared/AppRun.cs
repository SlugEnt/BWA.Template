using Microsoft.Extensions.Configuration;
using Serilog;
using Sheakley.ICS.Common.FunctionClasses;
using Sheakley.ICS.Common.UtilityObjects;
using Sheakley.ICS.VaultClientConnectors;
using System.Reflection;
using System.Text;

namespace Sheakley.ICS.Template.Blazor.Shared;

/// <summary>
/// Core Application runtime information
/// </summary>
public class AppRun
{
    /// <summary>
    /// AppSettings Configuration Builder for the application
    /// </summary>
    public AppSettingsConfig AppSettings { get; private set; }

    public VaultClientConnector VaultClientConnector { get; private set; }

    /// <summary>
    /// Application Program Information
    /// </summary>
    public ServiceProgramInfo ServiceProgramInfo { get; private set; }

    /// <summary>
    /// The Microsoft Configuration object for the application
    /// </summary>
    public IConfiguration AppConfiguration { get; private set; }


    public Serilog.ILogger Logger { get; private set; }

    /// <summary>
    /// Object that builds the configuration
    /// </summary>
    private ConfigurationBuilder ConfigurationBuilder { get;  set; }

    public string AppFullName { get; private set; }

    /// <summary>
    /// Object that holds all Vault information
    /// </summary>
    public VaultRuntimeSetup VaultRuntimeSetup { get; private set; }

    public ServiceProgramInfo SPI { get; private set; }


    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="appFullName"></param>
    /// <param name="vaultRuntimeSetup"></param>
    public AppRun(string appFullName, VaultRuntimeSetup vaultRuntimeSetup)
    {
        AppFullName = appFullName;
        VaultRuntimeSetup = vaultRuntimeSetup;
        ConfigurationBuilder = new ConfigurationBuilder();
        AppSettings = new(ConfigurationBuilder);
    }
    


    /// <summary>
    /// Performs the Initial Logging setup for the app.  Also builds the AppSettings Information
    /// </summary>
    /// <param name="builder"></param>
    public async Task<bool> InitialLoggingSetup()
    {
        // B.  AppSettings Logic
        AppSettings.Build();
        if (await SetupVault()) return true;

        AppConfiguration = ConfigurationBuilder.Build();


        // Logging Setup this is initial for logging during the build process! This is later replaced by the UseSerilog section later once the builder is built!
        LoggerConfiguration logconfig = new LoggerConfiguration()
                                        .ReadFrom.Configuration(AppConfiguration);


        // This provides the context for this initial logger.  If not provided then there is no context!
        ILogger logger = logconfig.CreateLogger();
        Logger = logger.ForContext("SourceContext", AppFullName);
        Log.Information("Starting " + Assembly.GetEntryAssembly().FullName);
        return false;
    }


    /// <summary>
    /// Performs Vault related initialization 
    /// </summary>
    /// <returns>True if the app should exit on return.  False the app should continue running</returns>
    /// <exception cref="ApplicationException"></exception>
    private async Task<bool> SetupVault()
    {
        VaultClientConnector = new(VaultRuntimeSetup.AssemblyQualifiedName) { DebugMode = VaultRuntimeSetup.DebugMode };

        if (VaultRuntimeSetup.DevelopmentMode) { VaultClientConnector.UseDevelopmentHash = true; }

        ServiceProgramInfo = VaultClientConnector.GenerateSPI();

        // If GenerateHash was one of the arguments we exit immediately as the caller just wants our hash code.
        if (VaultRuntimeSetup.GenerateHash)
        {
            ServiceProgramInfo.PrintHashToConsole();
            ServiceProgramInfo.PrintToConsole();
            return true;
        }

        if (VaultRuntimeSetup.UseVault && !await VaultClientConnector.Connect(vaultURL: VaultRuntimeSetup.VaultUrl))
        {
            string vaultMsg = "Connection to Vault Failed.  See previous errors.";

            if (VaultRuntimeSetup.SkipVaultErrors)
                ConsoleExtended.WriteError(vaultMsg + "  Will attempt to continue on using Appsettings files");
            else { throw new ApplicationException("Connection to vault failed"); }
        }

        // Print Secrets if running in debug mode
        if (VaultRuntimeSetup.DebugMode)
        {
            try
            {
                VaultClientConnector.AppConfigSvse.PrintAllSecretsAndAttributes();
            }
            catch (Exception e) { ConsoleExtended.WriteError("Error during DebugMode Print", e.ToString()); }
        }

        return false;
    }
}
