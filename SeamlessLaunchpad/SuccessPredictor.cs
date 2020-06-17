using SeamlessLaunchpad.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SeamlessLaunchpad
{
    public class SuccessPredictor
    {
        public static int MeasureTeam(ApiStartup p1, List<FeedbackContainer> f1)
        {
                int totalfeedback = 0;
                int teamMeasurement = 0;
   
            foreach (FeedbackContainer fbc1 in f1)
            {
                totalfeedback += fbc1.Fields.TeamStrength;
            }

            if (f1.Count == 0)
            {
                return p1.Team;
            }
             int avg = totalfeedback / f1.Count;
             int newAvg = (avg + p1.Team) / 2;
            if (newAvg >= 5)
            {
                teamMeasurement = 4;
            }
            else if (newAvg == 4)
            {
                teamMeasurement = 3;
            }
            else if (newAvg == 3)
            {
                teamMeasurement = 2;
            }
            else if (newAvg == 2)
            {
                teamMeasurement = 1;
            }
            else if (newAvg == 1)
            {
                teamMeasurement = 0;
            }
            return teamMeasurement;
        }

        public static int MeasureInterest(List<FeedbackContainer> f1)
        {
            if (f1==null || f1.Count== 0)
            {
                return 0;
            }

            int interest = 0;
            //Counts number of ?'s 
            foreach (FeedbackContainer fc in f1)
            {
                Feedback f = fc.Fields;
                if (f.Questions == null)
                {
                    continue;
                }
                int questions = f.Questions.Count(x => x.Equals('?'));

                interest += questions;
            }

            return interest;

        }

        public static int PredictSuccess(ApiStartup s1, List<FeedbackContainer> fc1)
        {
            int success = MeasureTeam(s1, fc1) + MeasureInterest(fc1);
            return success;
        }

    }
}
