namespace BasicWebServer.Server.Controllers;

using HTTP;
using Routing.Contracts;

public static class RoutingTableExtensions
{
    private static TController CreateController<TController>(Request request)
        => (TController)Activator
            .CreateInstance(typeof(TController), request);

    public static IRoutingTable MapGet<TController>(
        this IRoutingTable routingTable,
        string path,
        Func<TController, Response> controllerFunction)
        where TController : Controller
        => routingTable.MapGet(path, request => controllerFunction(
            CreateController<TController>(request)));

    public static IRoutingTable MapPost<TController>(
        this IRoutingTable routingTable,
        string path,
        Func<TController, Response> controllerFunction)
        where TController : Controller
        => routingTable.MapPost(path, request => controllerFunction(
            CreateController<TController>(request)));
}