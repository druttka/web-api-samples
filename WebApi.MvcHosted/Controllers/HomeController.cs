using System.Web.Mvc;
using System.Linq;
using WebApi.Data;

namespace WebApi.MvcHosted.Controllers
{
    public class HomeController : Controller
    {
        private readonly ISpeakerRepository _speakerRepository;

        public HomeController(ISpeakerRepository speakerRepository)
        {
            _speakerRepository = speakerRepository;
        }

        //
        // GET: /Home/

        public ActionResult Index()
        {
            var speakers = _speakerRepository.All.ToArray();
            return View(speakers);
        }
    }
}
