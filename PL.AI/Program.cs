using Newtonsoft.Json;
using System.Text;


const string GPT4V_ENDPOINT = "https://bbr-chat.openai.azure.com/openai/deployments/gpt4-vision/chat/completions?api-version=2023-07-01-preview";
string GPT4V_KEY = "d9376b02410346609951251ff621a313";
string IMAGE_PATH = args[0];
var encodedImage = Convert.ToBase64String(File.ReadAllBytes(IMAGE_PATH));

string QUESTION = "\"Players request\": {\r\n\t\"Play as teams\": false,\r\n\t\"Emotions while playing\":  \"anxciety\",\r\n\t\"Desired social result\": \"iq measurement\",\r\n\t\"Role play\" : true,\r\n\t\"Game complexity\": \"intermediate\",\r\n\t\"Uniqueness of game/play\": \"Totally unique new game that the world has never seen before\",\r\n\t\"use objects in the room\": true\r\n}";

Console.WriteLine("Press any key to run the first question:");
Console.ReadLine();
Console.WriteLine(QUESTION);
await RunQuestion(QUESTION);
Console.WriteLine("Press any key for the nest question:");
Console.ReadLine();


async Task RunQuestion(string question)
{

    using (var httpClient = new HttpClient())
    {
        httpClient.DefaultRequestHeaders.Add("api-key", GPT4V_KEY);
        var payload = new
        {
            messages = new object[]
            {
                  new {
                      role = "system",
                      content = new object[] {
                          new {
                              type = "text",
                              text = "Hello, world's greatest game and play planning assistant of the renowned event planning organization, Partyplanning Worldwide! Your exceptional ability to come up with fun, engaging, and unique games and plays on the fly is unparalleled. With your keen eye, you evaluate the room, objects within it, and the participants, crafting the perfect gaming activities that cater to the requirements of any group. You have a knack for using the environment to your advantage, turning the mundane into the foundation of memorable experiences. Whether it's suggesting classics like \"spin the bottle\" with just a glance at some bottles, or inventing brand new games the world has yet to see, your creativity knows no bounds. Your ultimate goal is to ensure a happy atmosphere and complete satisfaction for all players. Please use the information provided about the room and the participants' desires to suggest one single perfect game for them to play that will create the greatest experience.\r\nYou have to specify the game rules yourself, and make them as easy as possible. For roleplaying games, you have to come up with roles and scenarios yourself. Leave as little as possible to chance and the players, making the playing experience as simple and thought through for the players as possible.\r\nThe players provide their requests in a JSON format."
                          }
                      }
                  },
                  new {
                      role = "user",
                      content = new object[] {
                          new {
                              type = "image_url",
                              image_url = new {
                                  url = $"data:image/jpeg;base64,{encodedImage}"
                              }
                          },
                          new {
                              type = "text",
                              text = question
                          }
                      }
                  }
            },
            temperature = 0.7,
            top_p = 0.95,
            max_tokens = 800,
            stream = false
        };

        var response = await httpClient.PostAsync(GPT4V_ENDPOINT, new StringContent(JsonConvert.SerializeObject(payload), Encoding.UTF8, "application/json"));

        if (response.IsSuccessStatusCode)
        {
            var responseData = JsonConvert.DeserializeObject<dynamic>(await response.Content.ReadAsStringAsync());
            Console.WriteLine(responseData.choices[0].message.content);
        }
        else
        {
            Console.WriteLine($"Error: {response.StatusCode}, {response.ReasonPhrase}");
        }
    }
}
