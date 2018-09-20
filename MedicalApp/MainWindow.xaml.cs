﻿using System;
using System.Collections.Generic;
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
			using (DataModel db = new DataModel())
			{
				try
				{
					// select only not archived patients
					datagridPatiens.ItemsSource = db.Pacients.Where(p => p.IsArchived == false).ToList();
				}
				catch(Exception ex)
				{
					MessageBox.Show(ex.Message);
				}
			}
		}

		// PreviewTextInput event to make numeric textbox
		private void textbox_OnlyNumeric(object sender, TextCompositionEventArgs e)
		{
			var textBox = sender as TextBox;
			e.Handled = Regex.IsMatch(e.Text, "[^0-9]+");
		}

		// search button click 
		private void buttonSearch_Click(object sender, RoutedEventArgs e)
		{
			using (DataModel db = new DataModel())
			{
				// first select all not archived patiens
				IQueryable<Pacient> queryable = db.Pacients.Where(p => p.IsArchived == false);
				// if number card field not empty
				if (!string.IsNullOrWhiteSpace(textboxNumberCard.Text))
				{
					// selection of patients with the entered card number
					int cardNum = Convert.ToInt32(textboxNumberCard.Text);
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
				fillDataFromDBtoDatagrid();
		}

		// edit patient button click
		private void buttonEdit_Click(object sender, RoutedEventArgs e)
		{
			// check patient was chosen in list
			if (datagridPatiens.SelectedItems.Count <= 0)
				return;

			// open add patient window with filled fields
			AddChangePatient addEditWindow = new AddChangePatient((Pacient)datagridPatiens.SelectedItem);
			addEditWindow.Title = "Edit patient";

			// update data grid if patient was changed 
			if (addEditWindow.ShowDialog() == true)
				fillDataFromDBtoDatagrid();
		}

		// remove patient button click
		private void buttonRemove_Click(object sender, RoutedEventArgs e)
		{
			// check patient was chosen in list
			if (datagridPatiens.SelectedItems.Count <= 0)
				return;
			
			// get selected patient
			Pacient pacient = datagridPatiens.SelectedItem as Pacient;
			// show confirmation message box
			if (MessageBox.Show($"Are you sure you want to archive {pacient.FirstName + " " + pacient.LastName} ?",
				"Confirmation", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
			{
				using (DataModel db = new DataModel())
				{
					// find patient in db
					pacient = db.Pacients.FirstOrDefault(p => p.Id == pacient.Id);
					// remove
					pacient.IsArchived = true;
					// save
					db.SaveChanges();
					// update data grid list
					fillDataFromDBtoDatagrid();
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
			buttonEdit.IsEnabled = buttonRemove.IsEnabled = (datagridPatiens.SelectedItems.Count > 0);
		}

		// button click on data grid 
		private void datagridPatiens_PreviewKeyDown(object sender, KeyEventArgs e)
		{
			// open patient's card by enter key
			if (e.Key == Key.Enter)
				openPatientsCard();
			// remove patient by delete key
			else if (e.Key == Key.Delete)
				buttonRemove.RaiseEvent(new RoutedEventArgs(Button.ClickEvent));
		}

		// mouse double click on patient in data grid
		private void datagridPatiens_MouseDoubleClick(object sender, MouseButtonEventArgs e)
		{
			if (datagridPatiens.SelectedItems.Count > 0)
			{
				openPatientsCard();
			}
		}

		// open patient card window
		void openPatientsCard()
		{
			// TODO
			// PatientCardWindow patientCardWindow = new PatientCardWindow();
			// patientCardWindow.Show();
		}

		// search by enter key
		private void textbox_EnterKeyDown(object sender, KeyEventArgs e)
		{
			// if enter key was pressed in textbox to raise button search event
			if (e.Key == Key.Enter)
				buttonSearch.RaiseEvent(new RoutedEventArgs(Button.ClickEvent));
		}
	}
}
