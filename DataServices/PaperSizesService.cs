using Data;
using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataServices
{
    public class PaperSizesService : IPaperSizesService
    {
        private readonly IRepository<PaperSize> db;

        public PaperSizesService(IRepository<PaperSize> db)
        {
            this.db = db;
        }

        public IQueryable<PaperSize> GetPaperSizes()
        {
           return db.All();
        }


        public IQueryable<PaperSize> GetPaperSizeByName(string PaperSizeName)
        {
            return db.All().Where(t => t.Name == PaperSizeName);
        }
    }
}
