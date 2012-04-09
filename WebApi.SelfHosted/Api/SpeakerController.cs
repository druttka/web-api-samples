using System.Linq;
using System.Web.Http;
using System.Collections.Generic;
using WebApi.Common;
using System.Net.Http;
using System.Net;
using System;
using System.Threading.Tasks;

namespace WebApi.SelfHosted.Api.Controllers
{
    public class SpeakerController : ApiController
    {
        private readonly ISpeakerRepository _speakerRepository;

        public override Task<HttpResponseMessage> ExecuteAsync(System.Web.Http.Controllers.HttpControllerContext controllerContext, System.Threading.CancellationToken cancellationToken)
        {
            return base.ExecuteAsync(controllerContext, cancellationToken)
                .ContinueWith(task =>
                {
                    using (_speakerRepository)
                    {
                        if (task != null && task.Status != TaskStatus.Faulted)
                            _speakerRepository.SaveChanges();
                    }

                    return task;
                }).Unwrap();
        }

        public SpeakerController(ISpeakerRepository speakerRepository)
        {
            if (speakerRepository == null)
                throw new ArgumentNullException("speakerRepository");

            _speakerRepository = speakerRepository;
        }

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