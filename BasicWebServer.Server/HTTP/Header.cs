namespace BasicWebServer.Server.HTTP;

using Common;

public class Header
{
    public Header(string name, string value)
    {
        Guard.AgainstNull(name, nameof(name));
        Guard.AgainstNull(value, nameof(value));

        this.Name = name;
        this.Value = value;
    }

    public const string CONTENT_TYPE = "Content-Type";
    public const string CONTENT_LENGTH = "Content-Length";
    public const string DATE = "Date";
    public const string LOCATION = "Location";
    public const string SERVER = "Server";


    public string Name { get; init; }

    public string Value { get; set; }

    public override string ToString()
        => $"{this.Name}: {this.Value}";
}