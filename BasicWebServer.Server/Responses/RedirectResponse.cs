namespace BasicWebServer.Server.Responses;

using HTTP;
using HTTP.Enums;

public class RedirectResponse : Response
{
    public RedirectResponse(string location)
        : base(StatusCode.Found)
    {
        this.Headers.Add(Header.LOCATION, location);
    }
}