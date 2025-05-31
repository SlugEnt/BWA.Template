using BWA.Client.Classes;

namespace SlugEnt.BWA.Client.Classes;

/// <summary>
/// This class holds information about the application.  This can be used to store global information about the app, such as version, name, or other relevant details.
/// <para>It should be implemented as a Singleton service</para>
/// </summary>

public class AppInfo : IAppInfo
{
    /// <summary>
    /// Constructor
    /// </summary>
    public AppInfo(string baseUrl)
    {
        ApiBaseUrl = new Uri(baseUrl);
    }


    /// <summary>
    /// The IP address or URL of the API Controller. 
    /// </summary>
    public Uri ApiBaseUrl { get; set; }

    /// <summary>
    /// This property returns the base URL for the API. This is useful for constructing API endpoints.
    /// </summary>
    public string EndpointApi => ApiBaseUrl.ToString() + "api";

    /// <summary>
    /// This property returns the URL for the OpenAPI specification. This is useful for generating API documentation or for clients to understand the API structure.
    /// </summary>
    public string EndpointOpenApi => ApiBaseUrl.ToString() + "openapi/v1.json";

    /// <summary>
    /// This property returns the URL for the Scalar OpenAPI interactive component
    /// </summary>
    public string EndpointScalar => ApiBaseUrl.ToString() + "scalar";

    /// <summary>
    /// This property returns the URL for the API info endpoint. This is useful for retrieving information about the API, such as version, title, and other metadata.
    /// </summary>
    public string EndpointInfo => ApiBaseUrl.ToString() + "info";

    /// <summary>
    /// This property returns the URL for the API ping endpoint. This is useful for checking if the API is alive and reachable.
    /// </summary>
    public string EndpointPing => ApiBaseUrl.ToString() + "info/ping";

    /// <summary>
    /// This property returns the URL for the API health check endpoint. This is useful for checking the health of the API and ensuring that it is functioning correctly.
    /// </summary>
    public string EndpointHealth => ApiBaseUrl.ToString() + "info/health";

    /// <summary>
    /// Provides the simplified info endpoint for the API. This is useful for retrieving basic information about the API without any additional details or metadata.
    /// </summary>
    public string EndpointSimple => ApiBaseUrl.ToString() + "info/simple";

    /// <summary>
    /// Provides the URL for the API configuration endpoint. This is useful for retrieving configuration settings or parameters that the API might be using.
    /// </summary>
    public string EndpointConfig => ApiBaseUrl.ToString() + "info/config";
}
