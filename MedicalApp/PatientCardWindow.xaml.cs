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

namespace MedicalApp
{
    /// <summary>
    /// Interaction logic for PatientCard.xaml
    /// </summary>
    public partial class PatientCardWindow : Window
    {
        public PatientCardWindow()
        {
            InitializeComponent();
        }

        public PatientCardWindow(int idPatient)
        {
            InitializeComponent();

            this.FillTheCardWithPatientData(idPatient);

            this.dataGridDocumentList.SelectionChanged += DataGridDocumentList_SelectionChanged;
        }

        private void DataGridDocumentList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //this.txbInfo.Text = (sender as DataGridCell).Column
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

                    // doc type
                    var documentsOfTheCurrentPatient
                        = (
                        from doc in db.MedicalDocs
                        join docType in db.MedicalDocTypes
                        on doc.idMedicalDocType equals docType.Id
                        where doc.idPacient == idPatient
                        select new { doc.Id, DocumentType = docType.Name, doc.Name }
                        //select doc
                        //select new { DocumentType = docType.Name, doc.Name, doc.Info }
                        )
                        .ToList();

                    this.dataGridDocumentList.ItemsSource 
                        = documentsOfTheCurrentPatient
                        .Select( x => new { x.DocumentType, x.Name })
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
