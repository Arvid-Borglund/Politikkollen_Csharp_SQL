using DAL;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
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

        public int deleteCounty(String county)
        {
            int success = dal.DeleteCounty(county);
            return success;
        }

        public int CreateProposal(String countyName, String ProposalName, String info)
        {
           
           
            int succces = dal.CreateProposal(countyName, ProposalName, info);
            return succces;

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

        public byte[] ProcessLogin(string userName, string bankId, string type)
        {
            // Step 1: Fetch the salt associated with the username from the DAL
            byte[] salt = dal.GetSaltByUserName(userName, type);

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
            exists = dal.CheckCitizenExistence(bankIdHash, type, out message);

            if (exists)
            {
                return bankIdHash;

            }
            else
            {
                Console.WriteLine(message); // This will print the message like "BankIdHash not found. Would you like to add a new citizen?"
                return null;                            // Handle this case, e.g., show an option to register as a new citizen
            }
        }

        public DataTable getCitizenData(byte[] bankIdHash, String county)
        {
            //String county = dal.GetCountyByBankIdHash(bankIdHash);

           
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

        public void CreateNewCitizen(string userName, string bankId)
        {
            // Step 1: Generate a new salt
            byte[] salt = utilities.SaltHelper.GenerateSalt(); // Assume GenerateSalt is a method that returns a byte array

            // Step 2: Hash the BankId with the generated salt
            byte[] bankIdHash = HashHelper.GenerateHash(bankId, salt); // Assume HashHelper.GenerateHash is a method that returns a hashed byte array

            // Step 3: Create the new citizen record in the database
            dal.CreateAdmin(bankIdHash, userName, salt); 
        }

        public (string CitizenName, string CountyName) GetCitizenData(byte[] hash)
        {
            // Assuming your DAL method returns a tuple (CitizenName, CountyName)
            var result = dal.GetCitizenDataByUserHash(hash);

            return result; // This will contain both CitizenName and CountyName
        }

        public String getAdminData(byte[] hash)
        {
            String result = dal.GetAdminDataByUserHash(hash);
            return result;
        }

        public int deleteUser(byte[] id)
        {

            int success = dal.DeleteUser(id);
            return success;
        }

        public void CreateNewAdmin(string adminName, string adminBankId)
        {
            // Step 1: Generate a new salt
            byte[] salt = utilities.SaltHelper.GenerateSalt(); // Assume GenerateSalt is a method that returns a byte array

            // Step 2: Hash the BankId with the generated salt
            byte[] bankIdHash = HashHelper.GenerateHash(adminBankId, salt); // Assume HashHelper.GenerateHash is a method that returns a hashed byte array

            // Step 3: Create the new admin record in the database
            dal.CreateAdmin(bankIdHash, adminName, salt); // Call the CreateAdmin method in your DAL
        }

        public int editUser(byte[] id, string county)
        {
            int success = dal.UpdateCountyNameByBankIDHash(id, county);
            return success;
        }

        public int saveOpinion(byte[] id, string proposal, string county, bool voteFor, bool voteAgainst)
        {
            int intVoteFor = voteFor ? 1 : 0;
            int intVoteAgainst = voteAgainst ? 1 : 0;

            

            int success = dal.SaveOpinion(id, county, proposal, intVoteFor, intVoteAgainst);
            return success;
        }




    }
}
