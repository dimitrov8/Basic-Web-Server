namespace BasicWebServer.Server;

using HTTP;
using Routing;
using Routing.Contracts;
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


    public HttpServer(string ipAddress, int port, Action<IRoutingTable> routingTableConfiguration)
    {
        this.ipAddress = IPAddress.Parse(ipAddress);
        this.port = port;

        this.serverListener = new TcpListener(this.ipAddress, this.port);

        routingTableConfiguration(this.routingTable = new RoutingTable());
    }

    public HttpServer(int port, Action<IRoutingTable> routingTable)
        : this("127.0.0.1", port, routingTable)
    {
    }

    public HttpServer(Action<IRoutingTable> routingTable)
        : this(8080, routingTable)
    {
    }

    public void Start()
    {
        this.serverListener.Start();

        Console.WriteLine($"Server started on port {this.port}.");
        Console.WriteLine("Listening for requests...");

        while (true)
        {
            var connection = this.serverListener.AcceptTcpClient();

            var networkStream = connection.GetStream();

            string requestText = this.ReadRequest(networkStream);

            Console.WriteLine(requestText);
            var request = Request.Parse(requestText);
            var response = this.routingTable.MatchRequest(request);

            // Execute pre-render action for the response
            if (response.PreRenderAction != null)
            {
                response.PreRenderAction(request, response);
            }

            WriteResponse(networkStream, response);

            connection.Close();
        }
    }

    private string ReadRequest(NetworkStream networkStream)
    {
        byte[] buffer = new byte[BUFFER_LENGTH];
        var requestBuilder = new StringBuilder();

        int totalBytes = 0;

        do
        {
            int bytesRead = networkStream.Read(buffer, 0, BUFFER_LENGTH);
            totalBytes += bytesRead;

            if (totalBytes > MAX_REQUEST_SIZE)
            {
                throw new InvalidOperationException("Request is too large.");
            }

            requestBuilder.Append(Encoding.UTF8.GetString(buffer, 0, bytesRead));
        } while (networkStream.DataAvailable); // May not run correctly over the Internet

        return requestBuilder.ToString();
    }

    static void WriteResponse(NetworkStream networkStream, Response response)
    {
        byte[] responseBytes = Encoding.UTF8.GetBytes(response.ToString());
        networkStream.Write(responseBytes, 0, responseBytes.Length);
    }
}