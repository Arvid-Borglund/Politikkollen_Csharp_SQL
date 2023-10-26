using System;
using System.Collections.Generic;
using System.Data;
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

namespace PresentationLayer
{
    /// <summary>
    /// Interaction logic for Citizen.xaml
    /// </summary>
    public partial class Citizen : Window
    {
        Controller.MainController controller;
        byte[] id;
        DataTable data;
        String userName;

        public Citizen(byte[] citizen)
        {
            InitializeComponent();

            this.id = citizen;
            controller = new Controller.MainController();
            data = controller.getCitizenData(id);
            UpdateDataGridView();
            userName = controller.GetCitizenName(id);
            txtBoxUserName.Text = userName.ToString();


        }

        private void UpdateDataGridView()
        {
            // Call the controller's method to get all counties as a DataTable



            dGridCitizen.ItemsSource = data.DefaultView;

        }



    }
}
