using System;
using System.Collections.Generic;
using System.Linq;
using Raven.Client;

namespace WebApi.Common
{
    public class RavenSpeakerRepository : ISpeakerRepository, IDisposable
    {
        private readonly IDocumentSession _session;
        private bool disposed;
        
        public RavenSpeakerRepository(IDocumentSession session)
        {
            _session = session;
        }

        public IQueryable<Speaker> All
        {
            get { return _session.Query<Speaker>(); }
        }

        public Speaker Find(int id)
        {
            return _session.Load<Speaker>(id);
        }

        public void Store(Speaker speaker)
        {
            _session.Store(speaker);
        }

        public void Delete(int id)
        {
            var speaker = _session.Load<Speaker>(id);
            if (speaker != null) _session.Delete<Speaker>(speaker);
        }

        public void SaveChanges()
        {
            _session.SaveChanges();
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            if (!disposed)
            {
                if (disposing)
                {
                    if (_session != null) _session.Dispose();
                    disposed = true;
                }
            }
        }
    }
}
