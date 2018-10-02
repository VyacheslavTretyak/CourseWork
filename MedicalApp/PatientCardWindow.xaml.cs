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
        /// <summary>
        /// Documents received after the request (search).
        /// </summary>
        List<RedefinedMedicalDoc> documentsAfterSearch = null;
        /// <summary>
        /// Patient id (open card).
        /// </summary>
        private int idPatient;
        /// <summary>
        /// Index of the selected document in the document table.
        /// </summary>
        private int selectedIndexDocument;

        /// <summary>
        /// Override class MedicalDoc.
        /// </summary>
        private class RedefinedMedicalDoc
        {
            public int Id { get; set; }
            public string DocumentType { get; set; }
            public string Name { get; set; }
            public string Info { get; set; }
            public DateTime BeginTime { get; set; }
            public DateTime? EndTime { get; set; }
        }


        /// <summary>
        /// The default constructor is not designed to work in the current application.
        /// </summary>
        public PatientCardWindow()
        {
            InitializeComponent();

            MessageBox.Show("An error occurred while getting the patient's Id.\n"
                + "The window will be closed.",
                "Error while retrieving data",
                MessageBoxButton.OK,
                MessageBoxImage.Error);

            this.Close();
        }

        /// <summary>
        /// The constructor accepts the patient ID to open a card of the patient.
        /// </summary>
        /// <param name="idPatient">Patient id.</param>
        public PatientCardWindow(int idPatient)
        {
            InitializeComponent();

            this.idPatient = idPatient;

            this.Loaded += PatientCardWindow_Loaded;
        }

        /// <summary>
        /// Installation of initial window data.
        /// Register event handlers.
        /// </summary>
        private void PatientCardWindow_Loaded(object sender, RoutedEventArgs e)
        {
            this.btnDocEdit.IsEnabled = false;

            this.FillTheCardWithPatientData();

            this.datePicStartData.ToolTip = "Enter the date in the format 00.00.0000";
            this.datePicFinalData.ToolTip = "Enter the date in the format 00.00.0000";

            this.documentsAfterSearch = new List<RedefinedMedicalDoc>();

            // DataGrid
            this.dataGridDocumentList.SelectionChanged += DataGridDocumentList_SelectionChanged;

            // Button
            this.btnDocAdd.Click += BtnDocAdd_Click;
            this.btnDocEdit.Click += BtnDocEdit_Click;
            this.buttonEraser.Click += ButtonEraser_Click;

            // DatePicker
            this.datePicStartData.PreviewTextInput += DatePic_PreviewTextInput;
            this.datePicFinalData.PreviewTextInput += DatePic_PreviewTextInput;
        }

        /// <summary>
        /// Processing of input characters (the ban letters).
        /// </summary>
        private void DatePic_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = Regex.IsMatch(e.Text, @"[^0-9.\-/]+");
        }

        /// <summary>
        /// Erase button click processing (search data).
        /// </summary>
        private void ButtonEraser_Click(object sender, RoutedEventArgs e)
        {
            this.txbName.Text = "";
            this.datePicStartData.Text = "";
            this.datePicFinalData.Text = "";
        }

        /// <summary>
        /// Handler for the document edit button.
        /// </summary>
        private void BtnDocEdit_Click(object sender, RoutedEventArgs e)
        {
            int currentDocId
                = (this.dataGridDocumentList.SelectedItem as RedefinedMedicalDoc)
                .Id;


            //Opening the edit window.
            AddEditDocument addDocument
                = new AddEditDocument(this.idPatient, currentDocId);

            bool? result = addDocument.ShowDialog();


            // Actions after closing the window.
            if (result == true)
            {
                this.ShowNotification("Document edited");
            }

            this.ReturningLatestDataInTable();

            this.SelectionInTableOfEditedPatient();
        }

        /// <summary>
        /// Selection in the table of the edited patient.
        /// </summary>
        private void SelectionInTableOfEditedPatient()
        {
            this.dataGridDocumentList.SelectedIndex = this.selectedIndexDocument;
        }

        /// <summary>
        /// Returning the latest data in the table.
        /// </summary>
        private void ReturningLatestDataInTable()
        {
            if (this.documentsAfterSearch.Count > 0)
            {
                this.dataGridDocumentList.ItemsSource = this.documentsAfterSearch;
            }
            else
            {
                this.ShowPatientDocsToADatagrid();
            }
        }

        /// <summary>
        /// Handler for the add document button.
        /// </summary>
        private void BtnDocAdd_Click(object sender, RoutedEventArgs e)
        {
            // Opening the add document window.
            AddEditDocument addDocument = new AddEditDocument(this.idPatient);

            bool? result = addDocument.ShowDialog();


            // Actions after closing the window.
            if (result == true)
            {
                this.documentsAfterSearch.Clear();

                this.ShowNotification("Document added");

                this.ShowPatientDocsToADatagrid();

                this.dataGridDocumentList.SelectedIndex = this.dataGridDocumentList.Items.Count - 1;

                this.dataGridDocumentList.ScrollIntoView(this.dataGridDocumentList.SelectedItem);
            }
            else
            {
                this.ReturningLatestDataInTable();

                this.SelectionInTableOfEditedPatient();
            }
        }

        /// <summary>
        /// Handler to change the selected document in the document table.
        /// </summary>
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

        /// <summary>
        /// Deactivation of the document editing button.
        /// </summary>
        private void DeactivationOfTheDocumentEditingButton()
        {
            this.btnDocEdit.IsEnabled = false;
        }

        /// <summary>
        /// Activation of the document editing button.
        /// </summary>
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
                    this.FillTheCardWithData(currentPatient);

                    this.ShowPatientDocsToADatagrid(db);
                }
            }
        }

        /// <summary>
        /// Fill the card with data.
        /// </summary>
        /// <param name="currentPatient">Current patient whose card is open.</param>
        private void FillTheCardWithData(Patient currentPatient)
        {
            this.labelFullName.Content = currentPatient.FirstName
                                    + " "
                                    + currentPatient.LastName
                                    + " "
                                    + currentPatient.MiddleName
                                    ;

            // TODO добавить пол пациента.
            //string gender = currentPatient.Gender == true ? "Male" : "Female";
            //MessageBox.Show(gender);
            //this.labelGender = currentPatient.Gender == true ? "Male" : "Female";

            this.DateOfBirthValue.Content = currentPatient.BirthDay.ToShortDateString();

            this.txbAdress.Text = currentPatient.Addres;
        }

        /// <summary>
        /// Show Patient docs to a Datagrid.
        /// </summary>
        private void ShowPatientDocsToADatagrid()
        {
            using (DataModel db = new DataModel())
            {
                this.LoadingFromDatabaseDocsThePatientInDatagrid(db);
            }
        }

        /// <summary>
        /// Show Patient docs to a Datagrid (with passing database context).
        /// </summary>
        /// <param name="db">Database context.</param>
        private void ShowPatientDocsToADatagrid(DataModel db)
        {
            this.LoadingFromDatabaseDocsThePatientInDatagrid(db);
        }

        /// <summary>
        /// Loading from the patient's database of documents in Datagrid.
        /// </summary>
        /// <param name="db">Database context.</param>
        private void LoadingFromDatabaseDocsThePatientInDatagrid(DataModel db)
        {
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

        /// <summary>
        /// Handler for clicking on the document search button.
        /// </summary>
		private void btnDocSearch_Click(object sender, RoutedEventArgs e)
        {
            this.ForgetTheLastSelectedDocument();

            if (this.IsSearchFieldsAreEmpty())
            {
                this.documentsAfterSearch.Clear();

                this.ShowPatientDocsToADatagrid();
            }
            else
            {
                this.SearchDocumentsBasedOnEnteredData();
            }
        }

        /// <summary>
        /// Date range is correct.
        /// </summary>
        /// <returns>true if the range is correct.</returns>
        private bool IsCorrectDateRange()
        {
            DateTime dateTimeStartData = this.GetStartingDateOfSearchRange();
            DateTime dateTimeFinalData = this.GetFinalDateOfSearchRange();

            if (dateTimeStartData <= dateTimeFinalData)
            {
                return true;
            }

            return false;
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
            this.documentsAfterSearch.Clear();

            using (DataModel db = new DataModel())
            {
                this.SearchDocumentsByName(db);

                this.SearchDocumentsByDateRange(db);
            }

            this.dataGridDocumentList.ItemsSource
                    = this.documentsAfterSearch;
        }

        /// <summary>
        /// Search documents by date range.
        /// </summary>
        /// <param name="db">Context database.</param>
        private void SearchDocumentsByDateRange(DataModel db)
        {
            if (this.IsBothDateFieldsAreFilled())
            {
                this.SearchByDateRange(db);
            }
            else if (this.IsOneOfDateRangeFieldsIsEmpty())
            {
                MessageBox.Show("To search by range, you need to enter both dates!",
                    "Date field is not filled",
                    MessageBoxButton.OK,
                    MessageBoxImage.Asterisk);
            }
        }

        /// <summary>
        /// One of the date range fields is empty.
        /// </summary>
        /// <returns>true if one of the date range fields is empty.</returns>
        private bool IsOneOfDateRangeFieldsIsEmpty()
        {
            return String.IsNullOrEmpty(this.datePicStartData.Text)
                && !String.IsNullOrEmpty(this.datePicFinalData.Text)
                || !String.IsNullOrEmpty(this.datePicStartData.Text)
                && String.IsNullOrEmpty(this.datePicFinalData.Text);
        }

        /// <summary>
        /// Both date fields are filled.
        /// </summary>
        /// <returns>true if both date range fields are filled.</returns>
        private bool IsBothDateFieldsAreFilled()
        {
            return !String.IsNullOrEmpty(this.datePicStartData.Text)
                && !String.IsNullOrEmpty(this.datePicFinalData.Text);
        }

        /// <summary>
        /// Search documents by name.
        /// </summary>
        /// <param name="db">Context database.</param>
        private void SearchDocumentsByName(DataModel db)
        {
            if (!String.IsNullOrEmpty(this.txbName.Text))
            {
                this.documentsAfterSearch
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
        }

        /// <summary>
        /// Search by date range.
        /// </summary>
        /// <param name="db">Context database.</param>
        private void SearchByDateRange(DataModel db)
        {
            if (this.IsCorrectDateRange())
            {
                if (this.documentsAfterSearch.Count > 0)
                {
                    this.documentsAfterSearch
                        = SearchByDateRange(this.documentsAfterSearch);
                }
                else
                {
                    this.documentsAfterSearch
                        = SearchByDateRange(db.MedicalDocs);
                }
            }
            else
            {
                MessageBox.Show("Invalid date range!",
                        "Invalid date range",
                        MessageBoxButton.OK,
                        MessageBoxImage.Asterisk);
            }
        }

        /// <summary>
        /// Search by date range.
        /// </summary>
        /// <param name="medicalDocs">MedicalDoc collection (in the database).</param>
        /// <returns>List of overridden documents (RedefinedMedicalDoc).</returns>
        private List<RedefinedMedicalDoc> SearchByDateRange(DbSet<MedicalDoc> medicalDocs)
        {
            List<RedefinedMedicalDoc> documents = null;

            DateTime dateTimeStartData = this.GetStartingDateOfSearchRange();
            DateTime dateTimeFinalData = this.GetFinalDateOfSearchRange();

            documents
                = (
                from doc in medicalDocs.Include("MedicalDocType")
                where doc.PatientId == this.idPatient
                where doc.BeginTime >= dateTimeStartData
                where doc.BeginTime <= dateTimeFinalData
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

        /// <summary>
        /// Search by date range.
        /// </summary>
        /// <param name="medicalDocs">RedefinedMedicalDoc collection (after searching by name).</param>
        /// <returns>List of overridden documents (RedefinedMedicalDoc).</returns>
        private List<RedefinedMedicalDoc> SearchByDateRange(List<RedefinedMedicalDoc> documentsOfTheCurrentPatient)
        {
            List<RedefinedMedicalDoc> documents = null;

            DateTime dateTimeStartData = this.GetStartingDateOfSearchRange();
            DateTime dateTimeFinalData = this.GetFinalDateOfSearchRange();

            documents
                = (
                from doc in documentsOfTheCurrentPatient
                where doc.BeginTime >= dateTimeStartData
                where doc.BeginTime <= dateTimeFinalData
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
        /// Get the final date of the search range.
        /// </summary>
        /// <returns>Final date of the search range.</returns>
        private DateTime GetFinalDateOfSearchRange()
        {
            return this.datePicFinalData.SelectedDate.Value;
        }

        /// <summary>
        /// Get the starting date of the search range.
        /// </summary>
        /// <returns>Starting date of the search range.</returns>
        private DateTime GetStartingDateOfSearchRange()
        {
            return this.datePicStartData.SelectedDate.Value;
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

        /// <summary>
        /// Show popup messages.
        /// </summary>
        /// <param name="message">Message text.</param>
        void ShowNotification(string message)
        {
            // SnackbarThree - xaml name of MaterialDesign.Snackbar  
            var messageQueue = SnackbarThree.MessageQueue;

            // The message queue can be called from any thread.
            Task.Run(() => messageQueue.Enqueue(message));
        }
    }
}
