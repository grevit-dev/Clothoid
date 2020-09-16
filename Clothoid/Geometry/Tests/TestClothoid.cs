﻿using NUnit.Framework;

namespace Clothoid.Geometry.Tests
{
    [TestFixture]
    public class TestClothoid
    {
        private const double Tol = 1e-6;

        private static void CheckConnectivity(Clothoid clothoid, Line line, bool atClothoidStart)
        {
            var length = atClothoidStart ? clothoid.StartLength : clothoid.EndLength;

            Assert.IsTrue(TestCircle.Contains(line, clothoid.GetPoint(length), Tol));
            Assert.IsTrue(TestCircle.AreSame(clothoid.GetTangent(length), line.Direction));
        }
        
        private static void CheckConnectivity(Clothoid clothoid, Circle circle, bool atClothoidStart)
        {
            var length = atClothoidStart ? clothoid.StartLength : clothoid.EndLength;

            Assert.IsTrue(TestCircle.Contains(circle, clothoid.GetPoint(length), Tol));
            Assert.IsTrue(TestCircle.AreSame(clothoid.GetTangent(length), circle.GetTangent(clothoid.GetPoint(length))));
        }
        
        [Test]
        public void TestConnectLineToCircle()
        {
            const double x0 = 1000000;
            const double y0 = -1000000;
            
            Clothoid clothoid;

            var line = new Line(new Point(-1000 + x0, -190 + y0), new Point(+1000 + x0, 130 + y0));
            var circle = new Circle(100 + x0, -90 + y0, -60);
            Clothoid.Connect(line, circle, true, Tol, out clothoid);
            CheckConnectivity(clothoid, line, true);
            CheckConnectivity(clothoid, circle, false);

            line = new Line(new Point(-1000 + x0, -190 + y0), new Point(+1000 + x0, 130 + y0));
            circle = new Circle(100 + x0, -90 + y0, -60);
            Clothoid.Connect(line, circle, false, Tol, out clothoid);
            CheckConnectivity(clothoid, line, false);
            CheckConnectivity(clothoid, circle, true);

            line = new Line(new Point(-1000 + x0, -190 + y0), new Point(+1000 + x0, 130 + y0));
            circle = new Circle(100 + x0, 80 + y0, 90);
            Clothoid.Connect(line, circle, true, Tol, out clothoid);
            CheckConnectivity(clothoid, line, true);
            CheckConnectivity(clothoid, circle, false);

            line = new Line(new Point(-1000 + x0, -190 + y0), new Point(+1000 + x0, 130 + y0));
            circle = new Circle(100 + x0, 80 + y0, 90);
            Clothoid.Connect(line, circle, false, Tol, out clothoid);
            CheckConnectivity(clothoid, line, false);
            CheckConnectivity(clothoid, circle, true);
        }

        [Test]
        public void TestConnectionCircleToCircle()
        {
            const double X0 = 1000000;
            const double Y0 = -1000000;
            
            Clothoid clothoid;

            var c1 = new Circle(10 + X0, 10 + Y0, 10);
            var c2 = new Circle(13 + X0, 15 + Y0, 4);
            Clothoid.Connect(c1, c2, Tol, out clothoid);
            CheckConnectivity(clothoid, c1, true);
            CheckConnectivity(clothoid, c2, false);

            c1 = new Circle(10 + X0, 10 + Y0, 10);
            c2 = new Circle(13 + X0, 15 + Y0, 4);
            Clothoid.Connect(c2, c1, Tol, out clothoid);
            CheckConnectivity(clothoid, c2, true);
            CheckConnectivity(clothoid, c1, false);

            c1 = new Circle(10 + X0, 10 + Y0, -10);
            c2 = new Circle(13 + X0, 15 + Y0, -4);
            Clothoid.Connect(c1, c2, Tol, out clothoid);
            CheckConnectivity(clothoid, c1, true);
            CheckConnectivity(clothoid, c2, false);

            c1 = new Circle(10 + X0, 10 + Y0, -10);
            c2 = new Circle(13 + X0, 15 + Y0, -4);
            Clothoid.Connect(c2, c1, Tol, out clothoid);
            CheckConnectivity(clothoid, c2, true);
            CheckConnectivity(clothoid, c1, false);

            c1 = new Circle(15 + X0, 0 + Y0, 7);
            c2 = new Circle(12 + X0, 0 + Y0, 2);
            Assert.IsTrue(Clothoid.Connect(c1, c2, Tol / 100, out clothoid) == 0);
            CheckConnectivity(clothoid, c1, true);
            CheckConnectivity(clothoid, c2, false);
        }

