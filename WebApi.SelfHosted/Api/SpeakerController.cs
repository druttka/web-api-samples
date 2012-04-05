using System.Linq;
using System.Web.Http;

namespace WebApi.SelfHosted.Api.Controllers
{
    public class SpeakerController : ApiController
    {
        // GET /api/<controller>
        public IQueryable<Speaker> Get()
        {
            return Speakers.AsQueryable();
        }

        private Speaker[] Speakers
        {
            get
            {
                return new[]
                    {
                        new Speaker { Name = "Dale Cooper", Fame = 100 },
                        new Speaker { Name = "Scott Hanselman", Fame = 8999},
                        new Speaker {Name = "Goku", Fame = 9001},
                        new Speaker { Name = "David Ruttka", Fame = 2 }
                    };
            }
        }
    }

    public class Speaker
    {
        public string Name { get; set; }
        public int Fame { get; set; }
    }
}