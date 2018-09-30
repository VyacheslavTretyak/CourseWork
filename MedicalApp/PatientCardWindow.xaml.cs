using System;
using System.Collections.Generic;
using System.ComponentModel;
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

        private int selectedIndexDocument;

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

            this.Loaded += PatientCardWindow_Loaded;
        }

        private void PatientCardWindow_Loaded(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("An error occurred while getting the patient's Id.\n"
                + "The window will be closed.",
                "Error while retrieving data",
                MessageBoxButton.OK,
                MessageBoxImage.Error);

            this.Close();
        }

        public PatientCardWindow(int idPatient)
        {
            InitializeComponent();

            this.idPatient = idPatient;

            this.btnDocEdit.IsEnabled = false;

            this.FillTheCardWithPatientData();

            this.datePicStartData.ToolTip = "Введите дату в формате 00.00.0000";
            this.datePicFinalData.ToolTip = "Введите дату в формате 00.00.0000";




            this.dataGridDocumentList.SelectionChanged += DataGridDocumentList_SelectionChanged;

            // Button
            this.btnDocAdd.Click += BtnDocAdd_Click;
            this.btnDocEdit.Click += BtnDocEdit_Click;
            this.buttonEraser.Click += ButtonEraser_Click;

            // DatePicker
            this.datePicStartData.PreviewTextInput += DatePicStartData_PreviewTextInput;
            this.datePicStartData.KeyDown += DatePicStartData_KeyDown;
            this.datePicStartData.PreviewKeyDown += DatePic_PreviewKeyDown;
            this.datePicFinalData.PreviewKeyDown += DatePic_PreviewKeyDown;


            // Temp method - do not delete.
            TempMethod();
        }

        private void ButtonEraser_Click(object sender, RoutedEventArgs e)
        {
            this.txbName.Text = "";
            this.datePicStartData.Text = "";
            this.datePicFinalData.Text = "";
        }

        private void DatePic_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key >= Key.D0 && e.Key <= Key.D9
                || e.Key >= Key.NumPad0 && e.Key <= Key.NumPad9
                || e.Key == Key.OemPeriod
                || e.Key == Key.Decimal
                || e.Key == Key.Back
                || e.Key == Key.Tab)
            {
                e.Handled = false;
            }
            else if (e.Key == Key.Space)
            {
                e.Handled = true;
            }
            else
            {
                e.Handled = true;
            }


        }

        private void DatePicStartData_KeyDown(object sender, KeyEventArgs e)
        {
            

            
        }

        private void DatePicStartData_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            //if (!Char.IsDigit(e.Text, 0)
            //    && e.Text != ".")
            //{
            //    e.Handled = true;
            //}

            //if (e.Text == " ")
            //{
            //    e.Handled = true;
            //}
            //MessageBox.Show(Regex.IsMatch(this.datePicStartData.Text, @"[0-9.]{3}").ToString());
            //e.Handled = Regex.IsMatch(e.Text, @"[0-9.]");
            //e.Handled = Regex.IsMatch(e.Text, @"\d{1,2}\.\d{1,2}\.\d{4}");
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

            // TODO выбрать (выделить) редактируемого пациента
            this.dataGridDocumentList.SelectedIndex = this.selectedIndexDocument;
        }

        private void BtnDocAdd_Click(object sender, RoutedEventArgs e)
        {
            AddEditDocument addDocument = new AddEditDocument(this.idPatient);

            bool? result = addDocument.ShowDialog();

            this.ShowPatientDocsToADatagrid();

            // TODO выбрать (выделить) добавленного пациента
            if (result == true)
            {
                this.dataGridDocumentList.SelectedIndex = this.dataGridDocumentList.Items.Count - 1;
            }
            else
            {
                this.dataGridDocumentList.SelectedIndex = this.selectedIndexDocument;
            }
        }

        private void DataGridDocumentList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (this.IsARowOfTheDocumentInTheTableIsHighlighted())
            {
                this.ActivationOfTheDocumentEditingButton();

                this.txbInfo.Text
                    = ((sender as DataGrid).SelectedItem as RedefinedMedicalDoc).Info;

                this.selectedIndexDocument = this.dataGridDocumentList.SelectedIndex;
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
            if (this.IsSearchFieldsAreEmpty())
            {
                this.ShowPatientDocsToADatagrid();

                this.ForgetTheLastSelectedDocument();
            }
            else
            {
                this.SearchDocumentsBasedOnEnteredData();
            }
        }

        /// <summary>
        /// Forget the last selected document.
        /// </summary>
        private void ForgetTheLastSelectedDocument()
        {
            this.selectedIndexDocument = -1;
        }

        /// <summary>
        /// Search for patient documents based on entered data.
        /// </summary>
        private void SearchDocumentsBasedOnEnteredData()
        {
            List<RedefinedMedicalDoc> documentsOfTheCurrentPatient = null;

            using (DataModel db = new DataModel())
            {
                if (!String.IsNullOrEmpty(this.txbName.Text))
                {
                    documentsOfTheCurrentPatient
                        = (
                        from doc in db.MedicalDocs.Include("MedicalDocType")
                        where doc.PatientId == this.idPatient
                        where doc.Name.Contains(this.txbName.Text)
                        select new RedefinedMedicalDoc
                        {
                            Id = doc.Id,
                            DocumentType = doc.MedicalDocType.Name,
                            Name = doc.Name,
                            Info = doc.Info,
                            BeginTime = doc.BeginTime,
                            EndTime = doc.EndTime
                        }
                        )
                        .ToList();
                }

                if (!String.IsNullOrEmpty(this.datePicStartData.Text)
                    && !String.IsNullOrEmpty(this.datePicFinalData.Text))
                {
                    MessageBox.Show(this.datePicStartData.ToString());

                    //this.SearchForDocumentsByDateRange(db, documentsOfTheCurrentPatient);
                }
                else if (String.IsNullOrEmpty(this.datePicStartData.Text)
                        || String.IsNullOrEmpty(this.datePicFinalData.Text))
                {
                    MessageBox.Show("To search by range, you need to enter both dates!",
                        "Date field is not filled",
                        MessageBoxButton.OK,
                        MessageBoxImage.Asterisk);
                }
            }


            this.dataGridDocumentList.ItemsSource
                    = documentsOfTheCurrentPatient;
        }

        /// <summary>
        /// Search for documents by date range.
        /// </summary>
        /// <param name="db"></param>
        /// <param name="documentsOfTheCurrentPatient"></param>
        private void SearchForDocumentsByDateRange(DataModel db, List<RedefinedMedicalDoc> documentsOfTheCurrentPatient)
        {
            if (documentsOfTheCurrentPatient.Count > 0)
            {
                documentsOfTheCurrentPatient
                    = SearchByDateRange(documentsOfTheCurrentPatient);
            }
            else
            {
                documentsOfTheCurrentPatient
                    = SearchByDateRange(db.MedicalDocs);
            } 
        }

        private List<RedefinedMedicalDoc> SearchByDateRange(DbSet<MedicalDoc> medicalDocs)
        {
            List<RedefinedMedicalDoc> documents = null;

            documents
                = (
                from doc in medicalDocs.Include("MedicalDocType")
                where doc.PatientId == this.idPatient
                // TODO поиск по начальной дате.
                select new RedefinedMedicalDoc
                {
                    Id = doc.Id,
                    DocumentType = doc.MedicalDocType.Name,
                    Name = doc.Name,
                    Info = doc.Info,
                    BeginTime = doc.BeginTime,
                    EndTime = doc.EndTime
                }
                )
                .ToList();

            return documents;
        }

        private List<RedefinedMedicalDoc> SearchByDateRange(List<RedefinedMedicalDoc> documentsOfTheCurrentPatient)
        {
            List<RedefinedMedicalDoc> documents = null;

            documents
                = (
                from doc in documentsOfTheCurrentPatient
                where doc.Id == this.idPatient
                // TODO поиск по начальной дате.
                select new RedefinedMedicalDoc
                {
                    Id = doc.Id,
                    DocumentType = doc.DocumentType,
                    Name = doc.Name,
                    Info = doc.Info,
                    BeginTime = doc.BeginTime,
                    EndTime = doc.EndTime
                }
                )
                .ToList();

            return documents;
        }



        /// <summary>
        /// Search fields are empty.
        /// </summary>
        /// <returns>true if all search fields are empty.</returns>
        private bool IsSearchFieldsAreEmpty()
        {
            if (String.IsNullOrEmpty(this.txbName.Text)
                && String.IsNullOrEmpty(this.datePicStartData.Text)
                && String.IsNullOrEmpty(this.datePicFinalData.Text))
            {
                return true;
            }

            return false;
        }
    }
}
