using System;
namespace ebikeshopserver.Exceptions
{
	public class NoPostsFoundException : Exception
	{
		public NoPostsFoundException(string message) : base(message)
		{
		}
	}
}

