using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using AzureTablesModel;
using Microsoft.Azure;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using Microsoft.WindowsAzure.Storage.Table;

namespace AzureTablesBusiness
{
    public class AzureTablesBusiness
    {
        public List<CustomerEntity> GetAllCustomers(string tableName)
        {
            var table = GetTableReference(tableName);

            //var returnList = new List<CustomerEntity>();

            // Initialize a default TableQuery to retrieve all the entities in the table.
            TableQuery<CustomerEntity> tableQuery = new TableQuery<CustomerEntity>();

            // Retrieve a segment (up to 1,000 entities).
            var tableQueryResult = table.ExecuteQuery(tableQuery);

            List<CustomerEntity> returnlist = new List<CustomerEntity>(tableQueryResult);

            return returnlist;
        }

        //Only return specific properties
        public List<string> GetCustomerEmails(string tableName)
        {
            CloudTable table = GetTableReference(tableName);
            // Define the query, and select only the Email property. 
            
            //Specify you just want a property called email
            TableQuery<DynamicTableEntity> projectionQuery = new TableQuery<DynamicTableEntity>().Select(new string[] { "Email" });

            //Run a query for specific data
            //TableQuery<DynamicTableEntity> projectionQuery = new TableQuery<DynamicTableEntity>().Where(TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, "Cassim"));

            // Define an entity resolver to work with the entity after retrieval. 
            EntityResolver<string> resolver = (pk, rk, ts, props, etag) => props.ContainsKey("Email") ? props["Email"].StringValue : null;

            var returnlist = table.ExecuteQuery(projectionQuery, resolver, null, null);

            return new List<string>(returnlist);
        }

        //Get all customers but run Async
        public async Task<List<CustomerEntity>> GetAllCustomersAsync(string tableName)
        {
            var table = GetTableReference(tableName);

            var returnList = new List<CustomerEntity>();

            // Initialize a default TableQuery to retrieve all the entities in the table.
            TableQuery<CustomerEntity> tableQuery = new TableQuery<CustomerEntity>();

            // Initialize the continuation token to null to start from the beginning of the table.
            TableContinuationToken continuationToken = null;

            do
            {
                // Retrieve a segment (up to 1,000 entities).
                TableQuerySegment<CustomerEntity> tableQueryResult =
                    await table.ExecuteQuerySegmentedAsync(tableQuery, continuationToken);

                // Assign the new continuation token to tell the service where to
                // continue on the next iteration (or null if it has reached the end).
                continuationToken = tableQueryResult.ContinuationToken;

                List<CustomerEntity> alist = new List<CustomerEntity>(tableQueryResult.Results.ToArray());
                returnList.AddRange(alist);              

                // Loop until a null continuation token is received, indicating the end of the table.
            } while (continuationToken != null);

            return await Task.FromResult(returnList);
        }

        public void InsertCustomer(string tableName, CustomerEntity customerEntity)
        {
            var table = GetTableReference(tableName);

            // Create the TableOperation object that inserts the customer entity.
            TableOperation insertOperation = TableOperation.Insert(customerEntity);

            // Execute the insert operation.
            table.Execute(insertOperation);
        }

        private static CloudTable GetTableReference(string tablename)
        {
            // Retrieve storage account from connection string.
            var storageAccount = CloudStorageAccount.Parse(
                CloudConfigurationManager.GetSetting("StorageConnectionString"));

            // Create the blob client.
            CloudTableClient tableClient = storageAccount.CreateCloudTableClient();

            // Retrieve reference to a previously created container.
            var table = tableClient.GetTableReference(tablename);

            // Create the table if it doesn't exist.
            table.CreateIfNotExists();

            return table;
        }
    }


}
