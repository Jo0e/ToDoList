using Microsoft.AspNetCore.Mvc;

namespace ToDoList.Controllers
{
    public class ErrorsController : Controller
    {
        public IActionResult NotFound()
        {
            return View();
        }

        public IActionResult FileError() 
        {
            return View();
        }
    }
}
