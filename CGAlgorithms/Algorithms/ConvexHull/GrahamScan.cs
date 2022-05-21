using CGUtilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CGAlgorithms.Algorithms.ConvexHull
{
  
    public class GrahamScan : Algorithm
    {
        
        public double AngleMeasure(Point Center, Point a, Line s2)
        {
            Line s1 = new Line(Center, a);
            double theta1 = Math.Atan2(s1.Start.Y - s1.End.Y, s1.Start.X - s1.End.X);
            double theta2 = Math.Atan2(s2.Start.Y - s2.End.Y, s2.Start.X - s2.End.X);
             
            double diff = Math.Abs(theta1 - theta2) * 180 / Math.PI;
            diff = 360 - diff;
            return diff;
        }

        public double Distance(Point a, Point b) //distance between two points
        {
            return (Math.Sqrt(Math.Pow(a.X - b.X, 2) + Math.Pow(a.Y - b.Y, 2)));
        }
        public List<Point> FindExtreme(List<Point> points)
        {
            Point minY = points[0];
            for (int i = 1; i < points.Count(); i++)
            {
                if (points[i].Y < minY.Y) { minY = points[i]; }
            }

            //measure angles around vertex
            SortedDictionary<double, Point> angles = new SortedDictionary<double, Point>();

            Line BaseLine = new Line(minY, new Point(minY.X + 3000, minY.Y));
            for (int i = 0; i < points.Count(); i++)
            {
                if (points[i] != minY)
                {
                    double ang = AngleMeasure(minY, points[i], BaseLine);
                    Point z;

                    if (angles.ContainsKey(ang) == false)
                        angles.Add(ang, points[i]);
                    else
                    {
                        while (angles.TryGetValue(ang, out z))
                        {
                            if(Distance(minY,z)< Distance(minY, points[i]))
                            { //if z is less far away from center(not extreme point) 
                                angles.Remove(ang); //remove z
                                angles.Add(ang, points[i]); // insert new point
                                break;
                            }
                            else //don't add new point as it is not extreme point
                            {

                                break; 

                            }
                           

                            
                        }


                    }


                }

            }
            Stack<Point> CH = new Stack<Point>();
            CH.Push(minY);
            CH.Push(angles.ElementAt(0).Value);
            angles.Remove(angles.ElementAt(0).Key);
            while (angles.Count() > 0)
            {
                Point pi = angles.ElementAt(0).Value;//current
                Point p = CH.Peek(); //top
                Point p_ = CH.ElementAt(1);//prev
                if (HelperMethods.CheckTurn(new Line(p_, p), pi) == Enums.TurnType.Left)
                {
                    CH.Push(pi);
                    angles.Remove(angles.ElementAt(0).Key);
                    continue;

                }
                else
                {
                    if(CH.Count()>=3) //if list has 3 or more points
                        do { CH.Pop(); p = CH.Peek(); p_ = CH.ElementAt(1); }
                        while (HelperMethods.CheckTurn(new Line(p_, p), pi) == Enums.TurnType.Right);

                    CH.Push(pi);
                }
                angles.Remove(angles.ElementAt(0).Key);
            }

            List<Point> elements = CH.ToList();            
           

            return elements;
        }
        public override void Run(List<Point> points, List<Line> lines, List<Polygon> polygons, ref List<Point> outPoints, ref List<Line> outLines, ref List<Polygon> outPolygons)
        {
            if (points.Count() <= 3) { outPoints = points; return; }

            List<Point> elements = Find_Extreme(points);
            for (int i= elements.Count - 1; i >=0; i--)
            {
                outPoints.Add(elements.ElementAt(i));
            }


        }

        public override string ToString()
        {
            return "Convex Hull - Graham Scan";
        }




        public List<Point> Find_Extreme(List<Point> points)
        {
            List<Point> elements = FindExtreme(points);
            return FindExtreme(elements);

        }

    }
}