        [Test]
        public void TestFittingG2Circle()
        {
            // "G2" stands for Continuity+Tangency+Curvature coincidence connection 

            const double FittingTol = Point.Eps / 10;
            const double SpatialTol = Point.Eps;

            Circle c;
            Clothoid clt;
            double l;

            var line = new Line(2, 1, 4, 2);
            var circle = new Circle(3, 4, 1.7);
            Assert.AreEqual(0, Clothoid.Connect(line, circle, true, Tol, out clt));
            Clothoid.FitCircle(clt, circle.Center + new Vector(circle.Radius, 0), FittingTol, out c, out l);
            Assert.That(c.Center.DistanceTo(circle.Center) < SpatialTol);
            Assert.AreEqual(clt.EndLength, l, SpatialTol);

            line = new Line(2, 1, 4, 2);
            circle = new Circle(3, 4, 1.7);
            Assert.AreEqual(0, Clothoid.Connect(line, circle, false, Tol, out clt));
            Clothoid.FitCircle(clt, circle.Center + new Vector(circle.Radius, 0), FittingTol, out c, out l);
            Assert.That(c.Center.DistanceTo(circle.Center) < SpatialTol);
            Assert.AreEqual(clt.StartLength, l, SpatialTol);

            line = new Line(2, 1, 4, 2);
            circle = new Circle(-3, -4, -1.7);
            Assert.AreEqual(0, Clothoid.Connect(line, circle, false, Tol, out clt));
            Clothoid.FitCircle(clt, circle.Center + new Vector(circle.Radius, 0), FittingTol, out c, out l);
            Assert.That(c.Center.DistanceTo(circle.Center) < SpatialTol);
            Assert.AreEqual(clt.StartLength, l, SpatialTol);

            line = new Line(2, 1, 4, 2);
            circle = new Circle(-3, -4, -1.7);
            Assert.AreEqual(0, Clothoid.Connect(line, circle, true, Tol, out clt));
            Clothoid.FitCircle(clt, circle.Center + new Vector(circle.Radius, 0), FittingTol, out c, out l);
            Assert.That(c.Center.DistanceTo(circle.Center) < SpatialTol);
            Assert.AreEqual(clt.EndLength, l, SpatialTol);

            line = new Line(2, 1, 4, 2);
            circle = new Circle(3, 4, 1.7);
            Assert.AreEqual(0, Clothoid.Connect(line, circle, true, Tol, out clt));

            Assert.IsFalse(Clothoid.FitCircle(clt, new Point(8, 4), FittingTol, out c, out l));

            Assert.IsFalse(Clothoid.FitCircle(clt, new Point(8, 2), FittingTol, out c, out l));
        }

        [Test]
        public void TestZeroClothoid()
        {
            const double x0 = 1000000;
            const double y0 = -1000000;
            
            // Circle to line
            
            Clothoid clothoid;
            Assert.IsTrue(Clothoid.Connect(new Line(0 + x0, 0 + y0, 10 + x0, 0 + y0), new Circle(10 + x0, 10 + y0, 10), true, Tol, out clothoid) == 0);
            Assert.AreEqual(0, clothoid.Length);

            var pt1 = new Point(10 + x0, 10 + y0);
            var pt2 = new Point(20 + x0, -10 + y0);
            var line = new Line(pt1, pt2);
            var circle = new Circle(pt2 + line.Normal*10, 10);
            Assert.That(Clothoid.Connect(line, circle, true, Tol, out clothoid) == 0);
            Assert.AreEqual(0, clothoid.Length);
            CheckConnectivity(clothoid, line, true);
            CheckConnectivity(clothoid, circle, false);

            // Circle to circle

            var circle1 = new Circle(3 + x0, 0 + y0, 4);
            var circle2 = new Circle(6 + x0, 0 + y0, 1);
            Assert.That(Clothoid.Connect(circle1, circle2, Tol, out clothoid) == 0);
            Assert.AreEqual(0, clothoid.Length);
            CheckConnectivity(clothoid, circle1, true);
            CheckConnectivity(clothoid, circle2, false);

            circle1 = new Circle(3 + x0, 0 + y0, 4);
            circle2 = new Circle(9 + x0, 0 + y0, -2);
            Assert.That(Clothoid.Connect(circle1, circle2, Tol, out clothoid) == 0);
            Assert.AreEqual(0, clothoid.Length);
            CheckConnectivity(clothoid, circle1, true);
            CheckConnectivity(clothoid, circle2, false);
        }

