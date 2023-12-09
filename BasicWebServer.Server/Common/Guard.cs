namespace BasicWebServer.Server.Common;

// Guard class contains utility methods for argument validation.
public static class Guard
{
    public static void AgainstNull(object value, string? name = null)
    {
        // Ensures that the provided object is not null; otherwise, throws an ArgumentException.
        // The optional 'name' parameter is used in the error message to identify the null value.
        if (value == null) // Check if the provided value is null.
        {
            name ??= "Value"; // If 'name' is null, set it to the default value "Value".

            throw new ArgumentException($"{name} cannot be null!"); // Throw an ArgumentException with an informative error message.
        }
    }
}