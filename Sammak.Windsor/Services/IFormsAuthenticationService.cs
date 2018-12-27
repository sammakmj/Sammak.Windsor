namespace Sammak.Windsor.Services
{
	public interface IFormsAuthenticationService
	{
		void SignIn(string username);

		void SignOut();
	}
}