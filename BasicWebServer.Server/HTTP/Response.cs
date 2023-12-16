﻿namespace BasicWebServer.Server.HTTP;

using Enums;
using System.Text;

public class Response
{
    public Response(StatusCode statusCode)
    {
        this.StatusCode = statusCode;

        this.Headers.Add(Header.SERVER, "My Web Server");
        this.Headers.Add(Header.DATE, $"{DateTime.UtcNow:R}");
    }

    public StatusCode StatusCode { get; init; }

    public HeaderCollection Headers { get; } = new();

    public string Body { get; set; }

    public Action<Request, Response> PreRenderAction { get; protected set; }

    public override string ToString()
    {
        var result = new StringBuilder();

        result.AppendLine($"HTTP/1.1 {(int)this.StatusCode} {this.StatusCode}");

        foreach (var header in this.Headers)
        {
            result.AppendLine(header.ToString());
        }

        result.AppendLine();

        if (!string.IsNullOrEmpty(this.Body))
        {
            result.Append(this.Body);
        }

        return result.ToString();
    }
}