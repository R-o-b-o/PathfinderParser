using System;
using System.Collections.Generic;
using System.Text;

namespace PathfinderParser
{
    public class AdventureConfig
    {
        public string name { get; set; }
        public string description { get; set; }
        public string creator { get; set; }
        public string imageurl { get; set; }
        public int plays { get; set; }

        public AdventureConfig(string name, string description, string creator, string imageurl, int plays)
        {
            this.name = name;
            this.description = description;
            this.creator = creator;
            this.imageurl = imageurl;
            this.plays = plays;
        }
    }
    public class AdventureSegment
    {
        public string maintext { get; set; }
        public string choicetext { get; set; }
        public List<AdventureChoice> choices { get; set; }

        public AdventureSegment()
        {
            this.maintext = "";
            this.choicetext = "";
            this.choices = new List<AdventureChoice>();
        }
    }
    public class AdventureChoice
    {
        public string emote { get; set; }
        public string text { get; set; }
        public string target { get; set; }

        public AdventureChoice(string text, string target, string emote)
        {
            this.emote = emote;
            this.text = text;
            this.target = target;
        }
    }
    public class Adventure
    {
        public AdventureConfig config { get; set; }
        public Dictionary<string, AdventureSegment> segments { get; set; }

        public Adventure(AdventureConfig config, Dictionary<string, AdventureSegment> segments)
        {
            this.config = config;
            this.segments = segments;
        }
    }
}
