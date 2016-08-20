using Models;
using System.Linq;

namespace DataServices
{
    public interface ITemplatesService
    {
        IQueryable<Template> GetTemplates();

        IQueryable<Template> GetTemplateByName(string TemplateName);
    }
}