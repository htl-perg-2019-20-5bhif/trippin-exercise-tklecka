using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace trippin_exercise
{
    class Program
    {
        private static HttpClient client = new HttpClient();
        private static string baseuri;
        //args[0] -> file Path
        //args[1] -> api key //(S(p5yv5fq0m4dka5xvfyxal1kx))
        static async Task Main(string[] args)
        {
            baseuri = @"https://services.odata.org/TripPinRESTierService/" + args[1] + "";
            if (args.Length == 2)
            {
                await ReadFile(args[0]);
            }
            else
            {
                Console.WriteLine("Use \"Path\" \"API Key\"");
            }
        }

        public static async Task ReadFile(string filename)
        {
            string content;
            try
            {
                content = await File.ReadAllTextAsync(filename);
                await ParseFileAsync(content);
            }
            catch (FileNotFoundException ex)
            {
                Console.WriteLine("Could not read File: " + filename);
                throw ex;
            }
        }

        public static async Task ParseFileAsync(string content)
        {
            IEnumerable<UserData> users = JsonSerializer.Deserialize<IEnumerable<UserData>>(content);
            foreach (UserData user in users)
            {
                await GetUserfromodataAsync(user);
            }
        }

        public static async Task GetUserfromodataAsync(UserData user)
        {
            var res = await client.GetAsync(baseuri + "/People('" + user.UserName + "')");
            if (!res.IsSuccessStatusCode)
            {
                await InserUserinodataAsync(new UserTripping(user));
            }
        }

        public static async Task InserUserinodataAsync(UserTripping user)
        {
            var content = new StringContent(JsonSerializer.Serialize(user), Encoding.UTF8, "application/json");
            await client.PostAsync(baseuri + "/People", content);
        }
    }
}
