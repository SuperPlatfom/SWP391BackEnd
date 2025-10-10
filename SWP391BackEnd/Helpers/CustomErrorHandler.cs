using Microsoft.AspNetCore.Mvc;

namespace SWP391BackEnd.Helpers;
public class CustomErrorHandler
{
    public static IActionResult ValidationError(Dictionary<string, string> errors)
    {
        var response = new Dictionary<string, object>
        {
            { "http_status", 400 },
            { "time_stamp", DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss") },
            { "message", "Validation Error" },
            { "errors", errors }
        };
        return new BadRequestObjectResult(response);
    }

    public static IActionResult SimpleError(string message, int status = 400)
    {
        var response = new Dictionary<string, object>
        {
            { "http_status", status },
            { "time_stamp", DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss") },
            { "message", message }
        };
        return new ObjectResult(response) { StatusCode = status };
    }
}

