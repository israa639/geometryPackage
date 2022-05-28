using CGUtilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CGAlgorithms.Algorithms.PolygonTriangulation
{
    class InsertingDiagonals : Algorithm
    {
        public int get_convex_point(List<CGUtilities.Line> lines)
        {

           // CGUtilities.Point min = lines[0].Start;
            int c_index = -1;
            for (int i = 0; i < lines.Count; i++)
            {
                if (CGUtilities.HelperMethods.CheckTurn(lines[i],lines[(i+1)%lines.Count].End)== CGUtilities.Enums.TurnType.Left)
                {
                   
                    c_index = i+1;
                    break;
                }
            }
            return c_index;

        }
        public int get_min_point(List<CGUtilities.Line> lines)
        {

            CGUtilities.Point min = lines[0].Start;
            int min_index = 0;
            for (int i=1;i<lines.Count;i++)
            {
                
                 if (lines[i].Start.Y>min.Y)
                {
                   
                    min = lines[i].Start;
                    min_index = i;

                }
            }
            return min_index;

        }
        public  void Inserting_diagonals(List<CGUtilities.Line> lines, ref List<CGUtilities.Line> outLines)
        {
            if (lines.Count > 3)
            {
               int c_point_index = get_min_point(lines);
                //int c_point_index = get_convex_point(lines);
                //if (c_point_index==-1)
                //{
                //    c_point_index = get_min_point(lines);
                //}
                int c_prev = (lines.Count + c_point_index - 1) % lines.Count;
                    int c_next=(c_point_index+1) % lines.Count;


                double x = 10000000000.0;
                int maxPointIndex=-1;
                int l = (c_next + 1) % lines.Count;
                while (l != c_prev)
                {
                    var point_location = CGUtilities.HelperMethods.PointInTriangle(lines[c_point_index].Start, lines[c_prev].Start, lines[c_next].Start, lines[l].Start);
                    if (point_location == Enums.PointInPolygon.Inside )
                    {
                        if(x> CGUtilities.HelperMethods.distanceBetween2Points(lines[c_point_index].Start, lines[l].Start))
                        {
                            x = CGUtilities.HelperMethods.distanceBetween2Points(lines[c_point_index].Start, lines[l].Start);
                            maxPointIndex = l;
                        }

                    }
                        l = ((l + 1) % lines.Count);
                }
                List<Line> l1 = new List<Line>();
                List<Line> l2 = new List<Line>();
                if (maxPointIndex == -1)
                {
                    outLines.Add(new Line(lines[c_prev].Start, lines[c_next].Start));


                    l1.Add(lines[c_prev]);
                    l1.Add(lines[c_point_index]);
                    l1.Add(new Line(lines[c_prev].Start, lines[c_next].Start));
                  
                    l2.Add(new Line(lines[c_prev].Start, lines[c_next].Start));
                    l = c_next;
                    while (l != c_prev)
                    {
                        l2.Add(lines[l]);


                        l = (l + 1) % lines.Count;
                    }
                
                }
                else
                {
                    outLines.Add(new Line(lines[c_point_index].Start, lines[maxPointIndex].End));
                    l1.Add((new Line(lines[c_point_index].Start, lines[maxPointIndex].Start)));
                    l1.Add(lines[c_prev]);
                    l1.Add(lines[(lines.Count + c_prev - 1) % lines.Count]);
                    l2.Add(new Line(lines[c_point_index].Start, lines[maxPointIndex].Start));
                    l = c_point_index;
                    while (l !=((lines.Count + c_prev - 1) % lines.Count))
                    {
                        l2.Add(lines[l]);


                        l = (l + 1) % lines.Count;
                    }

                }

                Inserting_diagonals(l1,ref outLines);
                Inserting_diagonals(l2, ref outLines);
            }

        }

        public override void Run(List<CGUtilities.Point> points, List<CGUtilities.Line> lines, List<CGUtilities.Polygon> polygons, ref List<CGUtilities.Point> outPoints, ref List<CGUtilities.Line> outLines, ref List<CGUtilities.Polygon> outPolygons)
        {

            Inserting_diagonals(lines, ref outLines);

        }

        public override string ToString()
        {
            return "Inserting Diagonals";
        }
    }
}
