using BWA.Client.Classes;
using Global;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Web;
using MudBlazor.Services;
using Scalar.AspNetCore;
using Serilog;
using SlugEnt.APIInfo;
using SlugEnt.APIInfo.HealthInfo;
using SlugEnt.BWA.Client.Classes;
using SlugEnt.BWA.Client.Weather;
using SlugEnt.BWA.Server;
using SlugEnt.BWA.Server.Components;
using SlugEnt.IS.AppRun;
using SlugEnt.ResourceHealthChecker;


/// <summary>
/// This is the template for Building a Blazor App that has Interactive (Server and Client) Rendering.  
/// If you Search for TODO Items, each thing that you need to change or Add to for your custom application has been marked.  
/// //TODO
/// Note: 
/// You do not need to change the actual project names for the server or client (ie. BWA.Server and BWA.Client).  You only NEED to change the
/// Application Namespace for the Server Application, by editing the .csproj file.
/// //TODO You also need to change the Namespace in the Common and Database projects.
/// </summary>
public class Program
{
    private static AppRuntime    _appRun;
    private static ProgramCustom _programCustom;


    /// <summary>
    ///     Performs the actual "Building of the application / Consuming of the Services"
    /// </summary>
    /// <param name="builder"></param>
    /// <returns></returns>
    public static WebApplication BuildApp(WebApplicationBuilder builder)
    {
        WebApplication app = builder.Build();

        //        loggerApp = app.Logger;
        _appRun.Logger!.Warning("API Is Starting Up...");
        // OpenAPI Setup
        // NOTE ******  THIS MUST BE BEFORE the UseRouting statement.
        if (app.Environment.IsDevelopment())
        {
            app.MapOpenApi();
            app.MapScalarApiReference(options =>
            {
                options.WithDefaultHttpClient(ScalarTarget.CSharp, ScalarClient.HttpClient);
            });
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

        app.MapGet("/weather-forecast",
                   ([FromServices] IWeatherForecaster WeatherForecaster) =>
                   {
                       return WeatherForecaster.GetWeatherForecastAsync();
                   }).RequireAuthorization();

        app.MapRazorComponents<App>()
           .AddInteractiveServerRenderMode()
           .AddInteractiveWebAssemblyRenderMode()
           .AddAdditionalAssemblies(typeof(SlugEnt.BWA.Client._Imports).Assembly);

        // Call Custom use logic
        _programCustom.BuildApp_UsesMiddle(app);


        // 4.C Endpoints
        app.MapControllers(); // Map all the controllers for the API to work.  This is not a default in the template, but it is required for the API to work.
        app.MapSlugEntPing(); // provides the SlugEnt EndpointPing endpoint
        app.MapSlugEntSimpleInfo(); // Provides SlugEnt Simple EndpointInfo endpoint
        app.MapSlugEntConfig(); // This is the default endpoint for the API to work.  It is not a default in the template, but it is required for the API to work.
        app.MapSlugEntHealth(); // Provides SlugEnt EndpointHealth Checks
        

        // TODO AddAsync Any other application end points.

        app.MapGroup("/authentication").MapLoginAndLogout();

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
                AssemblyQualifiedName = typeof(Program).AssemblyQualifiedName!
            };
            _appRun = new AppRuntime("SlugEnt.BWA.Server", appRuntimeInfo);


            // Build the object that is used to customize the services of the app outside of this class
            _programCustom = new ProgramCustom(_appRun);


            // IF you want to use a Sensitive File for storing secrets, put the path and name here.  Or Add Enviuronment varables, etc
            _appRun.AppSettings.SensitiveName = ApplicationGlobals.GetSensitiveAppSettingFullPath();

            // B.  App AppRun to builder.  Startup Logging immediately - even before we try to construct the host.
            if (_appRun.Build(builder.Configuration))
            {
                return 0;
            }
        }
        catch (Exception ex)
        {
            string msg = "An unexpected error occured during initial application startup.  There is likely no log of this anywhere else.";
            if (_appRun.Logger == null)
            {
                Console.WriteLine(msg);
                Console.WriteLine(ex.ToString());
                Console.WriteLine(ex.Message);

                return 1969;
            }

            _appRun.Logger.Fatal(ex, msg);
            Log.CloseAndFlush();
            return 1;
        }


