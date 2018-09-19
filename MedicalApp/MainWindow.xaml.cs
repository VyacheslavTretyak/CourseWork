using System;
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
				catch(Exception ex) { MessageBox.Show(ex.Message); }
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
			// TODO
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
			// check patient was choosed in list
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
			// check patient was choosed in list
			if (datagridPatiens.SelectedItems.Count <= 0)
				return;

			// TODO
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
		}
	}
}
