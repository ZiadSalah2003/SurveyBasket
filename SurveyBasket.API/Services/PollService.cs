
using SurveyBasket.API.Models;

namespace SurveyBasket.API.Services
{
	public class PollService : IPollService
	{
		private static readonly List<Poll> _poll = [];
		public IEnumerable<Poll> GetAll()
		{
			return _poll;
		}
		public Poll? Get(int id)
		{
			var poll = _poll.SingleOrDefault(x => x.Id == id);
			return poll;
		}

		public Poll Add(Poll poll)
		{
			_poll.Add(poll);
			return poll;
		}

		public bool Update(int id, Poll poll)
		{
			var updated = _poll.SingleOrDefault(x => x.Id == id);
			if (updated is null)
				return false;
			updated.Title = poll.Title;
			updated.Description = poll.Description;
			return true;
		}

		public bool Delete(int id)
		{
			var deleted = _poll.SingleOrDefault(x => x.Id == id);
			if (deleted is null)
				return false;
			_poll.Remove(deleted);
			return true;
		}
	}
}
