using Microsoft.AspNetCore.Http;

namespace FCG.Games.Application.Exceptions;

public class NotFoundException(string message = "Recurso não encontrado.") : BaseCustomException(StatusCodes.Status404NotFound, message)
{
}