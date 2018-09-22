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
    /// Interaction logic for AddChangePatient.xaml
    /// </summary>
    public partial class AddChangePatient : Window
    {
        public AddChangePatient()
        {
            InitializeComponent();
            //commit
        }

        private void RadioButton_Checked(object sender, RoutedEventArgs e)
        {

        }

        private void btnAddEdit_Click(object sender, RoutedEventArgs e)
        {
            using (DataModel db = new DataModel()) 
            {
                
            }
            this.Close();
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
		public AddChangePatient(Pacient patient) : this()
		{
			// TODO fill all fields from received patient object
		}
    }
}
