namespace BasicWebServer.Demo.Controllers;

using Server.HTTP;
using Server.Routing.Contracts;
using System.Reflection;

public static class RoutingTableExtensions
{
    private static TController CreateController<TController>(Request request)
        => (TController)Activator
            .CreateInstance(typeof(TController), request);

    private static Controller CreateController(Type controllerType, Request request)
    {
        var controller = (Controller)Request.ServiceCollection.CreateInstance(controllerType);

        controllerType
            .GetProperty("Request", BindingFlags.Instance | BindingFlags.NonPublic)
            .SetValue(controller, request);

        return controller;
    }

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