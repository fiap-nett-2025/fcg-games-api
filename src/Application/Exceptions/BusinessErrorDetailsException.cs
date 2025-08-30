using Microsoft.AspNetCore.Http;

namespace Application.Exceptions;

public class BusinessErrorDetailsException : BaseCustomException
{
    public BusinessErrorDetailsException(string message)
        : base(StatusCodes.Status400BadRequest, message) { }
}