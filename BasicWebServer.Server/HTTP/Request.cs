namespace BasicWebServer.Server.HTTP;

using Enums;

// Represents an HTTP request with method, URL, headers, and body.
public class Request
{
    public Method Method { get; private set; } // HTTP method (GET, POST, etc.).

    public string Url { get; private set; } // Requested URL.

    public HeaderCollection Headers { get; private set; } // Collection of HTTP headers.

    public string Body { get; private set; } // Request body content.

    /// <summary>
    ///     Parses a raw HTTP request string into a <see cref="Request" /> object.
    /// </summary>
    /// <param name="request">The raw HTTP request string to parse.</param>
    /// <returns>A <see cref="Request" /> object representing the parsed HTTP request.</returns>
    public static Request Parse(string request)
    {
        string[] lines = request.Split("\r\n"); // Split each line.

        string[] startLine = lines.First().Split(" "); // First Line contains our method and URL.
        var method = ParseMethod(startLine[0]); // Tries to parse the parse the method.
        string url = startLine[1]; // Get the url
        var headers = ParseHeaders(lines.Skip(1)); // Tries to parse the headers.
        string[] bodyLines = lines.Skip(headers.Count + 2).ToArray(); // Get body lines by skipping the first line and the header liens.
        string body = string.Join("\r\n", bodyLines); // Join the body lines to form the body part. 

        // Return the parsed HTTP Request.
        return new Request
        {
            Method = method,
            Url = url,
            Headers = headers,
            Body = body
        };
    }

    /// <summary>
    ///     Helper method to parse the HTTP method.
    /// </summary>
    /// <param name="method">The string representation of the HTTP method.</param>
    /// <returns>The parsed <see cref="Method" /> enum value.</returns>
    private static Method ParseMethod(string method)
    {
        try
        {
            return (Method)Enum.Parse(typeof(Method), method, true); // Ignores casing 
        }
        catch (Exception)
        {
            throw new InvalidOperationException($"Method '{method}' is not supported");
        }
    }

    /// <summary>
    ///     Helper method to parse HTTP headers.
    /// </summary>
    /// <param name="headerLines">The collection of header lines as strings.</param>
    /// <returns>A <see cref="HeaderCollection" /> representing the parsed HTTP headers.</returns>
    private static HeaderCollection ParseHeaders(IEnumerable<string> headerLiens)
    {
        var headerCollection = new HeaderCollection();

        foreach (string headerLine in headerLiens)
        {
            if (headerLine == string.Empty)
            {
                break; // Stop parsing headers if an empty line is encountered.
            }

            string[] headerParts = headerLine.Split(":", 2); // Separates the header 'name' and 'value'.

            if (headerParts.Length != 2) // Ensure that each header line is in the expected format (name and value separated by ':').
            {
                throw new InvalidOperationException("Request is not valid."); // Throw an exception if the format is not as expected.
            }

            string headerName = headerParts[0];
            string value = headerParts[1].Trim();

            headerCollection.Add(headerName, value); // Add the parsed header to the collection.
        }

        return headerCollection;
    }
}