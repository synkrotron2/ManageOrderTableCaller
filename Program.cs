using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Threading.Tasks;
using System.Net;
using System.IO;
using Newtonsoft.Json.Converters; // Must be added with Nuget to project

// Referenced but not copied : https://docs.microsoft.com/en-us/aspnet/web-api/overview/advanced/calling-a-web-api-from-a-net-client
// GW Corona studies 20200429
// Testprogram to call REST api built with Azure functions in ManageOrderTable project. For local running only in this version with table storage in Azure
// Runs on both Linux and Windows

namespace OrderCaller
{
    class Program
    {
        private static readonly HttpClient client = new HttpClient();
        private static string OrderUrl = "http://localhost:7071/api/order";
        

        static async Task Main(string[] args) // Main Console program
        {
        
        int d = 1;
        while ( d < 8)
        {
            Console.WriteLine("Chose function:");  // Obs run with dotnet run under bash prompt in terminal
            Console.WriteLine("1. Get All orders");
            Console.WriteLine("2. Get order by id");
            Console.WriteLine("3. Create new order");
            Console.WriteLine("4. Update an order");
            Console.WriteLine("5. Delete an order");
            Console.WriteLine("9. Exit");
 
            string input =Console.ReadLine();

            if (int.TryParse(input, out d))
             {
                Console.WriteLine("You entered alternative: " + d.ToString() );
              }
             else
             {
               d=9;
               Console.WriteLine("Wrong input!");               
             }
            switch(d) 
            {
            case 1:
               await getAllOrders(); 
                break;
            case 2:
                await getOrderByid();
                break;
            case 3:
                await createNewOrder();
                break;
             case 4:
                await updateOrder();
                break;
             case 5:
                await deleteOrder();
                break; 
            case 9:
                 Environment.Exit(0);
                break ;
           
            }
        }
       
        }
         private static async Task getAllOrders()
            {
 
            var orders = await ProcessOrders();

            if (orders != null )
            {
                foreach (var order in orders)
                {
                    Console.WriteLine(order.Id);
                    Console.WriteLine(order.CreatedTime);
                    Console.WriteLine(order.OrderDescription);
                    Console.WriteLine(order.IsCompleted);
                    Console.WriteLine();
                }
            }
        }
          private static async Task getOrderByid()
            {
            Console.WriteLine("Enter order id:");               
            string id =Console.ReadLine(); 

            var order = await ProcessOrder(id);

            if (order != null )
            {
                
                Console.WriteLine(order.Id);
                Console.WriteLine(order.CreatedTime);
                Console.WriteLine(order.OrderDescription);
                Console.WriteLine(order.IsCompleted);
                Console.WriteLine();
                
            }
        }
  
         private static async Task createNewOrder()
        {
            Console.WriteLine("Enter new order description");               
            string OrderDescription_in =Console.ReadLine(); 

                Order order = new Order
                {
                    OrderDescription = OrderDescription_in,
                    IsCompleted = false
                };
            var message = await CreateNewOrderAsync(order);

            if (message.IsSuccessStatusCode)
                    Console.WriteLine("New Order created"); 
            else
                    Console.WriteLine("Order creation failed");    
        }
    private static async Task updateOrder()
            {
            Console.WriteLine("Enter order id to update:");               
            string id =Console.ReadLine(); 

            var order = await ProcessOrder(id);

            if (order != null )
            {
                
                Console.WriteLine(order.Id);
                Console.WriteLine(order.CreatedTime);
                Console.WriteLine(order.OrderDescription);
                Console.WriteLine(order.IsCompleted);
                Console.WriteLine();
            }
            Console.WriteLine("Enter updated order description");               
            string OrderDescription_in =Console.ReadLine(); 

             Order updatedorder = new Order
                {
                    Id = order.Id,
                    CreatedTime = order.CreatedTime,
                    OrderDescription = OrderDescription_in,
                    IsCompleted = true
                };

                var message = await updateOrderAsync(updatedorder);

            if (message.IsSuccessStatusCode)
                    Console.WriteLine("Order updated"); 
            else
                    Console.WriteLine("Order update failed"); 
            }

            private static async Task deleteOrder()
            {
            Console.WriteLine("Enter order id to delete:");               
            string id =Console.ReadLine(); 

            var order = await ProcessOrder(id);

            if (order != null )
            {
               Console.WriteLine("Order to being deleted:9");   
                Console.WriteLine(order.Id);
                Console.WriteLine(order.CreatedTime);
                Console.WriteLine(order.OrderDescription);
                Console.WriteLine(order.IsCompleted);
                Console.WriteLine();
            }

                var message = await deleteOrderAsync(id);

            if (message.IsSuccessStatusCode)
                    Console.WriteLine("Order deleted"); 
            else
                    Console.WriteLine("Order delete failed"); 
            }

        private static async Task<List<Order>> ProcessOrders() // Get all orders
        {
           client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));     
            
            var streamTask = client.GetStreamAsync(OrderUrl);
        
            try
            {
            
                var orders = await JsonSerializer.DeserializeAsync<List<Order>>(await streamTask);
                return orders;
            }
             catch (HttpRequestException e)
            {
                Console.WriteLine("Error calling Url: "+ OrderUrl+" Error:"+e.Message);
                return null;
            }
        }

          private static async Task <Order>ProcessOrder(string id) // Get single Order
        {
                
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));
            
            var streamTask = client.GetStreamAsync(OrderUrl+"/"+id);
        
            try
            {
            
                var order = await JsonSerializer.DeserializeAsync<Order>(await streamTask);
                return order;
            }
             catch (HttpRequestException e)
            {
                Console.WriteLine("Error calling Url: "+ OrderUrl+" Error:"+e.Message);
                return null;
            }
        }

      static async Task<HttpResponseMessage> CreateNewOrderAsync(Order order) // Create a new order
        {
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, OrderUrl);

            string json =  Newtonsoft.Json.JsonConvert.SerializeObject(order);

            request.Content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");

            HttpClient http = new HttpClient();
            HttpResponseMessage response = await http.SendAsync(request);
 
            return response;              
        }

              static async Task<HttpResponseMessage> updateOrderAsync(Order order) //Update an order
        {
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Put, OrderUrl+"/"+order.Id);

            string json =  Newtonsoft.Json.JsonConvert.SerializeObject(order);

            request.Content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");

            HttpClient http = new HttpClient();
            HttpResponseMessage response = await http.SendAsync(request);
 
            return response;              
        }
            static async Task<HttpResponseMessage> deleteOrderAsync(string id) // Delete and order
        {
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Delete, OrderUrl+"/"+id);

            HttpClient http = new HttpClient();
            HttpResponseMessage response = await http.SendAsync(request);
 
            return response;              
        }
        
    }
}
