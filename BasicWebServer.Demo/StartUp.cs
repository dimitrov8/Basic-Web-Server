﻿namespace BasicWebServer.Demo;

using Server;
using Server.HTTP;
using Server.Responses;
using System.Text;
using System.Web;

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
        await DownloadSitesAsTextFile(FILE_NAME, new[] { "https://judge.softuni.org", "https://softuni.org" });

        await new HttpServer(routes => routes
                .MapGet("/", new TextResponse("Hello from the server!"))
                .MapGet("/Redirect", new RedirectResponse("https://softuni.org/"))
                .MapGet("/HTML", new HtmlResponse(HTML_FORM))
                .MapPost("/HTML", new TextResponse("", AddFormDataAction))
                .MapGet("/Content", new HtmlResponse(DOWNLOAD_FORM))
                .MapPost("/Content", new TextFileResponse(FILE_NAME))
                .MapGet("/Cookies", new HtmlResponse("", AddCookiesAction))
                .MapGet("/Session", new TextResponse("", DisplaySessionInfoAction)))
            .Start();
    }

    private static void AddCookiesAction(Request request, Response response)
    {
        bool requestHasCookies = request.Cookies.Any(c => c.Name != Session.SESSION_COOKIE_NAME);
        string bodyText = "";

        if (requestHasCookies)
        {
            var cookieText = new StringBuilder();
            cookieText.AppendLine("<h1>Cookies</h1>");

            cookieText.Append("<table border='1'><tr><th>Name</th><th>Value</th></tr>");

            foreach (var cookie in request.Cookies)
            {
                cookieText
                    .Append("<tr>")
                    .Append($"<td>{HttpUtility.HtmlEncode(cookie.Name)}</td>")
                    .Append($"<td>{HttpUtility.HtmlEncode(cookie.Value)}</td>")
                    .Append("</tr>");
            }

            cookieText.Append("</table>");

            bodyText = cookieText.ToString();
        }
        else
        {
            bodyText = "<h1>Cookies set!</h1>";
        }

        if (!requestHasCookies)
        {
            response.Cookies.Add("My-Cookie", "My-Value");
            response.Cookies.Add("My-Second-Cookie", "My-Second-Value");
        }

        response.Body = bodyText;
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

    private static void DisplaySessionInfoAction(Request request, Response response)
    {
        bool sessionExists = request.Session
            .ContainsKey(Session.SESSION_CURRENT_DATE_KEY);

        string bodyText = "";

        if (sessionExists)
        {
            string currentDate = request.Session[Session.SESSION_CURRENT_DATE_KEY];
            bodyText = $"Stored date: {currentDate}!";
        }
        else
        {
            bodyText = "Current date stored!";
        }

        response.Body = "";
        response.Body += bodyText;
    }
}