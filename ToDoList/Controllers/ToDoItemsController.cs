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
            var userIdCookie = Request.Cookies["UserId"];
            //so the user can keep his data for a day
            if (string.IsNullOrEmpty(userIdCookie))
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

                var options = new CookieOptions
                {
                    Expires = DateTime.Now.AddDays(1),
                    Secure = true,
                };
                Response.Cookies.Append("UserId", user.Id.ToString(), options);

                return RedirectToAction("Items");
        }

        public IActionResult Items()
        {
            //if the cookie still exists it show the list of the items he has
            var userIdCookie = Request.Cookies["UserId"];
            if (int.TryParse(userIdCookie, out int userId))
            {
                var userData = context.Users.Where(e => e.Id == userId).FirstOrDefault();
                ViewBag.userId = userData.Id;
                ViewBag.userName = userData.Name;
                var userToDo = context.ToDoItems.Include(e => e.User).Where(e => e.UserId == userId).ToList();
                return View(userToDo);
            }
            //in case there is no user
            return RedirectToAction("Create");
        }
        public IActionResult CreateNew(int id)
        {
            //SecurityCheck method just to make sure the user use his own data only with his id
            if (SecurityCheck(id))
            {
                return View(id);
            }
            return RedirectToAction("NotFound", "Errors");
        }
        [HttpPost]
        public IActionResult CreateNew(ToDoItem toDoItem, IFormFile File)
        {
            //no files bigger than 10 MB
            if (File.Length > 0 && File.Length < 10 * 1024 * 1024)
            {
                var fileName = Guid.NewGuid().ToString() + Path.GetExtension(File.FileName);
                var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\files", fileName);

                using (var stream = System.IO.File.Create(filePath))
                {
                    File.CopyTo(stream);
                }
                toDoItem.File = fileName;

                context.ToDoItems.Add(toDoItem);
                context.SaveChanges();
                TempData["CreateNew"] = "You have created " + toDoItem.Name + " successfully";
                return RedirectToAction("Items");
            }
            return RedirectToAction("FileError", "Errors");
        }

        public IActionResult Edit(int id)
        {
            var item = context.ToDoItems.Where(e => e.Id == id).FirstOrDefault();
            if (item != null)
            {
                var userId = item.UserId;
                if (SecurityCheck(userId))
                {
                    return View(item);
                }
            }
            return RedirectToAction("NotFound", "Errors");

        }

        [HttpPost]
        public IActionResult Edit(ToDoItem toDoItem)
        {
            var oldItem = context.ToDoItems.Where(e => e.Id == toDoItem.Id).AsNoTracking().FirstOrDefault();
            //to keep the same file 
            toDoItem.File = oldItem.File;
            context.ToDoItems.Update(toDoItem);
            context.SaveChanges();
            return RedirectToAction("Items");
        }

        public IActionResult Delete(int itemId)
        {
            var deletedItem = context.ToDoItems.Where(e => e.Id == itemId).AsNoTracking().FirstOrDefault();
            if (deletedItem != null)
            {
                var userId = deletedItem.UserId;
                if (SecurityCheck(userId))
                {
                    //delete the file if the user deleted the item
                    var deletedItemPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\files", deletedItem.File);
                    if (System.IO.File.Exists(deletedItemPath))
                    {
                        System.IO.File.Delete(deletedItemPath);
                    }
                    context.ToDoItems.Remove(new() { Id = itemId });
                    context.SaveChanges();
                    return RedirectToAction("Items");
                }
            }
            return RedirectToAction("NotFound", "Errors");
        }
        public IActionResult LogOut(int id)
        {
            if (SecurityCheck(id))
            {
                Response.Cookies.Delete("UserId");
                context.Users.Remove(new() { Id = id });
                context.SaveChanges();
                return RedirectToAction("Create");
            }
            return RedirectToAction("NotFound", "Errors");
        }
        public IActionResult DownloadFile(int id)
        {
            var file = context.ToDoItems.Where(e => e.Id == id).AsNoTracking().FirstOrDefault();
            if (file != null)
            {
                var userId = file.UserId;
                //making sure the user can only download his files only
                if (SecurityCheck(userId))
                {
                    var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\files", file.File);
                    var fileBytes = System.IO.File.ReadAllBytes(filePath);
                    var contentType = "application/octet-stream";
                    var fileName = file.File;

                    return File(fileBytes, contentType, fileName);
                }
            }
            return RedirectToAction("NotFound", "Errors");
        }

        public bool SecurityCheck(int id)
        {
            //method to make sure if the id sent in the link is the same in the cookie of the user
            var userIdCookie = Request.Cookies["UserId"];
            if (int.TryParse(userIdCookie, out int userId))
            {
                if (id == int.Parse(userIdCookie))
                {
                    return true;
                }
            }

            return false; 

        }

    }


}
