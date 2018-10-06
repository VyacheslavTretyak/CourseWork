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
using System.Windows.Shapes;

namespace MedicalApp
{
    /// <summary>
    /// Interaction logic for AddChangePatient.xaml
    /// </summary>
    public partial class AddChangePatient : Window
    {
        Patient pacient;
        public AddChangePatient()
        {
            InitializeComponent();
            //commit
            btnAddEdit.Content = "Add";
            btnAddEdit.IsEnabled = false;
        }

        //add/edit button click
        private void btnAddEdit_Click(object sender, RoutedEventArgs e)
        {
            using (DataModel db = new DataModel())
            {
                //edit current user 
                if (pacient != null)
                {
                    try
                    {
                        db.Pacients.Find(pacient.Id).FirstName = txbFirstName.Text.Trim();
                        db.Pacients.Find(pacient.Id).LastName = txbLastName.Text.Trim();
                        db.Pacients.Find(pacient.Id).MiddleName = txbMiddleName.Text.Trim();
                        db.Pacients.Find(pacient.Id).Addres = txbAdress.Text.Trim();
                        db.Pacients.Find(pacient.Id).BirthDay = DateTime.ParseExact(txbBirth.Text, "dd.MM.yyyy",
                            System.Globalization.CultureInfo.InvariantCulture);

                        if (rdbMale.IsChecked == false)
                            db.Pacients.Find(pacient.Id).Gender = false;
                        else
                            db.Pacients.Find(pacient.Id).Gender = true;
                        db.SaveChanges();
                        DialogResult = true;
                        this.Close();
                    }
                    catch (Exception ex) { }
                }
                //create new user
                else
                {
                    try
                    {
                        pacient = new Patient();
                        pacient.FirstName = txbFirstName.Text.Trim();
                        pacient.LastName = txbLastName.Text.Trim();
                        pacient.MiddleName = txbMiddleName.Text.Trim();
                        pacient.Addres = txbAdress.Text.Trim();

                        pacient.BirthDay = DateTime.ParseExact(txbBirth.Text, "dd.MM.yyyy",
                            System.Globalization.CultureInfo.InvariantCulture);

                        if (rdbMale.IsChecked == false)
                            pacient.Gender = false;
                        else
                            pacient.Gender = true;
                        db.Pacients.Add(pacient);
                        db.SaveChanges();
                        DialogResult = true;
                        this.Close();
                    }
                    catch (Exception ex) { }
                }
            }

        }

        // PreviewTextInput event to make numeric textbox
        private void textbox_OnlyNumeric(object sender, TextCompositionEventArgs e)
        {
            var textBox = sender as TextBox;
            e.Handled = Regex.IsMatch(e.Text, "[^0-9.]");
        }

        //cancel button click
        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        //display information about selected user
        public AddChangePatient(Patient patient) : this()
        {
            pacient = patient;
            btnAddEdit.Content = "Save";
            txbFirstName.Text = patient.FirstName;
            txbLastName.Text = patient.LastName;
            txbMiddleName.Text = pacient.MiddleName;
            txbAdress.Text = patient.Addres;
            txbBirth.Text = patient.BirthDay.ToShortDateString();
            if (!patient.Gender)
                rdbFemale.IsChecked = true;
        }

        private void txbFirstName_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (String.IsNullOrEmpty(txbFirstName.Text))
                btnAddEdit.IsEnabled = false;
            else if (!String.IsNullOrEmpty(txbFirstName.Text) &&
                !String.IsNullOrEmpty(txbLastName.Text) &&
                !String.IsNullOrEmpty(txbAdress.Text) &&
                !String.IsNullOrEmpty(txbBirth.Text))
                btnAddEdit.IsEnabled = true;
        }

        private void txbLastName_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (String.IsNullOrEmpty(txbLastName.Text))
                btnAddEdit.IsEnabled = false;
            else if (!String.IsNullOrEmpty(txbFirstName.Text) &&
                !String.IsNullOrEmpty(txbLastName.Text) &&
                !String.IsNullOrEmpty(txbAdress.Text) &&
                !String.IsNullOrEmpty(txbBirth.Text))
                btnAddEdit.IsEnabled = true;
        }

        private void txbAdress_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (String.IsNullOrEmpty(txbAdress.Text))
                btnAddEdit.IsEnabled = false;
            else if (!String.IsNullOrEmpty(txbFirstName.Text) &&
                !String.IsNullOrEmpty(txbLastName.Text) &&
                !String.IsNullOrEmpty(txbAdress.Text) &&
                !String.IsNullOrEmpty(txbBirth.Text))
                btnAddEdit.IsEnabled = true;
        }

        private void txbBirth_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            if (String.IsNullOrEmpty(txbBirth.Text))
                btnAddEdit.IsEnabled = false;
            else if (!String.IsNullOrEmpty(txbFirstName.Text) &&
                !String.IsNullOrEmpty(txbLastName.Text) &&
                !String.IsNullOrEmpty(txbAdress.Text) &&
                !String.IsNullOrEmpty(txbBirth.Text))
                btnAddEdit.IsEnabled = true;
        }
    }
}