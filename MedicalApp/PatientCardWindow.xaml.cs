using System;
using System.Collections.Generic;
using System.ComponentModel;
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

namespace MedicalApp
{
    /// <summary>
    /// Interaction logic for PatientCard.xaml
    /// </summary>
    public partial class PatientCardWindow : Window
    {
        //private List<RedefinedMedicalDoc> documentsOfTheCurrentPatient = null;   // TODO HACK ???

        private int idPatient;

        //private Type type = null;
        private class RedefinedMedicalDoc /*: MedicalDoc*/
        {
            public int Id { get; set; }
            public string DocumentType { get; set; }
            public string Name { get; set; }
            public string Info { get; set; }
            public DateTime BeginTime { get; set; }
            public DateTime? EndTime { get; set; }
        }

        public PatientCardWindow()
        {
            InitializeComponent();
        }

        public PatientCardWindow(int idPatient)
        {
            InitializeComponent();

            this.idPatient = idPatient;

            this.btnDocEdit.IsEnabled = false;

            this.FillTheCardWithPatientData();

            this.dataGridDocumentList.SelectionChanged += DataGridDocumentList_SelectionChanged;

            // Button
            this.btnDocAdd.Click += BtnDocAdd_Click;
            this.btnDocEdit.Click += BtnDocEdit_Click;


            // Temp method - do not delete.
            TempMethod();
        }

        private void TempMethod()
        {
            
        }

        private void BtnDocEdit_Click(object sender, RoutedEventArgs e)
        {
            int currentDocId
                = (this.dataGridDocumentList.SelectedItem as RedefinedMedicalDoc)
                .Id;

            AddEditDocument addDocument 
                = new AddEditDocument(this.idPatient, currentDocId);

            addDocument.ShowDialog();

            this.ShowPatientDocsToADatagrid();
        }

        private void BtnDocAdd_Click(object sender, RoutedEventArgs e)
        {
            AddEditDocument addDocument = new AddEditDocument(this.idPatient);

            addDocument.ShowDialog();

            this.ShowPatientDocsToADatagrid();
        }

        

        //private void DataGridDocumentList_AutoGeneratingColumn(object sender, DataGridAutoGeneratingColumnEventArgs e)
        //{
        //    PropertyDescriptor propertyDescriptor = (PropertyDescriptor)e.PropertyDescriptor;
        //    e.Column.Header = propertyDescriptor.DisplayName;
        //    if (IsColumnNotDisplayed(propertyDescriptor))
        //    {
        //        e.Cancel = true;
        //    }
        //}

        //private static bool IsColumnNotDisplayed(PropertyDescriptor propertyDescriptor)
        //{
        //    if (propertyDescriptor.DisplayName == "Id"
        //        || propertyDescriptor.DisplayName == "idPacient"
        //        || propertyDescriptor.DisplayName == "BeginTime"
        //        || propertyDescriptor.DisplayName == "EndTime"
        //        || propertyDescriptor.DisplayName == "Info"
        //        || propertyDescriptor.DisplayName == "Pacient_Id")
        //    {
        //        return true;
        //    }

        //    return false;
        //}

        private void DataGridDocumentList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (this.IsARowOfTheDocumentInTheTableIsHighlighted())
            {
                this.ActivationOfTheDocumentEditingButton();

                this.txbInfo.Text
                    = ((sender as DataGrid).SelectedItem as RedefinedMedicalDoc).Info;
            }
            else
            {
                this.DeactivationOfTheDocumentEditingButton();

                this.txbInfo.Text = "";
            }
        }

        /// <summary>
        /// A row of the document in the table is highlighted.
        /// </summary>
        /// <returns>true, if selected.</returns>
        private bool IsARowOfTheDocumentInTheTableIsHighlighted()
        {
            if (this.dataGridDocumentList.SelectedIndex != -1)
            {
                return true;
            }

            return false;
        }

        private void DeactivationOfTheDocumentEditingButton()
        {
            this.btnDocEdit.IsEnabled = false;
        }

        private void ActivationOfTheDocumentEditingButton()
        {
            this.btnDocEdit.IsEnabled = true;
        }

        /// <summary>
        /// Fill the card with patient data.
        /// </summary>
        private void FillTheCardWithPatientData()
        {
            using (DataModel db = new DataModel())
            {
                var currentPatient 
                    = (
                    from patient in db.Pacients
                    where patient.Id == this.idPatient
                    select patient
                    )
                    .FirstOrDefault();

                if (currentPatient != null)
                {
                    this.labelFullName.Content = currentPatient.FirstName
                        + " "
                        + currentPatient.LastName
                        + " "
                        + currentPatient.MiddleName
                        ;

                    this.DateOfBirthValue.Content = currentPatient.BirthDay.ToShortDateString();

                    this.txbAdress.Text = currentPatient.Addres;


                    this.ShowPatientDocsToADatagrid(db);
                }
            }
        }

        private void ShowPatientDocsToADatagrid()
        {
            using (DataModel db = new DataModel())
            {
                this.LoadingFromDatabaseDocsThePatientInDatagrid(db);
            }
        }

        private void ShowPatientDocsToADatagrid(DataModel db)
        {
            this.LoadingFromDatabaseDocsThePatientInDatagrid(db);
        }

        private void LoadingFromDatabaseDocsThePatientInDatagrid(DataModel db)
        {
            // db search doc type
            var documentsOfTheCurrentPatient
                = (
                from doc in db.MedicalDocs.Include("MedicalDocType")
                where doc.PatientId == this.idPatient
                select new RedefinedMedicalDoc{
                    Id =  doc.Id,
                    DocumentType = doc.MedicalDocType.Name,
                    Name = doc.Name,
                    Info = doc.Info,
                    BeginTime = doc.BeginTime,
                    EndTime = doc.EndTime
                }
                )
                .ToList();


            this.dataGridDocumentList.ItemsSource
                = documentsOfTheCurrentPatient;
        }

		private void btnDocSearch_Click(object sender, RoutedEventArgs e)
		{
            // TODO реализовать поиск документов.
            MessageBox.Show("Пока еще не реализовано");
		}
	}
}
