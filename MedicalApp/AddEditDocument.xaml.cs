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
using System.Windows.Shapes;

//Для того кто делает Карточка пациента
//Передайте в обработчике на кнопку Add - new AddEditDocument(Id пациента)
//Передайте в обработчике на кнопку edit - new AddEditDocument(Id пациента, IdMedicalDoc)

namespace MedicalApp
{
    /// <summary>
    /// Логика взаимодействия для AddEditDocument.xaml
    /// </summary>
    public partial class AddEditDocument : Window
    {
        //variables from the constructor
        int IdPacient;
        int IdMedicalDoc;

        //one-parameter constructor
        public AddEditDocument(int _IdPacient)
        {
            InitializeComponent();
            IdPacient = _IdPacient;
            this.Title = "Add Document";
            Add.Content = "Add";
            InitAdd();
        }

        //init window one-parameter
        private void InitAdd()
        {
            using (DataModel db = new DataModel())
            {
                //check the patient's presence
                Patient pac = new Patient();
                pac = db.Pacients.Where(a => a.Id == IdPacient).FirstOrDefault();
                if (pac == null)
                {
                    MessageBox.Show("Pacient not found");
                    Close();
                }
                //init combo Type
                foreach (var item in db.MedicalDocTypes.ToList())
                {
                    ComboType.Items.Add(item.Name);
                }
            }
        }

        //constructor with two parameters
        public AddEditDocument(int _IdPacient, int _IdMedicalDoc)
        {
            IdPacient = _IdPacient;
            IdMedicalDoc = _IdMedicalDoc;
            InitializeComponent();
            this.Title = "Edit Document";
            Add.Content = "Edit";
            InitEdit();
        }

        //init windows with two parameters
        private void InitEdit()
        {
            using (DataModel db = new DataModel())
            {
                //check the patient's presence
                Patient pac = new Patient();
                pac = db.Pacients.Where(a => a.Id == IdPacient).FirstOrDefault();
                if (pac == null)
                {
                    MessageBox.Show("Pacient not found");
                    Close();
                }
                //init combo Type
                foreach (var item in db.MedicalDocTypes.ToList())
                {
                    ComboType.Items.Add(item.Name);
                }
                //document verification
                MedicalDoc medicalDocType = new MedicalDoc();
                medicalDocType = db.MedicalDocs.Where(a => a.Id == IdMedicalDoc).FirstOrDefault();
                if (medicalDocType == null)
                {
                    MessageBox.Show("document not found");
                    Close();
                }
                //filling fields
                foreach (var item in ComboType.Items)
                {
                    if (item.ToString() == db.MedicalDocTypes.Where(a => a.Id == medicalDocType.MedicalDocTypeId).FirstOrDefault().Name)
                    {
                        ComboType.SelectedItem = item as ComboBoxItem;
                        ComboType.Text = item.ToString();
                    }
                }
                TxBxName.Text = medicalDocType.Name;
                TxBxInfo.Text = medicalDocType.Info;
                DateBegin.SelectedDate = medicalDocType.BeginTime;
                DateEnd.SelectedDate = medicalDocType.EndTime;
            }
        }

        //add button handling
        private void Add_Click(object sender, RoutedEventArgs e)
        {
            using (DataModel db = new DataModel())
            {
                //Add MedicalDoc
                if (IdMedicalDoc == 0)
                {
                    MedicalDoc medicalDoc = new MedicalDoc
                    {
                        Name = TxBxName.Text.Trim(),
                        PatientId = IdPacient,
                        MedicalDocTypeId = db.MedicalDocTypes.Where(a => a.Name == ComboType.SelectedValue.ToString()).FirstOrDefault().Id,
                        BeginTime = (DateTime)DateBegin.SelectedDate,
                        EndTime = DateEnd.SelectedDate,
                        Info = TxBxInfo.Text.Trim()
                    };
                    db.MedicalDocs.Add(medicalDoc);
                }
                else
                {
                    //Edit MedicalDoc
                    db.MedicalDocs.Where(x => x.Id == IdMedicalDoc).FirstOrDefault().Name = (TxBxName.Text).Trim();
                    db.MedicalDocs.Where(x => x.Id == IdMedicalDoc).FirstOrDefault().MedicalDocTypeId = db.MedicalDocTypes.Where(a => a.Name == ComboType.SelectedValue.ToString()).FirstOrDefault().Id;
                    db.MedicalDocs.Where(x => x.Id == IdMedicalDoc).FirstOrDefault().BeginTime = (DateTime)DateBegin.SelectedDate;
                    db.MedicalDocs.Where(x => x.Id == IdMedicalDoc).FirstOrDefault().EndTime = DateEnd.SelectedDate;
                    db.MedicalDocs.Where(x => x.Id == IdMedicalDoc).FirstOrDefault().Info = TxBxInfo.Text.Trim();
                }
                db.SaveChanges();
            }

            this.DialogResult = true;

            Close();
        }

        //clickable button cancel
        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;

            Close();
        }

        //handling changes in input fields
        private void EnabledAdd(object sender, RoutedEventArgs e)
        {
            if (ComboType.SelectedIndex == -1 || string.IsNullOrWhiteSpace(TxBxName.Text) || string.IsNullOrWhiteSpace(TxBxInfo.Text) || DateBegin.SelectedDate == null)
            {
                Add.IsEnabled = false;
            }
            else
            {
                if (DateEnd.SelectedDate != null)
                {
                    if (DateBegin.SelectedDate > DateEnd.SelectedDate)
                    {
						DateEnd.Foreground = new SolidColorBrush(Colors.Red);
                        Add.IsEnabled = false;
                        return;
                    }
					else
					{
						DateEnd.Foreground = DateBegin.Foreground;
					}
                }
                Add.IsEnabled = true;
            }
        }
    }
}
