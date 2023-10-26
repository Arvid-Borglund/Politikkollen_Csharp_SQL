using System;
using System.Collections.Generic;
using System.Data;
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
    /// Interaction logic for Citizen.xaml
    /// </summary>
    public partial class Citizen : Window
    {
        Controller.MainController controller;
        byte[] id;
        DataTable gridData;
        String userName;
        String userCounty;

        public Citizen(byte[] citizen)
        {
            InitializeComponent();

            this.id = citizen;
            controller = new Controller.MainController();
            pageStartup();

        }

        private void pageStartup()
        {
            var citizenData = controller.GetCitizenData(id);
            userName = citizenData.CitizenName;
            userCounty = citizenData.CountyName;

            comboUserCounty.Text = userCounty;
            lblUserNameInfo.Content = $"Welcome ({userName})";
            
            UpdateDataGridView();
        }

        private void UpdateDataGridView()
        {
            // Call the controller's method to get all counties as a DataTable
            gridData = controller.getCitizenData(id, userCounty);
            dGridCitizen.ItemsSource = gridData.DefaultView;


        }

        private void EditButton_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show("Are you sure you want to change county?", "Confirm", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                // Enable the dropdown and populate the ComboBox
                comboUserCounty.IsHitTestVisible = true;
                comboUserCounty.IsEditable = false;
                comboUserCounty.ItemsSource = controller.getAllCounties().DefaultView;
                comboUserCounty.DisplayMemberPath = "CountyName";

                // Open the dropdown immediately after clicking the edit button
                comboUserCounty.IsDropDownOpen = true;
            }
        }

        private void btnDeleteUser_Click(object sender, RoutedEventArgs e)
        {
            // Display a confirmation dialog
            MessageBoxResult result = MessageBox.Show("Are you sure you want to delete the account? This action cannot be undone.", "Confirmation", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                // User confirmed, proceed with deletion
                int deletionResult = controller.deleteUser(id); // You may need to pass user-specific information to the deleteUser method

                if (deletionResult == 0)
                {
                    // Deletion was successful
                    MessageBox.Show("Account deleted successfully.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else if (deletionResult == 1)
                {
                    // Deletion failed
                    MessageBox.Show("Account deletion failed.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                else
                {
                    // Handle other possible return values if needed
                    MessageBox.Show("An unexpected error occurred during account deletion.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            else
            {
                // User canceled the deletion, no action required
            }
        }

        private void comboUserCounty_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Check if an item is selected
            if (comboUserCounty.SelectedItem != null)
            {
                // Get the selected county from the ComboBox

                // Get the string representation of the selected item
                DataRowView selectedRow = (DataRowView)comboUserCounty.SelectedItem;
                string selectedCounty = selectedRow["CountyName"].ToString();


                // Now you have the selected county as a string
                MessageBox.Show(selectedCounty);
                


                // Call the controller.editUser method and capture the result
                int result = controller.editUser(id, selectedCounty);

                // Handle the result here as needed
                if (result == 0)
                {
                    MessageBox.Show($"County changed successfully. to {selectedCounty}", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                    userCounty = selectedCounty;
                    UpdateDataGridView();
                }
                else
                {
                    MessageBox.Show("An unexpected error occurred", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void dGridCitizen_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void SaveOpinionButton_Click(object sender, RoutedEventArgs e)
        {
            Button button = (Button)sender;
            DataRowView dataRow = (DataRowView)button.DataContext;
            dGridCitizen.CommitEdit(DataGridEditingUnit.Row, true);

           


            string proposal = dataRow["Proposal"].ToString();
            
            bool voteFor = Convert.ToBoolean(dataRow["VoteFor"]);    // Assuming VoteFor is stored as bool or bit in the DB
            bool voteAgainst = Convert.ToBoolean(dataRow["VoteAgainst"]);  // Assuming VoteAgainst is stored as bool or bit in the DB

            MessageBox.Show($"{proposal}, {id}, {userCounty}, {voteFor}, {voteAgainst}");

            
            // Check to ensure only one checkbox is checked
            if (voteFor && voteAgainst)
            {
                MessageBox.Show("You cannot vote both for and against a proposal. Please select only one.", "Invalid Selection", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            

            int success = controller.saveOpinion(id, userCounty, proposal, voteFor, voteAgainst);

            if (success == 0)
            {
                MessageBox.Show("Opinion saved successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else if (success == 1)
            {
                MessageBox.Show("Failed to save the opinion.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            else
            {
                MessageBox.Show("Unexpected return value.", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            UpdateDataGridView(); 
            
        }




    }
}
