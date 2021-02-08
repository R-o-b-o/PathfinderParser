using System;
using System.IO;
using System.Collections.Generic;
using Newtonsoft.Json;
using System.Security.Cryptography.X509Certificates;
using System.Xml.Serialization;
using System.Linq;

namespace PathfinderParser
{
    class Program
    {
        public const string blockStart = "::";
        public const bool tweeDecompiledTricks = true;

        static void ParseFile(string filename)
        {
            AdventureConfig config = new AdventureConfig(string.Join("", filename.Split('.')[0]), "", "", "", 0);

            string[] lines = File.ReadAllLines(filename);
            int linePos = 0;
            Dictionary<string, AdventureSegment> segments = new Dictionary<string, AdventureSegment>();

            while (linePos < lines.Length)
            {
                if (lines[linePos].StartsWith(blockStart))
                {
                    linePos += ParseSegment(lines[linePos..lines.Length], ref segments);
                }
                linePos++;
            }

            Adventure adventure = new Adventure(config, segments);
            Console.WriteLine(JsonConvert.SerializeObject(adventure));
            File.WriteAllText(adventure.config.name+ ".json", JsonConvert.SerializeObject(adventure, Formatting.Indented));
        }

        static int ParseSegment(string[] lines, ref Dictionary<string, AdventureSegment> segments)
        {
            string name = lines[0].Substring(blockStart.Length).Trim();


            /* SPECIAL CODE FOR TWEE CONVERSION */
            if (tweeDecompiledTricks)
            {
                if (new string[] { "StoryTitle", "Story Stylesheet [stylesheet]", "StoryData" }.Contains(name))
                {
                    return 1;
                }
                if (name.Contains('{') && name.Contains('}'))
                {
                    name = name.Substring(0, name.LastIndexOf('{')).Trim();
                }
            }
            /* -------------------------------- */

            AdventureSegment segment = new AdventureSegment();

            int i = 1;
            for (; i < lines.Length; i++)
            {
                if (lines[i].StartsWith(blockStart))
                {
                    Console.WriteLine(JsonConvert.SerializeObject(segment));
                    segment.maintext = segment.maintext.Trim();
                    break;
                }

                if (lines[i] == "") { continue; }

                ParseSegmentLine(lines[i], ref segment);
                segment.maintext += '\n';
            }

            List<string> emoteChoices = new List<string>() { "⚪", "⚫", "🔴", "🔵", "🟠", "🟡", "🟢", "🟣", "🟤" };
            Random random = new Random();
            int randIndex;

            for (int j = 0; j < segment.choices.Count; j++)
            {
                if (segment.choices[j].emote == "")
                {
                    randIndex = random.Next(emoteChoices.Count);
                    if (emoteChoices.Count == 0) { throw new System.IndexOutOfRangeException("Used up all emote choices");  }
                    segment.choices[j].emote = emoteChoices[randIndex];
                    emoteChoices.RemoveAt(randIndex);
                }
            }

            segments.Add(name, segment);
            return --i;
        }

        static void ParseSegmentLine(string line, ref AdventureSegment segment)
        {
            for (int i = 0; i < line.Length; i++)
            {
                if (line[i] == '<' && line[i..].Contains('>'))
                {
                    segment.choicetext = line[(i + 1)..(i + line.IndexOf('>'))];
                    i += line.IndexOf('>') + 1;
                }
                else if (line[i..].StartsWith("[[") && line[i..].Contains("]]"))
                {
                    string choiceText = line[(i + 2)..(i + line[i..].IndexOf("]]"))];
                    segment.choices.Add(ParseChoicetext(choiceText));

                    if ($"[[{choiceText}]]" == line) { return; }

                    segment.maintext += segment.choices.Last().text;
                    i += line[i..].IndexOf("]]") + 2;
                }
                else if (tweeDecompiledTricks && line[i] == '(' && line[i..].Contains(')')) //SPECIAL CODE FOR TWEE CONVERSION
                {
                    i += line[i..].IndexOf(")") + 1;
                }
                else if (!(tweeDecompiledTricks && "[]".Contains(line[i]))) //SPECIAL CODE FOR TWEE CONVERSION
                {
                    segment.maintext += line[i];
                }
            }
        }

        static AdventureChoice ParseChoicetext(string toParse)
        {
            char sep = '|';
            if (!toParse.Contains(sep))
            {
                return new AdventureChoice(toParse, toParse, "");
            } else {
                string[] textTargetEmote = toParse.Split(sep);

                for (int i = 0; i < textTargetEmote.Length; i++)
                {
                    textTargetEmote[i] = textTargetEmote[i].Trim();
                }

                if (textTargetEmote.Length == 2)
                {
                    return new AdventureChoice(textTargetEmote[0], textTargetEmote[1], "");
                }

                return new AdventureChoice(textTargetEmote[0], textTargetEmote[1], textTargetEmote[2]);
            }
        }

        static void Main(string[] args)
        {
            Console.WriteLine("This is the PathFinder parser application");
            Console.Write("Enter the filename to convert: ");
            string filename = Console.ReadLine();

            ParseFile(filename);
        }
    }
}
