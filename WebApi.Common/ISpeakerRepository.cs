using System;
using System.Collections.Generic;
using System.Linq;

namespace WebApi.Data
{
    public interface ISpeakerRepository : IDisposable
    {
        IQueryable<Speaker> All { get; }
        Speaker Find(int id);
        void Store(Speaker speaker);
        void Delete(int id);
        void SaveChanges();
    }
}
