namespace BasicWebServer.Server.Responses;

using HTTP;

public class TextResponse : ContentResponse
{
    public TextResponse(string text)
        : base(text, ContentType.PLAIN_TEXT)
    {
    }
}