using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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

// для того кто делает бэкэнд AddChangePatient окна:
// после добавления или изменения пациента перед закрытием окна возвращайте DialogResult = true

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
			try
			{
				InitFirstData();
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message);
			}
			//Test();
			// window center to screen 
			WindowStartupLocation = WindowStartupLocation.CenterScreen;

			// fill data grid
			fillDataFromDBtoDatagrid();

			// disable edit and remove buttons
			buttonsEditRemoveStateChange();
		}

		// fill data grid from db
		void fillDataFromDBtoDatagrid()
		{
			try
			{
				using (DataModel db = new DataModel())
				{
					// select only not archived patients
					datagridPatiens.ItemsSource = db.Pacients.Where(p => p.IsArchived == false).ToList();
				}
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message);
			}
		}

		// PreviewTextInput event to make numeric textbox
		private void textbox_OnlyNumeric(object sender, TextCompositionEventArgs e)
		{
			e.Handled = Regex.IsMatch(e.Text, "[^0-9]+");
		}

		// PreviewKeyDown event to restrict space key because of PreviewTextInput doesn't catch space
		private void textbox_restrictSpace(object sender, KeyEventArgs e)
		{
			if (e.Key == Key.Space)
				e.Handled = true;
		}

		// search button click 
		private void buttonSearch_Click(object sender, RoutedEventArgs e)
		{
			using (DataModel db = new DataModel())
			{
				// first select all not archived patiens
				IQueryable<Patient> queryable = db.Pacients.Where(p => p.IsArchived == false);
				// if number card field not empty
				if (!string.IsNullOrWhiteSpace(textboxNumberCard.Text))
				{
					// selection of patients with the entered card number
					long cardNum = Convert.ToInt64 (textboxNumberCard.Text);
					queryable = queryable.Where(p => p.Id == cardNum);
				}
				// if last name field not empty
				if (!string.IsNullOrWhiteSpace(textboxLastName.Text))
				{
					// selection of patients whose names contain the entered text 
					queryable = queryable.Where(p => p.LastName.Contains(textboxLastName.Text));
				}
				// if year of birth field not empty
				if (!string.IsNullOrWhiteSpace(textboxDateOfBirth.Text))
				{
					// selection of patients with the entered year of birth
					int year = Convert.ToInt32(textboxDateOfBirth.Text);
					queryable = queryable.Where(p => p.BirthDay.Year == year);
				}
				// if address field not empty
				if (!string.IsNullOrWhiteSpace(textboxAddress.Text))
				{
					// selection of patients whose address contain the entered text 
					queryable = queryable.Where(p => p.Addres.Contains(textboxAddress.Text));
				}
				// put query result to data grid table
				datagridPatiens.ItemsSource = queryable.ToList();
			}
		}

		// add patient button click 
		private void buttonAdd_Click(object sender, RoutedEventArgs e)
		{
			// open add patient window
			AddChangePatient addEditWindow = new AddChangePatient();
			addEditWindow.Title = "Add patient";

			// update data grid if patient was added 
			if (addEditWindow.ShowDialog() == true)
			{
				fillDataFromDBtoDatagrid();
				// focus on the added patient 
				datagridPatiens.SelectedIndex = datagridPatiens.Items.Count - 1;
				// scroll patient list to the added patient
				datagridPatiens.ScrollIntoView(datagridPatiens.SelectedItem);
				// popup notification
				showNotification("The patient was added");
			}
		}

		// edit patient button click
		private void buttonEdit_Click(object sender, RoutedEventArgs e)
		{
			// check patient was chosen in list
			if (datagridPatiens.SelectedItems.Count <= 0)
				return;
			
			// open add patient window with filled fields
			AddChangePatient addEditWindow = new AddChangePatient(datagridPatiens.SelectedItem as Patient);
			addEditWindow.Title = "Edit patient";

			// update data grid if patient was changed 
			if (addEditWindow.ShowDialog() == true)
			{
				// save position to restore
				int selectedIndex = datagridPatiens.SelectedIndex;

				fillDataFromDBtoDatagrid();
				// focus on the changed patient from saved position
				datagridPatiens.SelectedIndex = selectedIndex;
				// scroll patient list to the changed patient
				datagridPatiens.ScrollIntoView(datagridPatiens.SelectedItem);
				// popup notification
				showNotification("The patient was edited");
			}
		}

		// remove patient button click
		private void buttonRemove_Click(object sender, RoutedEventArgs e)
		{
			// check patient was chosen in list
			if (datagridPatiens.SelectedItems.Count <= 0)
				return;
			
			// get selected patient
			Patient pacient = datagridPatiens.SelectedItem as Patient;
			// show confirmation message box
			if (MessageBox.Show($"Are you sure you want to archive {pacient.FirstName + " " + pacient.LastName} ?",
				"Confirmation", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
			{
				using (DataModel db = new DataModel())
				{
					// save position to restore
					int selectedIndex = datagridPatiens.SelectedIndex;

					// find patient in db
					pacient = db.Pacients.FirstOrDefault(p => p.Id == pacient.Id);
					// remove
					pacient.IsArchived = true;
					// save
					db.SaveChanges();
					// update data grid list
					fillDataFromDBtoDatagrid();
					// scroll patient list to the previous position
					datagridPatiens.ScrollIntoView(datagridPatiens.Items[Math.Min(selectedIndex,datagridPatiens.Items.Count-1)]);
					// popup notification
					showNotification("The patient was removed");
				}
			}
		}

		// data grid selection changed event
		private void datagridPatiens_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			buttonsEditRemoveStateChange();
		}

		// activate/deactivate buttons
		void buttonsEditRemoveStateChange()
		{
			// to make buttons enabled only when patient was chosen
			buttonEdit.IsEnabled = buttonRemove.IsEnabled = buttonOpen.IsEnabled = (datagridPatiens.SelectedItems.Count > 0);
		}

		// button click on data grid 
		private void datagridPatiens_PreviewKeyDown(object sender, KeyEventArgs e)
		{
			// open patient's card by enter key
			if (e.Key == Key.Enter)
				openPatientsCard(this.GetIdOfTheSelectedPatient());
			// remove patient by delete key
			else if (e.Key == Key.Delete)
				buttonRemove.RaiseEvent(new RoutedEventArgs(Button.ClickEvent));
		}

		// mouse double click on patient in data grid
		private void datagridPatiens_MouseDoubleClick(object sender, MouseButtonEventArgs e)
		{
			if (datagridPatiens.SelectedItems.Count > 0)
            {
                openPatientsCard(this.GetIdOfTheSelectedPatient());
            }
        }

        /// <summary>
        /// Get the id of the selected patient.
        /// </summary>
        /// <returns>Id of the selected patient in the datagridPatiens.</returns>
        private int GetIdOfTheSelectedPatient()
        {
            return (datagridPatiens.SelectedItem as Patient).Id;
        }

        // open patient card window
        void openPatientsCard(int idPatient)
		{
			// TODO

            PatientCardWindow patientCard = new PatientCardWindow(idPatient);
			patientCard.Show();
		}

		// search by enter key
		private void textbox_EnterKeyDown(object sender, KeyEventArgs e)
		{
			// if enter key was pressed in textbox to raise button search event
			if (e.Key == Key.Enter)
				buttonSearch.RaiseEvent(new RoutedEventArgs(Button.ClickEvent));
		}
			//kuku
            //test
            //testkate
            //lisa

		// open card button
		private void buttonOpen_Click(object sender, RoutedEventArgs e)
		{
			// check patient was chosen in list
			if (datagridPatiens.SelectedItems.Count <= 0)
				return;

			openPatientsCard(this.GetIdOfTheSelectedPatient());
		}

		// button to clear all fields
		private void buttonEraser_Click(object sender, RoutedEventArgs e)
		{
			textboxNumberCard.Text = textboxAddress.Text = textboxLastName.Text = textboxDateOfBirth.Text = "";
		}

		// show popup notification
		void showNotification(string message)
		{
			// SnackbarThree - xaml name of MaterialDesign.Snackbar  
			var messageQueue = SnackbarThree.MessageQueue;

			//the message queue can be called from any thread
			Task.Run(() => messageQueue.Enqueue(message));
		}



		private void InitFirstData()
		{
			//Первичные данные для DB
			Patient[] patients = {
			new Patient()
			{
				FirstName = "Bart",
				LastName = "Simpson",
                MiddleName = "mBartSimps",
				BirthDay = new DateTime(1991, 1, 1),
				Addres = "Springfield Sicheslavska str. 11/13",
				Gender = true
			},
			new Patient()
			{
				FirstName = "Bender",
				LastName = "Rodriguez",
                MiddleName = "S",
				BirthDay = new DateTime(1988, 12, 17),
				Addres = "New York Myru str. 121/15",
				Gender = true
			},
			new Patient()
			{
				FirstName = "Lisa",
				LastName = "Simpson",
                MiddleName = "mLisaSimps",
                BirthDay = new DateTime(2001, 9, 11),
				Addres = "Springfield Almasna str. 49/51",
				Gender = false
			},
			};
			MedicalDocType[] types =
			{
				new MedicalDocType()
				{
					Name = "Hospital"
				},
				new MedicalDocType()
				{
					Name = "Referral on analizes"
				},
				new MedicalDocType()
				{
					Name = "Result of analizes"
				}
			};
			MedicalDoc[] docs =
		    {
				new MedicalDoc()
				{
					Name = "Лікарняний Перелом",
					Patient = patients[0],
					MedicalDocType = types[0],
					BeginTime = new DateTime(2002,04,21),
					EndTime = new DateTime(2002,05,21),
					Info = "Aliquam gravida mauris ut mi. Duis risus odio"
				},
				new MedicalDoc()
				{
					Name = "Направлення на анализ крови",
					Patient = patients[1],
					MedicalDocType = types[1],
					BeginTime = new DateTime(2004,04,22),
					EndTime = new DateTime(2004,04,22),
					Info = "ut, nulla. Cras eu tellus eu augue"
				},
				new MedicalDoc()
				{
					Name = "Результати аналізів крови",
					Patient = patients[2],
					MedicalDocType = types[2],
					BeginTime = new DateTime(2006,04,23),
					EndTime = new DateTime(2006,05,23),
					Info = "sit amet, consectetuer adipiscing elit. Aliquam auctor"
				}
			};

			using (DataModel db = new DataModel())
			{
				foreach (Patient patient in patients)
				{
					if (db.Pacients.FirstOrDefault(p => p.FirstName == patient.FirstName) == null)
					{
						db.Pacients.Add(patient);
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
					if (db.MedicalDocs.FirstOrDefault(p => p.Name== docum.Name) == null)
					{
						db.MedicalDocs.Add(docum);
					}
				}
				db.SaveChanges();
			}
		}

		private void Test()
		{
			using (DataModel db = new DataModel())
			{
				MedicalDoc doc = db.MedicalDocs.FirstOrDefault(d => d.Id == 1);
				MessageBox.Show(doc.Patient.FirstName);
			}
		}

		// unused method
		//private void btnAdd_Click(object sender, RoutedEventArgs e)
		//{
		//    AddChangePatient addChangePatient = new AddChangePatient();
		//    addChangePatient.Show();
		//}
	}
}
