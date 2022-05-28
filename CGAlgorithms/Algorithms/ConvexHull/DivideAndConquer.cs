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
        public List<Point> mergeHulls(List<Point> left_hull, List<Point> right_hull)
        {
            int MaxLH = findMaxInLeftHull(left_hull);
            int MinRH = findMinInRighttHull(right_hull);
            int ULP = MaxLH;
            int URP = MinRH;
            bool brk = false;
            while (!brk)
            {
                brk = true;
                while (true)
                {
                    
                    Line l = new Line( left_hull[(ULP + 1)%left_hull.Count], left_hull[ULP]);

                    if (HelperMethods.CheckTurn(l, right_hull[URP]) == Enums.TurnType.Left)
                    {
                        ULP = (ULP + 1) % left_hull.Count;
                        continue;
                    }
                    break;
                }

                while (true)
                {

                    Line l = new Line(right_hull[(right_hull.Count + URP - 1) % right_hull.Count], right_hull[URP]);

                    if (HelperMethods.CheckTurn(l, left_hull[ULP]) == Enums.TurnType.Right)
                    {
                        URP = (right_hull.Count + URP - 1) % right_hull.Count;
                        brk = false;
                        continue;
                    }
                    break;
                }
            }
            int upperLP = ULP, upperRP = URP;
             ULP = MaxLH;
             URP = MinRH;
            brk = false;
            while (!brk)
            {
                brk = true;
               

                while (true)
                {

                    Line l = new Line(right_hull[URP],right_hull[(URP + 1)%right_hull.Count]);

                    if (HelperMethods.CheckTurn(l, left_hull[ULP]) == Enums.TurnType.Right)
                    {
                        URP =(URP+ 1)%right_hull.Count;
                        
                        continue;
                    }
                    break;
                }
                while (true)
                {
                    Line l = new Line(left_hull[ULP], left_hull[(left_hull.Count + ULP - 1) % left_hull.Count]);

                    if (HelperMethods.CheckTurn(l, right_hull[URP]) == Enums.TurnType.Left)
                    {
                        ULP = (left_hull.Count + ULP - 1) % left_hull.Count;
                        brk = false;
                        continue;
                    }
                    break;
                }

            }
            
            int downLP = ULP, downRP = URP;
            List<Point> points = new List<Point>();
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
        public List<Point> divide(List<Point> points, ref List<Point> outPoints, ref List<Line> outLines, ref List<Polygon> outPolygons)
        {

            if (points.Count < 6)
            {

                JarvisMarch ch = new JarvisMarch();
            
                ch.Run(points, new List<Line>(), new List<Polygon>(), ref outPoints, ref outLines, ref outPolygons);
                return outPoints;
            }
            int mid = (points.Count / 2) -1;

            List<Point> lch = getPoints(points, 0, mid);
            List<Point> rch = getPoints(points, mid + 1, points.Count - 1);
              List<Point> outPoints1=new List<Point>();
            List<Point> left_hull = divide(lch, ref outPoints1, ref outLines, ref outPolygons);
            List<Point> outPoints2 = new List<Point>();
            List<Point> right_hull = divide(rch,  ref outPoints2, ref outLines, ref outPolygons);

            return mergeHulls(left_hull, right_hull);

        }
        public override void Run(List<Point> points, List<Line> lines, List<Polygon> polygons, ref List<Point> outPoints, ref List<Line> outLines, ref List<Polygon> outPolygons)
        {
            HelperMethods.filterPoints(points);
            List<Point> sortedList = points.OrderBy(x => x.X).ToList();//.ThenBy(y => y.Y).ToList();
            outPoints = divide(sortedList, ref outPoints, ref outLines, ref outPolygons);


        }

        public override string ToString()
        {
            return "Convex Hull - Divide & Conquer";
        }

    }
}
