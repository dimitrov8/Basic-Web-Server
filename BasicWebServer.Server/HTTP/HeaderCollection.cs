namespace BasicWebServer.Server.HTTP;

using System.Collections;

public class HeaderCollection : IEnumerable<Header>
{
    private readonly Dictionary<string, Header> headers;

    public HeaderCollection()
        => this.headers = new Dictionary<string, Header>();

    public string this[string name]
        => this.headers[name].Value;

    public int Count => this.headers.Count;

    public bool Contains(string name)
        => this.headers.ContainsKey(name);

    public void Add(string name, string value)
    {
        var header = new Header(name, value);

        this.headers.TryAdd(name, header); // Because favicon closes the connection.
    }

    public IEnumerator<Header> GetEnumerator()
        => this.headers.Values.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator()
        => this.GetEnumerator();
}