﻿using System;
namespace ebikeshopserver.Exceptions
{
	public class AddressNotFoundException : Exception
	{
		public AddressNotFoundException(string? message) :base(message) { }
	}
}

