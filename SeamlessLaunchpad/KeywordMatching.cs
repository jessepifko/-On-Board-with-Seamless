using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;
using SeamlessLaunchpad.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.NetworkInformation;
using System.Reflection.Metadata.Ecma335;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SeamlessLaunchpad
{
    public class KeywordMatching
    {
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

        public static List<KeyValuePair<string, double>> FindSimilar(string desc, List<string> themes, List<string> techAreas, StartupListRootObject startupList, SLPADDBContext context)
        {
            List<string> keywords = GetKeywords(desc);
            List <StartupContainer> containers = startupList.Records;
            List<Models.Startup> dbStartups = context.Startup.ToList();
            Dictionary<string, double> scoreDictionary = new Dictionary<string, double>();
            foreach (StartupContainer sc in containers)
            {
                if (desc == sc.Fields.CompanySummary)
                {
                    continue;
                }
                ApiStartup startup = sc.Fields;
                double score = GetScore(keywords, techAreas, themes, GetKeywords(startup.CompanySummary), startup.TechAreas, startup.Theme);
                scoreDictionary.Add(startup.CompanyName, score);
            }
            foreach (Models.Startup s in dbStartups)
            {
                try
                {
                    double score = GetScore(keywords, techAreas, themes, GetKeywords(s.Summary), s.TechArea, s.Theme);
                    scoreDictionary.Add(s.Name, score);
                }
                catch
                {

                }
            }
            List<KeyValuePair<string, double>> topScores = new List<KeyValuePair<string, double>>();
            List<KeyValuePair<string, double>> returnValue = new List<KeyValuePair<string, double>>();
            topScores = scoreDictionary.ToList().OrderByDescending(x=>x.Value).ToList();
            for (int i = 0; i < Math.Min(3, topScores.Count); i++)
            {
                returnValue.Add(topScores[i]);
            }
            return returnValue;
        }

        public static double GetScore(List<string> keywords1, List<string> techAreas1, List<string> themes1, List<string> keywords2, string techAreas2, string themes2)
        {
            double score = 0.0d;
            foreach (string s in keywords1)
            {
                if (keywords2.Contains(s))
                {
                    score += 2;
                }
            }
            foreach (string s in themes1)
            {
                try
                {
                    if (themes2.Contains(s))
                    {
                        score += 1;
                    }
                }
                catch
                {

                }
            }
            foreach (string s in techAreas1)
            {
                try
                {
                    if (techAreas2.Contains(s))
                    {
                        score += 1.5;
                    }
                }
                catch
                {

                }
            }
            return score;
        }
    }
}
