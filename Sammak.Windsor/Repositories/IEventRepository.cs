using Sammak.Windsor.Domain;
using Sammak.Windsor.Plumbing;

namespace Sammak.Windsor.Repositories
{
    public interface IEventRepository
    {
        Page<Event> GetPage(int pageNumber);
    }
}
