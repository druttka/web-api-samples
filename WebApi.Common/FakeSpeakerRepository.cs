using System;
using System.Collections.Generic;
using System.Linq;

namespace WebApi.Common
{
    public class FakeSpeakerRepository : ISpeakerRepository
    {
        private Dictionary<int, Speaker> _speakers = new Dictionary<int, Speaker>();

        public FakeSpeakerRepository()
        {
            AddDummy(1, "David Ruttka", 2);
            AddDummy(2, "Scott Hanselman", 8999);
            AddDummy(3, "Dale Cooper", 100);
            AddDummy(4, "Goku", 9001);
        }

        public IQueryable<Speaker> Speakers
        {
            get
            {
                return _speakers.Values.AsQueryable<Speaker>();
            }
        }

        private void AddDummy(int id, string name, int fame)
        {
            var speaker = new Speaker { Id = id, Name = name, Fame = fame };
            _speakers[id] = speaker;
        }
    }
}