using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;
using ToDoList.Models;

namespace ToDoList.Data
{
    public class ApplicationDbContext : DbContext
    {
        public DbSet<User> Users {  get; set; }
        public DbSet<ToDoItem> ToDoItems { get; set; }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
            var builder = new ConfigurationBuilder().AddJsonFile("appsettings.json", true, true).Build();
            var connection = builder.GetConnectionString("DefualtConnection");
            optionsBuilder.UseSqlServer(connection);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<User>().HasData(new User { Id=1,Name="Youssef"});
            modelBuilder.Entity<ToDoItem>().HasData(new ToDoItem
            {
                Id = 1,
                Name = "Task",
                UserId = 1,
                Description =
                "Finish the task of the asp.net",
                Deadline = new DateTime(2024, 10, 8, 12, 00, 00),
                File = "1.jpg"
            });
            }
    }
}
