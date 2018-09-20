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
        int IdMedicalDoc;
        public AddEditDocument()
        {
            InitializeComponent();
            InitAdd();
        }

        private void InitAdd()
        {
            using (DataModel db = new DataModel())
            {
                foreach (var item in db.MedicalDocTypes.ToList())
                {
                    ComboType.Items.Add(item.Name);
                }
            }
        }

        public AddEditDocument(int _IdMedicalDoc)
        {
            IdMedicalDoc = _IdMedicalDoc;
            InitializeComponent();
            InitEdit(); 
        }

        private void InitEdit()
        {
            using (DataModel db = new DataModel())
            {
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
    }
}
