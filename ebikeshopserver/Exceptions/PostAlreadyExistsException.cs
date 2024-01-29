using System;
namespace ebikeshopserver.Exceptions
{
	public class PostAlreadyExistsException : Exception
	{
		public PostAlreadyExistsException(string message) : base(message) { }
	}
}

