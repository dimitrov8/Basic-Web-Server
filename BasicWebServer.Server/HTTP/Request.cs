namespace BasicWebServer.Server.HTTP;

using Enums;
using System.Web;

public class Request
{
    public Method Method { get; private set; }

    public string Url { get; private set; }

    public HeaderCollection Headers { get; private set; }

    public string Body { get; private set; }

    public IReadOnlyDictionary<string, string> Form { get; private set; }

    public static Request Parse(string request)
    {
        string[] lines = request.Split("\r\n");

        string[] startLine = lines.First().Split(" ");
        var method = ParseMethod(startLine[0]);
        string url = startLine[1];

        var headers = ParseHeaders(lines.Skip(1));
        string[] bodyLines = lines.Skip(headers.Count + 2).ToArray();
        string body = string.Join("\r\n", bodyLines);

        Dictionary<string, string> form = ParseForm(headers, body);

        return new Request
        {
            Method = method,
            Url = url,
            Headers = headers,
            Body = body,
            Form = form
        };
    }


    private static Method ParseMethod(string method)
    {
        try
        {
            return (Method)Enum.Parse(typeof(Method), method, true);
        }
        catch (Exception)
        {
            throw new InvalidOperationException($"Method '{method}' is not supported");
        }
    }

    private static HeaderCollection ParseHeaders(IEnumerable<string> headerLiens)
    {
        var headerCollection = new HeaderCollection();

        foreach (string headerLine in headerLiens)
        {
            if (headerLine == string.Empty)
            {
                break;
            }

            string[] headerParts = headerLine.Split(":", 2);

            if (headerParts.Length != 2)
            {
                throw new InvalidOperationException("Request is not valid.");
            }

            string headerName = headerParts[0];
            string headerValue = headerParts[1].Trim();

            headerCollection.Add(headerName, headerValue);
        }

        return headerCollection;
    }

    private static Dictionary<string, string> ParseForm(HeaderCollection headers, string body)
    {
        var formCollection = new Dictionary<string, string>();

        if (headers.Contains(Header.CONTENT_TYPE)
            && headers[Header.CONTENT_TYPE] == ContentType.FORM_URL_ENCODED)
        {
            Dictionary<string, string> parsedResult = ParseFormData(body);

            foreach ((string name, string value) in parsedResult)
            {
                formCollection.Add(name, value);
            }
        }

        return formCollection;
    }

    private static Dictionary<string, string> ParseFormData(string bodyLines)
        => HttpUtility.UrlDecode(bodyLines)
            .Split('&')
            .Select(part => part.Split('='))
            .Where(part => part.Length == 2)
            .ToDictionary(
                part => part[0],
                part => part[1],
                StringComparer.InvariantCultureIgnoreCase);
}