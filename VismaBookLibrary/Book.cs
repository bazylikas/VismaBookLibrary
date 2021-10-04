using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VismaBookLibrary
{
	public class Book
	{
		public string title { get; set; }
		public string author { get; set; }
		public string category { get; set; }
		public string language { get; set; }
		public string publicationDate { get; set; }
		public string ISBN { get; set; }
		public bool taken { get; set; }
		public string takeDate { get; set; }
		public string returnDate { get; set; }
		public string reader { get; set; }
	}
}
