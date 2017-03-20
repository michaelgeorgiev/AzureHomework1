using Microsoft.Azure; // Namespace for CloudConfigurationManager
using Microsoft.WindowsAzure.Storage; // Namespace for CloudStorageAccount
using Microsoft.WindowsAzure.Storage.Blob; // Namespace for Blob storage types
using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Threading;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TableStorageHandsOn;

namespace TableStorage
{
    class Program
    {
        static void Main(string[] args)
        {
            // 01 - Connect to your azure storage account
            string connectionString = ConfigurationManager.AppSettings["StorageConnectionString"];
            CloudStorageAccount account = CloudStorageAccount.Parse(connectionString);
            CloudTableClient tableClient = account.CreateCloudTableClient();

            // 02 - Create a table called "customers"
            CloudTable table = tableClient.GetTableReference("customers");
            table.CreateIfNotExists();

            // 03 - Insert single entity (instance of the CustomerEntity class) into the table
            CustomerEntity customer = new CustomerEntity("Bulgaria", "Ivan");
            customer.BirthDate = DateTime.Today;
            TableOperation insert = TableOperation.Insert(customer);
            table.Execute(insert);

            // 04 - Insert two additional CustomerEntity objects using batching (use PartitionKey "Netherlands")
            CustomerEntity customer1 = new CustomerEntity("Netherlands", "Holan");
            customer1.BirthDate = DateTime.Today;
            CustomerEntity customer2 = new CustomerEntity("Netherlands", "Lohan");
            customer2.BirthDate = DateTime.Today;

            // Create batch operation
            TableBatchOperation operation = new TableBatchOperation();
            operation.Insert(customer1);
            operation.Insert(customer2);
            table.ExecuteBatch(operation);

            // 05 - Retrieve one of the entities using TableOperation.Retrieve and print its PartitionKey using Console.WriteLine()
            TableOperation tops = TableOperation.Retrieve<CustomerEntity>("Netherlands", "Lohan");
            TableResult tRes = table.Execute(tops);
            var res = tRes.Result as CustomerEntity;
            Console.WriteLine(res.PartitionKey);

            // 06 - Retrieve all entities with PartitionKey "Netherlands" using TableQuery and print their RowKey using Console.WriteLine()
            TableQuery<CustomerEntity> query = new TableQuery<CustomerEntity>().Where(TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, "Netherlands"));

            foreach (CustomerEntity element in table.ExecuteQuery(query))
            {
                Console.WriteLine("Name:" + element.RowKey);
            }

            // 07 - Delete one of the entities from the table
            tops = TableOperation.Retrieve<CustomerEntity>("Netherlands", "Lohan");
            tRes = table.Execute(tops);
            CustomerEntity deleteEntity = (CustomerEntity)tRes.Result;
            // Delete entity
            TableOperation delete = TableOperation.Delete(deleteEntity);
            table.Execute(delete);

            // 08 - Insert a new entity into "customers" using DynamicTableEntity instead of CustomerEntity
            DynamicTableEntity dynamicEntity = new DynamicTableEntity("BG", "Misho");
            dynamicEntity.Properties.Add("Standing", new EntityProperty("sophomore"));
            //Insert
            TableOperation tOperation = TableOperation.Insert(dynamicEntity);
            table.Execute(tOperation);
            //Get
            tops = TableOperation.Retrieve("BG", "Misho");
            var rezultat = (DynamicTableEntity)table.Execute(tops).Result;

            Console.WriteLine("Dynamic:" + rezultat.PartitionKey);

        }
    }
}
