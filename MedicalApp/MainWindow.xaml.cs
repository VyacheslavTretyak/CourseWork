using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace MedicalApp
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		//For example
		public MainWindow()
		{
			InitializeComponent();
            //Database.SetInitializer(new DropCreateDatabaseAlways<DataModel>());

            InitFirstData();


            AddEditDocument addEditDocument = new AddEditDocument(1);
            addEditDocument.Show();

        }
		private void InitFirstData()
		{
			Pacient[] pacients = {
			new Pacient()
			{
				FirstName = "Pomber",
				LastName = "Asekrot",
				BirthDay = new DateTime(1991, 1, 1),
				Addres = "Krivoy Rog Sicheslavska str. 11/13",
				Gender = true
			},
			new Pacient()
			{
				FirstName = "Arkport",
				LastName = "Shurtrych",
				BirthDay = new DateTime(1988, 12, 17),
				Addres = "Krivoy Rog Myru str. 121/15",
				Gender = true
			},
			new Pacient()
			{
				FirstName = "Roska",
				LastName = "Viaerkova",
				BirthDay = new DateTime(2001, 9, 11),
				Addres = "Krivoy Rog Almasna str. 49/51",
				Gender = false
			},
			};
			MedicalDocType[] types =
			{
				new MedicalDocType()
				{
					Name = "Лікарняний"
				},
				new MedicalDocType()
				{
					Name = "Направлення на аналізи"
				},
				new MedicalDocType()
				{
					Name = "Результати аналізів"
				},
                new MedicalDocType()
                {
                    Name = "Направлення на процедури"
                }
            };
            MedicalDoc[] docs =
            {
                new MedicalDoc()
                {
                    Name = "Лікарняний Перелом",
                    idPacient =1,
                    idMedicalDocType = 1,
                    BeginTime = new DateTime(2002,04,21),
                    EndTime = new DateTime(2002,05,21),
                    Info = "Aliquam gravida mauris ut mi. Duis risus odio"
                },
                new MedicalDoc()
                {
                    Name = "Направлення на анализ крови",
                    idPacient =2,
                    idMedicalDocType = 2,
                    BeginTime = new DateTime(2004,04,22),
                    EndTime = new DateTime(2004,04,22),
                    Info = "ut, nulla. Cras eu tellus eu augue"
                },
                new MedicalDoc()
                {
                    Name = "Результати аналізів крови",
                    idPacient =3,
                    idMedicalDocType = 3,
                    BeginTime = new DateTime(2006,04,23),
                    EndTime = new DateTime(2006,05,23),
                    Info = "sit amet, consectetuer adipiscing elit. Aliquam auctor"
                }
            };

            using (DataModel db = new DataModel())
			{
				foreach (Pacient pacient in pacients)
				{
					if (db.Pacients.FirstOrDefault(p => p.FirstName == pacient.FirstName) == null)
					{
						db.Pacients.Add(pacient);
					}
				}
				foreach (MedicalDocType doc in types)
				{
					if (db.MedicalDocTypes.FirstOrDefault(p => p.Name == doc.Name) == null)
					{
						db.MedicalDocTypes.Add(doc);
					}
				}
                foreach (MedicalDoc docum in docs)
                {
                    if (db.MedicalDocs.FirstOrDefault(p => p.Info == docum.Info) == null)
                    {
                        db.MedicalDocs.Add(docum);
                    }
                }
                db.SaveChanges();
			}
		}

        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            AddChangePatient addChangePatient = new AddChangePatient();
            addChangePatient.Show();
        }
    }
}
