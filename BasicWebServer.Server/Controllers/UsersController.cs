namespace BasicWebServer.Server.Controllers;

using HTTP;

public class UsersController : Controller
{
    private const string LOGIN_FORM = @"<form action='/Login' method='POST'>
        Username: <input type='text' name='Username'/>
        Password: <input type='text' name='Password'/>
        <input type='submit' value='Log In' />
</form>";

    private const string USERNAME = "user";
    private const string PASSWORD = "user123";

    public UsersController(Request request)
        : base(request)
    {
    }

    public Response Login() => this.Html(LOGIN_FORM);

    public Response LogInUser()
    {
        this.Request.Session.Clear();

        bool usernameMatches = this.Request.Form["Username"] == USERNAME;
        bool passwordMatches = this.Request.Form["Password"] == PASSWORD;

        if (usernameMatches && passwordMatches)
        {
            if (!this.Request.Session.ContainsKey(Session.SESSION_USER_KEY))
            {
                this.Request.Session[Session.SESSION_USER_KEY] = "MyUserId";

                var cookies = new CookieCollection();
                cookies.Add(Session.SESSION_COOKIE_NAME,
                    this.Request.Session.Id);

                return this.Html("<h3>Logged successfully!</h3>");
            }

            return this.Html("<h3>Logged successfully!</h3>");
        }

        return this.Redirect("/Login");
    }

    public Response Logout()
    {
        this.Request.Session.Clear();

        return this.Html("<h3>Logged out successfully!</h3>");
    }

    public Response GetUserData()
    {
        if (this.Request.Session.ContainsKey(Session.SESSION_USER_KEY))
        {
            return this.Html($"<h3>Currently logged-in user " +
                             $"is with username '{USERNAME}'</h3>");
        }

        return this.Redirect("/Login");
    }
}