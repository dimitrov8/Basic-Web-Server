namespace BasicWebServer.Server.Responses;

using HTTP;
using HTTP.Enums;

public class NotFoundResponse : Response
{
    public NotFoundResponse()
        : base(StatusCode.NotFound)
    {
    }
}