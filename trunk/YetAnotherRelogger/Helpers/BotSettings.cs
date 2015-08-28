using System;
using System.ComponentModel;
using System.IO;
using System.Windows.Forms;
using System.Xml.Serialization;
using YetAnotherRelogger.Helpers.Bot;

namespace YetAnotherRelogger.Helpers
{

    #region BotSettings

    public sealed class BotSettings
    {
        #region singleton

        private static readonly BotSettings instance = new BotSettings();

        static BotSettings()
        {
        }

        private BotSettings()
        {
            Bots = new BindingList<BotClass>();
            settingsdirectory = Path.Combine(Path.GetDirectoryName(Application.ExecutablePath), "Settings"); 
        }

        public static BotSettings Instance
        {
            get { return instance; }
        }

        #endregion

        private readonly string settingsdirectory;
        public BindingList<BotClass> Bots;

        public static string SettingsDirectory
        {
            get { return instance.settingsdirectory; }
        }

        public string SettingsFileName
        {
            get { return Path.Combine(SettingsDirectory, "Bots.xml"); }
        }

        public void Save()
        {
            var xml = new XmlSerializer(Bots.GetType());

            if (!Directory.Exists(SettingsDirectory))
                Directory.CreateDirectory(SettingsDirectory);


            using (var writer = new StreamWriter(SettingsFileName))
            {
                xml.Serialize(writer, Bots);
            }
        }

        public void Load()
        {
            try
            {
                var xml = new XmlSerializer(Bots.GetType());

                if (!File.Exists(SettingsFileName))
                    return;

                using (var reader = new StreamReader(SettingsFileName))
                {
                    Bots = xml.Deserialize(reader) as BindingList<BotClass>;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error Loading BotSettings");
            }
        }

        /// <summary>
        /// Clones a Bot. Returns the index of the clone.
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public int Clone(int index)
        {
            var cloned = (BotClass)Bots[index].Clone();
            int nextIndex = index + 1;
            if (index == Bots.Count - 1)
                Bots.Add(cloned);
            else
                Bots.Insert(nextIndex, cloned);
            return nextIndex;
        }

        public int MoveDown(int index)
        {
            if (index == Bots.Count - 1)
                return index;
            var bot = Bots[index];
            Bots.Remove(bot);
            int newIdx = index + 1;
            if (newIdx == Bots.Count - 1)
                Bots.Add(bot);
            else
                Bots.Insert(newIdx, bot);

            return newIdx;
        }
        public int MoveUp(int index)
        {
            if (index == 0)
                return index;
            int newIdx = index - 1;
            var bot = Bots[index];
            Bots.Remove(bot);
            Bots.Insert(newIdx, bot);
            return newIdx;

        }
    }

    #endregion
}