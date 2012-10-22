using System;
using System.Linq;

namespace WebApi.Data
{
    public class EFSpeakerRepository : ISpeakerRepository
    {
        private readonly EFDemoContext _context = new EFDemoContext();
        private bool disposed;

        #region ISpeakerRepository Members

        public IQueryable<Speaker> All
        {
            get { return _context.Speakers.AsQueryable(); }
        }

        public Speaker Find(int id)
        {
            return _context.Speakers.Find(id);
        }

        public void Store(Speaker speaker)
        {
            var exists = _context.Speakers.Find(speaker.Id);
            if (exists == null)
            {
                _context.Speakers.Add(speaker);
            }
            else
            {
                exists.Name = speaker.Name;
                exists.Fame = speaker.Fame;
            }
        }

        public void Delete(int id)
        {
            var exists = _context.Speakers.Find(id);
            if (exists != null) _context.Speakers.Remove(exists);
        }

        public void SaveChanges()
        {
            _context.SaveChanges();
        }

        #endregion

        #region IDisposable Members

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
                    if (_context != null) _context.Dispose();
                    disposed = true;
                }
            }
        }

        #endregion
    }
}
