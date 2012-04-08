using System;
using System.Collections.Generic;
using System.Linq;

namespace WebApi.Common
{
    public class FakeSpeakerRepository : ISpeakerRepository
    {
        private Dictionary<string, Speaker> _speakers = new Dictionary<string, Speaker>();

        public FakeSpeakerRepository()
        {
            AddDummy("David Ruttka", 2);
            AddDummy("Scott Hanselman", 8999);
            AddDummy("Dale Cooper", 100);
            AddDummy("Goku", 9001);
        }

        public IQueryable<Speaker> Speakers
        {
            get
            {
                return _speakers.Values.AsQueryable<Speaker>();
            }
        }

        private void AddDummy(string name, int fame)
        {
            var speaker = new Speaker { Name = name, Fame = fame };
            _speakers[name] = speaker;
        }
    }
}
