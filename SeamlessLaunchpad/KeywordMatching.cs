using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SeamlessLaunchpad
{
    public class KeywordMatching
    {

        public static Dictionary<string, List<string>> themeToAlignmentPairing;
        public static Dictionary<string, List<string>> techAreaToAlignmentPairing;

        static KeywordMatching()
        {
            themeToAlignmentPairing = new Dictionary<string, List<string>>();
            themeToAlignmentPairing.Add("Health Beyond the Hospital", new List<string> { "Amway", "Trinity", "Whirlpool" });
            themeToAlignmentPairing.Add("The Good Life", new List<string> { "Bissel", "Amway", "Emergent" });
            themeToAlignmentPairing.Add("Robust Future", new List<string> { "Whirlpool", "Wakestream", "Emergent" });
            techAreaToAlignmentPairing = new Dictionary<string, List<string>>();
            techAreaToAlignmentPairing.Add("Products", new List<string> { "Amway", "Bissel", "Steelcase" });
            techAreaToAlignmentPairing.Add("Sensing", new List<string> { "Whirlpool", "Faurecia", "Emergent" });
            techAreaToAlignmentPairing.Add("Software / AI", new List<string> { "Whirlpool", "Amway", "Emergent", "Trinity" });
            techAreaToAlignmentPairing.Add("Robotics", new List<string> { "Bissel", "Emergent", "Trinity" });
        }

        public static List<string> GetKeywords(string desc)
        {
            List<string> ret = new List<string>();

            string shortWordRegex = @"\b[a-z]{0,3}\b";
            string singleLetterRegex = @"\b[A-Z]\b";
            string descTrimmed = Regex.Replace(desc, shortWordRegex, "");
            string finalTrim  = Regex.Replace(descTrimmed, singleLetterRegex, "").Replace(".", "").Replace("  ", " ").Trim();

            ret = finalTrim.Split(' ').ToList();

            return ret;
        }

        public static string GetAlignment(List<string> themes, List<string> techAreas)
        {
            Dictionary<string, int> alignments = new Dictionary<string, int>();
            foreach (string theme in themes)
            {
                if (string.IsNullOrEmpty(theme))
                {
                    break;
                }
                if (!themeToAlignmentPairing.ContainsKey(theme))
                {
                    continue;
                }
                foreach (string company in themeToAlignmentPairing[theme])
                {
                    if (alignments.ContainsKey(company))
                    {
                        alignments[company] += 1;
                    }
                    else
                    {
                        alignments.Add(company, 1);
                    }
                }
            }
            foreach (string techArea in techAreas)
            {
                if (string.IsNullOrEmpty(techArea))
                {
                    break;
                }
                if (!techAreaToAlignmentPairing.ContainsKey(techArea))
                {
                    continue;
                }
                foreach(string company in techAreaToAlignmentPairing[techArea])
                {
                    if (alignments.ContainsKey(company))
                    {
                        alignments[company] += 1;
                    }
                    else
                    {
                        alignments.Add(company, 1);
                    }
                }
            }
            List<string> alignedCompanies = new List<string>();
            List<KeyValuePair<string, int>> potentialAlignments = alignments.Where(x => x.Value >= 3).ToList();

            for (int i = 0; i < 3; i++)
            {
                if (potentialAlignments.Count == 0)
                {
                    break;
                }
                int highestScore = potentialAlignments.Max(x => x.Value);
                KeyValuePair<string, int> highestCompany = potentialAlignments.Where(x => x.Value == highestScore).FirstOrDefault();
                alignedCompanies.Add(highestCompany+":"+highestScore);
                potentialAlignments.Remove(highestCompany);
            }
            if (alignedCompanies.Count < 1)
            {
                return "";
            }
            return string.Join(",", alignedCompanies);
        }
    }
}
