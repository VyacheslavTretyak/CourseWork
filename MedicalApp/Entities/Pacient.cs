using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MedicalApp
{
	public class Patient
    {
		public Patient()
		{

		}
		public int Id { get; set; }
		public string LastName { get; set; }
		public string FirstName { get; set; }
		public string MiddleName { get; set; }
		public DateTime BirthDay { get; set; }
		public string Addres { get; set; }
		/// <summary>
		/// 0 - female
		/// 1 - male
		/// </summary>
		public bool Gender { get; set; }
		public bool IsArchived { get; set; }

		public virtual List<MedicalDoc> Docs { get; set; }
	}
}
