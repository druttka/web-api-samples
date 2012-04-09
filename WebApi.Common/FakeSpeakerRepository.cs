using System;
using System.Collections.Generic;
using System.Linq;

namespace WebApi.Common
{
    public class FakeSpeakerRepository : ISpeakerRepository
    {
        private static Dictionary<int, Speaker> _speakers;

        public FakeSpeakerRepository()
        {
            if (_speakers == null)
            {
                _speakers = new Dictionary<int, Speaker>();
                AddDummy(1, "David Ruttka", 2);
                AddDummy(2, "Scott Hanselman", 8999);
                AddDummy(3, "Dale Cooper", 100);
                AddDummy(4, "Goku", 9001);
            }
        }

        public IQueryable<Speaker> All
        {
            get
            {
                return _speakers.Values.AsQueryable<Speaker>();
            }
        }

        public Speaker Find(int id)
        {
            return _speakers[id];
        }

        public void Store(Speaker speaker)
        {
            if (speaker.Id == 0)
                speaker.Id = _speakers.Count() == 0 ? 1 : _speakers.Keys.Max() + 1;

            _speakers[speaker.Id] = speaker;
        }

        public void Delete(int id)
        {
            // If it's not there, no big deal
            _speakers.Remove(id);
        }

        public void SaveChanges()
        {
            // Not much to do in this one
        }

        public void Dispose()
        {
            // Not much to do in this one
        }

        private void AddDummy(int id, string name, int fame)
        {
            var speaker = new Speaker { Id = id, Name = name, Fame = fame };
            _speakers[id] = speaker;
        }
    }
}