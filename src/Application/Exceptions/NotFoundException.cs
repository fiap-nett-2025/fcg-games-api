using Microsoft.AspNetCore.Http;

namespace Application.Exceptions;

public class NotFoundException : BaseCustomException
{
    public NotFoundException(string message = "Recurso não encontrado.")
        : base(StatusCodes.Status404NotFound, message) { }
}