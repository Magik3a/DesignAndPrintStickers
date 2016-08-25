using Data.Migrations;
using Microsoft.AspNet.Identity.EntityFramework;
using Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data
{
    public class AppDbContext : IdentityDbContext<AppUser>, IDbContext
    {
        public AppDbContext()
            : base("DefaultConnection", throwIfV1Schema: false)
        {
            Database.SetInitializer<AppDbContext>(new CreateDatabaseIfNotExists<AppDbContext>());
        }

        public virtual IDbSet<PaperSize> PaperSizes { get; set; }

        public virtual IDbSet<Template> Templates { get; set; }

        public static AppDbContext Create()
        {
            return new AppDbContext();
        }
    }
}
