namespace BasicWebServer.Demo;

using Server;
using Server.HTTP;
using Server.Responses;

public class StartUp
{
    private const string HTML_FORM = @"<form action='/HTML' method='POST'>
        Name: <input type='text' name='Name'/>
        Age: <input type='number' name='Age'/>
        <input type='submit' value='Save' />
</form>";

    private static void AddFormDataAction(Request request, Response response)
    {
        response.Body = "";

        foreach ((string key, string value) in request.Form)
        {
            response.Body += $"{key} - {value}";
            response.Body += Environment.NewLine;
        }
    }

    public static void Main()
    {
        new HttpServer(routes => routes
                .MapGet(" / ", new TextResponse("Hello from the server!"))
                .MapGet("/Redirect", new RedirectResponse("https://softuni.org/"))
                .MapGet("/HTML", new HtmlResponse(HTML_FORM))
                .MapPost("/HTML", new TextResponse("", AddFormDataAction)))
            .Start();
    }
}