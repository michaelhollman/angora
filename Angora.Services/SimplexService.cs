using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.SolverFoundation.Common;
using Microsoft.SolverFoundation.Services;
using Angora.Data.Models;

namespace Angora.Services
{
    public class SimplexService : ServiceBase, ISimplexService
    {
        private IFooCDNService _fooCDNService;
        private IEventService _eventService;

        public SimplexService(IFooCDNService fooService, IEventService eService)
        {
            _fooCDNService = fooService;
            _eventService = eService;
        }
        public void PerformSimplex()
        {
            IEnumerable<Event> events = _eventService.FindEventsWithBlobs();
            List<string> ids = new List<string>();
            List<ulong> size = new List<ulong>();
            List<int> attendees = new List<int>();
            List<DateTime> createDate = new List<DateTime>();
            List<DateTime> startDate = new List<DateTime>();

            if (events.Count() > 0)
            {
                foreach (Event e in events)
                {
                    foreach (Post p in e.Posts)
                    {
                        if (p.MediaItem != null)
                        {
                            ids.Add(p.MediaItem.FooCDNBlob);
                            size.Add(p.MediaItem.Size);
                            if (e.RSVPs != null && e.RSVPs.Count > 0)
                            {
                                attendees.Add(e.RSVPs.Count(r => r.Response == RSVPStatus.Yes)); 
                                createDate.Add(e.CreationTime);
                                startDate.Add(e.EventTime.StartTime);
                            }
                        }
                    }
                }

                SolverContext context = SolverContext.GetContext();
                Model model = context.CreateModel();

                //string[] ids = { "1", "2", "3" };
                string[] types = { "MEMCACHE", "DISK", "TAPE" };

                //ulong[] size = { 334, 567, 278 }; // in bytes
                //int[] invitees = { 20, 12, 14 };
                //DateTime[] createDate = { new DateTime(2014, 4, 20), new DateTime(2014, 4, 18), new DateTime(2014, 4, 22) };
                //DateTime[] eventDate = { new DateTime(2014, 4, 26), new DateTime(2014, 4, 21), new DateTime(2014, 4, 24) };
                int[] scores = new int[ids.Count];
                for (int i = 0; i < scores.Length; i++)
                {
                    scores[i] = GetScores(attendees[i], createDate[i], startDate[i]);
                }
                Decision[][] decs = new Decision[ids.Count][];
                for (int i = 0; i < ids.Count; i++)
                {
                    decs[i] = new Decision[3];
                    for (int j = 0; j < 3; j++)
                    {
                        decs[i][j] = new Decision(Domain.IntegerNonnegative, String.Format("dec_{0}_{1}", i, j));
                    }
                    model.AddDecisions(decs[i][0], decs[i][1], decs[i][2]);
                    model.AddConstraint(String.Format("con_{0}", i), decs[i][0] + decs[i][1] + decs[i][2] == 1);
                }
                double[] storageCosts = { .000000000025, .0000000000025, 0 };
                double[] servingcosts = { .0000000003, .0000000001, 0 };
                int[] typeWeights = { 10, 4, 1 };
                string constraintString = String.Format("{0} * {1} * {2} + {0} * {1} * {3} * {4} + {5} * {1} * {6} + {5} * {1} * {3} * {7} + {8} * {1} * {9} + {8} * {1} * {3} * {10}", decs[0][0].Name, size[0], storageCosts[0], scores[0], servingcosts[0], decs[0][1].Name, storageCosts[1], servingcosts[1], decs[0][2].Name, storageCosts[2], servingcosts[2]);
                string goalString = String.Format("{1} * {0} * {2} + {3} * {0} * {4} + {5} * {0} * {6}", scores[0], decs[0][0].Name, typeWeights[0], decs[0][1].Name, typeWeights[1], decs[0][2].Name, typeWeights[2]);
                for (int i = 1; i < decs.Length; i++)
                {
                    for (int j = 0; j < 3; j++)
                    {
                        constraintString = String.Format("{0} + {1} * {2} * {3} + {1} * {2} * {4} * {5}", constraintString, decs[i][j].Name, size[i], storageCosts[j], scores[i], servingcosts[j]);
                        goalString = String.Format("{0} + {1} * {2} * {3}", goalString, decs[i][j].Name, scores[i], typeWeights[j]);
                    }
                }
                constraintString = String.Format("{0} <= 15", constraintString);
                model.AddConstraint("money_con", constraintString);
                model.AddGoal("goal", GoalKind.Maximize, goalString);

                Solution solution = context.Solve(new SimplexDirective());
                for (int i = 0; i < ids.Count; i++)
                {

                    for (int j = 0; j < 3; j++)
                    {
                        if (decs[i][j].GetDouble() == 1)
                        {
                            _fooCDNService.PutBlob(ids[i], types[j]);
                        }
                    }
                }
            }
            
        }

        private int GetScores(int attendees, DateTime created, DateTime eventDate)
        {
            DateTime now = DateTime.Now.ToLocalTime();
            created = created.AddHours(DateTimeOffset.Now.Offset.Hours);
            if (eventDate > now)
            {
                int sinceCreated = (int)(now - created).TotalSeconds;
                int totalSpan = (int)(eventDate - created).TotalSeconds;
                totalSpan = totalSpan != 0 ? totalSpan : 1;
                attendees = attendees + 5; // to account for people that check to see if they want to go
                return (int)(attendees * (((totalSpan / 2.0) + Math.Abs(sinceCreated - (totalSpan / 2.0))) / totalSpan));
            }
            else
            {
                int sinceEvent = (int)(now - eventDate).TotalDays;
                return (int)(2 * attendees * Math.Pow(Math.E, -0.5 * sinceEvent));
            }
        }
    }
}
