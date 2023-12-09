namespace BasicWebServer.Server.HTTP;

using Enums;

// Represents an HTTP response with status code, headers, and body
public class Response
{
    public Response(StatusCode statusCode) // Constructor: Initializes a new Response with the provided status code
    {
        this.StatusCode = statusCode; // Set the status code for the response

        // Add default headers for the response
        this.Headers.Add("Server", "My Web Server");
        this.Headers.Add("Date", $"{DateTime.UtcNow}:r");
    }

    public StatusCode StatusCode { get; init; } // Property: Gets the status code of the response (settable only during initialization)

    public HeaderCollection Headers { get; } = new(); // Property: Gets the headers collection for the response (initialized with default headers)

    public string Body { get; set; } // Property: Gets or sets the body content of the response
}