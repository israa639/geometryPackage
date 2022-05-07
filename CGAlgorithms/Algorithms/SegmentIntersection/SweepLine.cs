using CGUtilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace CGAlgorithms.Algorithms.SegmentIntersection
{
    public class Event
    {
        public Point p;
        public Enums.EventType enums;
        public int SegmentIndex;
        public int Segment2Index;
        public Event(Point x, Enums.EventType type, int ind)
        {
            p = (Point)x.Clone();
            enums = type;
            SegmentIndex = ind;
        }
        public Event(Point x, Enums.EventType type, int ind, int ind2)
        {
            p = (Point)x.Clone();
            enums = type;
            SegmentIndex = ind; //higher y
            Segment2Index = ind2; //lower y
        }

    }
    public class sweep
    {
        public double y;
        public int index;
        public sweep(double Y,int ind)
        {
            this.y = Y;
            this.index = ind;
        }
        public sweep()
        {
        }

    }
    public class SortLines: IComparer<sweep>
    {
        public int Compare(sweep a, sweep b)
        {
            if (a.y > b.y)
                return 1;
            else if (a.y < b.y)
                return -1;
            else return 0;
        }
    }
    public class queues
    {
        public double y;
        public int index;
        public queues(double Y, int ind)
        {
            this.y = Y;
            this.index = ind;
        }
        public queues()
        {
        }

    }
    public class queuesSort : IComparer<Event>
    {
        public int Compare(Event a, Event b)
        {
            if (a.p.X > b.p.X)
                return 1;
            else if (a.p.X < b.p.X)
                return -1;
            else
            {
                if (a.p.Y > b.p.Y)
                    return 1;
                else if (a.p.Y < b.p.Y)
                    return -1;
                else return 0;
            };
        }
    }


    public class SweepLine : Algorithm
    {
        public SortedSet<sweep> sweeps = new SortedSet<sweep>(new SortLines());
        public SortedSet<Event> eventqueue = new SortedSet<Event>(new queuesSort());
        public double currKey; //current event processed
        public List<Point> intersections = new List<Point>(); //list of output intersection points
        public List<Event> output = new List<Event>(); //list of output intersection events(to show intersecting segments)
       
        
        public int get_line_intersection(double p0_x, double p0_y, double p1_x, double p1_y,
            double p2_x, double p2_y, double p3_x, double p3_y, ref double i_x,ref double i_y, ref Point Points)
        {
            double s1_x, s1_y, s2_x, s2_y;
            s1_x = p1_x - p0_x; s1_y = p1_y - p0_y;
            s2_x = p3_x - p2_x; s2_y = p3_y - p2_y;

            double s, t;
            s = (-s1_y * (p0_x - p2_x) + s1_x * (p0_y - p2_y)) / (-s2_x * s1_y + s1_x * s2_y);
            t = (s2_x * (p0_y - p2_y) - s2_y * (p0_x - p2_x)) / (-s2_x * s1_y + s1_x * s2_y);

            Points = null;
            if (s >= 0 && s <= 1 && t >= 0 && t <= 1)
            {
                // Collision detected
                if (i_x != null)
                    i_x = p0_x + (t * s1_x);
                if (i_y != null)
                    i_y = p0_y + (t * s1_y);
                Points = new Point(i_x, i_y);
                return 1;
            }

            return 0; // No collision
        }
        
        public override void Run(List<CGUtilities.Point> points, List<CGUtilities.Line> lines, List<CGUtilities.Polygon> polygons, ref List<CGUtilities.Point> outPoints, ref List<CGUtilities.Line> outLines, ref List<CGUtilities.Polygon> outPolygons)
        {


            //initializaing event queue

            for (int i = 0; i < lines.Count(); i++)
            {   //start event
                eventqueue.Add(new Event(lines[i].Start, Enums.EventType.Start, i));

                
                //end event
                eventqueue.Add(new Event(lines[i].End, Enums.EventType.End, i));

                
            }

            while (eventqueue.Count() > 0) //while queue not empty
            {
                Event curr = eventqueue.ElementAt(0);
                double keys = eventqueue.ElementAt(0).p.X;
                currKey = keys;
                eventqueue.Remove(curr);//remove event
                HandleEvent(curr, lines);//handle event

            }
            outPoints = intersections;
            
        }
        public void HandleEvent(Event eve, List<Line> lines)
        {
            if (eve.enums == Enums.EventType.Start)//if start event
            {

                sweeps.Add(new sweep(eve.p.Y, eve.SegmentIndex));
                int next;
                int prev;

                
                foreach (var item in sweeps.Select((value, index) => new { index, value }))
                {
                    if (item.value.y == eve.p.Y)
                    {

                        next = item.index + 1;
                        if(next<sweeps.Count())//check for intersection of inserted segment with next segment in sweepline
                            CheckIntersect(lines[sweeps.ElementAt(next).index], lines[eve.SegmentIndex], sweeps.ElementAt(next).index, eve.SegmentIndex);
                        prev = item.index - 1;
                        if(prev >= 0)
                            CheckIntersect(lines[eve.SegmentIndex], lines[sweeps.ElementAt(prev).index], eve.SegmentIndex,sweeps.ElementAt(prev).index);//check prev
                        break;
                    }

                }
             
            }
            else if (eve.enums == Enums.EventType.End)//if end event
            {

                int next;
                int prev;
       
                foreach (var item in sweeps.Select((value, index) => new { index, value }))
                {
                    if (item.value.index == eve.SegmentIndex)
                    {
                        
                        next = item.index+1;
                        prev = item.index - 1;
                        if (sweeps.Count() < next && prev >=0)
                        {
                            CheckIntersect(lines[sweeps.ElementAt(next).index], lines[sweeps.ElementAt(prev).index], sweeps.ElementAt(next).index, sweeps.ElementAt(prev).index);//check prev with next
                        }
                        sweeps.Remove( sweeps.ElementAt(item.index));//remove line
                        break;
                    }

                }
               

               


            }
            else if (eve.enums == Enums.EventType.Intersection)//if intersection event
            {
                int seg1 = eve.SegmentIndex;//higher seg
                int seg2 = eve.Segment2Index;
                double seg1y = sweeps.First(w => w.index == seg1).y;
                double seg2y = sweeps.First(w => w.index == seg2).y;
                int sweeplineIndex;
                int next = 0;
                int prev = 0 ;
                sweep nexts = new sweep();
                sweep below = new sweep();
                foreach (var item in sweeps.Select((value, index) => new {index, value }))
                {
                    if (item.value.y == seg2y)
                    {

                        prev = item.index - 1; //prev seg
                        if (prev>=0)
                        {
                            below = sweeps.ElementAt(prev);
                        }
                    }
                    if (item.value.y >seg1y)
                    {

                        next = item.index;
                        if (next < sweeps.Count())
                            nexts = item.value;
                        

                        break;
                             
                    }

                }
                CheckIntersect(lines[nexts.index], lines[eve.Segment2Index], nexts.index, eve.Segment2Index);//checks1 with prev
                CheckIntersect(lines[eve.SegmentIndex], lines[below.index], eve.SegmentIndex,below.index);//checkhigher y with above

                //swap lines
                
                sweep temp = new sweep();
                sweep temp2 = new sweep();
                if(next==0) temp = sweeps.ElementAt(sweeps.Count()-1);
                else
                  temp = sweeps.ElementAt(next - 1); //higher
                temp2 = sweeps.ElementAt(prev + 1);//lower
               
                sweeps.Remove(temp); //higher
                sweeps.Add(new sweep(temp.y, temp2.index));
                sweeps.Remove(temp2);
                sweeps.Add(new sweep(temp2.y, temp.index));





            }

        }
        public void CheckIntersect(Line s1, Line s2,int index1, int index2)//s1 above s2 (has higher y)
        {
            
            if(HelperMethods.CheckTurn(s1,s2.Start)!= HelperMethods.CheckTurn(s1, s2.End)){//they intersect
                double x=0.000001;
                double y=0.000001;
                Point inter = null; ;
                int intersects = get_line_intersection(s1.Start.X, s1.Start.Y, s1.End.X, s1.End.Y, s2.Start.X, s2.Start.Y, s2.End.X, s2.End.Y,ref x,ref y,ref inter);
                if (intersects == 1)
                {
                    if ((!eventqueue.Contains(new Event(inter, Enums.EventType.Intersection, index1, index2))) && (inter.X > currKey))
                    {
                        eventqueue.Add(new Event(inter, Enums.EventType.Intersection, index1, index2));
                        intersections.Add(inter);
                        output.Add(new Event(inter, Enums.EventType.Intersection, index1, index2));
                    }
                }
            }
        }
       
    
        public override string ToString()
        {
            return "Sweep Line";
        }
    }
}
