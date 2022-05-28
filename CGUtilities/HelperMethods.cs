﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CGUtilities
{
    public class HelperMethods
    {










	public static double area(double x1, double y1, double x2,
                    double y2, double x3, double y3)
	{
		return Math.Abs((x1 * (y2 - y3) +
						x2 * (y3 - y1) +
						x3 * (y1 - y2)) / 2.0);
	}


	public static bool isInside(double x1, double y1, double x2,
                    double y2, double x3, double y3,
                        double x, double  y)
	{
		
		double A = area(x1, y1, x2, y2, x3, y3);

	
		double A1 = area(x, y, x2, y2, x3, y3);

		double A2 = area(x1, y1, x, y, x3, y3);

	
		double A3 = area(x1, y1, x2, y2, x, y);

		return (A == A1 + A2 + A3);
	}



        public static Enums.PointInPolygon PointInTriangle(Point p, Point a, Point b, Point c)
        {
            if (a.Equals(b) && b.Equals(c))
            {
                if (p.Equals(a) || p.Equals(b) || p.Equals(c))
                    return Enums.PointInPolygon.OnEdge;
                else
                    return Enums.PointInPolygon.Outside;
            }

            Line ab = new Line(a, b);
            Line bc = new Line(b, c);
            Line ca = new Line(c, a);

            if (GetVector(ab).Equals(Point.Identity)) return (PointOnSegment(p, ca.Start, ca.End)) ? Enums.PointInPolygon.OnEdge : Enums.PointInPolygon.Outside;
            if (GetVector(bc).Equals(Point.Identity)) return (PointOnSegment(p, ca.Start, ca.End)) ? Enums.PointInPolygon.OnEdge : Enums.PointInPolygon.Outside;
            if (GetVector(ca).Equals(Point.Identity)) return (PointOnSegment(p, ab.Start, ab.End)) ? Enums.PointInPolygon.OnEdge : Enums.PointInPolygon.Outside;

            if (CheckTurn(ab, p) == Enums.TurnType.Colinear)
                return PointOnSegment(p, a, b)? Enums.PointInPolygon.OnEdge : Enums.PointInPolygon.Outside;
            if (CheckTurn(bc, p) == Enums.TurnType.Colinear && PointOnSegment(p, b, c))
                return PointOnSegment(p, b, c) ? Enums.PointInPolygon.OnEdge : Enums.PointInPolygon.Outside;
            if (CheckTurn(ca, p) == Enums.TurnType.Colinear && PointOnSegment(p, c, a))
                return PointOnSegment(p, a, c) ? Enums.PointInPolygon.OnEdge : Enums.PointInPolygon.Outside;

            if (CheckTurn(ab, p) == CheckTurn(bc, p) && CheckTurn(bc, p) == CheckTurn(ca, p))
                return Enums.PointInPolygon.Inside;
            return Enums.PointInPolygon.Outside;
        }
        public static Enums.TurnType CheckTurn(Point vector1, Point vector2)
        {
            double result = CrossProduct(vector1, vector2);
            if (result < 0) return Enums.TurnType.Right;
            else if (result > 0) return Enums.TurnType.Left;
            else return Enums.TurnType.Colinear;
        }
        public static double dotProduct(Point vector1, Point vector2)
        {
            double crossProd = ((vector1.X * vector2.X) + (vector1.Y * vector2.Y));
            crossProd /=( Math.Sqrt((vector1.X * vector1.X)+ (vector1.Y * vector1.Y)) * Math.Sqrt((vector2.X * vector2.X) + (vector2.Y * vector2.Y)));
            return Math.Acos(crossProd);
        }

        public static double CrossProduct(Point a, Point b)
        {
            return a.X * b.Y - a.Y * b.X;
        }
        public static bool PointOnRay(Point p, Point a, Point b)
        {
            if (a.Equals(b)) return true;
            if (a.Equals(p)) return true;
            var q = a.Vector(p).Normalize();
            var w = a.Vector(b).Normalize();
            return q.Equals(w);
        }
        public static double distanceBetween2Points(Point a, Point b)
        {
            return Math.Sqrt(Math.Pow((a.X-b.X),2)+ Math.Pow((a.Y - b.Y), 2));
        }
        public static bool PointOnSegment(Point p, Point a, Point b)
        {
            if (a.Equals(b))
                return p.Equals(a);

            if (b.X == a.X)
                return p.X == a.X && (p.Y >= Math.Min(a.Y, b.Y) && p.Y <= Math.Max(a.Y, b.Y));
            if (b.Y == a.Y)
                return p.Y == a.Y && (p.X >= Math.Min(a.X, b.X) && p.X <= Math.Max(a.X, b.X));
            double tx = (p.X - a.X) / (b.X - a.X);
            double ty = (p.Y - a.Y) / (b.Y - a.Y);

            return (Math.Abs(tx - ty) <= Constants.Epsilon && tx <= 1 && tx >= 0);
        }
        /// <summary>
        /// Get turn type from cross product between two vectors (l.start -> l.end) and (l.end -> p)
        /// </summary>
        /// <param name="l"></param>
        /// <param name="p"></param>
        /// <returns></returns>
        public static Enums.TurnType CheckTurn(Line l, Point p)
        {
            Point a = l.Start.Vector(l.End);
            Point b = l.End.Vector(p);
            return HelperMethods.CheckTurn(a, b);
        }
        public static Point GetVector(Line l)
        {
            return l.Start.Vector(l.End);
        }
        public static void filterPoints(List<Point>points)
        {
            Dictionary<KeyValuePair<double, double>, int> visted = new Dictionary<KeyValuePair<double, double>, int>();
            for (int i = 0; i < points.Count; i++)
            {
                var pointi = new KeyValuePair<double, double>(points[i].X, points[i].Y);
                if (visted.ContainsKey(pointi) == false)
                {
                    visted[new KeyValuePair<double, double>(points[i].X, points[i].Y)] = i;

                }
                else
                {
                    points.RemoveAt(i);
                }
            }

        }
        public static  int findMaxPoint(List<Point> points)
        {
            int maxInd = 0;
            double max_X_coord = points[0].X;
            for (int i = 1; i < points.Count; i++)
            {
                if (points[i].X > max_X_coord)
                {
                    max_X_coord = points[i].X;
                    maxInd = i;
                }
            }
            return maxInd;
        }
        public static  int findMinpoint(List<Point> points)
        {
            int minInd = 0;
            double min_X_coord = points[0].X;
            for (int i = 1; i < points.Count; i++)
            {
                if (points[i].X < min_X_coord)
                {
                    min_X_coord = points[i].X;
                    minInd = i;
                }
            }
            return minInd;
        }
        public static  double Intersection_between_2Lines(Line l1,Line l2,string maxORmin)
        {
            double a1 = l1.End.Y - l1.Start.Y;
            double b1 = l1.Start.X - l1.End.X;
            double c1 = a1 * (l1.Start.X) + b1 * l1.Start.Y;


            // Line CD represented as a2x + b2y = c2
            double a2 = l2.End.Y - l2.Start.Y;
            double b2 = l2.Start.X - l2.End.X;
            double c2 = a2 * l2.Start.X + b2 * l2.Start.Y;
           

            double determinant =Math.Abs( a1 * b2 - a2 * b1);

            if (determinant == 0)
            {
                if (maxORmin == "max")
                {
                    return l1.Start.Y > l1.End.Y ? l1.Start.Y : l1.End.Y;

                }
                else
                { return l1.Start.Y < l1.End.Y ? l1.Start.Y : l1.End.Y; }

            }
            else
            {
                //double x = Math.Abs((b2 * c1 - b1 * c2) / determinant);
                double y = Math.Abs((a1 * c2 - a2 * c1) / determinant);
                return  y;
            }
        }
    }
}
