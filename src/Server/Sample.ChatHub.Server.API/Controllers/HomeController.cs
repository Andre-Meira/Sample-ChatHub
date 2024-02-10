using Microsoft.AspNetCore.Mvc;

namespace Sample.ChatHub.API.Controllers;

public class HomeController : Controller
{
    public IActionResult Index()
    {
        return Redirect("~/swagger");
    }
}
