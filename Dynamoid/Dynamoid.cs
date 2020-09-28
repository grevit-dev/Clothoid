using System;
using System.Collections.Generic;
using Autodesk.DesignScript.Runtime;
using Autodesk.DesignScript.Geometry;
using Clolib = Clothoid;

namespace Dynamoid
{
    /// <summary>
    /// Dynamo Clothoid Nodes
    /// </summary>
    public static class Clothoid
    {
        /// <summary>
        /// Connects any given line to any given circle geometry by a clothoid function and returns it's points.
        /// This node throws errors if line and circle are intersecting or too far away from each other.
        /// </summary>
        /// <param name="circle">Circle geometry</param>
        /// <param name="line">Line geometry</param>
        /// <param name="fromLineToCircle">Create Clothoid from line to circle or the other way round</param>
        /// <param name="stepsize">Stepsize for points returned along the clothoid</param>
        /// <param name="tolerance">Clothoid generation tolerance</param>
        /// <returns>Points along a Clothoid function</returns>
        public static IEnumerable<Point> ByLineAndCircle(Circle circle, Line line, bool fromLineToCircle, double stepsize, double tolerance = 0.0)
        {
            Clolib.Geometry.Circle cCircle = circle.ToCGeo();
            Clolib.Geometry.Line cLine = line.ToCGeo();
            Clolib.Geometry.Clothoid clothoid = new Clolib.Geometry.Clothoid();

            int result = Clolib.Geometry.Clothoid.Connect(cLine, cCircle, fromLineToCircle, tolerance, out clothoid);
            switch (result)
            {
                case -1:
                    throw new Exception("Circle and Line are intersecting.");
                case 1:
                    throw new Exception("Connecting clothoid becomes too long.");
            }

            List<Point> points = new List<Point>();
            foreach (var point in clothoid.GetPoints(stepsize, true))
            {
                points.Add(point.ToAGeo());
            }
            return points;
        }

        /// <summary>
        /// Connects two given circles by a clothoid function and returns it's points.
        /// This node throws errors if both circles are intersecting or too far away from each other.
        /// </summary>
        /// <param name="circleStart">Circle to start from</param>
        /// <param name="circleEnd">Circle to connect to</param>
        /// <param name="stepsize">Stepsize for points returned along the clothoid</param>
        /// <param name="tolerance">Clothoid generation tolerance</param>
        /// <returns>Points along a Clothoid function</returns>
        public static IEnumerable<Point> ByCircleAndCircle(Circle circleStart, Circle circleEnd, double stepsize, double tolerance = 0.0)
        {
            Clolib.Geometry.Circle cCircleStart = circleStart.ToCGeo();
            Clolib.Geometry.Circle cCircleEnd = circleEnd.ToCGeo();
            Clolib.Geometry.Clothoid clothoid = new Clolib.Geometry.Clothoid();

            int result = Clolib.Geometry.Clothoid.Connect(cCircleStart, cCircleEnd, tolerance, out clothoid);
            switch (result)
            {
                case -1:
                    throw new Exception("Circle and Circle are intersecting.");
                case 1:
                    throw new Exception("Connecting clothoid becomes too long.");
            }

            List<Point> points = new List<Point>();
            foreach (var point in clothoid.GetPoints(stepsize, true))
            {
                points.Add(point.ToAGeo());
            }
            return points;
        }

        /// <summary>
        /// Creates a new Clothoid function and returns it's points.
        /// </summary>
        /// <param name="origin">Origin</param>
        /// <param name="unitVector">Unit vector</param>
        /// <param name="curvatureRate">Curvature rate</param>
        /// <param name="startLength">Start length</param>
        /// <param name="endLength">End length</param>
        /// <param name="stepsize">Stepsize for points returned along the clothoid</param>
        /// <returns>Points along a Clothoid function</returns>
        public static IEnumerable<Point> ByParameters(Point origin, Vector unitVector, double curvatureRate, double startLength, double endLength, double stepsize)
        {
            Clolib.Geometry.Clothoid clothoid = new Clolib.Geometry.Clothoid(curvatureRate, origin.ToCGeo(), unitVector.ToCGeo(), startLength, endLength);
        
            List<Point> points = new List<Point>();
            foreach (var point in clothoid.GetPoints(stepsize, true))
            {
                points.Add(point.ToAGeo());
            }
            return points;
        }
    }

    [IsVisibleInDynamoLibrary(false)]
    public static class Geometry
    {
        [IsVisibleInDynamoLibrary(false)]
        public static Clolib.Geometry.Circle ToCGeo(this Circle circle)
        {
            return new Clolib.Geometry.Circle(circle.CenterPoint.X, circle.CenterPoint.Y, circle.Radius);
        }

        [IsVisibleInDynamoLibrary(false)]
        public static Clolib.Geometry.Line ToCGeo(this Line line)
        {
            return new Clolib.Geometry.Line(line.StartPoint.X, line.StartPoint.Y, line.EndPoint.X, line.EndPoint.Y);
        }

        [IsVisibleInDynamoLibrary(false)]
        public static Clolib.Geometry.Point ToCGeo(this Point point)
        {
            return new Clolib.Geometry.Point(point.X, point.Y);
        }

        [IsVisibleInDynamoLibrary(false)]
        public static Clolib.Geometry.UnitVector ToCGeo(this Vector vector)
        {
            return new Clolib.Geometry.UnitVector(vector.X, vector.Y);
        }

        [IsVisibleInDynamoLibrary(false)]
        public static Point ToAGeo(this Clolib.Geometry.Point point)
        {
            return Point.ByCoordinates(point.X, point.Y);
        }
    }
}
