using Microsoft.AspNetCore.Mvc;

namespace DTS.Web.Controllers;

[Area("User")]
public class DocumentStatusController : Controller
{
    // GET
    public IActionResult Outgoing()
    {
        return View();
    }
}

 