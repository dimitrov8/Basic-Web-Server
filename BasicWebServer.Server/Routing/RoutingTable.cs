namespace BasicWebServer.Server.Routing;

using Common;
using Contracts;
using HTTP;
using HTTP.Enums;
using Responses;

public class RoutingTable : IRoutingTable
{
    private readonly Dictionary<
        Method,
        Dictionary<string, Func<Request, Response>>> routes;

    public RoutingTable()
        => this.routes = new Dictionary<Method, Dictionary<string, Func<Request, Response>>>

        {
            [Method.Get] = new(),
            [Method.Post] = new(),
            [Method.Put] = new(),
            [Method.Delete] = new()
        };

    public IRoutingTable Map(Method method,
        string path,
        Func<Request, Response> responseFunction)
    {
        Guard.AgainstNull(path, nameof(path));
        Guard.AgainstNull(responseFunction, nameof(responseFunction));

        this.routes[method][path] = responseFunction;

        return this;
    }

    public IRoutingTable MapGet(string path,
        Func<Request, Response> responseFunction)
        => this.Map(Method.Get, path, responseFunction);

    public IRoutingTable MapPost(string path,
        Func<Request, Response> responseFunction)
        => this.Map(Method.Post, path, responseFunction);

    public Response MatchRequest(Request request)
    {
        var requestMethod = request.Method;
        string requestUrl = request.Url;

        if (!this.routes.ContainsKey(requestMethod)
            || !this.routes[requestMethod].ContainsKey(requestUrl))
        {
            return new NotFoundResponse();
        }

        Func<Request, Response> responseFunction = this.routes[requestMethod][requestUrl];

        return responseFunction(request);
    }
}