using Models;
using System.Linq;

namespace DataServices
{
    public interface IPaperSizesService
    {
        IQueryable<PaperSize> GetPaperSizes();

        IQueryable<PaperSize> GetPaperSizeByName(string PaperSizeName);
    }
}