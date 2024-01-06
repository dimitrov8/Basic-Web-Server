namespace BasicWebServer.Demo.Controllers;

using Models;
using Server.Controllers;
using Server.HTTP;
using Server.Responses;
using System.Text;
using System.Web;

public class HomeController : Controller
{
    private const string FILE_NAME = "content.txt";

    public HomeController(Request request)
        : base(request)
    {
    }

    public Response Index() => this.Text("Hello from the server!");

    public Response Redirect() => this.Redirect("https://softuni.org");

    public Response Html() => this.View();

    public Response HtmlFormPost()
    {
        string name = this.Request.Form["Name"];
        string age = this.Request.Form["Age"];

        var model = new FormViewModel
        {
            Name = name,
            Age = int.Parse(age)
        };

        return this.View(model);
    }

    public Response Content() => this.View();

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

    public Response DownloadContent()
    {
        DownloadSitesAsTextFile(FILE_NAME, new[] { "https://judge.softuni.org/", "https://softuni.org/" })
            .Wait();

        return new TextFileResponse(FILE_NAME);
    }

    public Response Cookies()
    {
        bool requestHasCookies = this.Request.Cookies.Any(c => c.Name != Server.HTTP.Session.SESSION_COOKIE_NAME);

        if (requestHasCookies)
        {
            var cookieText = new StringBuilder();
            cookieText.AppendLine("<h1>Cookies</h1>");

            cookieText.Append("<table border='1'><tr><th>Name</th><th>Value</th></tr>");

            foreach (var cookie in this.Request.Cookies)
            {
                cookieText
                    .Append("<tr>")
                    .Append($"<td>{HttpUtility.HtmlEncode(cookie.Name)}</td>")
                    .Append($"<td>{HttpUtility.HtmlEncode(cookie.Value)}</td>")
                    .Append("</tr>");
            }

            cookieText.Append("</table>");
            return this.Html(cookieText.ToString());
        }

        var cookies = new CookieCollection
        {
            { "My-Cookie", "My-Value" },
            { "My-Second-Cookie", "My-Second-Value" }
        };

        return this.Html("<h1>Cookies set!</h1>", cookies);
    }

    public Response Session()
    {
        string currentDateKey = "CurrentDate";
        bool sessionExists = this.Request.Session.ContainsKey(currentDateKey);

        if (sessionExists)
        {
            string currentDate = this.Request.Session[currentDateKey];

            return this.Text($"Stored date: {currentDate}!");
        }

        return this.Text("Current date stored!");
    }
}