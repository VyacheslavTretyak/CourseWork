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
			Pacient pacient = new Pacient()
			{
				FirstName = "Pomber",
				LastName = "Asekrot",
				BirthDay = new DateTime(1991, 1, 1)

			};
			using (DataModel db = new DataModel())
			{
				if (db.Pacients.FirstOrDefault(p => p.FirstName == pacient.FirstName) != null)
				{
					db.Pacients.Add(pacient);
				}
				
				//db.SaveChanges();
				//foreach (var pat in db.Pacients)
				//{
				//	MessageBox.Show(pat.FirstName);
				//}
			}
		}
	}
}
