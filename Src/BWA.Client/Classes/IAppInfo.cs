namespace BWA.Client.Classes;

public interface IAppInfo
{
    /// <summary>
    /// The IP address or URL of the API Controller. 
    /// </summary>
    Uri ApiBaseUrl { get; set; }

    /// <summary>
    /// This property returns the base URL for the API. This is useful for constructing API endpoints.
    /// </summary>
    string EndpointApi { get; }

    /// <summary>
    /// This property returns the URL for the OpenAPI specification. This is useful for generating API documentation or for clients to understand the API structure.
    /// </summary>
    string EndpointOpenApi { get; }

    /// <summary>
    /// This property returns the URL for the Scalar OpenAPI interactive component
    /// </summary>
    string EndpointScalar { get; }

    /// <summary>
    /// This property returns the URL for the API info endpoint. This is useful for retrieving information about the API, such as version, title, and other metadata.
    /// </summary>
    string EndpointInfo { get; }

    /// <summary>
    /// This property returns the URL for the API ping endpoint. This is useful for checking if the API is alive and reachable.
    /// </summary>
    string EndpointPing { get; }

    /// <summary>
    /// This property returns the URL for the API health check endpoint. This is useful for checking the health of the API and ensuring that it is functioning correctly.
    /// </summary>
    string EndpointHealth { get; }

    /// <summary>
    /// Provides the simplified info endpoint for the API. This is useful for retrieving basic information about the API without any additional details or metadata.
    /// </summary>
    string EndpointSimple { get; }

    /// <summary>
    /// Provides the URL for the API configuration endpoint. This is useful for retrieving configuration settings or parameters that the API might be using.
    /// </summary>
    string EndpointConfig { get; }
}

