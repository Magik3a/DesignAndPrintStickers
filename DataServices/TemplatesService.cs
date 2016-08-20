using Data;
using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataServices
{
    public class TemplatesService : ITemplatesService
    {
        private readonly IRepository<Template> db;

        public TemplatesService(IRepository<Template> db)
        {
            this.db = db;
        }



        public IQueryable<Template> GetTemplates()
        {
            return db.All();
        }

    }
}
