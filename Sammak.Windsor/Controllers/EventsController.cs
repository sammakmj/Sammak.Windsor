using Sammak.Windsor.Repositories;
using System.Web.Mvc;

namespace Sammak.Windsor.Controllers
{
    public class EventsController : Controller
    {
        private readonly IEventRepository _eventRepository;

        public EventsController(IEventRepository eventRepository)
        {
            _eventRepository = eventRepository;
        }

        public ActionResult Index(int? page)
        {
            var eventPage = _eventRepository.GetPage(page.GetValueOrDefault(1));
            return View(eventPage);
        }
    }
}
