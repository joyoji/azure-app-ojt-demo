using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Company.Function
{
    public static class UpdateProduct
    {
        [FunctionName("UpdateProduct")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "put", Route = "product")] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");
            try
            {
                string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
                Product productData = JsonConvert.DeserializeObject<Product>(requestBody);
                using (SqlConnection connection = new SqlConnection(Environment.GetEnvironmentVariable("DBConnectionString")))
                {
                    string queryString = @"UPDATE [dbo].[ProductInformation] SET [Product_Name] = @Product_Name,[Product_Description] = @Product_Description,[Product_Price] = @Product_Price,[Product_Quantity] = @Product_Quantity,[Category_Name] = @Category_Name WHERE [Product_ID] = @Product_ID";
                    using (SqlCommand cmd = new SqlCommand(queryString))
                    {
                        cmd.Parameters.AddWithValue("@Product_Name", productData.Product_Name);
                        cmd.Parameters.AddWithValue("@Product_Description", productData.Product_Description);
                        cmd.Parameters.AddWithValue("@Product_Price", productData.Product_Price);
                        cmd.Parameters.AddWithValue("@Product_Quantity", productData.Product_Quantity);
                        cmd.Parameters.AddWithValue("@Category_Name", productData.Category_Name);
                        cmd.Parameters.AddWithValue("@Product_ID", productData.Product_ID);
                        cmd.Connection = connection;
                        connection.Open();
                        cmd.ExecuteNonQuery();
                        connection.Close();
                    }
                }
                return new OkObjectResult(productData);
            }
            catch (Exception ex)
            {
                return new BadRequestObjectResult(ex.Message);
            }
        }
    }
}
