using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MedicalApp
{
    public class MedicalDoc
    {
		public MedicalDoc()
		{

		}
		public int Id { get; set; }
        public string Name { get; set; }
        public int idPacient { get; set; }
		public int idMedicalDocType { get; set; }
		public DateTime BeginTime { get; set; }
		public DateTime? EndTime { get; set; }
		public string Info { get; set; }

		public virtual Patient Patient { get; set; }
		public virtual MedicalDocType MedicalDocTypeId { get; set; }
	}
	
}
