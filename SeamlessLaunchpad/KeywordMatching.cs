using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;
using SeamlessLaunchpad.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SeamlessLaunchpad
{
    public class KeywordMatching
    {

        public static Dictionary<string, List<string>> themeToAlignmentPairing;
        public static Dictionary<string, List<string>> techAreaToAlignmentPairing;
        private static readonly string keywordApiKey;

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
            StreamReader sr = new StreamReader(File.OpenRead("keywordapi.txt"));
            keywordApiKey = sr.ReadToEnd().Trim(' ', '\r', '\n');
            sr.Close();
        }

        public static List<string> GetKeywords(string desc)
        {
            List<string> ret = new List<string>();

            string shortWordRegex = @"\b[a-z]{0,3}\b";
            string singleLetterRegex = @"\b[A-Z]\b";
            string descTrimmed = Regex.Replace(desc.Replace(",", ""), shortWordRegex, "");
            string finalTrim  = Regex.Replace(descTrimmed, singleLetterRegex, "").Replace(".", "").Replace("  ", " ").Trim();

            ret = finalTrim.Split(' ').ToList();

            return ret;
        }

        public static Dictionary<ApiStartup, double> FindSimilar(string desc, List<string> themes, List<string> techAreas, StartupListRootObject startupList)
        {
            List<string> keywords = GetKeywords(desc);
            List <StartupContainer> containers = startupList.Records;
            Dictionary<ApiStartup, double> scoreDictionary = new Dictionary<ApiStartup, double>();
            foreach (StartupContainer sc in containers)
            {
                ApiStartup startup = sc.Fields;
                double score = 0;
                foreach (string s in GetKeywords(startup.CompanySummary))
                {
                    if (keywords.Contains(s))
                    {
                        score += 1.5d;
                    }
                }
                foreach (string s in themes)
                {
                    if (startup.Theme.Contains(s))
                    {
                        score++;
                    }
                }
                foreach (string s in techAreas)
                {
                    if (startup.TechAreas.Contains(s))
                    {
                        score++;
                    }
                }
                scoreDictionary.Add(startup, score);
            }
            return scoreDictionary;
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

        public static async Task<List<StartupKeywords>> SetupKeywords(List<StartupContainer> startups, SLPADDBContext context)
        {
            List<StartupKeywords> lsk = new List<StartupKeywords>();
            foreach (StartupContainer s in startups)
            {
                try
                {
                    List<Keyword> keywords = (await Utilities.PostApiResponse<KeywordApiResponse>("v4", "keywords", "https://apis.paralleldots.com", "api_key", keywordApiKey, "text", Uri.EscapeDataString(s.Fields.CompanySummary))).FirstOrDefault().Keywords;

                    List<string> words = new List<string>();
                    keywords.ForEach(x => words.Add(x.Text));

                    StartupKeywords sk = new StartupKeywords { Keywords = string.Join(',', words.ToArray()), StartupName = s.Fields.CompanyName };

                    lsk.Add(sk);
                    context.StartupKeywords.Add(sk);

                    await context.SaveChangesAsync();
                }
                catch
                {

                }

                await Task.Delay(1000); //to avoid getting rate-limited by the api
            }
            return lsk;
        }

        public static async Task<List<Keyword>> GetKeywords(StartupContainer startup)
        {
            KeywordApiResponse keywords = (
                await Utilities.PostApiResponse<KeywordApiResponse>(
                    "v4",
                    "keywords",
                    "https://apis.paralleldots.com",
                    "api_key", keywordApiKey,
                    "text", startup.Fields.CompanySummary))
                    .FirstOrDefault();
            return keywords.Keywords;
        }
    }
}
