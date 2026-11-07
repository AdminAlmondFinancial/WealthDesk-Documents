namespace AI.Client.API.Validation;

public class ValidationException(string message, int statusCode = 400) : Exception(message)
{
    public int StatusCode { get; } = statusCode;
}

public sealed class ForbiddenException() : Exception("You do not have access to this area.");

public sealed class NotFoundException() : Exception("Not found.");
