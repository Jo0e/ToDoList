using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using ToDoList.Data;
using ToDoList.Models;

namespace ToDoList.Controllers
{
    public class ToDoItemsController : Controller
    {
        ApplicationDbContext context = new ApplicationDbContext();

        public IActionResult Create()
        {
            if (TempData.IsNullOrEmpty())
            {
                return View();
            }
            return RedirectToAction("Items");
        }
        [HttpPost]
        public IActionResult Create(string Name)
        {
            var user = new User { Name = Name };
            context.Users.Add(user);
            context.SaveChanges();
            TempData["UserId"] = user.Id;
            return RedirectToAction("Items");
        }
        public IActionResult Items()
        {
            var userId = TempData["UserId"] as int?;
            TempData.Keep("UserId");
            if (userId.HasValue)
            {
                var userData = context.Users.Where(e => e.Id == userId).FirstOrDefault();
                ViewBag.userData = userData;

                var userToDo = context.ToDoItems.Include(e => e.User).Where(e => e.UserId == userId.Value).ToList();
                return View(userToDo);
            }
            return View(new List<ToDoItem>());
        }
        public IActionResult CreateNew(int id)
        {
            return View(id);
        }
        [HttpPost]
        public IActionResult CreateNew(ToDoItem toDoItem)
        {
            context.ToDoItems.Add(toDoItem);
            context.SaveChanges();
            return RedirectToAction("Items");
        }

        public IActionResult Edit(int id)
        {
            var item = context.ToDoItems.Where(e => e.Id == id).FirstOrDefault();
            return View(item);
        }

        [HttpPost]
        public IActionResult Edit(ToDoItem toDoItem)
        {
            context.ToDoItems.Update(toDoItem);
            context.SaveChanges();
            return RedirectToAction("Items");
        }
        public IActionResult Delete(int itemId)
        {
            context.ToDoItems.Remove(new() { Id = itemId });
            context.SaveChanges();
            return RedirectToAction("Items");
        }

    }


}
