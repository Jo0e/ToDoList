using Microsoft.AspNetCore.Mvc;
using ToDoList.Data;
using ToDoList.Models;

namespace ToDoList.Controllers
{
    public class ToDoItemsController : Controller
    {
        ApplicationDbContext context = new ApplicationDbContext();

        public IActionResult Create() 
        {
            return View();
        }
        [HttpPost]
        public IActionResult Create(string Name)
        {
           
            return View();
        }
        public IActionResult Items() 
        {
            return View();
        }
        public IActionResult CreateNew() 
        {
            return View();
        }
        [HttpPost]
        public IActionResult CreateNew(ToDoItem toDoItem)
        {
            return View();
        }

        public IActionResult Delete() 
        {
            return View();
        }
        public IActionResult Edit() 
        {
            return View();
        }

        [HttpPost]
        public IActionResult Edit(ToDoItem toDoItem) 
        {
            return RedirectToAction("Index");
        }

    }

    
}
