using System.Web.Security;

namespace Sammak.Windsor.Services
{
	public class FormsAuthenticationService : IFormsAuthenticationService
	{
		public void SignIn(string username)
		{
			FormsAuthentication.SetAuthCookie(username, false);
		}

		public void SignOut()
		{
			FormsAuthentication.SignOut();
		}
	}
}