        [Test]
        public void TestNoClothoid()
        {
            const double x0 = 1000000;
            const double y0 = -1000000;
            
            Clothoid clothoid;
            
            // Line to circle

            var line = new Line(-1000 + x0, -190 + y0, +1000 + x0, 130 + y0);
            var circle = new Circle(100 + x0, -90 + y0, -100);
            Assert.IsTrue(Clothoid.Connect(line, circle, true, Tol, out clothoid) < 0);

            circle = new Circle(100 + x0, -90 + y0, -10);
            Assert.IsTrue(Clothoid.Connect(line, circle, true, Tol, out clothoid) > 0);
            
            // Circle to circle

            var circle1 = new Circle(15 + x0, 0 + y0, 7);
            var circle2 = new Circle(9 + x0, 0 + y0, 3);
            Assert.IsTrue(Clothoid.Connect(circle1, circle2, Tol, out clothoid) < 0);

            circle1 = new Circle(15 + x0 + y0, 0, 7);
            circle2 = new Circle(9 + x0 + y0, 0, -3);
            Assert.IsTrue(Clothoid.Connect(circle1, circle2, Tol, out clothoid) < 0);

            circle1 = new Circle(15 + x0, 0 + y0, 7);
            circle2 = new Circle(6 + x0, 0 + y0, -3);
            Assert.IsTrue(Clothoid.Connect(circle1, circle2, Tol, out clothoid) < 0);
        }

        [Test]
        public void TestTooLongClothoid()
        {
            const double x0 = 1000000;
            const double y0 = -1000000;
            
            Clothoid clothoid;
            
            // Line to circle

            var line = new Line(-1000 + x0, -190 + y0, +1000 + x0, 130);
            var circle = new Circle(100 + x0, -90 + y0, -10);
            Assert.IsTrue(Clothoid.Connect(line, circle, true, Tol, out clothoid) > 0);

            // Circle to circle

            var circle1 = new Circle(15 + x0, 0 + y0, 7);
            var circle2 = new Circle(0 + x0, 0 + y0, -1);
            Assert.IsTrue(Clothoid.Connect(circle1, circle2, Tol, out clothoid) > 0);
        }

        [Test]
        public void TestIntersect()
        {
            const double x0 = 1000000;
            const double y0 = -1000000;

            Clothoid clothoid;
            var c1 = new Circle(10 + x0, 10 + y0, 10);
            var c2 = new Circle(13 + x0, 15 + y0, 4);
            Clothoid.Connect(c1, c2, Tol, out clothoid);

            var secantLine = new Line(c1.Center, c2.Center);
            
            Point pt;
            Assert.IsTrue(clothoid.Intersect(secantLine, out pt));
            Assert.AreEqual(0, secantLine.DistanceTo(pt));

            Clothoid.Connect(c2, c1, Tol, out clothoid);
            Assert.IsTrue(clothoid.Intersect(secantLine, out pt));
            Assert.AreEqual(0, secantLine.DistanceTo(pt));

            secantLine = new Line(c1.Center, clothoid.StartPoint);
            Assert.IsTrue(clothoid.Intersect(secantLine, out pt));
            Assert.AreEqual(0, secantLine.DistanceTo(pt));
            Assert.AreEqual(0, pt.DistanceTo(clothoid.StartPoint));

            secantLine = new Line(c1.Center, clothoid.EndPoint);
            Assert.IsTrue(clothoid.Intersect(secantLine, out pt));
            Assert.AreEqual(0, secantLine.DistanceTo(pt));
            Assert.AreEqual(0, pt.DistanceTo(clothoid.EndPoint));

            secantLine = new Line(c1.Center, clothoid.EndPoint + (clothoid.EndPoint - clothoid.StartPoint));
            Assert.IsFalse(clothoid.Intersect(secantLine, out pt));
        }

