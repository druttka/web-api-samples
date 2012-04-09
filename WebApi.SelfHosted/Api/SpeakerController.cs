using System.Linq;
using System.Web.Http;
using System.Collections.Generic;
using WebApi.Common;
using System.Net.Http;
using System.Net;

namespace WebApi.SelfHosted.Api.Controllers
{
    public class SpeakerController : ApiController
    {
        private readonly ISpeakerRepository _speakerRepository;

        public SpeakerController(ISpeakerRepository speakerRepository)
        {
            _speakerRepository = speakerRepository;
        }

        public IQueryable<Speaker> Get()
        {
            return _speakerRepository.Speakers;
        }

        public HttpResponseMessage<Speaker> Get(int id)
        {
            var speaker = _speakerRepository.Speakers
                .FirstOrDefault(x => x.Id == id);

            if (speaker == null)
                throw new HttpResponseException(HttpStatusCode.NotFound);

            return new HttpResponseMessage<Speaker>(speaker);
        }

        public HttpResponseMessage<Speaker> Post(Speaker speaker)
        {
            _speakerRepository.Store(speaker);
            return new HttpResponseMessage<Speaker>(speaker, HttpStatusCode.Created);
        }

        public HttpResponseMessage Put(Speaker speaker)
        {
            var exists = _speakerRepository.Speakers.FirstOrDefault(x => x.Id == speaker.Id);
            if (exists == null)
                return new HttpResponseMessage(HttpStatusCode.NotFound);

            _speakerRepository.Store(speaker);
            return new HttpResponseMessage(HttpStatusCode.Accepted);
        }

    }
}