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
        
        public List<Point> FindExtremes (List<Point> points, ref List<Point> outPoints)
        {
            List<Point> outs = new List<Point>();
            bool segmentFound = false;
            for (int i = 0; i < points.Count(); i++) //first point in segment
            {

                for (int j = 0; j < points.Count(); j++) //second point in segment
                {
                    if (segmentFound) { segmentFound = false; break; }//start from last added point
                    if (j == i) continue;
                    int check = -5;
                    Enums.TurnType first_ori = new Enums.TurnType();
                    Enums.TurnType curr_ori;
                    bool belong = true;


                    //check if al points pk lie on same half plane
                    for (int k = 0; k < points.Count(); k++)
                    {
                        if (k != i & k != j) //point not = i or j
                            if (check == -5) //if first point to check
                            {
                                check = 0;
                                first_ori = HelperMethods.CheckTurn(new Line(points[i], points[j]), points[k]);
                                if (first_ori == Enums.TurnType.Colinear)
                                    check = -5;
                            }
                            else
                            {
                                curr_ori = HelperMethods.CheckTurn(new Line(points[i], points[j]), points[k]);
                                if (curr_ori == Enums.TurnType.Colinear)
                                {
                                    if (HelperMethods.PointOnSegment(points[k], points[i], points[j]) == true)
                                        continue;
                                    else
                                    {
                                        belong = false; break;
                                    }

                                }
                                if (curr_ori != first_ori)
                                { //not extreme segment
                                    belong = false;
                                    break;
                                }


                            }

                    }
                    if (belong == true)
                    {

                        bool containsItem = outs.Contains(points[i]);
                        if (containsItem == false) outs.Add(points[i]);
                        containsItem = outs.Contains(points[j]);
                        if (containsItem == false) outs.Add(points[j]);


                        segmentFound = true;


                    }

                }

            }
            return outs;


        }
        public override void Run(List<Point> points, List<Line> lines, List<Polygon> polygons, ref List<Point> outPoints, ref List<Line> outLines, ref List<Polygon> outPolygons)
        {   
            if (points.Count <= 3) { outPoints = points; return; } //if input is three points or less then return as convexHull 

            List<Point> outs = Find_Extremes(points, ref outPoints);
            for (int b = 0; b < outs.Count(); b++)
            {
                outPoints.Add(outs.ElementAt(b));
            }
            



        }
        public List<Point> Find_Extremes(List<Point> points, ref List<Point> outPoints)
        {
            List<Point> outs =FindExtremes(points, ref outPoints);
            List<Point> outs2 = FindExtremes(outs, ref outPoints);
            return outs2;

        }

        public override string ToString()
        {
            return "Convex Hull - Extreme Segments";
        }
    }
}
