using Microsoft.AspNetCore.Http;

namespace FCG.Game.Application.Exceptions;

public class NotFoundException(string message = "Recurso não encontrado.") : BaseCustomException(StatusCodes.Status404NotFound, message)
{
}