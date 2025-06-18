using Global;
using Microsoft.AspNetCore.Http.Features;
using Serilog;
using SlugEnt.APIInfo;
using SlugEnt.APIInfo.HealthInfo;
using SlugEnt.IS.AppRun;
using SlugEnt.ResourceHealthChecker;

//********************************************************************************************
//********************************************************************************************
//********************************************************************************************

// NO CHANGES SHOULD BE MADE TO THIS FILE BY ANY APPLICATION.  THIS SHOULD JUST BE A COOKIE
// CUTTER REPLACE FILE.  ALL CUSTOMIZATIONS SHOULD BE MADE IN THE
//                                ProgramCustom.cs FILE

//********************************************************************************************
//********************************************************************************************
//********************************************************************************************

namespace Service;



/// <summary>
///     This is the template for Building a Blazor App that has Interactive (Server and Client) Rendering.
///     If you Search for TODO Items, each thing that you need to change or AddAsync to for your custom application has been
///     marked.
///     //TODO
///     Note:
///     You do not need to change the actual project names for the server or client (ie. BWA.Server and BWA.Client).  You
///     only NEED to change the
///     Application Namespace for the Server Application, by editing the .csproj file.
///     //TODO You also need to change the Namespace in the Common and Database projects.
/// </summary>
public class Program
{
    private static AppRuntime?    _appRun;
    private static ProgramCustom? _programCustom;


    /// <summary>
    ///     Performs the actual "Building of the application / Consuming of the Services"
    /// </summary>
    /// <param name="builder"></param>
    /// <returns></returns>
    public static WebApplication BuildApp(WebApplicationBuilder builder)
    {
        WebApplication app = builder.Build();

        //        loggerApp = app.Logger;
        _appRun!.Logger!.Warning("API Is Starting Up...");

        // OpenAPI Setup
        // NOTE ******  THIS MUST BE BEFORE the UseRouting statement.
        if (app.Environment.IsDevelopment())
        {
        }

        app.UseRouting();
        app.UseAuthorization();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseWebAssemblyDebugging();
        }
        else
        {
            app.UseExceptionHandler("/AppError", true);

            // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
            app.UseHsts();
        }

        // Setup Serilog Request Logging:  Lots of Request Info
        app.UseSerilogRequestLogging();



        // Call custom Use logic
        _programCustom!.BuildApp_UsesStart(app);


        app.UseStatusCodePages();
        app.UseSerilogRequestLogging();
        app.UseHttpLogging();
        app.UseHttpsRedirection();

        app.MapStaticAssets();
        app.UseAntiforgery();
        app.MapStaticAssets();



        // Call Custom use logic
        _programCustom.BuildApp_UsesMiddle(app);


        // 4.C Endpoints
        app.MapControllers(); // Map all the controllers for the API to work.  This is not a default in the template, but it is required for the API to work.
        app.MapSlugEntPing(); // provides the SlugEnt EndpointPing endpoint
        app.MapSlugEntSimpleInfo(); // Provides SlugEnt Simple EndpointInfo endpoint
        app.MapSlugEntConfig(); // This is the default endpoint for the API to work.  It is not a default in the template, but it is required for the API to work.
        app.MapSlugEntHealth(); // Provides SlugEnt EndpointHealth Checks
        

        _programCustom.BuildApp_Final(app);

