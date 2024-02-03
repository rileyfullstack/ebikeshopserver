using System;
namespace ebikeshopserver.Exceptions
{
	public class OrderNotFoundException : Exception
	{
		public OrderNotFoundException(string message) : base(message)
		{
		}
	}
}

