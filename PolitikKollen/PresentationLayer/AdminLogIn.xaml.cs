using System;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
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
    /// Interaction logic for AdminLogIn.xaml
    /// </summary>
    public partial class AdminLogIn : Window
    {

        Controller.MainController controller;

        public AdminLogIn()
        {
            InitializeComponent();
            controller = new Controller.MainController();
        }

        private void btnAdminSubmit_Click(object sender, RoutedEventArgs e)
        {
            string AdminName = txtBoxAdminName.Text;
            string AdminBankId = txtBoxAdminBankId.Text;

            // Check if the fields are empty or whitespace
            if (string.IsNullOrWhiteSpace(AdminName) || string.IsNullOrWhiteSpace(AdminBankId))
            {
                MessageBox.Show("Both AdminName and AdminBankId fields must be filled out.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            // Here, I'm assuming there's a different method in the controller for verifying admin logins.
            // If you don't have this differentiation and it's the same method as for users, then you can adjust as needed.
            byte[] AdminFound = controller.ProcessLogin(AdminName, AdminBankId, "Admin");

            if (AdminFound == null)
            {
                // Admin not found or login failed.
                MessageBox.Show("Admin doesn't exist!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            else
            {
                County county = new County(AdminFound);
                county.Show();
            }
        }

        private void btnCreateAdmin_Click(object sender, RoutedEventArgs e)
        {
            string AdminName = txtBoxAdminName.Text;
            string AdminBankId = txtBoxAdminBankId.Text;

            // Check if the fields are empty or whitespace
            if (string.IsNullOrWhiteSpace(AdminName) || string.IsNullOrWhiteSpace(AdminBankId))
            {
                MessageBox.Show("Both AdminName and AdminBankId fields must be filled out.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            else
            {
                // If all fields are filled in, proceed to create the new citizen
                controller.CreateNewAdmin(AdminName, AdminBankId);

                byte[] AdminFound = controller.ProcessLogin(AdminName, AdminBankId, "Admin");
                
            
                County county = new County(AdminFound);
                county.Show();
            }
        }
    }
}