        return app;
    }


    /// <summary>
    /// </summary>
    /// <returns></returns>
    public static async Task<int> Main(bool developmentMode = false,
                                       bool debugMode = false)

    {
        WebApplicationBuilder builder = WebApplication.CreateBuilder();

        Console.WriteLine("API is Initiating");

        try
        {
            // A. Initialize AppRun Object
            AppRuntimeInfo appRuntimeInfo = new()
            {
                DevelopmentMode       = developmentMode,
                DebugMode             = debugMode,
                AssemblyQualifiedName = typeof(Program).AssemblyQualifiedName!,
            };
            _appRun = new AppRuntime("HRNextGen25 Services App", appRuntimeInfo);


            // Build the object that is used to customize the services of the app outside of this class
            _programCustom = new ProgramCustom(_appRun);


            // IF you want to use a Sensitive File for storing secrets, put the path and name here.  Or AddAsync Enviuronment varables, etc
            // Set Sensitive file if the Environment Variable is set.  Otherwise we skip it...
            _appRun.AppSettings.SensitiveName = ApplicationGlobals.GetSensitiveAppSettingFullPath();
            //_appRun.AppSettings.SensitiveName = @"D:\a_dev\AppSettings\Daborg69.HrNextGen25_AppSettingsSensitive.json";

            // B.  Startup Logging immediately - even before we try to construct the host.
            if (_appRun.Build(builder.Configuration))
            {
                return 0;
            }
        }
        catch (Exception ex)
        {
            string msg = "An unexpected error occured during initial application startup.  There is likely no log of this anywhere else.";
            if (_appRun!.Logger == null)
            {
                Console.WriteLine(msg);
                Console.WriteLine(ex.ToString());
                Console.WriteLine(ex.Message);

                return 1969;
            }

            _appRun!.Logger.Fatal(ex, msg);
            Log.CloseAndFlush();
            return 1;
        }


        // Build and Run App
        try
        {
            // AA.  Setup the Api / Blazor Server Side App - AddAsync Services, etc.
            SetupApp(builder);


            // AB.  Build the App
            WebApplication app = BuildApp(builder);

            // AC.  Start the App  - Await is purposefully left off, we want to possibly be able to run other code.  Await will happen on the WaitForShutdownAsync
#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
            app.StartAsync();
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed


            // AD. Do any post application startup logic
            await _programCustom.PostStartLogic(app);

            
            

            // AZ. Wait for app exit
            await app.WaitForShutdownAsync();

            
            Log.CloseAndFlush();
        }
        catch (Exception ex)
        {
            _appRun.Logger!.Fatal(ex, "An unexpected error occured during application running.");
            return -1;
        }

        return 0;
    }



    /// <summary>
    ///     Sets up the API EndpointInfo Base object
    /// </summary>
    /// <returns></returns>
    private static APIInfoBase SetupAPIInfoBase()
    {
        // This is how you override the api endpoint to something other than info
        APIInfoBase apiInfoBase = new();
        apiInfoBase.AddConfigHideCriteria("password");
        apiInfoBase.AddConfigHideCriteria("os");
        return apiInfoBase;
    }



    /// <summary>
    ///     Performs the Pre-Build configuration of the application.  Including adding and configuration of services
    /// </summary>
    /// <param name="builder"></param>
    public static void SetupApp(WebApplicationBuilder builder)
    {
        // Setup Kestrel
        string? listeningAt = builder.Configuration.GetSection("Kestrel:EndPoints:Https:Url").Value ?? builder.Configuration.GetSection("Kestrel:EndPoints:Http:Url").Value;


        
        // Configure Kestrel - Max upload size is 100MB
        builder.WebHost.ConfigureKestrel(options => { options.Limits.MaxRequestBodySize = 1024 * 1024 * 100; });


        _appRun!.Logger!.Warning("Application Started --> Version: {AppVersion}   ProcessID: {ProcessID}  ListeningAt: {URL}",
                                builder.Configuration["Internal:AppVersion"],
                                builder.Configuration["Internal:ProcessID"],
                                listeningAt);

        // This is the Serilog that will be used after build!
        builder.Host.UseSerilog((hostContext,
                                 AsyncServiceScope,
                                 configuration) =>
        {
            //configuration.ReadFrom.Configuration(hostContext.Configuration);
            configuration.ReadFrom.Configuration(_appRun!.AppConfiguration!);
        });
        

        APIInfoBase apiInfoBase = SetupAPIInfoBase();
        builder.Services.AddSingleton<IAPIInfoBase>(apiInfoBase);
        builder.Services.AddSingleton<HealthCheckProcessor>();
        builder.Services.AddHostedService<HealthCheckerBackgroundProcessor>();
        builder.Services.AddTransient<ISimpleInfoRetriever, SimpleRetrieverHostInfo>();

        //builder.Services.AddHostedService<BackgroundProcessor>();

        // AddAsync Razor and Auth services to the container.
        builder.Services.AddRazorComponents()
               .AddInteractiveServerComponents()
               .AddInteractiveWebAssemblyComponents()
               .AddAuthenticationStateSerialization(options => options.SerializeAllClaims = true);

        
        _programCustom!.ConfigureDB(builder);


        builder.Services.AddControllers();
        builder.Services.AddHttpLogging(options => { });
        
        // AddAsync customized error handling
        builder.Services.AddProblemDetails(opt =>
        {
            opt.CustomizeProblemDetails = context =>
            {
                context.ProblemDetails.Instance = $"{context.HttpContext.Request.Method} {context.HttpContext.Request.Path}";
                context.ProblemDetails.Extensions.TryAdd("requestId", context.HttpContext.TraceIdentifier);
                var activity = context.HttpContext.Features.Get<IHttpActivityFeature>()?.Activity;
                context.ProblemDetails.Extensions.TryAdd("traceid", activity?.Id);
            };
        });


        // Configure Kestrel - Max upload size is 100MB
        builder.WebHost.ConfigureKestrel(options => { options.Limits.MaxRequestBodySize = 1024 * 1024 * 100; });


        // AddAsync Application Specific Services
        _programCustom.AddServices(builder);
    }
}