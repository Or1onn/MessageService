using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Task.Models;

namespace Task.Controllers;

public class ClientsController : Controller
{
    private readonly ILogger<ClientsController> _logger;

    public ClientsController(ILogger<ClientsController> logger)
    {
        _logger = logger;
    }

    public IActionResult FirstClient()
    {
        return View();
    }

    public IActionResult SecondClient()
    {
        return View();
    }
    
    public IActionResult ThirdClient()
    {
        return View();
    }
}
