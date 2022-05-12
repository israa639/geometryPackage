using CGUtilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CGAlgorithms.Algorithms.ConvexHull
{
    public class QuickHull : Algorithm
    {
       double getMagnitudeOfVector(Line l)
        {
            return (double)Math.Sqrt(Math.Pow((l.Start.X - l.End.X), 2) + Math.Pow((l.Start.Y - l.End.Y), 2));
        }
        double getDistanceBetween_Line_Point(Point p,Line l)
        {
           double a = getMagnitudeOfVector(new Line(l.Start, l.End));
          double  b = getMagnitudeOfVector(new Line(l.Start, p));
          double  c = getMagnitudeOfVector(new Line(l.End, p));
            double s = (a + b + c) / 2;
            double dist = 2 * Math.Sqrt(s * (s - a) * (s - b) * (s - c)) / a;
            return dist;

        }
        int GetMaxDistance(List<Point> points, Line l)
        {
            double MaxDistance = getDistanceBetween_Line_Point(points[0],l);
            int MaxDistanceIndex=0;
            double dist;
            for(int i=1;i<points.Count;i++)
            {
                dist= getDistanceBetween_Line_Point(points[i], l);
                if(dist>MaxDistance)
                {
                    MaxDistance = dist;
                    MaxDistanceIndex = i;
                }

            }
            return MaxDistanceIndex;
        }
        List<Point> QuickHullRec(List<Point> points, Line l, List<Point> list)
        {
            if(points.Count==0)
            {
                return list;
            }
            int maxDistIndex = GetMaxDistance(points,l);

            Line leftline= new Line(l.Start, points[maxDistIndex]);
            List<Point> LeftPoints = getUpperPointsList(points, leftline);
            List<Point> Left_hull= QuickHullRec(LeftPoints, leftline,list);
            

            Line rightline = new Line( points[maxDistIndex],l.End);
            List<Point> rightPoints = getUpperPointsList(points, rightline);
            List<Point> Right_hull= QuickHullRec(rightPoints, rightline,list);


            Left_hull= Left_hull.Concat(Right_hull).ToList();
            Left_hull.Add(points[maxDistIndex]);
            return Left_hull;
           

        }
        List<Point> getUpperPointsList(List<Point> points,Line l)
        {
            List<Point> Upper_points = new List<Point>();
            for (int i=0; i<points.Count;i++)
            {
                if(HelperMethods.CheckTurn(l,points[i])== Enums.TurnType.Left)
                {
                    Upper_points.Add(points[i]);
                }
            }
            return Upper_points;

        }
        List<Point> getLowerPointsList(List<Point> points, Line l)
        {
            List<Point> Lower_points = new List<Point>();
            for (int i = 0; i < points.Count; i++)
            {
                if (HelperMethods.CheckTurn(l, points[i]) == Enums.TurnType.Right)
                {
                    Lower_points.Add(points[i]);
                }
            }
            return Lower_points;
        }
        public override void Run(List<Point> points, List<Line> lines, List<Polygon> polygons, ref List<Point> outPoints, ref List<Line> outLines, ref List<Polygon> outPolygons)
        {
            
            HelperMethods.filterPoints(points);
            if (points.Count == 1)
            {
                outPoints.Add(points[0]);
            }
            else
            {
                int maxIndex = HelperMethods.findMaxPoint(points);
                int minIndex = HelperMethods.findMinpoint(points);
                Point MinX = points[minIndex];
                Point MaxX = points[maxIndex];
                Line Upper_divider = new Line(MinX, MaxX);
                List<Point> Upper_points = getUpperPointsList(points, Upper_divider);
                List<Point> Lower_points = getLowerPointsList(points, Upper_divider);
                List<Point> UpperHull = QuickHullRec(Upper_points, Upper_divider, new List<Point>());

                Line Lower_divider = new Line(MaxX, MinX);
                List<Point> LowerrHull = QuickHullRec(Lower_points, Lower_divider, new List<Point>());

                UpperHull.Add(points[maxIndex]);
                LowerrHull.Add(points[minIndex]);

                outPoints = UpperHull.Concat(LowerrHull).ToList();
            }
           // HelperMethods.filterPoints(outPoints);
            
        }

        public override string ToString()
        {
            return "Convex Hull - Quick Hull";
        }
    }
}
