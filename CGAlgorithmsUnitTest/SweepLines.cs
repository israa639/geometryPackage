using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using CGAlgorithms;
using CGUtilities;
using System.Collections.Generic;
using CGAlgorithms.Algorithms.SegmentIntersection;


namespace CGAlgorithmsUnitTest
{
    [TestClass]
    public class SweepLines
    {
        protected Algorithm convexHullTester;
        protected List<Point> inputPoints;
        protected List<Point> outputPoints;
        protected List<Point> desiredPoints;
        protected List<Line> inputLines;
        protected List<Line> outputLines;
        protected List<Polygon> inputPolygons;
        protected List<Polygon> outputPolygons;

        [TestMethod]
        public void TestMethod1()
        {
           
            List<Event> output = new List<Event>(); 

            inputLines = new List<Line> {new Line(new Point(1,10),new Point(18,1)),
                                         new Line(new Point(3,6),new Point(10,12)),
                                         new Line(new Point(4,8),new Point(9,7)),
                                         new Line(new Point(6,1),new Point(17,6)),
                                         new Line(new Point(2,5),new Point(7,3)),
                                         new Line(new Point(11,4),new Point(13,2)) };
            SweepLine sw = new SweepLine();

            sw.Run(this.inputPoints, this.inputLines, this.inputPolygons,ref outputPoints,ref outputLines,ref outputPolygons);
            List<Point> outs = outputPoints;//MUST BE 5 POINTS
            output = sw.output;
            
            
            
        }
        [TestMethod]
        public void TestMethod2()
        {
            inputPoints = new List<Point>();
            outputPoints = new List<Point>();
            outputLines = new List<Line>();
            desiredPoints = new List<Point>();
            List<Event> output = new List<Event>();

            inputLines = new List<Line> {new Line(new Point(2,10),new Point(9,10)),
                                         new Line(new Point(1,20),new Point(10,20)),
                                         new Line(new Point(15,5),new Point(25,13)),
                                         new Line(new Point(20,12),new Point(26,7)),
                                         };
            SweepLine sw = new SweepLine();
            
            sw.Run(this.inputPoints, this.inputLines, this.inputPolygons, ref outputPoints, ref outputLines, ref outputPolygons);
            List<Point> outs = outputPoints; //MUST BE ONE POINT
            output = sw.output;



        }
        [TestMethod]
        public void TestMethod3()
        {
            inputPoints = new List<Point>();
            outputPoints = new List<Point>();
            outputLines = new List<Line>();
            desiredPoints = new List<Point>();
            List<Event> output = new List<Event>();

            inputLines = new List<Line> {new Line(new Point(2,2),new Point(6,8)),
                                         new Line(new Point(1,4),new Point(4,4)),
                                         new Line(new Point(1,6),new Point(5,6)),
                                         new Line(new Point(10,2),new Point(12,6)),
                                         new Line(new Point(10,6),new Point(12,2)),
                                         };
            SweepLine sw = new SweepLine();

            sw.Run(this.inputPoints, this.inputLines, this.inputPolygons, ref outputPoints, ref outputLines, ref outputPolygons);
            List<Point> outs = outputPoints; //MUST BE 3 POINTS
            output = sw.output;



        }
        [TestMethod]
        public void TestMethod4()
        {
            inputPoints = new List<Point>();
            outputPoints = new List<Point>();
            outputLines = new List<Line>();
            desiredPoints = new List<Point>();
            List<Event> output = new List<Event>();

            inputLines = new List<Line> {new Line(new Point(3,1),new Point(8,7)),
                                         new Line(new Point(2,6),new Point(6,2)),
                                         new Line(new Point(1,3),new Point(5,1)),
                                        
                                         };
            SweepLine sw = new SweepLine();

            sw.Run(this.inputPoints, this.inputLines, this.inputPolygons, ref outputPoints, ref outputLines, ref outputPolygons);
            List<Point> outs = outputPoints; //MUST BE 2 POINTS
            output = sw.output;



        }
    }
}
