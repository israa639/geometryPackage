using CGUtilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CGAlgorithms.Algorithms.ConvexHull
{
    public class Incremental : Algorithm
    {
        public double Distance(Point a , Point b) //distance between two points
        {
            return( Math.Sqrt(Math.Pow(a.X - b.X, 2) + Math.Pow(a.Y - b.Y, 2)));
        }
        public double AngleMeasure(Point Center,Point a, Line s2)
        {
            Line s1 = new Line(Center, a);
            double theta1 = Math.Atan2(s1.Start.Y - s1.End.Y, s1.Start.X - s1.End.X);
            double theta2 = Math.Atan2(s2.Start.Y - s2.End.Y, s2.Start.X - s2.End.X);

            double diff = Math.Abs(theta1 - theta2) * 180 / Math.PI;
            diff = 360 - diff;
            return diff;
        }
        public SortedDictionary<double, Point> FindExtreme(List<Point> points)
        {
            Line BaseLine;
            SortedDictionary<double, Point> CH = new SortedDictionary<double, Point>();//angle sort
            SortedDictionary<double, Point> point = new SortedDictionary<double, Point>();//x axis sort
            

            Point one = (Point)points[0].Clone();
            Point two = (Point)points[1].Clone();
            Point three = (Point)points[2].Clone();
            int count = 0;
            while (HelperMethods.CheckTurn(new Line(one, two), three) == Enums.TurnType.Colinear)
            {
                three = (Point)points[2 + count].Clone();
                count++;
            }


            //center
            double X = (one.X + two.X + three.X) / (double)3;
            double Y = (one.Y + two.Y + three.Y) / (double)3;
            Point center = new Point(X, Y);
            //BaseLine
            BaseLine = new Line(center, new Point(center.X + 10000, center.Y));

            //add initial points
            Double angle1 = AngleMeasure(center, one, BaseLine);
            Double angle2 = AngleMeasure(center, two, BaseLine);
            Double angle3 = AngleMeasure(center, three, BaseLine);

            CH.Add(angle1, one);
            CH.Add(angle2, two);
            CH.Add(angle3, three);




            for (int i = 0; i < points.Count(); i++)
            {
                if (CH.ContainsValue(points[i])) continue;

                Point temp = (Point)points[i].Clone();
                double angle = AngleMeasure(center, temp, BaseLine);//angle between new point and baseline

                Point prev = new Point(0, 0); //prev
                Point next = new Point(0, 0);//next
                int prevIndex = 0;
                int nextIndex = 0;


                //if angle already exist
                if (CH.ContainsKey(angle))
                {
                    Point old;
                    CH.TryGetValue(angle, out old);
                    if (Distance(temp, center) > Distance(old, center)) //new point is farther from center
                    {
                        CH.Remove(angle);
                        CH.Add(angle, (Point)temp.Clone());
                        continue;
                    }
                    else
                    {
                        continue;
                    }
                }
                //
                //if it doesn't exist

                if (angle > CH.ElementAt(CH.Count() - 1).Key || angle < CH.ElementAt(0).Key)//biggest oe smallest angle
                {
                    next = (Point)CH.ElementAt(0).Value.Clone(); // next
                    nextIndex = 0;
                    prev = (Point)CH.ElementAt(CH.Count() - 1).Value.Clone(); //prev
                    prevIndex = CH.Count() - 1;

                }
                else
                {
                    foreach (var item in CH.Select((value, index) => new { index, value }))
                    {
                        if (item.value.Key > angle)
                        {
                            next = (Point)item.value.Value.Clone(); // next
                            nextIndex = item.index;
                            prev = (Point)CH.ElementAt(item.index - 1).Value.Clone(); //prev
                            prevIndex = item.index - 1;
                            break;
                        }

                    }
                }


                if (HelperMethods.CheckTurn(new Line(prev, next), temp) == Enums.TurnType.Right)//point outside polygon
                {//left support line
                    Point newPrev;
                    int tempIndex;
                    if (prevIndex - 1 >= 0) //if index not out of bounds
                    {
                        newPrev = (Point)CH.ElementAt(prevIndex - 1).Value.Clone(); //new prev
                        tempIndex = prevIndex - 1; //new prev index
                    }
                    else
                    {
                        newPrev = (Point)CH.ElementAt(CH.Count() - 1).Value.Clone(); //new prev
                        tempIndex = CH.Count() - 1; //new prev index
                    }

                    while (HelperMethods.CheckTurn(new Line(temp, prev), newPrev) == Enums.TurnType.Left)
                    {
                        prev = (Point)newPrev.Clone(); //prev = new prev
                        prevIndex = tempIndex;
                        if (tempIndex - 1 < 0)
                        {
                            newPrev = (Point)CH.ElementAt(CH.Count() - 1).Value.Clone(); //new prev = prev of new prev
                            tempIndex = CH.Count() - 1;
                        }
                        else
                        {
                            newPrev = (Point)CH.ElementAt(tempIndex - 1).Value.Clone(); //new prev = prev of new prev
                            tempIndex--;
                        }
                    }
                    //right support line
                    Point newNext;
                    if (nextIndex + 1 < CH.Count()) //if  index not out of bounds
                    {
                        newNext = (Point)CH.ElementAt(nextIndex + 1).Value.Clone(); //new next
                        tempIndex = nextIndex + 1; //new next index
                    }
                    else
                    {
                        newNext = (Point)CH.ElementAt(0).Value.Clone(); //new next
                        tempIndex = 0; //new next index
                    }
                    while (HelperMethods.CheckTurn(new Line(temp, next), newNext) == Enums.TurnType.Right)
                    {
                        next = (Point)newNext.Clone();
                        nextIndex = tempIndex;
                        if (tempIndex + 1 >= CH.Count())
                        {
                            newNext = (Point)CH.ElementAt(0).Value.Clone();
                            tempIndex = 0;
                        }
                        else
                        {
                            newNext = (Point)CH.ElementAt(tempIndex + 1).Value.Clone();
                            tempIndex++;
                        }
                    }

                    //remove inner chain
                    if (prevIndex > nextIndex)
                    {

                        for (int y = prevIndex + 1; y < CH.Count(); y++)
                        {
                            CH.Remove(CH.ElementAt(prevIndex + 1).Key);

                        }
                        for (int y = 0; y < nextIndex; y++)
                        {
                            CH.Remove(CH.ElementAt(0).Key);

                        }

                        CH.Add(angle, (Point)temp.Clone());
                        continue;
                    }
                    int x = prevIndex + 1;
                    for (int y = prevIndex + 1; y < nextIndex; y++)
                    {
                        CH.Remove(CH.ElementAt(x).Key);

                    }

                    //add point
                    CH.Add(angle, (Point)temp.Clone());


                }


                
            }
            for (int b = 1; b < CH.Count - 1; b++)
            {
                if (HelperMethods.PointOnSegment(CH.ElementAt(b).Value, CH.ElementAt(b - 1).Value, CH.ElementAt(b + 1).Value))
                    CH.Remove(CH.ElementAt(b).Key);
            }
            return CH;
        }
        public override void Run(List<Point> points, List<Line> lines, List<Polygon> polygons, ref List<Point> outPoints, ref List<Line> outLines, ref List<Polygon> outPolygons)
        {

            if (points.Count() <= 3)
            {
                outPoints = points;
                return;
            }
            SortedDictionary<double, Point> CH1 = Find_Extreme(points);
            for (int b = 0; b < CH1.Count; b++)
                outPoints.Add(CH1.ElementAt(b).Value);

        }
       
        public override string ToString()
        {
            return "Convex Hull - Incremental";
        }



        public SortedDictionary<double, Point> Find_Extreme(List<Point> points)
        {
            SortedDictionary<double, Point> CH1 = FindExtreme(points);
            List<Point> outs = new List<Point>();
            for (int b = 0; b < CH1.Count; b++)
                outs.Add(CH1.ElementAt(b).Value);

            return FindExtreme(outs);

        }
    }
}
