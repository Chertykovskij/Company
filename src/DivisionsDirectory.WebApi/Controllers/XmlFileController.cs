using Microsoft.AspNetCore.Mvc;

namespace Company.WebApi.Controllers
{
    [ApiController]
    public class XmlFileController : Controller
    {
        [HttpGet]
        public async Task<IActionResult> ExportToXml()
        {
            // Выгрузка в Xml
        }

        [HttpPost]
        public async Task<IActionResult> ImportToXml(IFormFile file)
        {
            // Импорт из Xml
        }
    }
}
