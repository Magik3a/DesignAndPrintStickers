namespace Data.Migrations
{
    using Microsoft.AspNet.Identity;
    using Microsoft.AspNet.Identity.EntityFramework;
    using Models;
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;

    internal sealed class Configuration : DbMigrationsConfiguration<Data.AppDbContext>
    {
        public Configuration()
        {
            
            AutomaticMigrationsEnabled = false;
        }

        protected override void Seed(Data.AppDbContext context)
        {
            //  This method will be called after migrating to the latest version.

            //  You can use the DbSet<T>.AddOrUpdate() helper extension method 
            //  to avoid creating duplicate seed data. E.g.
            //
            //    context.People.AddOrUpdate(
            //      p => p.FullName,
            //      new Person { FullName = "Andrew Peters" },
            //      new Person { FullName = "Brice Lambson" },
            //      new Person { FullName = "Rowan Miller" }
            //    );
            //
            if (!(context.Users.Any(u => u.UserName == "svetlin.krastanov90@gmail.com")))
            {
                var userStore = new UserStore<AppUser>(context);
                var userManager = new UserManager<AppUser>(userStore);
                var userToInsert = new AppUser { UserName = "svetlin.krastanov90@gmail.com", PhoneNumber = "0888017004", Email = "svetlin.krastanov90@gmail.com" };
                userManager.Create(userToInsert, "svetlin90");
            }

            #region Seed Paper Sizes

            context.PaperSizes.AddOrUpdate(
                new PaperSize
                {
                    Name = "A4",
                    WIdth = "210",
                    Height = "297"
                },
                new PaperSize
                {
                    Name = "US Letter",
                    WIdth = "216",
                    Height = "279"
                }
            );
            context.SaveChanges();
            #endregion

            #region Seed Templates

            context.Templates.AddOrUpdate(
                new Template
                {
                    Name = "Credit card size (3.370x2.125'') ",
                    CssClass = "template-rectangle",
                    Css = "width:47.5%;",
                    BoxCount = 8,
                    BoxesPerRow = 2,
                    BorderRadiusPercent = 10,
                    BoxWidth = "86",
                    BoxHeight = "52",
                    MarginTop = "13",
                    MarginBottom = "0",
                    MarginLeft = "12",
                    MarginRIght = "12",
                    Order = 1
                },
                new Template
                {
                    Name = "Circle D 3\"",
                    CssClass = "template-roundx2",
                    Css = "width:43.3%;",
                    BoxCount = 6,
                    BoxesPerRow = 2,
                    BorderRadiusPercent = 100,
                    BoxWidth = "76",
                    BoxHeight = "76",
                    MarginTop = "5",
                    MarginBottom = "0",
                    MarginLeft = "12.3",
                    MarginRIght = "12.3",
                    Order = 2
                },
                new Template
                {
                    Name = "Circle D 2.25\"",
                    CssClass = "template-roundx3",
                    Css = "width:32%;",
                    BoxCount = 12,
                    BoxesPerRow = 3,
                    BorderRadiusPercent = 100,
                    BoxWidth = "56",
                    BoxHeight = "56",
                    MarginTop = "10.7",
                    MarginBottom = "0",
                    MarginLeft = "12.3",
                    MarginRIght = "12.3",
                    Order = 3
                },
                new Template
                {
                    Name = "Circle D 1\"",
                    CssClass = "template-roundx5",
                    Css = "width:17.6%;",
                    BoxCount = 35,
                    BoxesPerRow = 5,
                    BorderRadiusPercent = 100,
                    BoxWidth = "25.4",
                    BoxHeight = "25.4",
                    MarginTop = "21.1",
                    MarginBottom = "0",
                    MarginLeft = "24.5",
                    MarginRIght = "24.5",
                    Order = 4
                }                  
            );
            context.SaveChanges();
            #endregion



        }
    }
}
