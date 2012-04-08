using System.Linq;
using System.Web.Http;
using System.Collections.Generic;
using WebApi.Common;

namespace WebApi.SelfHosted.Api.Controllers
{
    public class SpeakerController : ApiController
    {
        private ISpeakerRepository _speakerRepository = new FakeSpeakerRepository();

        // GET /api/<controller>
        public IQueryable<Speaker> Get()
        {
            return _speakerRepository.Speakers.AsQueryable();
        }
    }


}