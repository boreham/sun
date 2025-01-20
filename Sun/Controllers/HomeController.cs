using Microsoft.AspNetCore.Mvc;

namespace Sun.Controllers;

public class HomeController : Controller
{

    public IActionResult Index()
    {
        return View();
    }
}