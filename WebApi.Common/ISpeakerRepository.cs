using System;
using System.Collections.Generic;
using System.Linq;

namespace WebApi.Common
{
    public interface ISpeakerRepository
    {
        IQueryable<Speaker> Speakers { get; }
        void Store(Speaker speaker);
    }
}
