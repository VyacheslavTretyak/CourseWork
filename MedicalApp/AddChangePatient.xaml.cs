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
    /// Interaction logic for AddChangePatient.xaml
    /// </summary>
    public partial class AddChangePatient : Window
    {
        public AddChangePatient()
        {
            InitializeComponent();
        }
    }
}
