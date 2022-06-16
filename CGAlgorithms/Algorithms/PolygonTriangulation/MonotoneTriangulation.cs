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
        public List<Point> Trun_AntiClock(List<Point> points)
        {
            points.Reverse();

            return points;
        }
        public List<Point> Check_antiClock(List<Line> lines)
        {
            List<Point> poly = new List<Point>();
            poly.Add((Point)lines[0].Start.Clone());
            Point minY =(Point)lines[0].Start.Clone();
            Point PrevMin = (Point)lines[lines.Count() - 1].Start.Clone(); ;//to check clockwise later
            Point NextMin = (Point)lines[1].Start.Clone(); ;
            for (int i = 1; i < lines.Count(); i++) // make sorted set of start points
            {
                poly.Add((Point)lines[i].Start.Clone());

          
                if (lines[i].Start.Y < minY.Y)
                {
                    PrevMin = (Point)lines[i - 1].Start.Clone();
                    minY = (Point)lines[i].Start.Clone();
                    if (i < lines.Count() - 1)
                        NextMin = (Point)lines[i + 1].Start.Clone();
                    else
                        NextMin = (Point)lines[0].Start.Clone();

                }

            }
            //check if polygon is in anticlock wise

            List<Point> temp = new List<Point>();
            if (HelperMethods.CheckTurn(new Line(PrevMin, minY), NextMin) == Enums.TurnType.Right)
            {
                Console.WriteLine("POLY IS IN CLOCKWISE : RESOLVE");
                temp = Trun_AntiClock(poly);

            }
            else
                temp = poly;

            return temp ;

        }
        public Boolean Check_Y_Monotone(List<Point> points)
        {
            int counterStart = 0;
            int counterEnd = 0;
            Boolean y_mono = true;

            
            for (int i = 0; i < points.Count(); i++)
            {
                if (i == 0)
                {
                    if ((points[points.Count()-1].Y > points[i].Y) && (points[i + 1].Y > points[i].Y)) //merge vertex 
                    {
                        counterEnd++;
                    }
                    else if ((points[points.Count() - 1].Y < points[i].Y) && (points[i + 1].Y < points[i].Y)) //split vertex
                    {
                        counterStart++;
                    }
                }
                else if (i == points.Count()-1)
                {
                    if ((points[i-1].Y > points[i].Y) && (points[0].Y > points[i].Y)) //merge vertex 
                    {
                        counterEnd++;
                    }
                    else if ((points[i - 1].Y < points[i].Y) && (points[0].Y < points[i].Y)) //split vertex
                    {
                        counterStart++;
                    }

                }
                else
                {
                    if ((points[i - 1].Y > points[i].Y) && (points[i + 1].Y > points[i].Y)) //merge vertex 
                    {
                        counterEnd++;
                    }
                    else if ((points[i - 1].Y < points[i].Y) && (points[i + 1].Y < points[i].Y)) //split vertex
                    {
                        counterStart++;
                    }
                }

            }
            if (counterEnd > 1 || counterStart > 1)
                y_mono = false;


            return y_mono;
        }
       



        public override void Run(List<Point> points,List<Line> lines, List<Polygon> polygons, ref List<Point> outPoints, ref List<Line> outLines, ref List<Polygon> outPolygons)
        {
            SortedSet<Points> point = new SortedSet<Points>(new SortPoints());
            Stack<Points> curr = new Stack<Points>();
            List<Line> diagonals = new List<Line>();

            List<Point> newPoints = Check_antiClock(lines);//has new set of start points
            if (Check_Y_Monotone(newPoints) == false)
            {
                Console.WriteLine("POLY IS NOT Y_MONOTONE : TERMINATE");
                return;
            }



            int count = 0;
            if(newPoints[1].Y< newPoints[0].Y)
                if (newPoints[0].Y> newPoints[newPoints.Count()-1].Y)
                    point.Add(new Points((Point)newPoints[0].Clone(), 2)); //max point
                else    
                    point.Add(new Points((Point)newPoints[0].Clone(), 0)); //left chain
            else if (newPoints[newPoints.Count() - 1].Y < newPoints[0].Y)
                point.Add(new Points((Point)newPoints[0].Clone(), 1)); //right chain
            else
                point.Add(new Points((Point)newPoints[0].Clone(), 2)); //min point


            for (int i = 1; i < newPoints.Count(); i++) // make sorted set of start points
            {
                if (newPoints[i - 1].Y > newPoints[i].Y)
                {
                    Point next;
                    if (i < newPoints.Count() - 1)
                        next = newPoints[i + 1];
                    else 
                        next = (Point)point.ElementAt(0).point.Clone();
                    if (next.Y > newPoints[i].Y)//point with min y
                    {

                        count++;
                        point.Add(new Points((Point)newPoints[i].Clone(), 2));
                        // will be true if merge vertex exist
                       
                        
                        
                    }
                    else
                        point.Add(new Points((Point)newPoints[i].Clone(), 0)); //left chain
                }
                else
                    point.Add(new Points((Point)newPoints[i].Clone(), 1));  //right chain

            }

            point.ElementAt(0).chain = 2;


            curr.Push(point.ElementAt(0));
            curr.Push(point.ElementAt(1));
            int j = 2;
            while (j < point.Count())
            {
                
                Points temp = point.ElementAt(j);
                
                Points top = curr.Peek();
                
                if (temp.chain == top.chain)//on same chain
                {
                    curr.Pop();
                    Points temp2 = curr.Peek();
                    if (((top.chain == 0) & (HelperMethods.CheckTurn(new Line(temp2.point, top.point), temp.point) == Enums.TurnType.Left)) //for left chain
                           || ((top.chain == 1) & (HelperMethods.CheckTurn(new Line(temp2.point, top.point), temp.point) == Enums.TurnType.Right)) //for right chain
                           )//convex angle so we can connect
                    {
                        diagonals.Add(new Line((Point)temp.point.Clone(), temp2.point));
                        if (curr.Count() == 1)
                        {
                            curr.Push(temp);
                            j++;
                        }
                        // temp2 = curr.Peek();


                    }
                    else
                    {
                        curr.Push(top);
                        curr.Push(temp);
                        j++;

                    }

                }
                else
                {
                    Boolean popped = false;
                    while (curr.Count() != 1)
                    {
                        Points top2 = curr.Pop();
                        diagonals.Add(new Line((Point)temp.point.Clone(), top2.point));
                        
                        popped = true;
                    }
                    curr.Pop();
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
