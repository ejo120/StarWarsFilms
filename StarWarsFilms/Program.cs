using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using CsvHelper;

namespace StarWarsFilms
{
    class Program
    {
        private static HttpClient apiClient;

        //Calling api
        static async Task Main(string[] args)
        {
            InitializeClient();
            string lookUpAnother = "";

            do
            { //Write Character Lookup
                Console.Write("Pick a number to look up different Star Wars Characters: ");
                string idText = Console.ReadLine();

                try
                { 
                    CharacterModel person = await GetStarWarsCharacter(idText);
                    //write response
                    Console.WriteLine($"{ person.name } is a { person.gender } born in { person.birth_year }. ");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error: { ex.Message }");
                }
                //Console.Write("Would you like to edit this Character's name? (yes/no:)");
                

                Console.Write("Do you want to look up another character (yes/no): ");
                lookUpAnother = Console.ReadLine();

                Console.Clear();
            } while (lookUpAnother.ToLower() == "yes");
            Console.ReadLine();
        }

        //reading API
        private static async Task<CharacterModel> GetStarWarsCharacter(string id)
        {
            string url = $"https://swapi.co/api/people/{ id }/";

            using (HttpResponseMessage response = await apiClient.GetAsync(url))
            {
                if (response.IsSuccessStatusCode)
                {
                    CharacterModel output = await response.Content.ReadAsAsync<CharacterModel>();
                    WriteRow(output);
                    return output;
                }
                else
                {
                    throw new Exception(response.ReasonPhrase);
                }
            }
        }
        //write to the csv file
        private static void WriteRow(CharacterModel character)
        {
            var path = AppDomain.CurrentDomain.BaseDirectory + @"UserCharNames.csv";
           
                var fileExsits = File.Exists(path);

            using (var writer = new StreamWriter(path, true))
            using (var csv = new CsvWriter(writer))
            {
                {
                    if (!fileExsits)
                    {

                        csv.WriteHeader<CharacterModel>();
                    }
                    csv.NextRecord();
                    csv.WriteRecord(character);
                }
            }
        }

        public static List<CharacterModel> CharacterText(string csvFileName)
        {
            List<CharacterModel> characterList = new List<CharacterModel>();

            using (TextReader reader = new StreamReader(csvFileName))
            {
                string line = "";

                while ((line = reader.ReadLine()) != null)
                {
                    //R5 - D4,name 
                    //97,   height 
                    //32,  mass 
                    //"n / a",     hair_color
                    //"white,red" skin_color
                    //red,  eye_color  
                    //unknown,   birth_year
                    //gender,  n/ a   
                    //homeworld, https://swapi.co/api/planets/1/
                    //films - not in csv
                    //species - not in csv
                    //vehicles - not in csv
                    //starships - not in csv
                    //12 /10/2014 15:57, created
                    //12/20/2014 21:17,	edited 
                    //https://swapi.co/api/people/8/, url 

                    string[] fields = line.Split(",".ToCharArray());
                    CharacterModel myCharacter = new CharacterModel();
                    myCharacter.name = fields[0];
                    myCharacter.height = fields[1];
                    myCharacter.mass = fields[2];
                    myCharacter.hair_color = fields[3];
                    myCharacter.skin_color = (fields[4] + "," + fields[5]).Replace("\"", "");
                    myCharacter.eye_color = fields[6];
                    myCharacter.birth_year = fields[7];
                    myCharacter.gender = fields[8];
                    myCharacter.homeworld = fields[9];
                    try { myCharacter.created = Convert.ToDateTime(fields[10]); } catch { myCharacter.created = new DateTime(); }
                    try { myCharacter.edited = Convert.ToDateTime(fields[11]); } catch { myCharacter.edited = new DateTime(); }
                    myCharacter.url = fields[12];

                    characterList.Add(myCharacter); 
                }

            }

            return characterList;
        }


        private static CharacterModel GetCharacterById(int id)
        {
            var path = AppDomain.CurrentDomain.BaseDirectory + @"UserCharNames.csv";
            var config = new CsvHelper.Configuration.Configuration();
            config.MissingFieldFound = null;
            config.HasHeaderRecord = true;
            config.HeaderValidated = null;
            using (var reader = new StreamReader(path))
            {
                using (var csv = new CsvReader(reader, config))
                {
                    var characters = csv.GetRecords<CharacterModel>();
                    var character = characters.Where(x => x.url == "https://swapi.co/api/people/" + id + "/").FirstOrDefault();
                    return character;
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

