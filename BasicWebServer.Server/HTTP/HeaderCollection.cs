namespace BasicWebServer.Server.HTTP;

// Represents a collection of HTTP headers.
public class HeaderCollection
{
    private readonly Dictionary<string, Header> headers; // Dictionary to store headers, where the key is the header name.

    public HeaderCollection() => this.headers = new Dictionary<string, Header>(); // Constructor: Initializes the header collection.

    public int Count => this.headers.Count; // Property: Gets the number of headers in the collection.

    public void Add(string name, string value) // Method: Adds a new header to the collection.
    {
        var header = new Header(name, value); // Create a new Header object with the provided name and value.

        this.headers.Add(name, header); // Add the header to the dictionary using the header name as the key.
    }
}