using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Navigation;
using static sldc.Stores.DRPClientStore;

namespace sldc.Converter.CovenantConverters
{
    public class ByteToCovenantConverter
    {
        public static string Convert(byte[] bytes, ENVTokens envToken)
        {
            var matchingEntry = GetMatchingEntry(bytes, envToken);
            return matchingEntry.Value;
        }
        public static KeyValuePair<byte[], string> GetMatchingEntry(byte[] bytes, ENVTokens envToken)
        {
            Dictionary<byte[], string> covenantDictionary = ReturnGameDict(envToken);
            var matchingEntry = covenantDictionary.FirstOrDefault(kv => kv.Key.SequenceEqual(bytes));
            return matchingEntry;
        }

        private static Dictionary<byte[], string> ReturnGameDict(ENVTokens envToken)
        {
            Dictionary<byte[], string> covenantDictionary;
            switch (envToken)
            {
                case ENVTokens.DS1_TOKEN:
                    covenantDictionary = new Dictionary<byte[], string>()
                    {
                        { new byte[1] {0}, "None" },
                        { new byte[1] {1}, "Way of White" },
                        { new byte[1] {2}, "Princess's Guard" },
                        { new byte[1] {3}, "Warrior of Sunlight" },
                        { new byte[1] {4}, "Darkwraith" },
                        { new byte[1] {5}, "Path of the Dragon" },
                        { new byte[1] {6}, "Gravelord Servant" },
                        { new byte[1] {7}, "Forest Hunter" },
                        { new byte[1] {8}, "Blade of the Dark Moon" },
                        { new byte[1] {9}, "Chaos Servant" },
                    };
                    break;
                case ENVTokens.DS2_TOKEN:
                    covenantDictionary = new Dictionary<byte[], string>()
                    {
                        { new byte[1] {0}, "None" },
                        { new byte[1] {1}, "Heirs of the Sun" },
                        { new byte[1] {2}, "Blue Sentinels" },
                        { new byte[1] {3}, "Brotherhood of Blood" },
                        { new byte[1] {4}, "Way of Blue" },
                        { new byte[1] {5}, "Rat King" },
                        { new byte[1] {6}, "Bell Keeper" },
                        { new byte[1] {7}, "Dragon Remnants" },
                        { new byte[1] {8}, "Company of Champions" },
                        { new byte[1] {9}, "Pilgrims of Dark" },
                    };
                    break;
                case ENVTokens.DS3_TOKEN:
                    covenantDictionary = new Dictionary<byte[], string>()
                    {
                        { new byte[4] { 0, 0, 0, 0 }, "None"},
                        { new byte[4] { 16, 39, 0, 160 }, "Blade of the Darkmoon" },
                        { new byte[4] { 36, 39, 0, 160 }, "Watchdogs of Farron"},
                        { new byte[4] { 46, 39, 0, 160 }, "Aldrich Faithful"},
                        { new byte[4] { 56, 39, 0, 160 }, "Warrior of Sunlight"},
                        { new byte[4] { 66, 39, 0, 160 }, "Mound Maker"},
                        { new byte[4] { 76, 39, 0, 160 }, "Way of Blue"},
                        { new byte[4] { 86, 39, 0, 160 }, "Blue Sentinels"},
                        { new byte[4] { 96, 39, 0, 160 }, "Rosarias Fingers"},
                        { new byte[4] { 106, 39, 0, 160 }, "Spear of the Church"},

                    };
                    break;
                default:
                    covenantDictionary = null;
                    break;
            }
            return covenantDictionary;
        }
    }
}
