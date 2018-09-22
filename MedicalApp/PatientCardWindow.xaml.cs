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
        private List<MedicalDoc> documentsOfTheCurrentPatient = null;

        public PatientCardWindow()
        {
            InitializeComponent();
        }

        public PatientCardWindow(int idPatient)
        {
            InitializeComponent();

            this.FillTheCardWithPatientData(idPatient);

            this.dataGridDocumentList.SelectionChanged += DataGridDocumentList_SelectionChanged;
            this.dataGridDocumentList.AutoGeneratingColumn += DataGridDocumentList_AutoGeneratingColumn;
        }

        private void DataGridDocumentList_AutoGeneratingColumn(object sender, DataGridAutoGeneratingColumnEventArgs e)
        {
            PropertyDescriptor propertyDescriptor = (PropertyDescriptor)e.PropertyDescriptor;
            e.Column.Header = propertyDescriptor.DisplayName;
            if (propertyDescriptor.DisplayName == "Id")
            {
                e.Cancel = true;
            }
        }

        private void DataGridDocumentList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //MessageBox.Show((sender as DataGrid).SelectedIndex.ToString());

            //this.txbInfo.Text = (this.dataGridDocumentList.SelectedItem as )
            //this.txbInfo.Text
                //= ((sender as DataGrid).SelectedItem as DataGridRow).Name.ToString();
                //= this.dataGridDocumentList.SelectedItem

            var ttt = this.dataGridDocumentList.SelectedItems[0];
        }

        /// <summary>
        /// Fill the card with patient data.
        /// </summary>
        private void FillTheCardWithPatientData(int idPatient)
        {
            // TODO #1

            using (DataModel db = new DataModel())
            {
                var currentPatient 
                    = (
                    from patient in db.Pacients
                    where patient.Id == idPatient
                    select patient
                    )
                    .FirstOrDefault();

                if (currentPatient != null)
                {
                    this.labelFullName.Content = currentPatient.FirstName
                        + " "
                        + currentPatient.LastName
                        // TODO add MiddleName in DB
                        //+ " "
                        //+ currentPatient.MiddleName
                        ;

                    this.DateOfBirthValue.Content = currentPatient.BirthDay.ToShortDateString();

                    this.txbAdress.Text = currentPatient.Addres;

                    // db search doc type
                    //var documentsOfTheCurrentPatient
                    this.documentsOfTheCurrentPatient
                        = (
                        from doc in db.MedicalDocs
                        join docType in db.MedicalDocTypes
                        on doc.idMedicalDocType equals docType.Id
                        where doc.idPacient == idPatient
                        //select new { DocumentType = docType.Name, doc.Name }
                        select doc                                                  // TODO up
                        //select new { DocumentType = docType.Name, doc.Name, doc.Info }
                        //select new { doc.Id, DocumentType = doc.MedicalDocType.Name, doc.Name }
                        )
                        .ToList();

                    // show docs patient
                    this.dataGridDocumentList.ItemsSource 
                        = documentsOfTheCurrentPatient
                        //.Select( x => new { x.DocumentType, x.Name })
                        .Select(x => new { x.Id, x.idMedicalDocType, x.Name })  // TODO up
                        .ToList();
                }
            }
        }

        private void txbAdress_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

		private void btnDocSearch_Click(object sender, RoutedEventArgs e)
		{

		}
	}
}
