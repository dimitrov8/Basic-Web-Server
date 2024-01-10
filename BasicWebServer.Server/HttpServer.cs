namespace BasicWebServer.Server;

using Common;
using HTTP;
using Routing;
using Routing.Contracts;
using System.Globalization;
using System.Net;
using System.Net.Sockets;
using System.Text;

public class HttpServer
{
    private readonly IPAddress ipAddress;
    private readonly int port;
    private readonly TcpListener serverListener;

    private const int BUFFER_LENGTH = 1024;
    private const int MAX_REQUEST_SIZE = 10 * 1024;

    private readonly RoutingTable routingTable;

    public readonly IServiceCollection ServiceCollection;

    public HttpServer(string ipAddress, int port, Action<IRoutingTable> routingTableConfiguration)
    {
        this.ipAddress = IPAddress.Parse(ipAddress);
        this.port = port;

        this.serverListener = new TcpListener(this.ipAddress, this.port);

        routingTableConfiguration(this.routingTable = new RoutingTable());
        this.ServiceCollection = new ServiceCollection();
    }

    public HttpServer(int port, Action<IRoutingTable> routingTable)
        : this("127.0.0.1", port, routingTable)
    {
    }

    public HttpServer(Action<IRoutingTable> routingTable)
        : this(8080, routingTable)
    {
    }

    public async Task Start()
    {
        this.serverListener.Start();

        Console.WriteLine($"Server started on port {this.port}.");
        Console.WriteLine("Listening for requests...");

        while (true)
        {
            var connection = await this.serverListener.AcceptTcpClientAsync();

            _ = Task.Run(async () =>
            {
                var networkStream = connection.GetStream();

                string requestText = await this.ReadRequest(networkStream);

                Console.WriteLine(requestText);
                var request = Request.Parse(requestText, this.ServiceCollection);
                var response = this.routingTable.MatchRequest(request);

                this.AddSession(request, response);

                await WriteResponse(networkStream, response);

                connection.Close();
            });
        }
    }

    private async Task<string> ReadRequest(NetworkStream networkStream)
    {
        byte[] buffer = new byte[BUFFER_LENGTH];
        var requestBuilder = new StringBuilder();

        int totalBytes = 0;

        do
        {
            int bytesRead = await networkStream.ReadAsync(buffer, 0, BUFFER_LENGTH);
            totalBytes += bytesRead;

            if (totalBytes > MAX_REQUEST_SIZE)
            {
                throw new InvalidOperationException("Request is too large.");
            }

            requestBuilder.Append(Encoding.UTF8.GetString(buffer, 0, bytesRead));
        } while (networkStream.DataAvailable); // May not run correctly over the Internet

        return requestBuilder.ToString();
    }

    private void AddSession(Request request, Response response)
    {
        bool sessionExists = request.Session
            .ContainsKey(Session.SESSION_CURRENT_DATE_KEY);

        if (!sessionExists)
        {
            string currentDate = DateTime.UtcNow.ToString("dd-MMM-yyyy hh:mm:ss tt", CultureInfo.InvariantCulture);
            request.Session[Session.SESSION_CURRENT_DATE_KEY] = currentDate;
            response.Cookies
                .Add(Session.SESSION_COOKIE_NAME, request.Session.Id);
        }
    }

    static async Task WriteResponse(NetworkStream networkStream, Response response)
    {
        byte[] responseBytes = Encoding.UTF8.GetBytes(response.ToString());
        await networkStream.WriteAsync(responseBytes, 0, responseBytes.Length);
    }
}