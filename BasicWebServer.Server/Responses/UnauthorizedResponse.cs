namespace BasicWebServer.Server.Responses;

using HTTP;
using HTTP.Enums;

public class UnauthorizedResponse : Response
{
    public UnauthorizedResponse()
        : base(StatusCode.Unauthorized)
    {
    }
}