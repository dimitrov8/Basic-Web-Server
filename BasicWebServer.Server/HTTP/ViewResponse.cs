namespace BasicWebServer.Server.HTTP;

public class ViewResponse : ContentResponse
{
    private const char PATH_SEPARATOR = '/';

    private string PopulateModel(string viewContent, object model)
    {
        var data = model
            .GetType()
            .GetProperties()
            .Select(pr => new
            {
                pr.Name,
                Value = pr.GetValue(model)
            });

        foreach (var entry in data)
        {
            const string openingBrackets = "{{";
            const string closingBrackets = "}}";

            viewContent = viewContent.Replace(
                $"{openingBrackets}{entry.Name}{closingBrackets}",
                entry.Value.ToString());
        }

        return viewContent;
    }

    public ViewResponse(string viewName, string controllerName, object model = null)
        : base("", ContentType.HTML)
    {
        if (!viewName.Contains(PATH_SEPARATOR))
        {
            viewName = controllerName + PATH_SEPARATOR + viewName;
        }

        string viewPath = Path.GetFullPath(
            "./Views/" +
            viewName.TrimStart(PATH_SEPARATOR)
            + ".cshtml");

        string viewContent = File.ReadAllText(viewPath);

        if (model != null)
        {
            viewContent = this.PopulateModel(viewContent, model);
        }

        this.Body = viewContent;
    }
}