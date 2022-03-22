using Microsoft.AspNetCore.Mvc;

namespace Backend;

public class ErrorHandler : ControllerBase
{
    [Route("/error")]
    public IActionResult HandleError() =>
        Problem();
}