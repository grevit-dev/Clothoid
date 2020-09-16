using System;
using System.Collections.Generic;
using Autodesk.DesignScript.Runtime;
using Autodesk.DesignScript.Geometry;

namespace Dynamoid
{
    /// <summary>
    /// Dynamo Clothoid Nodes
    /// </summary>
    public static class Dynamoid
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
        public static IEnumerable<Point> ConnectLineAndCircle(Circle circle, Line line, bool fromLineToCircle, double stepsize, double tolerance = 0.0)
        {
            Clothoid.Geometry.Circle cCircle = circle.ToCGeo();
            Clothoid.Geometry.Line cLine = line.ToCGeo();
            Clothoid.Geometry.Clothoid clothoid = new Clothoid.Geometry.Clothoid();

            int result = Clothoid.Geometry.Clothoid.Connect(cLine, cCircle, fromLineToCircle, tolerance, out clothoid);
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
        public static IEnumerable<Point> ConnectCircleAndCircle(Circle circleStart, Circle circleEnd, double stepsize, double tolerance = 0.0)
        {
            Clothoid.Geometry.Circle cCircleStart = circleStart.ToCGeo();
            Clothoid.Geometry.Circle cCircleEnd = circleEnd.ToCGeo();
            Clothoid.Geometry.Clothoid clothoid = new Clothoid.Geometry.Clothoid();

            int result = Clothoid.Geometry.Clothoid.Connect(cCircleStart, cCircleEnd, tolerance, out clothoid);
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
        /// Creates a new Clothoid function and returns it's points.
        /// </summary>
        /// <param name="origin">Origin</param>
        /// <param name="unitVector">Unit vector</param>
        /// <param name="curvatureRate">Curvature rate</param>
        /// <param name="startLength">Start length</param>
        /// <param name="endLength">End length</param>
        /// <param name="stepsize">Stepsize for points returned along the clothoid</param>
        /// <returns>Points along a Clothoid function</returns>
        public static IEnumerable<Point> Create(Point origin, Vector unitVector, double curvatureRate, double startLength, double endLength, double stepsize)
        {
            Clothoid.Geometry.Clothoid clothoid = new Clothoid.Geometry.Clothoid(curvatureRate, origin.ToCGeo(), unitVector.ToCGeo(), startLength, endLength);
        
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
        public static Clothoid.Geometry.Circle ToCGeo(this Circle circle)
        {
            return new Clothoid.Geometry.Circle(circle.CenterPoint.X, circle.CenterPoint.Y, circle.Radius);
        }

        [IsVisibleInDynamoLibrary(false)]
        public static Clothoid.Geometry.Line ToCGeo(this Line line)
        {
            return new Clothoid.Geometry.Line(line.StartPoint.X, line.StartPoint.Y, line.EndPoint.X, line.EndPoint.Y);
        }

        [IsVisibleInDynamoLibrary(false)]
        public static Clothoid.Geometry.Point ToCGeo(this Point point)
        {
            return new Clothoid.Geometry.Point(point.X, point.Y);
        }

        [IsVisibleInDynamoLibrary(false)]
        public static Clothoid.Geometry.UnitVector ToCGeo(this Vector vector)
        {
            return new Clothoid.Geometry.UnitVector(vector.X, vector.Y);
        }

        [IsVisibleInDynamoLibrary(false)]
        public static Point ToAGeo(this Clothoid.Geometry.Point point)
        {
            return Point.ByCoordinates(point.X, point.Y);
        }
    }
}
