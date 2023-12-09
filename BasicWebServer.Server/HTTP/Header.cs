namespace BasicWebServer.Server.HTTP;

using Common;

// Represents an HTTP header with a name and value.
public class Header
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="Header" /> class with the provided name and value.
    /// </summary>
    /// <param name="name">The name of the header.</param>
    /// <param name="value">The value of the header.</param>
    public Header(string name, string value) // Constructor: Initializes a new Header instance with the provided name and value.
    {
        // Guard against null values for 'name' and 'value'.
        Guard.AgainstNull(name, nameof(name));
        Guard.AgainstNull(value, nameof(value));

        this.Name = name;
        this.Value = value;
    }

    public string Name { get; init; } // Property: Gets the name of the header (settable only during initialization).

    public string Value { get; set; } // Property: Gets or sets the value of the header.
}