namespace BasicWebServer.Server.Responses;

using HTTP;
using HTTP.Enums;

public class TextFileResponse : Response
{
    public TextFileResponse(string fileName)
        : base(StatusCode.OK)
    {
        this.FileName = fileName;

        this.Headers.Add(Header.CONTENT_TYPE, ContentType.PLAIN_TEXT);
    }

    public string FileName { get; init; }

    public override string ToString()
    {
        if (File.Exists(this.FileName))
        {
            this.Body = File.ReadAllTextAsync(this.FileName).Result;
            long fileBytesCount = new FileInfo(this.FileName).Length;
            this.Headers.Add(Header.CONTENT_LENGTH, fileBytesCount.ToString());
            this.Headers.Add(Header.CONTENT_DISPOSITION, $"attachment; filename=\"{this.FileName}\"");
        }

        return base.ToString();
    }
}