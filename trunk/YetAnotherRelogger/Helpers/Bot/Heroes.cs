using System.Collections.Generic;

namespace YetAnotherRelogger.Helpers.Bot
{
    public class Heroes
    {
        public int HeroCount { get; set; }
        public bool Unique { get; set; }
        public bool UseMaxLevelOnly { get; set; }

        public List<HeroInfo> HeroList { get; set; }

        public class HeroInfo
        {
            public int id { get; set; }
            public string Name { get; set; }
        }
    }
}