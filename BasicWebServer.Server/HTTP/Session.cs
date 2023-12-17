namespace BasicWebServer.Server.HTTP;

using Common;

public class Session
{
    public const string SESSION_COOKIE_NAME = "MyWebServerSID";
    public const string SESSION_CURRENT_DATE_KEY = "CurrentDate";

    private readonly Dictionary<string, string> data;

    public Session(string id)
    {
        Guard.AgainstNull(id, nameof(id));

        this.Id = id;

        this.data = new Dictionary<string, string>();
    }

    public string Id { get; init; }

    public string this[string key]
    {
        get => this.data[key];
        set => this.data[key] = value;
    }

    public bool ContainsKey(string key)
        => this.data.ContainsKey(key);
}