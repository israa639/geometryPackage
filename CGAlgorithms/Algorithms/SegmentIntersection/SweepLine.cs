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


    public class SweepLine : Algorithm
    {
        public SortedDictionary<double, int> SweepLines = new SortedDictionary<double, int>(); //sweepline(Y-cooridante(order by y), index of segment in points list)
        public SortedDictionary<double, Event> eventQueue = new SortedDictionary<double, Event>(); //EventQueue(X-coordinate of point,Event description)
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
                if (!eventQueue.ContainsKey(lines[i].Start.X)) 
                    eventQueue.Add(lines[i].Start.X, new Event(lines[i].Start, Enums.EventType.Start, i));
                else //handle 2 points with same X
                {
                    Event temp;
                    eventQueue.TryGetValue(lines[i].Start.X, out temp);
                    if (temp.p.Y < lines[i].Start.Y)
                        eventQueue.Add(lines[i].Start.X + 0.00000001, new Event(lines[i].Start, Enums.EventType.Start, i));
                    else
                        eventQueue.Add(lines[i].Start.X - 0.00000001, new Event(lines[i].Start, Enums.EventType.Start, i));

                }
                //end event
                if (!eventQueue.ContainsKey(lines[i].End.X))
                    eventQueue.Add(lines[i].End.X, new Event(lines[i].End, Enums.EventType.End, i));
                else  //handle 2 points with same X
                {
                    Event temp;
                    eventQueue.TryGetValue(lines[i].End.X, out temp);
                    if (temp.p.Y< lines[i].End.Y)
                        eventQueue.Add(lines[i].End.X + 0.00000001, new Event(lines[i].End, Enums.EventType.End, i));
                    else
                        eventQueue.Add(lines[i].End.X - 0.00000001, new Event(lines[i].End, Enums.EventType.End, i));

                }
            }

            while (eventQueue.Count() > 0) //while queue not empty
            {
                Event curr = eventQueue.ElementAt(0).Value;
                double keys = eventQueue.ElementAt(0).Key;
                currKey = keys;
                eventQueue.Remove(keys);//remove event
                HandleEvent(curr, lines);//handle event

            }
            outPoints = intersections;
            
        }
        public void HandleEvent(Event eve, List<Line> lines)
        {
            if (eve.enums == Enums.EventType.Start)//if start event
            {

                SweepLines.Add(eve.p.Y, eve.SegmentIndex);
                int next;
                int prev;


                foreach (var item in SweepLines.Select((value, index) => new { index, value }))
                {
                    if (item.value.Key == eve.p.Y)
                    {

                        next = item.index + 1;
                        if(next<SweepLines.Count())//check for intersection of inserted segment with next segment in sweepline
                            CheckIntersect(lines[SweepLines.ElementAt(next).Value], lines[eve.SegmentIndex], SweepLines.ElementAt(next).Value, eve.SegmentIndex);
                        prev = item.index - 1;
                        if(prev >= 0)
                            CheckIntersect(lines[eve.SegmentIndex], lines[SweepLines.ElementAt(prev).Value], eve.SegmentIndex,SweepLines.ElementAt(prev).Value);//check prev
                        break;
                    }

                }
             
            }
            else if (eve.enums == Enums.EventType.End)//if end event
            {

                int next;
                int prev;
       
                foreach (var item in SweepLines.Select((value, index) => new { index, value }))
                {
                    if (item.value.Value == eve.SegmentIndex)
                    {
                        
                        next = item.index+1;
                        prev = item.index - 1;
                        if (SweepLines.Count() < next && prev >=0)
                        {
                            CheckIntersect(lines[SweepLines.ElementAt(next).Value], lines[SweepLines.ElementAt(prev).Value], SweepLines.ElementAt(next).Value, SweepLines.ElementAt(prev).Value);//check prev with next
                        }
                        SweepLines.Remove( SweepLines.ElementAt(item.index).Key);//remove line
                        break;
                    }

                }
               

               


            }
            else if (eve.enums == Enums.EventType.Intersection)//if intersection event
            {
                int seg1 = eve.SegmentIndex;//higher seg
                int seg2 = eve.Segment2Index;
                double seg1y = SweepLines.First(w => w.Value == seg1).Key;
                double seg2y = SweepLines.First(w => w.Value == seg2).Key;
                int sweeplineIndex;
                int next = 0;
                int prev = 0 ;
                KeyValuePair<double, int> nexts = new KeyValuePair<double, int>();
                KeyValuePair<double, int> below = new KeyValuePair<double, int>();
                foreach (var item in SweepLines.Select((value, index) => new {index, value }))
                {
                    if (item.value.Key == seg2y)
                    {

                        prev = item.index - 1; //prev seg
                        if (prev>=0)
                        {
                            below = SweepLines.ElementAt(prev);
                        }
                    }
                    if (item.value.Key >seg1y)
                    {

                        next = item.index;
                        if (next < SweepLines.Count())
                            nexts = item.value;
                        

                        break;
                             
                    }

                }
                CheckIntersect(lines[nexts.Value], lines[eve.Segment2Index], nexts.Value, eve.Segment2Index);//checks1 with prev
                CheckIntersect(lines[eve.SegmentIndex], lines[below.Value], eve.SegmentIndex,below.Value);//checkhigher y with above

                //swap lines
                
                KeyValuePair<double, int> temp = new KeyValuePair<double, int>();
                KeyValuePair<double, int> temp2 = new KeyValuePair<double, int>();
                if(next==0) temp = SweepLines.ElementAt(SweepLines.Count()-1);
                else
                  temp = SweepLines.ElementAt(next - 1); //higher
                temp2 = SweepLines.ElementAt(prev + 1);//lower
               
                SweepLines.Remove(temp.Key); //higher
                SweepLines.Add(temp.Key, temp2.Value);
                SweepLines.Remove(temp2.Key);
                SweepLines.Add(temp2.Key, temp.Value);





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
                    if ((!eventQueue.ContainsKey(inter.X)) && (inter.X > currKey))
                    {
                        eventQueue.Add(inter.X, new Event(inter, Enums.EventType.Intersection, index1, index2));
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
