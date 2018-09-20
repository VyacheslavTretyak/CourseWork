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
    /// Логика взаимодействия для AddEditDocument.xaml
    /// </summary>
    public partial class AddEditDocument : Window
    {
        int IdPacient;
        int IdMedicalDoc;
        public AddEditDocument(int _IdPacient)
        {
            InitializeComponent();
            IdPacient = _IdPacient;
            InitAdd();
        }

        private void InitAdd()
        {
            using (DataModel db = new DataModel())
            {
                Pacient pac = new Pacient();
                pac = db.Pacients.Where(a => a.Id == IdPacient).FirstOrDefault();
                if (pac==null)
                {
                    MessageBox.Show("Pacient not found");
                    Close();
                }
                foreach (var item in db.MedicalDocTypes.ToList())
                {
                    ComboType.Items.Add(item.Name);
                }
            }
        }

        public AddEditDocument(int _IdPacient, int _IdMedicalDoc)
        {
            IdPacient = _IdPacient;
            IdMedicalDoc = _IdMedicalDoc;
            InitializeComponent();
            InitEdit(); 
        }

        private void InitEdit()
        {
            using (DataModel db = new DataModel())
            {
                Pacient pac = new Pacient();
                pac = db.Pacients.Where(a => a.Id == IdPacient).FirstOrDefault();
                if (pac == null)
                {
                    MessageBox.Show("Pacient not found");
                    Close();
                }
                foreach (var item in db.MedicalDocTypes.ToList())
                {
                    ComboType.Items.Add(item.Name);
                }
                MedicalDoc medicalDocType = new MedicalDoc();
                medicalDocType = db.MedicalDocs.Where(a => a.Id == IdMedicalDoc).FirstOrDefault();
                if (medicalDocType == null)
                {
                    MessageBox.Show("document not found");
                    Close();
                }
                foreach (var item in ComboType.Items)
                {
                    if (item.ToString() == db.MedicalDocTypes.Where(a => a.Id == medicalDocType.idMedicalDocType).FirstOrDefault().Name)
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

        private void Add_Click(object sender, RoutedEventArgs e)
        {
            DateTime? date = DateBegin.SelectedDate;
            using (DataModel db = new DataModel())
            {
                if (IdMedicalDoc==0)
                {
                    MedicalDoc medicalDoc = new MedicalDoc
                    {
                        Name = TxBxName.Text,
                        idPacient = IdPacient,
                        idMedicalDocType = db.MedicalDocTypes.Where(a => a.Name == ComboType.SelectedValue.ToString()).FirstOrDefault().Id,
                        BeginTime = (DateTime)DateBegin.SelectedDate,
                        EndTime = DateEnd.SelectedDate,
                        Info = TxBxInfo.Text
                    };
                    db.MedicalDocs.Add(medicalDoc);
                }
                else
                {
                    db.MedicalDocs.Where(x => x.Id == IdMedicalDoc).FirstOrDefault().Name = TxBxName.Text;
                    db.MedicalDocs.Where(x => x.Id == IdMedicalDoc).FirstOrDefault().idMedicalDocType = db.MedicalDocTypes.Where(a => a.Name == ComboType.SelectedValue.ToString()).FirstOrDefault().Id;
                    db.MedicalDocs.Where(x => x.Id == IdMedicalDoc).FirstOrDefault().BeginTime = (DateTime)DateBegin.SelectedDate;
                    db.MedicalDocs.Where(x => x.Id == IdMedicalDoc).FirstOrDefault().EndTime = DateEnd.SelectedDate;
                    db.MedicalDocs.Where(x => x.Id == IdMedicalDoc).FirstOrDefault().Info = TxBxInfo.Text;
                }
                db.SaveChanges();
            }
            Close();
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
        private void EnabledAdd(object sender, RoutedEventArgs e)
        {
            if (ComboType.SelectedIndex==-1||string.IsNullOrWhiteSpace(TxBxName.Text)|| string.IsNullOrWhiteSpace(TxBxInfo.Text)|| DateBegin.SelectedDate==null)
            {
                Add.IsEnabled = false;
            }
            else
            {
                Add.IsEnabled = true;
            }
            
        }
    }
}
