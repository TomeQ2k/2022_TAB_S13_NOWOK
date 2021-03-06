using System;
using Library.Shared.Exceptions;
using Library.Shared.Extensions;
using Library.Shared.Logging;
using Library.Shared.Models.Response;
using Library.Shared.Options;

namespace Library.Shared.Utils
{
    public static class BehaviourUtils<TRequest, TResponse> where TResponse : BaseResponse
    {
        public static TResponse HandleException(Exception e, string errorCode, ILogger logger)
        {
            var requestName = typeof(TRequest).Name;
            logger.Error($"{requestName}: '{e.Message}'", e);

            var invalidResponse = Activator.CreateInstance(typeof(TResponse),
                new Error(errorCode, e.Message));

            return invalidResponse as TResponse;
        }

        public static TResponse HandleValidationException(ValidationException e, string errorCode, ILogger logger)
        {
            var requestName = typeof(TRequest).Name;
            logger.Error(
                $"{requestName}: Validation failed: \n{e.Errors.ToJSON(JsonOptions.JsonSerializerOptions)}");

            var invalidResponse = Activator.CreateInstance(typeof(TResponse),
                new Error(errorCode, e.Message, e.Errors));

            return invalidResponse as TResponse;
        }
    }
}