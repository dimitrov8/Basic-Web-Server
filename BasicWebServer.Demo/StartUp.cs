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

    private const string DOWNLOAD_FORM = @"<form action='/Content' method='POST'>
        <input type='submit' value='Download Sites Content' />
</form>";

    private const string FILE_NAME = "content.txt";

    public static async Task Main()
    {
        await DownloadSitesAsTextFile(FILE_NAME, new string[] { "https://judge.softuni.org", "https://softuni.org" });

        await new HttpServer(routes => routes
                .MapGet(" / ", new TextResponse("Hello from the server!"))
                .MapGet("/Redirect", new RedirectResponse("https://softuni.org/"))
                .MapGet("/HTML", new HtmlResponse(HTML_FORM))
                .MapPost("/HTML", new TextResponse("", AddFormDataAction))
                .MapGet("/Content", new HtmlResponse(DOWNLOAD_FORM))
                .MapPost("/Content", new TextFileResponse(FILE_NAME)))
            .Start();
    }


    private static void AddFormDataAction(Request request, Response response)
    {
        response.Body = "";

        foreach ((string key, string value) in request.Form)
        {
            response.Body += $"{key} - {value}";
            response.Body += Environment.NewLine;
        }
    }

    private static async Task<string> DownloadWebSiteContent(string url)
    {
        var httpClient = new HttpClient();

        using var client = httpClient;

        var response = await httpClient.GetAsync(url);
        string html = await response.Content.ReadAsStringAsync();

        return html.Substring(0, 2000);
    }

    private static async Task DownloadSitesAsTextFile(string fileName, string[] urls)
    {
        List<Task<string>> downloads = urls.Select(DownloadWebSiteContent).ToList();
        string[] responses = await Task.WhenAll(downloads);

        string responseString = string.Join(Environment.NewLine + new string('-', 100), responses);
        await File.WriteAllTextAsync(fileName, responseString);
    }
}