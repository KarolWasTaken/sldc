using System.Windows;
using System.IO;

namespace sldc
{
    public static class DotEnv
    {
        public static void Load()
        {
            Stream stream = null;
            try {
                stream = Application.GetResourceStream(new Uri($"pack://application:,,,/.env")).Stream;
            } catch (Exception e)
            {
                MessageBox.Show(e.Message, "Error loading .env");
                Environment.Exit(-1);
            }

            if (stream == null)
            {
                MessageBox.Show("Cannot load .env file", "Error loading .env");
                Environment.Exit(-1);
            }
            string[] envFileLines = null;
            using (var reader = new StreamReader(stream))
            {
                var lines = new List<string>();
                while (!reader.EndOfStream)
                {
                    lines.Add(reader.ReadLine());
                }
                envFileLines = lines.ToArray();
            }
            if(envFileLines.Length == 0)
            {
                MessageBox.Show(".env is empty. Please report on GitHub.", "Error loading .env");
                Environment.Exit(-1);
            }
            foreach (var line in envFileLines)
            {
                var parts = line.Split(
                    '=',
                    StringSplitOptions.RemoveEmptyEntries);

                if (parts.Length != 2)
                    continue;

                Environment.SetEnvironmentVariable(parts[0], parts[1]);
            }
        }
    }
}