        // Build and Run App
        try
        {
            // AA.  Setup the Api / Blazor Server Side App - Add Services, etc.
            SetupApp(builder);


            // AB.  Build the App
            WebApplication app = BuildApp(builder);

            // AC.  Start the App  - Await is purposefully left off, we want to possibly be able to run other code.  Await will happen on the WaitForShutdownAsync
            app.StartAsync();


            // AD. Do any post application startup logic
            _programCustom.PostStartLogic(app);

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
    ///     Sets up the API Info Base object
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

        // API Info needs to be setup prior to app start.
        AppInfo appInfo = new(listeningAt);
        builder.Services.AddSingleton<IAppInfo>(appInfo);


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

        // AddAsync Authentication and Authorization
        builder.Services.AddAuthentication(OpenIdConnectDefaults.AuthenticationScheme)
               .AddMicrosoftIdentityWebApp(builder.Configuration!.GetSection("AzureAd"));
        builder.Services.AddAuthorization();
        /*  I am leaving this here.  This is M$ preferred solution but it is not expected to be fixed until .Net 10
                                          builder.Services.AddAuthorization(options =>
                                          {
                                              options.FallbackPolicy = new AuthorizationPolicyBuilder().RequireAuthenticatedUser().Build();
                                          });
*/

        builder.Services.AddOpenApi();


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


        // AddAsync MudBlazor Services
        builder.Services.AddMudServices();


        // AddAsync Application Specific Services
        _programCustom.AddServices(builder);

    }
}
/*
public class Program
{
    private static AppRuntime _appRun;


    /// <summary>
    ///
    /// </summary>
    /// <param name="db">An optional override of the database connection string</param>
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
                AssemblyQualifiedName = typeof(Program).AssemblyQualifiedName
            };
            _appRun = new("Sheakley.TestConsole", appRuntimeInfo);

            // IF you want to use a Sensitive File for storing secrets, put the path and name here.  Or Add Enviuronment varables, etc


            // B.  Startup Logging immediately - even before we try to construct the host.
            if (_appRun.SetupLogging())
                return 0;
        }
        catch (Exception ex)
        {
            string msg = "An unexpected error occured during initial application startup.  There is likely no log of this anywhere else.";
            if (_appRun.Logger == null)
            {
                Console.WriteLine(msg);
                Console.WriteLine(ex.ToString());
                Console.WriteLine(ex.Message);

                return 1969;
            }

            _appRun.Logger.Fatal(ex, msg);
            Log.CloseAndFlush();
            return 1;
        }


        // Build and Run App
        try
        {
            // AA.  Setup the Api / Blazor Server Side App - Add Services, etc.
            SetupApp(builder);


            // AB.  Build the App
            WebApplication app = BuildApp(builder);

            // AC.  Start the App
            app.StartAsync();


            // AD. Do any post application startup logic
            // someEngine engine = app.Services.GetService<someEngine>();
            // engine.DoSomePostStartupLogic();

            // AZ. Wait for app exit
            await app.WaitForShutdownAsync();

            Log.CloseAndFlush();
            return 0;
        }
        catch (Exception ex)
        {
            _appRun.Logger.Fatal(ex, "An unexpected error occured during application running.");
            return -1;
        }

        return 0;
    }



    /// <summary>
    /// Performs the Pre-Build configuration of the application.  Including adding and configuration of services
    /// </summary>
    /// <param name="builder"></param>
    public static void SetupApp(WebApplicationBuilder builder)
    {
        string listeningAt = builder.Configuration.GetSection("Kestrel:EndPoints:Https:Url").Value ?? builder.Configuration.GetSection("Kestrel:EndPoints:Http:Url").Value;
        _appRun.Logger.Warning("Application Started --> Version: {AppVersion}   ProcessID: {ProcessID}  ListeningAt: {URL}",
                               builder.Configuration["Internal:AppVersion"],
                               builder.Configuration["Internal:ProcessID"],
                               listeningAt);

        // This is the Serilog that will be used after build!
        builder.Host.UseSerilog((hostContext,
                                 AsyncServiceScope,
                                 configuration) =>
        {
            configuration.ReadFrom.Configuration(hostContext.Configuration);
        });


        APIInfoBase apiInfoBase = SetupAPIInfoBase();
        builder.Services.AddSingleton<IAPIInfoBase>(apiInfoBase);
        builder.Services.AddSingleton<HealthCheckProcessor>();
        builder.Services.AddHostedService<HealthCheckerBackgroundProcessor>();

        //builder.Services.AddHostedService<BackgroundProcessor>();

        // Add Razor and Auth services to the container.
        builder.Services.AddRazorComponents()
               .AddInteractiveServerComponents()
               .AddInteractiveWebAssemblyComponents()
               .AddAuthenticationStateSerialization();

        // Add Authentication and Authorization
        builder.Services.AddAuthentication(OpenIdConnectDefaults.AuthenticationScheme)
               .AddMicrosoftIdentityWebApp(builder.Configuration.GetSection("AzureAd"));
        builder.Services.AddAuthorization();


        builder.Services.AddControllers();

        //        builder.Services.AddSingleton<PrintManagementEngine>();
        //        builder.Services.AddControllers();
        builder.Services.AddHttpLogging(options => { });

        //        builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        //        builder.Services.AddEndpointsApiExplorer();
        //        builder.Services.AddSwaggerGen();
        builder.Services.AddProblemDetails();


        // Configure Kestrel - Max upload size is 100MB
        builder.WebHost.ConfigureKestrel(options => { options.Limits.MaxRequestBodySize = 1024 * 1024 * 100; });


        // Add MudBlazor Services
        builder.Services.AddMudServices();


        // TODO aDD Your Application Specific Services
        builder.Services.AddScoped<IWeatherForecaster, ServerWeatherForecaster>();
    }



    /// <summary>
    ///     Sets up the API Info Base object
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
    /// Performs the actual "Building of the application / Consuming of the Services"
    /// </summary>
    /// <param name="builder"></param>
    /// <returns></returns>
    public static WebApplication BuildApp(WebApplicationBuilder builder)
    {
        WebApplication app = builder.Build();

        //        loggerApp = app.Logger;
        _appRun.Logger.Warning("API Is Starting Up...");

        // Print configuration to terminal window
//        IConfigurationPrinter.PrintToConsole(app.Configuration);


        app.UseRouting();
        app.UseAuthorization();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseWebAssemblyDebugging();
        }
        else
        {
            app.UseExceptionHandler("/Error", createScopeForErrors: true);

            // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
            app.UseHsts();
        }

        //app.UseExceptionHandler();
        app.UseStatusCodePages();
        app.UseSerilogRequestLogging();
        app.UseHttpLogging();
        app.UseHttpsRedirection();

        //        app.UseAuthorization();

        app.UseStaticFiles();
        app.UseAntiforgery();
        app.MapStaticAssets();


        app.MapRazorComponents<App>()
           .AddInteractiveServerRenderMode()
           .AddInteractiveWebAssemblyRenderMode()
           .AddAdditionalAssemblies(typeof(SlugEnt.BWA.Client._Imports).Assembly);


        // 4.C Endpoints
        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllers();
            endpoints.MapSlugEntPing();
            endpoints.MapSlugEntSimpleInfo();
            endpoints.MapSlugEntConfig();
            endpoints.MapSlugEntHealth();
        });


        // TODO Add Any other application end points.

        app.MapGet("/weather-forecast", ([FromServices] IWeatherForecaster WeatherForecaster) => { return WeatherForecaster.GetWeatherForecastAsync(); }).RequireAuthorization();


        app.MapGroup("/authentication").MapLoginAndLogout();

        return app;
    }
}
*/