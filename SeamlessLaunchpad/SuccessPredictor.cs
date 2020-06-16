using SeamlessLaunchpad.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SeamlessLaunchpad
{
    public class SuccessPredictor
    {
        public static int MeasureTeam(ApiStartup p1)
        {
            int teamMeasurement = 0;
            if (p1.Team == 5)
            {
                teamMeasurement = 4;
            }
            else if (p1.Team == 4)
            {
                teamMeasurement = 3;
            }
            else if (p1.Team == 3)
            {
                teamMeasurement = 2;
            }
            else if (p1.Team == 2)
            {
                teamMeasurement = 1;
            }
            else if (p1.Team == 1)
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

                if (questions == 1)
                {
                    interest += 1;
                }
                else if (questions == 2)
                {
                    interest += 2;
                }
                else if (questions == 3)
                {
                    interest += 3;
                }
                else if (questions == 4)
                {
                    interest += 4;
                }
            }

            return interest;

        }

        public static int PredictSuccess(ApiStartup s1, List<FeedbackContainer> fc1)
        {
            int success = MeasureTeam(s1) + MeasureInterest(fc1);
            return success;
        }

    }
}
