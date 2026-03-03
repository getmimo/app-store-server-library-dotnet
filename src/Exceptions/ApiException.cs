using System.Net;
using Mimo.AppStoreServerLibrary.Models;

namespace Mimo.AppStoreServerLibrary.Exceptions;

public class ApiException(
    HttpStatusCode? httpStatusCode,
    ErrorResponse? errorResponse = null,
    Exception? innerException = null
) : Exception(errorResponse?.ErrorMessage, innerException)
{
    public HttpStatusCode? HttpStatusCode { get; } = httpStatusCode;

    public ApiErrorCode? ApiErrorCode { get; } = errorResponse?.ErrorCode;

    public string? ApiErrorMessage { get; } = errorResponse?.ErrorMessage;

    public override string ToString()
    {
        return base.ToString()
            + $" HttpStatusCode: {this.HttpStatusCode}, ApiErrorCode: {this.ApiErrorCode}, ApiErrorMessage: {this.ApiErrorMessage}";
    }
}
