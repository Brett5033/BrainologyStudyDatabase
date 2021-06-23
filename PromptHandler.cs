using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrainologyStudyDatabase
{
    public class Prompt : IComparable
    {
        // * Ensure " ___ " has the spaces
        // Full Line of Prompt
        //  Which Participants saw Segment ___ /
        //  Select p.* FROM " + DatabaseHandler.JoinStudyKey + " WHERE seg.SEGMENT_ID = {1}/
        //  SELECT SEGMENT_ID NAME FROM SEGMENT

        // Usage rank to rate prompts
        public int rank { get; set; }

        // "Which Participants saw Segment ___"
        public string prompt { get; set; }

        // Select p.* FROM " + DatabaseHandler.JoinStudyKey + " WHERE seg.SEGMENT_ID = {1}";
        // {_} represent option select, break on them
        public string query { get; set; }

        // option select query, results with 0 index as primary
        public string[] options { get; set; }

        public Prompt(int rank,string prompt, string query, string[] options)
        {
            this.rank = rank;
            this.prompt = prompt;
            this.query = query;
            this.options = options;
        }

        public override string ToString()
        {
            string result = prompt;
            result += "\n\t" + query;
            foreach (string s in options)
                result += "\n\t\t" + s;
            return result;
        }


        public int CompareTo(object obj)
        {
            if (obj.GetType() != typeof(Prompt))
                throw new TypeAccessException("Non-Prompt found in prompt list");

            return this.rank >= ((Prompt)obj).rank ? -1 : 1;
        }
    }


    public class PromptHandler
    {

        string PromptSavesPath = @"../../PromptSaves.txt";

        public List<Prompt> prompts;

        public PromptHandler()
        {
            prompts = new List<Prompt>();
            readInPrompts();
        }


        private bool readInPrompts()
        {

            string line;
            Console.WriteLine("Reading prompt file");

            using (StreamReader file = new StreamReader(PromptSavesPath))
            {
                Console.WriteLine("Prompt file open");
                while((line = file.ReadLine()) != null && line != "")
                {

                    // Anotha one
                    string[] sections = line.Split(new char[] {'/'},StringSplitOptions.RemoveEmptyEntries);

                    // Split each line into / sperated strings
                    int rank;
                    if (!int.TryParse(sections[0], out rank))
                        rank = 0;

                    string prompt = sections[1];
                    string query = sections[2];
                    string[] options = new string[sections.Length - 3];
                    for(int i = 0; i < options.Length; i++)
                    {
                        options[i] = sections[i + 3];
                    }
                    prompts.Add(new Prompt(rank,prompt, query, options));
                    Console.WriteLine(prompts[prompts.Count - 1].ToString());
                }
                file.Close();
            }
            prompts.Sort();
            return true;
        }

        public void writePrompts()
        {
            using (StreamWriter sw = new StreamWriter(PromptSavesPath))
            {
                Console.WriteLine("Saving Prompts"); 
                foreach(Prompt p in prompts)
                {
                    string line = p.rank + "/" + p.prompt + "/" + p.query + "/";
                    for(int s = 0; s < p.options.Length; s++)
                    {
                        line += p.options[s];
                        if (s < p.options.Length - 1)
                            line += "/";
                    }
                    Console.WriteLine("Prompt: {" + line + "}");
                    sw.WriteLine(line);
                }
                sw.Close();
            }
        }
    }
}
