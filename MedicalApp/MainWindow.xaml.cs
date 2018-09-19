using System;
using System.Collections.Generic;
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
		public MainWindow()
		{
			InitializeComponent();
			InitFirstData();
			//kuku
            //test
            //testkate

            //slav test v2 gihub

            // Test Makc v2 | origin/Alikberov  --- test v2
            
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
