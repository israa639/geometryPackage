using CGUtilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CGAlgorithms.Algorithms.ConvexHull
{
    public class JarvisMarch : Algorithm
    {
        public int GetMinLexoPoint(List<Point> points)
        {
            double MinPoint = points[0].Y;
            int minPointIndex = 0;
            for (int i = 1; i < points.Count; i++)
            {
                if (points[i].Y < MinPoint)
                {
                    MinPoint = points[i].Y;
                    minPointIndex = i;
                }
            }

            return minPointIndex;


        }
        
        public int findMaxAngle(List<KeyValuePair<double, int>> angles, List<Point> points,Point current)
        {
            int maxIndex = 0;
            double maxAngle = angles[0].Key;
            for(int i=1;i<angles.Count;i++)
            {
               if (angles[i].Key == angles[maxIndex].Key)
                {
                    
                   
                    if (current.X> points[angles[i].Value].X&& current.X > points[angles[i].Value].X)
                    {
                        maxIndex = points[angles[i].Value].X < points[angles[maxIndex].Value].X ? i : maxIndex;
                        maxAngle = angles[maxIndex].Key;
                        continue;
                    }
                    if (current.X < points[angles[i].Value].X && current.X < points[angles[i].Value].X)
                    {
                        maxIndex = points[angles[i].Value].X > points[angles[maxIndex].Value].X ? i : maxIndex;
                        maxAngle = angles[maxIndex].Key;
                        continue;
                    }
                }
                if (angles[i].Key>angles[maxIndex].Key)
                {
                   
                    maxIndex = i;
                    maxAngle = angles[i].Key;
                }    
            }
            return maxIndex;
        }
        public int GetAngularyRightMostPoint(List<Point> points, int CurrentPointIndex, Point CurrentVector, Point last)
        {

            Line l;

            List<KeyValuePair<double, int>> angles = new List<KeyValuePair<double, int>>();
            List<double> Xcoord = new List<double>();

            for (int i = 0; i < points.Count; i++)
            {
                if (i == CurrentPointIndex||points[i]==last)
                    continue;
                l = new Line(points[CurrentPointIndex], points[i]);
                var vec1 = HelperMethods.GetVector(l);
                var angle = HelperMethods.dotProduct(CurrentVector, vec1);
                Xcoord.Add(points[i].X);

                angles.Add(new KeyValuePair<double, int>(angle, i));
            }
            
            int maxIndex = findMaxAngle(angles,points,points[CurrentPointIndex]);
            int rightMostIndex =angles[maxIndex].Value ;
            return rightMostIndex;

        }
        public override void Run(List<Point> points, List<Line> lines, List<Polygon> polygons, ref List<Point> outPoints, ref List<Line> outLines, ref List<Polygon> outPolygons)
        {
            HelperMethods.filterPoints(points);
            if (points.Count==2)
            {
                outPoints.Add(points[0]);
                outPoints.Add(points[1]);
                return;
            }
            else if(points.Count == 1)
            {
                outPoints.Add(points[0]);
                
                return;
            }
            int minPointIndex = GetMinLexoPoint(points);
            outPoints.Add(points[minPointIndex]);
            Point p = new Point(points[minPointIndex].X-5, points[minPointIndex].Y);
            Line l = new Line(points[minPointIndex], p);
            var vec1 = HelperMethods.GetVector(l);
            bool IsFirstPoint = false;
            int  currentIndx = minPointIndex;
            

            while (!IsFirstPoint)
            {
                if (currentIndx == minPointIndex)
                {

                    currentIndx = GetAngularyRightMostPoint(points, currentIndx, vec1, outPoints[outPoints.Count - 1]);
                }
                else
                {
                    currentIndx = GetAngularyRightMostPoint(points, currentIndx, vec1, outPoints[outPoints.Count - 2]);
                }
                if (outPoints[0] == points[currentIndx])
                {
                    IsFirstPoint = true;
                    break;
                }

                l = new Line(points[currentIndx], outPoints.Last());
                vec1 = HelperMethods.GetVector(l);


                outPoints.Add(points[currentIndx]);
                

            }

        }

        public override string ToString()
        {
            return "Convex Hull - Jarvis March";
        }
    }
}
