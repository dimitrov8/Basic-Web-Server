namespace BasicWebServer.Server.Responses;

using HTTP;
using HTTP.Enums;

public class BadRequestResponse : Response
{
    public BadRequestResponse()
        : base(StatusCode.BadRequest)
    {
    }
}