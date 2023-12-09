namespace BasicWebServer.Server;

using System.Net;
using System.Net.Sockets;
using System.Text;

// Represents a basic HTTP server that listens for incoming client requests.
public class HttpServer
{
    private readonly IPAddress ipAddress; // IP address the server will listen on.
    private readonly int port; // Port the server will listen on.
    private readonly TcpListener serverListener; // TCP listener for handling client connections.

    private bool isRunning = true; // Flag indicating whether the server is running.
    private readonly object lockObject = new(); // Add a lock object for thread safety.

    /// <summary>
    ///     Initializes a new instance of the <see cref="HttpServer" /> class with the specified IP address and port.
    /// </summary>
    /// <param name="ipAddress">The IP address the server will listen on.</param>
    /// <param name="port">The port the server will listen on.</param>
    public HttpServer(string ipAddress, int port)
    {
        // Parse IP address and set up the TcpListener.
        this.ipAddress = IPAddress.Parse(ipAddress);
        this.port = port;

        this.serverListener = new TcpListener(this.ipAddress, this.port);
    }

    // Starts the HTTP server, listening for incoming client requests.
    public void Start()
    {
        this.serverListener.Start(); // Start the server listener.

        Console.WriteLine($"Server started on port {this.port}.");
        Console.WriteLine("Listening for requests...");

        while (this.isRunning)
        {
            try
            {
                this.HandleClientRequest(); // Handle incoming client requests.
            }
            catch (SocketException ex)
            {
                Console.WriteLine($"SocketException: {ex.Message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception: {ex.Message}");
            }
        }
    }

    // Stops the HTTP server.
    public void Stop()
    {
        // Stop the server listener.
        this.isRunning = false;
        this.serverListener.Stop();
    }

    // Handles an incoming client request asynchronously.
    private void HandleClientRequest()
    {
        // Accept a new TCP client connection.
        var connection = this.serverListener.AcceptTcpClient();

        // Handle client request asynchronously.
        Task.Run(() =>
        {
            lock (this.lockObject) // Lock to ensure thread safety.
                try
                {
                    using var networkStream = connection.GetStream(); // Get the network stream from the client connection.
                    string request = this.ReadRequest(networkStream); // Read the client's request.
                    string response = this.ProcessRequest(request); // Process the request and generate a response.
                    WriteResponse(networkStream, response); // Send the response to the client.
                }
                finally
                {
                    connection.Close(); // Ensure the connection is closed even if an exception occurs.
                }
        });
    }

    /// <summary>
    ///     Processes the client's HTTP request and generates an appropriate response.
    /// </summary>
    /// <param name="request">The client's HTTP request.</param>
    /// <returns>The HTTP response generated based on the request.</returns>
    private string ProcessRequest(string request)
    {
        // Generate the appropriate content for the HTTP response.
        string content = request.Contains("GET /favicon.ico")
            ? string.Empty // If the request is for the favicon, respond with an empty content.
            : "Hello from the server!"; // Else if the request is not for the favicon, respond with "Hello from the server!".

        int contentLength = Encoding.UTF8.GetByteCount(content);

        // Construct the HTTP response with headers.
        return $@"HTTP/1.1 200 OK
Content-Type: text/plain; charset=UTF-8
Content-Length: {contentLength}

{content}";
    }

    /// <summary>
    ///     Writes the HTTP response to the client's network stream.
    /// </summary>
    /// <param name="networkStream">The client's network stream.</param>
    /// <param name="response">The HTTP response to be sent to the client.</param>
    static void WriteResponse(NetworkStream networkStream, string response)
    {
        // Convert the response to bytes and send it to the client.
        byte[] responseBytes = Encoding.UTF8.GetBytes(response);
        networkStream.Write(responseBytes, 0, responseBytes.Length);

        // The using statement will automatically close the networkStream.
    }

    /// <summary>
    ///     Reads the client's HTTP request from the network stream.
    /// </summary>
    /// <param name="networkStream">The client's network stream.</param>
    /// <returns>The client's HTTP request as a string.</returns>
    private string ReadRequest(NetworkStream networkStream)
    {
        int bufferLength = 1024;
        byte[] buffer = new byte[bufferLength];
        var requestBuilder = new StringBuilder(); // Append the requested strings and return them as a string.

        int bytesRead;
        string? contentLengthHeader;
        int totalBytesRead = 0;
        int maxRequestSize = 10 * 1024;

        do
        {
            bytesRead = networkStream.Read(buffer, 0, bufferLength); // Read data from the network stream into the buffer.
            requestBuilder.Append(Encoding.UTF8.GetString(buffer, 0, bytesRead)); // Append the read data to the requestBuilder.
            contentLengthHeader = this.GetContentLengthHeader(requestBuilder.ToString()); // Check for the Content-Length header in the request.
            totalBytesRead += bytesRead;

            if (totalBytesRead > maxRequestSize)
            {
                string errorResponse = "HTTP/1.1 413 Request Entity Too Large\r\nContent-Length: 0\r\n\r\n";
                WriteResponse(networkStream, errorResponse);
                throw new InvalidOperationException("Request size exceeds the maximum limit.");
            }
        } while (bytesRead > 0 && !requestBuilder.ToString().EndsWith("\r\n\r\n") && contentLengthHeader == null);

        if (contentLengthHeader != null)
        {
            int contentLength = int.Parse(contentLengthHeader);
            int totalByteRead = bytesRead;

            // Continue reading until the entire content has been received.
            while (totalByteRead < contentLength)
            {
                // Read additional data and append to the requestBuilder.
                bytesRead = networkStream.Read(buffer, 0, bufferLength);
                requestBuilder.Append(Encoding.UTF8.GetString(buffer, 0, bytesRead));
                totalByteRead += bytesRead;
            }
        }

        Console.WriteLine(requestBuilder.ToString()); // Prints the assembled HTTP request string in the console.
        return requestBuilder.ToString(); // Return the assembled HTTP request string.
    }

    /// <summary>
    ///     Helper method to extract the Content-Length header value from HTTP headers.
    /// </summary>
    /// <param name="headers">The HTTP headers as a string.</param>
    /// <returns>The value of the Content-Length header, or null if not found.</returns>
    private string? GetContentLengthHeader(string headers)
    {
        string[] lines = headers.Split("\r\n");

        return lines
            .Where(line => line.StartsWith("Content-Length:"))
            .Select(line => line.Split(":")[1].Trim())
            .FirstOrDefault();
    }
}