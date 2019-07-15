using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.IO;




namespace StarWarsFilms
{
    class Program
    {
        private static HttpClient apiClient;

        static async Task Main(string[] args)
        {
            InitializeClient();
            string lookUpAnother = "";

            do
            {
                Console.Write("Pick a number to look up different Star Wars Characters: ");
                string idText = Console.ReadLine();

                try
                {
                    CharacterModel person = await GetStarWarsCharacter(idText);

                    Console.WriteLine($"{ person.name } is a { person.gender }. ");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error: ex.Message");
                }

                Console.Write("Do you want to look up another character (yes/no): ");
                lookUpAnother = Console.ReadLine();

                Console.Clear();
            } while (lookUpAnother.ToLower() == "yes");
            Console.ReadLine();
        }

        private static async Task<CharacterModel> GetStarWarsCharacter(string id)
        {
            string url = $"https://swapi.co/api/people/{ id }/";

            using (HttpResponseMessage response = await apiClient.GetAsync(url))
            {
                if (response.IsSuccessStatusCode)
                {
                    CharacterModel output = await response.Content.ReadAsAsync<CharacterModel>();

                    return output;
                }
                else
                {
                    throw new Exception(response.ReasonPhrase);
                }
            }
        }

        private static void InitializeClient()
        {
            apiClient = new HttpClient();
            apiClient.DefaultRequestHeaders.Accept.Clear();
            apiClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

        }
    }
}

