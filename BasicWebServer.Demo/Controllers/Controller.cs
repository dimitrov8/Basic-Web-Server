namespace BasicWebServer.Demo.Controllers;

using Server.HTTP;
using Server.Responses;
using System.Runtime.CompilerServices;

public abstract class Controller
{
    protected Controller(Request request)
    {
        this.Request = request;
    }

    protected Request Request { get; private init; }

    protected Response Text(string text) => new TextResponse(text);

    protected Response Html(string html, CookieCollection cookies = null)
    {
        var response = new HtmlResponse(html);

        if (cookies != null)
        {
            foreach (var cookie in cookies)
            {
                response.Cookies.Add(cookie.Name, cookie.Value);
            }
        }

        return response;
    }

    private string GetControllerName()
        => this.GetType().Name
            .Replace(nameof(Controller), string.Empty);

    protected Response View([CallerMemberName] string viewName = "")
        => new ViewResponse(viewName, this.GetControllerName());

    protected Response View(object model, [CallerMemberName] string viewName = "")
        => new ViewResponse(viewName, this.GetControllerName(), model);

    protected Response BadRequest() => new BadRequestResponse();

    protected Response Unauthorized() => new UnauthorizedResponse();

    protected Response NotFound() => new NotFoundResponse();

    protected Response Redirect(string location) => new RedirectResponse(location);

    protected Response BadRequest(string fileName) => new TextFileResponse(fileName);
}