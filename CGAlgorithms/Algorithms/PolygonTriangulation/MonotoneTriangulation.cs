using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CGUtilities;



namespace CGAlgorithms.Algorithms.PolygonTriangulation
{
    class MonotoneTriangulation : Algorithm
    {
        public class Points
        {
            public Point point;
            public int chain;// 0 left // 1 right // 3 for min point(used to terminate algo)
            public Points(Point x, int Chain)
            {
                point = (Point)x.Clone();
                chain = Chain;
            }
        }
        public class SortPoints : IComparer<Points>
        {
            public int Compare(Points a, Points b)
            {
                if (a.point.Y < b.point.Y) //put the one with higher y first
                    return 1;
                else if (a.point.Y > b.point.Y)
                    return -1;
                else {    //if a tie put one with lower x first
                    if (a.point.X > b.point.X)
                        return 1;
                    else
                        return -1;
                };
            }
        }
        public Boolean Check_Y_Monotone(List<Points> points, Point minY)
        {
            Boolean y_mono = true;
            for (int i = 1; i < points.Count()-1; i++)
            {
                if ((points[i - 1].point.Y > points[i].point.Y) && (points[i + 1].point.Y > points[i].point.Y)) //merge vertex 
                {
                    y_mono = false;
                    break;
                }
                else if ((points[i - 1].point.Y < points[i].point.Y) && (points[i + 1].point.Y < points[i].point.Y) && !points[i].point.Equals(minY)) //split vertex
                {
                    y_mono = false;
                    break;
                }

            }


            return y_mono;
        }
        
            
        public override void Run(List<Point> points,List<Line> lines, List<Polygon> polygons, ref List<Point> outPoints, ref List<Line> outLines, ref List<Polygon> outPolygons)
        {
            SortedSet<Points> point = new SortedSet<Points>(new SortPoints());
            Stack<Points> curr = new Stack<Points>();
            List<Line> diagonals = new List<Line>();


            Point minY = new Point(0,0);
            int count = 0;
            point.Add(new Points((Point)lines[0].Start.Clone(), 0)); //left chain
            for (int i = 1; i < lines.Count(); i++) // make sorted set of start points
            {
                if (lines[i - 1].Start.Y > lines[i].Start.Y)
                {
                    Point next;
                    if (i < lines.Count() - 1)
                        next = lines[i + 1].Start;
                    else 
                        next = (Point)point.ElementAt(0).point.Clone();
                    if (next.Y > lines[i].Start.Y)//point with min y
                    {
                        
                        minY = (Point)lines[i].Start.Clone();
                        count++;
                        point.Add(new Points((Point)lines[i].Start.Clone(), 2));
                        // will be true if merge vertex exist
                        if(count>1)
                        {
                            Console.WriteLine("POLY IS NOT Y_MONOTONE : TERMINATE");
                            return;
                        }
                        //check if polygon is in anticlock wise
                        if (count<2 && HelperMethods.CheckTurn(new Line((Point)lines[i - 1].Start.Clone(), (Point)lines[i].Start.Clone()), (Point)next.Clone()) == Enums.TurnType.Right)
                        {
                            Console.WriteLine("POLY IS IN CLOCK WISE : TERMINATE");
                            return;
                        }
                        
                    }
                    else
                        point.Add(new Points((Point)lines[i].Start.Clone(), 0)); //left chain
                }
                else
                    point.Add(new Points((Point)lines[i].Start.Clone(), 1));  //right chain

            }

            if (count>1||Check_Y_Monotone(point.ToList(), minY) == false) //not y_montone
            {
                Console.WriteLine("POLY IS NOT Y_MONOTONE : TERMINATE");
                return;
            }
            else
                Console.WriteLine("Y_MONTONE");


            curr.Push(point.ElementAt(0));
            curr.Push(point.ElementAt(1));
            int j = 2;
            while (j < point.Count())
            {
                
                Points temp = point.ElementAt(j);
                Points top = curr.Pop();
                
                if (temp.chain == top.chain)//on same chain
                {

                    Points temp2 = curr.Peek();
                     while (((temp2.chain == 0 && temp.chain == 0)&(HelperMethods.CheckTurn(new Line(temp2.point, top.point), temp.point) == Enums.TurnType.Left)) //for left chain
                            ||((temp2.chain == 1 && temp.chain == 1) & (HelperMethods.CheckTurn(new Line(temp2.point, top.point), temp.point) == Enums.TurnType.Right)) //for right chain
                            )//convex angle so we can connect
                     { 
                            top = curr.Pop();
                            diagonals.Add(new Line((Point)top.point.Clone(), temp.point));
                            if (curr.Count() == 0) break;
                            temp2 = curr.Peek();


                     }
                    curr.Push(top);
                    curr.Push(temp);
                    j++;

                }
                else
                {
                    Boolean popped = false;
                    while (curr.Count() > 0)
                    {

                        diagonals.Add(new Line((Point)top.point.Clone(), temp.point));
                        top =curr.Pop();
                        popped = true;
                    }
                    if(!popped)curr.Pop();
                    curr.Push(top);
                    curr.Push(temp);
                    j++;
                }


            }
            outLines = diagonals;



        }

       
        public override string ToString()
        {
            return "Monotone Triangulation";
        }
    }
}
