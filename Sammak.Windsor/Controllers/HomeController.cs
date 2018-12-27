using Castle.Core.Logging;
using NHibernate;
using Sammak.Windsor.Domain;
using System.Web.Mvc;

namespace Sammak.Windsor.Controllers
{
    public class HomeController : Controller
    {
        // this is Castle.Core.Logging.ILogger, not log4net.Core.ILogger
        public ILogger Logger { get; set; }

        private readonly ISession _session;

        public HomeController(ISession session)
        {
            _session = session;
        }


        public ActionResult Index()
        {
            Logger.Info($"In the Home Controller's Index Action.");

            ViewBag.Message = "Welcome to Windsor Tutorial WebSite!";
            ViewBag.EventCount = _session.QueryOver<Event>().RowCount();

            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            //Logger.WarnFormat($"User {"Warning"} attempted login but password validation failed");
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}