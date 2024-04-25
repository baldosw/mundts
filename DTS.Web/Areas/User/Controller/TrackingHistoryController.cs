using Microsoft.AspNetCore.Mvc;

namespace DTS.Web.Controllers;
[Area("User")]
public class TrackingHistoryController : Controller
{
    // GET
    public IActionResult Index()
    {
        return View();
    }
}