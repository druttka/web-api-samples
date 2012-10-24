﻿using System.Linq;
using System.Web.Http;
using WebApi.Data;
using System.Net.Http;
using System.Net;
using System;
using System.Threading.Tasks;
using System.Threading;
using WebApi.MvcHosted.Filters;

namespace WebApi.MvcHosted.Api
{
    [TokenAuth]
    [Queryable]
    //[AllYourErrorsAreTeapotsFilter]
    public class SpeakerController : ApiController
    {
        private bool _isDisposed = false;
        private readonly ISpeakerRepository _speakerRepository;

        public SpeakerController(ISpeakerRepository speakerRepository)
        {
            _speakerRepository = speakerRepository;
        }

        public IQueryable<Speaker> Get()
        {
            return _speakerRepository.All;
        }

        public HttpResponseMessage Get(int id)
        {
            var speaker = _speakerRepository.Find(id);

            if (speaker == null)
                throw new HttpResponseException(HttpStatusCode.NotFound);

            return Request.CreateResponse<Speaker>(HttpStatusCode.OK, speaker);
        }

        public HttpResponseMessage Post(Speaker speaker)
        {
            _speakerRepository.Store(speaker);
            return Request.CreateResponse<Speaker>(HttpStatusCode.Created, speaker);
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

        public override Task<HttpResponseMessage> ExecuteAsync(System.Web.Http.Controllers.HttpControllerContext controllerContext, CancellationToken cancellationToken)
        {
            return base.ExecuteAsync(controllerContext, cancellationToken)
                .ContinueWith(task =>
                {
                    if (task != null && task.Status != TaskStatus.Faulted)
                        _speakerRepository.SaveChanges();
                 
                    return task;
                }).Unwrap();
        }

        protected override void Dispose(bool disposing)
        {
            if (_isDisposed)
                return;

            if (disposing)
            {
                if (_speakerRepository != null)
                    _speakerRepository.Dispose();
            }

            base.Dispose(disposing);
            _isDisposed = true;
        }

        ~SpeakerController()
        {
            this.Dispose(false);
        }
    }
}