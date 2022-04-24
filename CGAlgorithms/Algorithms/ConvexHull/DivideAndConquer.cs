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
        public List<Point> mergeHulls(List<Point> left_hull, List<Point> right_hull)
        {

        }
        public List<Point> divide(List<Point> points, List<Line> lines, List<Polygon> polygons, ref List<Point> outPoints, ref List<Line> outLines, ref List<Polygon> outPolygons)
        {

            if (points.Count < 6)
            {
                JarvisMarch jv = new JarvisMarch();
                jv.Run(points, lines, polygons, ref outPoints, ref outLines, ref outPolygons);
                return outPoints;
            }
            int mid = (points.Count / 2) - 1;

            List<Point> lch = getPoints(points, 0, mid);
            List<Point> rch = getPoints(points, mid + 1, points.Count - 1);
            List<Point> left_hull = divide(lch, lines, polygons, ref outPoints, ref outLines, ref outPolygons);
            List<Point> right_hull = divide(rch, lines, polygons, ref outPoints, ref outLines, ref outPolygons);

            return mergeHulls(left_hull, right_hull);

        }
        public override void Run(List<Point> points, List<Line> lines, List<Polygon> polygons, ref List<Point> outPoints, ref List<Line> outLines, ref List<Polygon> outPolygons)
        {
            HelperMethods.filterPoints(points);
            List<Point> sortedList = points.OrderBy(x => x.X).ToList();//.ThenBy(y => y.Y).ToList();


        }

        public override string ToString()
        {
            return "Convex Hull - Divide & Conquer";
        }

    }
}
