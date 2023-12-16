namespace BasicWebServer.Server.Routing.Contracts;

using HTTP;
using HTTP.Enums;

public interface IRoutingTable
{
    IRoutingTable Map(string url, Method method, Response response);

    IRoutingTable MapGet(string url, Response response);

    IRoutingTable MapPost(string url, Response response);
}