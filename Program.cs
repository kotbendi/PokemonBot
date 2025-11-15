using System;
using System.Text;
using System.Net.Http;
using System.Threading.Tasks;
using Telegram.Bot;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types;
using System.Runtime.CompilerServices;
using System.Collections.Generic;
//Version 1.0.0
namespace pokemonbot
{
    

class Program
{
    
    public static  string token = "Your_Telegram_Bot_Token_Here";
    public static long chatid = "Your_Chat_ID_Here";
    
    
public static async Task<string> getstat(string pokemon)
{
    try
    {
        string url = "https://pokeapi.co/api/v2/pokemon/" + pokemon.ToLower();
        using var http = new HttpClient();
        
        HttpResponseMessage resp = await http.GetAsync(url);
        
        if (!resp.IsSuccessStatusCode)
        {
            return $"Pokemon '{pokemon}' not found!";
        }
        
        string json = await resp.Content.ReadAsStringAsync(); 
        
        if (string.IsNullOrEmpty(json)) return "Null response from API.";

        dynamic? data = JsonConvert.DeserializeObject(json);
        if (data == null) return "Error deserializing JSON.";

        StringBuilder sb = new StringBuilder(); 
        
        string name = data.name != null ? (string)data.name : "unknown";
        string id = data.id != null ? data.id.ToString() : "unknown";
        string height = data.height != null ? data.height.ToString() : "unknown";
        string weight = data.weight != null ? data.weight.ToString() : "unknown";
        
        string types = "Unknown";
        if (data.types != null)
        {
            var typeList = new List<string>();
            foreach (var typeInfo in data.types)
            {
                if (typeInfo.type?.name != null)
                {
                    typeList.Add(typeInfo.type.name.ToString());
                }
            }
            types = string.Join(", ", typeList);
        }
        
        sb.AppendLine($"🐉 **{char.ToUpper(name[0]) + name.Substring(1)}**");
        sb.AppendLine($"🆔 ID: {id}");
        sb.AppendLine($"📏 Height: {height}");
        sb.AppendLine($"⚖️ Weight: {weight}");
        sb.AppendLine($"🎯 Types: {types}");
        
        
        return sb.ToString();
    }
    catch (Exception ex)
    {
        return $"Error getting Pokemon data: {ex.Message}";
    }
}
public static string RandomPokemonName()
{
    Random rnd = new Random();
    string[] pokemon =
    {
        "pikachu", "bulbasaur", "charmander", "squirtle", "eevee",
        "mewtwo", "snorlax", "gengar", "dragonite", "charizard",
        "venusaur", "blastoise", "meowth", "psyduck", "jigglypuff",
        "machamp", "alakazam", "arbok", "raichu", "lapras",
        "vaporeon", "jolteon", "flareon", "lucario", "greninja"
    };
    return pokemon[rnd.Next(pokemon.Length)];
}
public static async  Task Main()
        {
    try
    {
        var bot = new TelegramBotClient(token);
        Console.WriteLine("Bot started!");
        
        bot.StartReceiving(UpdateHandler, ErrorHandler);
        
        
        while (true)
        {
            await Task.Delay(1000);
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Critical error: {ex}");
        Console.ReadLine();
    }
}
    private static async Task UpdateHandler(ITelegramBotClient bot, Update update, CancellationToken ct)
    {

       
        if (update.Message?.Text != null)
        {
            long chatId = update.Message.Chat.Id;
            string messageText = update.Message.Text.ToLower();
            try
            {
                if (messageText == "get pokemon"){
                string pokemonName = RandomPokemonName();
                string pokemonInfo = await getstat(pokemonName);
                await bot.SendMessage(
                chatId: chatId,
                text: pokemonInfo,
                cancellationToken: ct
                );
                }
            }
            catch (Exception ex)
            {
                await bot.SendMessage(
                chatId: chatId,
                text: $"❌ Error: {ex.Message}",
                cancellationToken: ct
                );
            }
        }
        
    }

    
    private static Task ErrorHandler(ITelegramBotClient bot, Exception ex, CancellationToken ct)
    {
        Console.WriteLine($"Erorr: {ex.Message}");
        return Task.CompletedTask;
    }
}
}
