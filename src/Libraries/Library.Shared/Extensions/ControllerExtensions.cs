using Library.Shared.Dictionaries;
using Library.Shared.Models.Response;
using Microsoft.AspNetCore.Mvc;

namespace Library.Shared.Extensions
{
    public static class ControllerExtensions
    {
        public static IActionResult CreateResponse(this ControllerBase controller, BaseResponse response)
            => response.IsSucceeded
                ? controller.Ok(response)
                : controller.StatusCode((int)ErrorStatusCodeDictionary.GetStatusCode(response.Error.ErrorCode), response);
    }
}