using Controller;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics.Metrics;
using System.Linq;
using System.Runtime.CompilerServices;
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
    /// Interaction logic for County.xaml
    /// </summary>
    public partial class County : Window
    {

        private Controller.MainController controller;
        byte[] Id;
        String AdminName;

        public County(byte[] Admin)
        {
            InitializeComponent();
            controller = new Controller.MainController();
            this.Id = Admin;
            pageStartup();
        }

        private void pageStartup()
        {
            AdminName = controller.getAdminData(Id);
            
            lblAdminName.Content = $"Welcome ({AdminName})";
              
            UpdateDataGridView();
        }


        private void btnAddCounty_Click(object sender, RoutedEventArgs e)
        {
            string county = txtBoxAddCounty.Text;
            int success = controller.addCounty(county);

            if (success == 0)
            {
                lblOutPut.Content = $"County with {county} name already exists.";
            }
            else if (success == 1)
            {
                lblOutPut.Content = $"{county} added.";
            }
            else
            {
                lblOutPut.Content = "Unexpected return value.";
            }
            UpdateDataGridView();

        }

        private void UpdateDataGridView()
        {
            // Call the controller's method to get all counties as a DataTable
            DataTable countiesDataTable = controller.getAllCounties();
            DataTable proposalsDataTable = controller.getAllProposals();

            dGrid.ItemsSource = countiesDataTable.DefaultView;
            dGridProposals.ItemsSource = proposalsDataTable.DefaultView;
            
        }

        private void DeleteCountyButton_Click(object sender, RoutedEventArgs e)
        {
            Button button = (Button)sender;
            DataRowView dataRow = (DataRowView)button.DataContext;

            string countyName = dataRow["CountyName"].ToString();

            MessageBoxResult result = MessageBox.Show($"Are you sure you want to delete {countyName}?",
                                          "Confirmation",
                                          MessageBoxButton.YesNo,
                                          MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                int success = controller.deleteCounty(countyName);

                if (success == 0)
                {
                    lblOutPut.Content = $"There was a problem with deleting {countyName}";
                }
                else if (success == 1)
                {
                    lblOutPut.Content = $"{countyName} deleted.";
                }
                else
                {
                    lblOutPut.Content = "Unexpected return value.";
                }
                UpdateDataGridView();
            }

        }

        private void DeleteProposalButton_Click(object sender, RoutedEventArgs e)
        {
            Button button = (Button)sender;
            DataRowView dataRow = (DataRowView)button.DataContext;

            string countyName = dataRow["CountyName"].ToString();
            string proposal = dataRow["Proposal"].ToString();

            MessageBoxResult result = MessageBox.Show($"Are you sure you want to delete the proposal '{proposal}' for {countyName}?",
                                          "Confirmation",
                                          MessageBoxButton.YesNo,
                                          MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                int success = controller.DeleteProposal(countyName, proposal);

                if (success == 0)
                {
                    lblOutPut.Content = $"There was a problem with deleting the proposal '{proposal}' for {countyName}";
                }
                else if (success == 1)
                {
                    lblOutPut.Content = $"Proposal '{proposal}' for {countyName} deleted.";
                }
                else
                {
                    lblOutPut.Content = "Unexpected return value.";
                }
                UpdateDataGridView();
            }
        }

        /*
        private void EditButton_Click(object sender, RoutedEventArgs e)
        {
            Button button = (Button)sender;
            DataRowView dataRow = (DataRowView)button.DataContext;

            string countyName = dataRow["CountyName"].ToString();

            MessageBoxResult result = MessageBox.Show($"Are you sure you want to delete {countyName}?",
                                          "Confirmation",
                                          MessageBoxButton.YesNo,
                                          MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                int success = controller.deleteCounty(countyName);

                if (success == 0)
                {
                    lblOutPut.Content = $"There was a problem with deleting {countyName}";
                }
                else if (success == 1)
                {
                    lblOutPut.Content = $"{countyName} deleted.";
                }
                else
                {
                    lblOutPut.Content = "Unexpected return value.";
                }
                UpdateDataGridView();
            }

        }
        */

        private void btnAddProposal_Click(object sender, RoutedEventArgs e)
        {
            // Check if a cell is selected in dGrid
            if (dGrid.SelectedCells.Count == 0)
            {
                MessageBox.Show("Please select a cell in the grid.");
                return;
            }

            // Get the value from the selected cell
            var cellInfo = dGrid.SelectedCells[0];
            var content = cellInfo.Column.GetCellContent(cellInfo.Item);
            string selectedCellValue = (content as TextBlock)?.Text;
            string ProposalName = txtBoxProposalName.Text;
            string ProposalInfo = txtBoxProposalInfo.Text;

            // Check if txtBoxProposalInfo has a value
            if (string.IsNullOrWhiteSpace(txtBoxProposalInfo.Text))
            {
                MessageBox.Show("Please enter a value in the text box.");
                return;
            }

            // Call the controller.CreateProposal method with the values
            int success = controller.CreateProposal(selectedCellValue, ProposalName, ProposalInfo);

            if (success == 1)
            {
                // Proposal creation was successful
                MessageBox.Show("Proposal created successfully.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else if (success == 0)
            {
                // Proposal for the county already exists
                MessageBox.Show("A proposal for the selected county already exists.", "Duplicate Proposal", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            else
            {
                // Handle other possible return values if needed
                MessageBox.Show("An unexpected error occurred while creating the proposal.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            UpdateDataGridView();
        }



        private void EditProposalButton_Click(object sender, RoutedEventArgs e)
        {
            // Get the selected row from dGridProposal
            DataRowView selectedRow = dGridProposals.SelectedItem as DataRowView;

            if (selectedRow != null)
            {
                string countyName = selectedRow["CountyName"].ToString();
                string proposal = selectedRow["Proposal"].ToString();
                string newInfo = txtBoxProposalInfo.Text;

                if (string.IsNullOrWhiteSpace(newInfo))
                {
                    MessageBox.Show("Info field cannot be empty.", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                // Call the EditProposal method from controller with the obtained values
                int result = controller.EditProposal(countyName, proposal, newInfo);
                

                if (result == 0)
                {
                    MessageBox.Show($"There was a problem editing the proposal for {countyName}.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                else if (result == 1)
                {
                    MessageBox.Show($"Proposal for {countyName} edited successfully.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    MessageBox.Show("Unexpected return value.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            else
            {
                MessageBox.Show("Please select a proposal from the list to edit.", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            UpdateDataGridView();
        }

    }//PROPOSAL NAMN

}
