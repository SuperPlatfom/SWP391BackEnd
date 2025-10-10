using System.Net;
using Microsoft.AspNetCore.Mvc;

namespace SWP391BackEnd.Helpers;

public static class CustomSuccessHandler
{
    public static IActionResult ResponseBuilder(HttpStatusCode statusCode, string message, object responseObject)
    {
        var response = new Dictionary<string, object>
        {
            { "httpStatus", statusCode },
            { "timeStamp", DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss") },
            { "message", message },
            { "data", responseObject }
        };

        return new ObjectResult(response) { StatusCode  = (int)statusCode };
    }
}