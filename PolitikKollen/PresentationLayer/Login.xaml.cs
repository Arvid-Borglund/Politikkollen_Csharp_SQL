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

using System;
using System.Windows;

namespace PresentationLayer
{
    /// <summary>
    /// Interaction logic for Login.xaml
    /// </summary>
    public partial class Login : Window
    {
        Controller.MainController controller;

        public Login()
        {
            InitializeComponent();
            controller = new Controller.MainController();
            cBoxCounty.ItemsSource = controller.getAllCounties().DefaultView;
            cBoxCounty.DisplayMemberPath = "CountyName";

        }


        // This method is invoked when the "Submit" button is clicked.
        private void OnSubmitClicked(object sender, RoutedEventArgs e)
        {
            // Capture the UserName and BankId from the text boxes.
            string UserName = UserNameTextBox.Text;
            string BankId = BankIdTextBox.Text;

            // Check if the fields are empty or whitespace
            if (string.IsNullOrWhiteSpace(UserName) || string.IsNullOrWhiteSpace(BankId))
            {
                MessageBox.Show("Both UserName and BankId fields must be filled out.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            byte[] UserFound = controller.ProcessLogin(UserName, BankId, "User");

            if (UserFound == null)
            {
                // Username not found or login failed. Show the GroupBox to create a new account.
                gBoxNewUserPromt.Visibility = Visibility.Visible;

                txtBoxNewUser.Text = UserName;
                txtBoxNewBankId.Text = BankId;
                cBoxCounty.SelectedIndex = 0;
                MessageBox.Show("User doesn't exist, please create a new user!", "Notification", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            else
            {
                // Successful login. Open the Citizen window and hide the GroupBox.
                gBoxNewUserPromt.Visibility = Visibility.Collapsed;
                
                Citizen citizen = new Citizen(UserFound);
                citizen.Show();
            }
        }




        private void btnSubmitNewUser_Click(object sender, RoutedEventArgs e)
        {
            string user = txtBoxNewUser.Text;
            string bankID = txtBoxNewBankId.Text;
            string county = cBoxCounty.Text;

            // Check if any of the fields are empty
            if (string.IsNullOrEmpty(user) || string.IsNullOrEmpty(bankID) || string.IsNullOrEmpty(county))
            {
                MessageBox.Show("All fields must be filled in.", "Empty Fields", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // If all fields are filled in, proceed to create the new citizen
            controller.CreateNewCitizen(user, bankID, county);

            byte[] UserHash = controller.ProcessLogin(user, bankID, "User");
            gBoxNewUserPromt.Visibility = Visibility.Collapsed;
            Citizen citizen = new Citizen(UserHash);
            citizen.Show();
        }
    }
}

