using System;
namespace ebikeshopserver.Models.GlobalModels
{
	public class Image
	{
		public string Url { get; set; }
        public string Alt { get; set; }
		public Image(string url, string alt)
		{
			Url = url;
			Alt = alt;
		}
	}
}

