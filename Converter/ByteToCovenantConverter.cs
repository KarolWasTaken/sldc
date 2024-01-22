using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sldc.Converter
{
    public class ByteToCovenantConverter
    {
        public static string Convert(byte[] bytes)
        {
            //byte[] None = new byte[4] { 0, 0, 0, 0 };
            //byte[] BladeOfTheDarkmoon = new byte[4] { 16, 39, 0, 160 };
            //byte[] WatchdogsOfFarron = new byte[4] { 36, 39, 0, 160 };
            //byte[] AldrichFaithful = new byte[4] { 46, 39, 0, 160 };
            //byte[] WarriorOfSunlight = new byte[4] { 56, 39, 0, 160 };
            //byte[] MoundMakers = new byte[4] { 66, 39, 0, 160 };
            //byte[] WayOfBlue = new byte[4] { 76, 39, 0, 160 };
            //byte[] BlueSentinels = new byte[4] { 86, 39, 0, 160 };
            //byte[] RosariasFinger = new byte[4] { 96, 39, 0, 160 };
            //byte[] SpearOfChurch = new byte[4] { 106, 39, 0, 160 };
            var matchingEntry = GetMatchingEntry(bytes);
            return matchingEntry.Value;
        }
        public static KeyValuePair<byte[], string> GetMatchingEntry(byte[] bytes)
        {
            Dictionary<byte[], string> covenantDictionary = new Dictionary<byte[], string>()
            {
                { new byte[4] { 0, 0, 0, 0 }, "None"},
                { new byte[4] { 16, 39, 0, 160 }, "Blade of the Darkmoon" },
                { new byte[4] { 36, 39, 0, 160 }, "Watchdogs of Farron"},
                { new byte[4] { 46, 39, 0, 160 }, "Aldrich Faithful"},
                { new byte[4] { 56, 39, 0, 160 }, "Warrior of Sunlight"},
                { new byte[4] { 66, 39, 0, 160 }, "Mound Maker"},
                { new byte[4] { 76, 39, 0, 160 }, "Way of Blue"},
                { new byte[4] { 86, 39, 0, 160 }, "Blue Sentinels"},
                { new byte[4] { 96, 39, 0, 160 }, "Rosarias Finger"},
                { new byte[4] { 106, 39, 0, 160 }, "Spear of the Church"},

            };
            var matchingEntry = covenantDictionary.FirstOrDefault(kv => kv.Key.SequenceEqual(bytes));
            return matchingEntry;
        }
    }
}
