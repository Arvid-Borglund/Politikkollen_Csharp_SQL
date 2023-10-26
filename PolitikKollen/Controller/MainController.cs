using DAL;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics.Metrics;
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

        public byte[] ProcessLogin(string userName, string bankId)
        {
            // Step 1: Fetch the salt associated with the username from the DAL
            byte[] salt = dal.GetSaltByUserName(userName);

            // Step 2: Check if salt is null (meaning the username is not found in the database)
            if (salt == null)
            {
                // Handle this case, e.g., show an error message, log the attempt, etc.
                Console.WriteLine("Username not found.");
                return null;
            }

            // Step 3: Combine the salt with the entered BankId and perform hashing to generate the BankIdHash
            byte[] bankIdHash = HashHelper.GenerateHash(bankId, salt);  

            // Step 4: Use the generated BankIdHash to validate the user, continue to the next step, etc.
            bool exists;
            string message;
            exists = dal.CheckCitizenExistence(bankIdHash, out message);

            if (exists)
            {
                return bankIdHash;
                
                //DataTable dt = dal.GetProposalDataAsDataTable(bankIdHash, dal.GetCountyByBankIdHash(bankIdHash));
                // Continue to the next step in your application logic, e.g., opening the main window
            }
            else
            {
                Console.WriteLine(message); // This will print the message like "BankIdHash not found. Would you like to add a new citizen?"
                return null;                            // Handle this case, e.g., show an option to register as a new citizen
            }
        }

        public DataTable getCitizenData(byte[] bankIdHash)
        {
            String county = dal.GetCountyByBankIdHash(bankIdHash);

           
            DataTable dt = dal.GetProposalDataAsDataTable(bankIdHash, county);
            return dt;
        }

        public void CreateNewCitizen(string userName, string bankId, String County)
        {
            // Step 1: Generate a new salt
            byte[] salt = utilities.SaltHelper.GenerateSalt(); // Assume GenerateSalt is a method that returns a byte array

            // Step 2: Hash the BankId with the generated salt
            byte[] bankIdHash = HashHelper.GenerateHash(bankId, salt); // Assume HashHelper.GenerateHash is a method that returns a hashed byte array

            // Step 3: Create the new citizen record in the database
            dal.CreateUser(bankIdHash, userName, salt, County); // Assume CreateUser is a method that adds a new record to your database
        }

        public String GetCitizenName(Byte[] hash)
        {
            String citizenName = dal.GetCitizenNameByUserHash(hash);
            return citizenName;
        }




    }
}
