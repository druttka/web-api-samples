using System.Web.Mvc;
using WebApi.Common;

namespace WebApi.MvcHosted.Controllers
{
    using System.Linq;

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
