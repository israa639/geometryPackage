using CGUtilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CGAlgorithms.Algorithms.ConvexHull
{
    public class ExtremeSegments : Algorithm
    {

        public int Orientation(Point p, Point q, Point r)
        {
            double qpx = q.X - p.X;
            double qpy = q.Y - p.Y;
            double rpx = r.X - p.X;
            double rpy = r.Y - p.Y;
            double ans = (qpx * rpy) - (rpx * qpy);
            //same line : 0
            //turn left : 1
            //turn right: -1
            if (ans == 0) return 0;
            else if (ans > 0) return 1; //turn left
            else return -1; //turn right

        }
        public override void Run(List<Point> points, List<Line> lines, List<Polygon> polygons, ref List<Point> outPoints, ref List<Line> outLines, ref List<Polygon> outPolygons)
        {
            bool segmentFound = false;
            for (int i = 0; i < points.Count(); i++) //first point in segment
            {

                for (int j = 0; j < points.Count(); j++) //second point in segment
                {
                    if (segmentFound) { segmentFound = false; break; }//start from last added point
                    if (j == i) continue;
                    int check = -5;
                    int first_ori = 0;
                    bool belong = true;


                    //check if al points pk lie on same half plane
                    for (int k = 0; k < points.Count(); k++)
                    {
                        if (k != i & k != j) //point not = i or j
                            if (check == -5) //if first point to check
                            {
                                check = 0;
                                first_ori = Orientation(points[i], points[j], points[k]);
                                if (first_ori == 0)
                                    check = -5;
                            }
                            else
                            {
                                check = Orientation(points[i], points[j], points[k]);
                                if (check != first_ori)
                                { //not extreme segment
                                    belong = false;
                                    break;
                                }
                                else Console.WriteLine("i " + i + " j " + j);

                            }

                    }
                    if (belong == true)
                    {
                        bool containsItem = outPoints.Contains(points[i]);
                        if (containsItem == false) outPoints.Add(points[i]);
                        containsItem = outPoints.Contains(points[j]);
                        if (containsItem == false) outPoints.Add(points[j]);


                        segmentFound = true;


                    }

                }

            }
        }

        public override string ToString()
        {
            return "Convex Hull - Extreme Segments";
        }
    }
}