using NHibernate;
using Sammak.Windsor.Domain;
using Sammak.Windsor.Plumbing;

namespace Sammak.Windsor.Repositories
{
    public class EventRepository : IEventRepository
	{
		private readonly int _pageSize;

		private readonly ISession _session;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pageSize"></param>
        /// <param name="session"></param>
        /// <remarks>
        /// NOTE: the injecting of the pageSize primitive parameter is configured in the RepositoriesInstaller's line of code:
        ///          .Configure(c => c.DependsOn(new { pageSize = 20 })));
        /// IMO, this parameter should be configurable using web.config AppSetting.  Or, via some get method with page size parameter done programatically.
        /// However, this is a good example of how a primitive parameter could be injected.
        /// </remarks>
		public EventRepository(int pageSize, ISession session)
		{
			_pageSize = pageSize;
			_session = session;
		}

		public Page<Event> GetPage(int pageNumber)
		{
			var firstResult = _pageSize * (pageNumber - 1);
			using (var tx = _session.BeginTransaction())
			{
				var totalCount = _session.QueryOver<Event>().ToRowCountQuery().FutureValue<int>();
				var events = _session.QueryOver<Event>().Take(_pageSize).Skip(firstResult).Future();
				var page = new Page<Event>(events, pageNumber, totalCount.Value, _pageSize);
				tx.Commit();
				return page;
			}
		}
	}
}