using System;
namespace ebikeshopserver.Exceptions
{
	public class UserAlreadyExistsException : Exception
	{
		public UserAlreadyExistsException(string? message) : base(message) { }
    }
}

