using sldc.Stores;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace sldc.Model
{
    public class GameDeathDataSerialiser
    {
        private const string FILENAME = "NonHookGameData.json";
        public enum NON_HOOKABLE_GAME
        {
            BLOODBORNE,
            DEMON_SOULS_PS3,
            DEMON_SOULS_PS5
        }

        public static void SaveData(NonHookGamesDataStore nonHookGamesDataStore)
        {
            // Configure options for indented formatting
            JsonSerializerOptions options = new JsonSerializerOptions
            {
                WriteIndented = true
            };

            // Serialize the object to indented JSON
            string jsonString = JsonSerializer.Serialize(nonHookGamesDataStore, options);
            Console.WriteLine(jsonString);
            File.WriteAllText(FILENAME, jsonString);
        }
        public static void SaveData(NonHookGamesDataStore nonHookGamesDataStore, Dictionary<string, int> playthroughs, NON_HOOKABLE_GAME game)
        {
            switch (game)
            {
                case NON_HOOKABLE_GAME.BLOODBORNE:
                    nonHookGamesDataStore.Bloodborne = playthroughs;
                    break;
                case NON_HOOKABLE_GAME.DEMON_SOULS_PS3:
                    nonHookGamesDataStore.Bloodborne = playthroughs;
                    break;
                case NON_HOOKABLE_GAME.DEMON_SOULS_PS5:
                    nonHookGamesDataStore.Bloodborne = playthroughs;
                    break;
                default:
                    throw new NotImplementedException();
            }
            SaveData(nonHookGamesDataStore);
        }
        public static NonHookGamesDataStore LoadData()
        {
            try 
            {
                string jsonContent = File.ReadAllText(FILENAME);
                NonHookGamesDataStore gamedata = JsonSerializer.Deserialize<NonHookGamesDataStore>(jsonContent);
                return gamedata;
            }
            catch (FileNotFoundException e)
            {
                NonHookGamesDataStore ds = new NonHookGamesDataStore();
                ds.Bloodborne = new Dictionary<string, int>();
                ds.DemonSoulsPS3 = new Dictionary<string, int>();
                ds.DemonSoulsPS5 = new Dictionary<string, int>();
                return ds;
            }
        }
        public static Dictionary<string, int> LoadData(NON_HOOKABLE_GAME game)
        {
            NonHookGamesDataStore data = LoadData();
            switch (game)
            {
                case NON_HOOKABLE_GAME.BLOODBORNE:
                    return data.Bloodborne;
                case NON_HOOKABLE_GAME.DEMON_SOULS_PS3:
                    return data.DemonSoulsPS3;
                case NON_HOOKABLE_GAME.DEMON_SOULS_PS5:
                    return data.DemonSoulsPS5;
                default: 
                    throw new NotImplementedException();
            }
        }

        // exception handle later
        public void DeletePlaythroughs(NON_HOOKABLE_GAME gameToProcess, NonHookGamesDataStore nonHookGamesDataStore, string playthroughName, bool SaveFile = true)
        {
            switch(gameToProcess) 
            {
                case NON_HOOKABLE_GAME.BLOODBORNE:
                    nonHookGamesDataStore.Bloodborne.Remove(playthroughName);
                    break;
                case NON_HOOKABLE_GAME.DEMON_SOULS_PS3:
                    nonHookGamesDataStore.DemonSoulsPS3.Remove(playthroughName);
                    break;
                case NON_HOOKABLE_GAME.DEMON_SOULS_PS5:
                    nonHookGamesDataStore.DemonSoulsPS5.Remove(playthroughName);
                    break;
                default:
                    throw new NotImplementedException();
            }
            if (SaveFile)
                SaveData(nonHookGamesDataStore);
        }
    }
}
