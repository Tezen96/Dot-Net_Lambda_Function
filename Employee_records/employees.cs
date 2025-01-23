using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Amazon.DynamoDBv2.DataModel;

namespace Employee_records
{
    [DynamoDBTable("employees")]
    public class Employees
    {
        [DynamoDBHashKey("id")]
        public Int32? Id { get; set; }

        [DynamoDBProperty("First_name")]
        public string? FirstName { get; set; }

        [DynamoDBProperty("Last_name")]
        public string? LastName { get; set; }

        [DynamoDBProperty("Address")]
        public string? Address { get; set; }
    }
}