using CGUtilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace CGAlgorithms.Algorithms.ConvexHull
{
    

    public class DivideAndConquer : Algorithm
    {
        public List<Point> getPoints(List<Point> points, int start, int end)
        {
            List<Point> outPoints = new List<Point>();
            for (int i = start; i <= end; i++)
            {
                outPoints.Add(points[i]);
            }
            return outPoints;
        }
        public int findMaxInLeftHull(List<Point> left_hull)
        {
            int maxInd = 0;
            double max_X_coord = left_hull[0].X;
            for (int i = 1; i < left_hull.Count; i++)
            {
                if(left_hull[i].X>=max_X_coord)
                {
                    max_X_coord = left_hull[i].X;
                    maxInd = i;
                }
            }
            return maxInd;
        }
        public int findMinInRighttHull(List<Point> right_hull)
        {
            int minInd = 0;
            double min_X_coord = right_hull[0].X;
            for (int i = 1; i < right_hull.Count; i++)
            {
                if (right_hull[i].X < min_X_coord)
                {
                    min_X_coord = right_hull[i].X;
                    minInd = i;
                }
            }
            return minInd;
        }
        public double findMaXY(List<Point> points)
        {
            int maxInd = 0;
            double max_Y_coord = points[0].Y;
            for (int i = 1; i < points.Count; i++)
            {
                if (points[i].Y >= max_Y_coord)
                {
                    max_Y_coord = points[i].Y;
                    maxInd = i;
                }
            }
            return max_Y_coord;
        }
        public double findMinY(List<Point> points)
        {
            int minInd = 0;
            double min_Y_coord = points[0].Y;
            for (int i = 1; i < points.Count; i++)
            {
                if (points[i].Y <= min_Y_coord)
                {
                    min_Y_coord = points[i].Y;
                    minInd = i;
                }
            }
            return min_Y_coord;
        }
        public List<Point> mergeHulls(List<Point> left_hull, List<Point> right_hull, double minY, double maxY,Point mid)
        {
            

            List<Point> points = new List<Point>();
            int MaxLH = findMaxInLeftHull(left_hull);
            int MinRH = findMinInRighttHull(right_hull);
            double mid_point = (left_hull[MaxLH].X + right_hull[MinRH].X) / 2;

            Line vertical_line = new Line(new Point(mid_point, maxY + 10), new Point(mid_point, minY - 10));
            int ULP = MaxLH;
            int URP = MinRH;
            bool brk = false;
            double max_intersection_point = HelperMethods.Intersection_between_2Lines(new Line(left_hull[ULP], right_hull[URP]), vertical_line,"max");
            while (!brk)
            {
                brk = true;
                while (true)
                {
                    
                    Line l = new Line(  left_hull[ULP],right_hull[(right_hull.Count + URP - 1) % right_hull.Count]);
                   

                    if (HelperMethods.Intersection_between_2Lines(l, vertical_line, "max") > max_intersection_point)  {
                        URP = (right_hull.Count + URP - 1) % right_hull.Count;
                        max_intersection_point = HelperMethods.Intersection_between_2Lines(l, vertical_line, "max");
                        //ULP = (ULP + 1) % left_hull.Count;
                        brk = false;
                        continue;
                    }
                    break;
                }

                while (true)
                {

                    Line l = new Line(right_hull[ URP ], left_hull[(ULP+1)%left_hull.Count]);

                    if (HelperMethods.Intersection_between_2Lines(l, vertical_line, "max") > max_intersection_point)   {

                        ULP = (ULP + 1) % left_hull.Count;
                        max_intersection_point = HelperMethods.Intersection_between_2Lines(l, vertical_line, "max");
                        brk = false;
                        continue;
                    }
                    
                    break;
                }
            }
            int upperLP = ULP, upperRP = URP;
             ULP = MaxLH;
             URP = MinRH;

            if(left_hull[ULP].Y==minY)
            {
                points.Add(left_hull[ULP]);


            }
            double min_intersection_point = HelperMethods.Intersection_between_2Lines(new Line(left_hull[ULP], right_hull[URP]), vertical_line, "max");
            brk = false;
            while (!brk)
            {
                brk = true;
               

                while (true)
                {


                    Line l = new Line(left_hull[ULP], right_hull[( URP +1) % right_hull.Count]);


                    if (HelperMethods.Intersection_between_2Lines(l, vertical_line,"min") < min_intersection_point)  {
                        URP = (URP + 1) % right_hull.Count;
                        min_intersection_point = HelperMethods.Intersection_between_2Lines(l, vertical_line,"min");
                        
                        brk = false;
                        continue;
                    }
                    break;
                }
                while (true)
                {
                    Line l = new Line(right_hull[URP], left_hull[(left_hull.Count+ULP - 1) % left_hull.Count]);

                    if (HelperMethods.Intersection_between_2Lines(l, vertical_line,"min")< min_intersection_point)
                        {
                        ULP = (left_hull.Count + ULP - 1) % left_hull.Count;
                        min_intersection_point = HelperMethods.Intersection_between_2Lines(l, vertical_line,"min");

                        brk = false;
                        continue;
                    }
                    break;
                }

            }
            
            int downLP = ULP, downRP = URP;
           
            int start = upperLP;
            points.Add(left_hull[start]);
           
            while (start!=downLP)
            {
                start = (start + 1) % left_hull.Count;
                points.Add(left_hull[start]);
            }
            start = downRP;
            points.Add(right_hull[start]);
            while (start != upperRP)
            {
                start = (start + 1) % right_hull.Count;
                points.Add(right_hull[start]);
            }
            return points;
        }
        public List<Point> divide(List<Point> points, ref List<Point> outPoints, ref List<Line> outLines, ref List<Polygon> outPolygons, double minY, double maxY)
        {

            if (points.Count < 6)
            {
               
               JarvisMarch ch = new JarvisMarch();
            
                ch.Run(points, new List<Line>(), new List<Polygon>(), ref outPoints, ref outLines, ref outPolygons);
                return outPoints;
            }
            int mid = (points.Count / 2) ;

            List<Point> lch = getPoints(points, 0, mid);
            List<Point> rch = getPoints(points, mid + 1, points.Count - 1);
              List<Point> outPoints1=new List<Point>();
            List<Point> left_hull = divide(lch, ref outPoints1, ref outLines, ref outPolygons,minY,maxY);
            List<Point> outPoints2 = new List<Point>();
            List<Point> right_hull = divide(rch,  ref outPoints2, ref outLines, ref outPolygons, minY, maxY);

            return mergeHulls(left_hull, right_hull, minY, maxY,points[mid]);

        }
        public override void Run(List<Point> points, List<Line> lines, List<Polygon> polygons, ref List<Point> outPoints, ref List<Line> outLines, ref List<Polygon> outPolygons)
        {
            double minY = findMinY(points);
            double maxY = findMaXY(points);
           
            HelperMethods.filterPoints(points);
            List<Point> sortedList = points.OrderBy(x => x.X).ThenBy(x=>x.Y).ToList();
            outPoints = divide(sortedList, ref outPoints, ref outLines, ref outPolygons,minY,maxY);


        }

        public override string ToString()
        {
            return "Convex Hull - Divide & Conquer";
        }

    }
}
