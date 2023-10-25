using DAL;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using System.Windows.Controls;

namespace Controller
{
    public class MainController
    {

        DAL.DbCommunication dal = new();


        public MainController()
        {
            this.dal = dal;

        }


        public int addCounty(String county)
        {
            int success = dal.AddCounty(county);
            return success;
        }

        public DataTable getAllCounties()
        {
            DataTable dt = dal.GetAllCounties();
            return dt;
        }

        public DataTable getAllProposals()
        {
            DataTable dt = dal.GetAllProposals();
            return dt;
        }

        public int deleteCounty(string county)
        {
            int success = dal.DeleteCounty(county);
            return success;
        }

        public void CreateProposal(string countyName, string info)
        {
            // Generate a unique Proposal ID
            string uniqueProposalID = GenerateUniqueProposalForCounty(countyName);

            // Call the DAL method to create a new proposal with the generated ID
            dal.CreateProposal(countyName, uniqueProposalID, info);
        }

        public string GenerateUniqueProposalForCounty(string countyName)
        {
            DataTable dt = dal.GetAllProposals();  // Assuming dal is an instance of your data access layer class
            int maxProposalValue = 0;

            // Iterate through the rows to find the maximum Proposal value for the given county
            foreach (DataRow row in dt.Rows)
            {
                if (row["CountyName"].ToString() == countyName)
                {
                    int currentProposalValue;
                    if (int.TryParse(row["Proposal"].ToString(), out currentProposalValue))
                    {
                        maxProposalValue = Math.Max(maxProposalValue, currentProposalValue);
                    }
                }
            }

            // Return the next available Proposal value for the county
            return (maxProposalValue + 1).ToString();
        }

        public int DeleteProposal(string countyName, string proposal)
        {
            int success = dal.DeleteProposal(countyName, proposal);
            return success;
        }

        public int EditProposal(string countyName, string proposal, string info)
        {
            int success = dal.EditProposal(countyName, proposal, info);
            return success;
        }


    }
}
