using System.Linq;
using System.Web.Http;
using WebApi.Common;
using System.Net.Http;
using System.Net;
using System;

namespace WebApi.SelfHosted.Api.Controllers
{
    public class SpeakerController : ApiController
    {
        private readonly ISpeakerRepository _speakerRepository = new FakeSpeakerRepository();

        public IQueryable<Speaker> Get()
        {
            return _speakerRepository.All;
        }

        public HttpResponseMessage<Speaker> Get(int id)
        {
            var speaker = _speakerRepository.Find(id);

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
            if (speaker.Id == 0) throw new HttpResponseException(HttpStatusCode.BadRequest);

            var exists = _speakerRepository.Find(speaker.Id);
            if (exists == null)
                throw new HttpResponseException(HttpStatusCode.NotFound);

            _speakerRepository.Store(speaker);
            return new HttpResponseMessage(HttpStatusCode.NoContent);
        }

        public HttpResponseMessage Delete(int id)
        {
            _speakerRepository.Delete(id);
            return new HttpResponseMessage(HttpStatusCode.NoContent);
        }
    }
}