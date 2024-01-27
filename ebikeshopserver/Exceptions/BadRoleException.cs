using System;
namespace ebikeshopserver.Exceptions
{
	public class BadRoleException : Exception
	{
		public BadRoleException(string message) : base(message) { }
	}
}