        [Test]
        public void TestCalcTargetDistanceLineToCircle()
        {
            Line line;
            Circle circle;
            Clothoid clt;

            line = new Line(0, 0, 1, 0);
            circle = new Circle(4, 4, 3.8);
            Assert.That(Clothoid.Connect(line, circle, true, Tol, out clt) == 0);
            Assert.AreEqual(line.DistanceTo(circle.Center), Clothoid.CalcTargetDistance(circle.Radius, clt.Length), Tol);

            line = new Line(0, 0, 1, 0);
            circle = new Circle(4, -4, -3.8);
            Assert.That(Clothoid.Connect(line, circle, true, Tol, out clt) == 0);
            Assert.AreEqual(line.DistanceTo(circle.Center), Clothoid.CalcTargetDistance(circle.Radius, clt.Length), Tol);
        }

        [Test]
        public void TestCalcTargetDistanceCircleToCircle()
        {
            Circle circle1;
            Circle circle2;
            Clothoid clt;

            circle1 = new Circle(4, 4, 3);
            circle2 = new Circle(5.5, 4, 1.25);
            Assert.That(Clothoid.Connect(circle1, circle2, Tol, out clt) == 0);
            Assert.AreEqual(circle1.Center.DistanceTo(circle2.Center), Clothoid.CalcTargetDistance(circle1.Radius, circle2.Radius, clt.Length), Tol);

            circle1 = new Circle(4, 4, -3);
            circle2 = new Circle(5.5, 4, -1.25);
            Assert.That(Clothoid.Connect(circle1, circle2, Tol, out clt) == 0);
            Assert.AreEqual(circle1.Center.DistanceTo(circle2.Center), Clothoid.CalcTargetDistance(circle1.Radius, circle2.Radius, clt.Length), Tol);

            // Reversed connection

            circle1 = new Circle(4, 4, 3);
            circle2 = new Circle(8.5, 4, -1.25);
            Assert.That(Clothoid.Connect(circle1, circle2, Tol, out clt) == 0);
            Assert.AreEqual(circle1.Center.DistanceTo(circle2.Center), Clothoid.CalcTargetDistance(circle1.Radius, circle2.Radius, clt.Length), Tol);

            circle1 = new Circle(4, 4, -3);
            circle2 = new Circle(8.5, 4, 1.25);
            Assert.That(Clothoid.Connect(circle1, circle2, Tol, out clt) == 0);
            Assert.AreEqual(circle1.Center.DistanceTo(circle2.Center), Clothoid.CalcTargetDistance(circle1.Radius, circle2.Radius, clt.Length), Tol);
        }


        [Test, MaxTime(milliseconds: 5000)]
        public void TestFittingPerformance()
        {
            const int SampleSize = 1000000;

            const double x0 = 1000000;
            const double y0 = -1000000;

            Clothoid clothoid;

            var line = new Line(new Point(-1000 + x0, -190 + y0), new Point(+1000 + x0, 130 + y0));
            var circle = new Circle(100 + x0, -90 + y0, -60);

            for (var i = 0; i < SampleSize; i++)
            {
                Clothoid.Connect(line, circle, true, Tol, out clothoid);
            }
        }

        [Test, MaxTime(milliseconds: 3000)]
        public void TestIntersectionPerformance()
        {
            const int SampleSize = 1000000;

            const double x0 = 1000000;
            const double y0 = -1000000;

            Clothoid clothoid;

            var c1 = new Circle(10 + x0, 10 + y0, 10);
            var c2 = new Circle(13 + x0, 15 + y0, 4);
            Clothoid.Connect(c1, c2, Tol, out clothoid);

            var secantLine = new Line(c1.Center, c2.Center);

            for (var i = 0; i < SampleSize; i++)
            {
                Point pt;
                Assert.IsTrue(clothoid.Intersect(secantLine, out pt));
            }
        }
        
        [Test]
        public void TestConnectRealCase1()
        {
            Clothoid clothoid;
            var c1 = new Circle(494112.23190607992, 465307.61364810419, 28426.027381502277);
            var c2 = new Circle(505272.21279380494, 491104.94119093328, 317.18744616210637);
            Clothoid.Connect(c1, c2, Tol, out clothoid);

            CheckConnectivity(clothoid, c1, true);
            CheckConnectivity(clothoid, c2, false);
        }

        [Test]
        public void TestConnectRealCase2()
        {
            Clothoid clothoid;
            var c1 = new Circle(-1294.93753657604, 2132.13051366211, -254.3);
            var c2 = new Circle(-1305.91209785884, 2121.9603166858, -239.0);
            Assert.That(Clothoid.Connect(c1, c2, Tol / 100, out clothoid) == 0);

            CheckConnectivity(clothoid, c1, true);
            CheckConnectivity(clothoid, c2, false);
        }
    }
}
