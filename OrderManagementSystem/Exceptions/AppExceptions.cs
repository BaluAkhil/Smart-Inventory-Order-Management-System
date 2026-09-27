using Microsoft.AspNetCore.Http;

namespace OrderManagementSystem.Exceptions;

public abstract class AppException(string message, int statusCode)
    : Exception(message)
{
    public int StatusCode { get; } = statusCode;
}

public class NotFoundException(string message)
    : AppException(message, StatusCodes.Status404NotFound);

public class BadRequestException(string message)
    : AppException(message, StatusCodes.Status400BadRequest);

public class ForbiddenException(string message)
    : AppException(message, StatusCodes.Status403Forbidden);

public class InsufficientStockException(string message)
    : AppException(message, StatusCodes.Status409Conflict);

public class ConflictException(string message)
    : AppException(message, StatusCodes.Status409Conflict);