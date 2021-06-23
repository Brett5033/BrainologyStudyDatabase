using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrainologyStudyDatabase
{
    public static class DatabaseEnums
    {
        public enum TABLES
        {
            STUDY,
            STUDY_VERSION,
            SEGMENT,
            SESSION,
            EEG_AMPLITUDE,
            PARTICIPANT,
            VISIT,
            LOCATION,
            NUMBER_OF_COLS
        }

        
        public enum STUDY_COl
        {
            STUDY_ID,
            NAME,
            TYPE,
            START_DATE,
            END_DATE,
            NUMBER_OF_COLS
        }

        public enum STUDY_VERSION_COL
        {
            VERSION_ID,
            STUDY_ID,
            VERSION_NUM,
            DESCRIPTION,
            NUMBER_OF_COLS
        }

        public enum SEGMENT_COL
        {
            SEGMENT_ID,
            VERSION_ID,
            NAME,
            DURATION,
            SOURCE,
            NUMBER_OF_COLS
        }


        public enum SESSION_COL
        {
            SESSION_ID,
            VERSION_ID,
            PARTICIPANT_ID,
            DATE_TESTED,
            INTERVIEW,
            REWARD,
            NUMBER_OF_COLS
        }
        public enum EEG_AMPLITUDE_COL
        {
            SESSION_ID,
            SEGMENT_ID,
            DELTA,
            THETA,
            ALPHA,
            BETA,
            NUMBER_OF_COLS
        }

        public enum PARTICIPANT_COL
        {
            PARTICIPANT_ID,
            NAME,
            PHONE,
            GENDER,
            AGE,
            ETHNICITY,
            OCCUPATION,
            DOB,
            NUMBER_OF_COLS
        }

        public enum VISIT_COL
        {
            VISIT_ID,
            PARTICIPANT_ID,
            LOCATION_ID,
            DATE,
            NUMBER_OF_COLS
        }

        public enum LOCATION_COL
        {
            LOCATION_ID,
            NAME,
            NUMBER_OF_COLS
        }

        

        public static string getEnumString(TABLES t, bool includeParen = true)
        {
            try
            {
                string[] names = Enum.GetNames(Type.GetType(t.ToString() + "_COL"));
                string output = "";
                if (includeParen)
                    output = "(";
                for (int i = 0; i < names.Length - 1; i++)
                {
                    output += names[i];
                    if (i < names.Length - 2)
                        output += ",";
                }
                if (includeParen)
                    output += ")";
                return output;
            }catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.WriteLine("Didnt find enum '" + t.ToString() + "_COL'");
                return "";
            }
        }

        public static string getEnumString(string table, bool includeParen = true)
        {
            string fullEnumName = table + "_COL";
            Console.WriteLine("Getting enum of type '" + fullEnumName + "'");
            string[] names = Enum.GetNames(Type.GetType(fullEnumName));
            Console.WriteLine("got enum");
            string output = "";
            if (includeParen)
                output = "(";
            for (int i = 0; i < names.Length - 1; i++)
            {
                output += names[i];
                if (i < names.Length - 2)
                    output += ",";
            }
            if (includeParen)
                output += ")";
            return output;
        }

    }
}
