using CGUtilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CGAlgorithms.Algorithms.ConvexHull
{
    public class ExtremePoints : Algorithm
    {
        public double max(double p1,double p2)
        {
            return p1 > p2 ? p1 : p2;
        }
        public double min(double p1, double p2)
        {
            return p1 < p2 ? p1 : p2;
        }
        //public enum PointInPolygon
        //{
        //    Inside,
        //    Outside,
        //    OnEdge
        //}
        public override void Run(List<Point> points, List<Line> lines, List<Polygon> polygons, ref List<Point> outPoints, ref List<Line> outLines, ref List<Polygon> outPolygons)
        {
         
            

            bool breakLoop = false;
          
            for (int i = 0; i < points.Count(); i++)
            {
                breakLoop = false;
                for (int j = 0; j < points.Count(); j++)
                {
                    if (j == i)
                        continue;
                    if (breakLoop == true)
                        break;
                    
                    for (int k = 0; k < points.Count(); k++)
                    {
                        if (k == i)
                            continue;
                        if (k == j)
                            continue;
                        if (breakLoop == true)
                            break;
                        for (int l = 0; l < points.Count(); l++)
                        {
                            if (l == i)
                                continue;
                            if (l == k)
                                continue;
                            if (l == j)
                                continue;
                            var point_location = HelperMethods.PointInTriangle(points[i],points[j], points[k], points[l]);

                            if (point_location == Enums.PointInPolygon.Inside || point_location == Enums.PointInPolygon.OnEdge)
                            {
                                //points.Remove(points[i]);
                                breakLoop = true;
                                break;
                            }
                           

                        }
                    }
                }
                if(breakLoop==false)
                {
                    outPoints.Add(points[i]);
                }
            }
            
        }

        public override string ToString()
        {
            return "Convex Hull - Extreme Points";
        }
    }
}
