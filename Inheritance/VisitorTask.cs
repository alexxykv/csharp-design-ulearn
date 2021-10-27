using System;
using System.Collections.Generic;

namespace Inheritance.Geometry.Visitor
{
    public abstract class Body
    {
        public Vector3 Position { get; }

        protected Body(Vector3 position)
        {
            Position = position;
        }

        public abstract Body Accept(IVisitor visitor);
    }

    public class Ball : Body
    {
        public double Radius { get; }

        public Ball(Vector3 position, double radius) : base(position)
        {
            Radius = radius;
        }

        public override Body Accept(IVisitor visitor) => visitor.Visit(this);
    }

    public class RectangularCuboid : Body
    {
        public double SizeX { get; }
        public double SizeY { get; }
        public double SizeZ { get; }

        public RectangularCuboid(Vector3 position, double sizeX, double sizeY, double sizeZ) : base(position)
        {
            SizeX = sizeX;
            SizeY = sizeY;
            SizeZ = sizeZ;
        }

        public override Body Accept(IVisitor visitor) => visitor.Visit(this);
    }

    public class Cylinder : Body
    {
        public double SizeZ { get; }

        public double Radius { get; }

        public Cylinder(Vector3 position, double sizeZ, double radius) : base(position)
        {
            SizeZ = sizeZ;
            Radius = radius;
        }

        public override Body Accept(IVisitor visitor) => visitor.Visit(this);
    }

    public class CompoundBody : Body
    {
        public IReadOnlyList<Body> Parts { get; }

        public CompoundBody(IReadOnlyList<Body> parts) : base(parts[0].Position)
        {
            Parts = parts;
        }

        public override Body Accept(IVisitor visitor) => visitor.Visit(this);
    }

    public static class VectorExtension
    {
        public static Vector3 Divide(this Vector3 v, int divisor)
        {
            return new Vector3(v.X / divisor, v.Y / divisor, v.Z / divisor);
        }
    }

    public interface IVisitor
    {
        Body Visit(Body body);
        Body Visit(Ball ball);
        Body Visit(RectangularCuboid rectangularCuboid);
        Body Visit(Cylinder cylinder);
        Body Visit(CompoundBody compoundBody);
    }

    public class BoundingBoxVisitor : IVisitor
    {
        public Body Visit(Body body)
        {
            if (body is Ball ball) return Visit(ball);
            if (body is RectangularCuboid rectCuboid) return Visit(rectCuboid);
            if (body is Cylinder cylinder) return Visit(cylinder);
            if (body is CompoundBody compoundBody) return Visit(compoundBody);
            throw new ArgumentException();
        }

        public Body Visit(Ball ball)
        {
            var diameter = ball.Radius * 2;
            return new RectangularCuboid(ball.Position, diameter, diameter, diameter);
        }

        public Body Visit(RectangularCuboid rectCuboid)
        {
            return new RectangularCuboid(rectCuboid.Position,
                rectCuboid.SizeX, rectCuboid.SizeY, rectCuboid.SizeZ);
        }

        public Body Visit(Cylinder cylinder)
        {
            var diameter = cylinder.Radius * 2;
            return new RectangularCuboid(cylinder.Position, diameter, diameter, cylinder.SizeZ);
        }

        public Body Visit(CompoundBody compoundBody)
        {
            var minPoint = new Vector3(double.MaxValue, double.MaxValue, double.MaxValue);
            var maxPoint = new Vector3(double.MinValue, double.MinValue, double.MinValue);

            foreach (var part in compoundBody.Parts)
            {
                var boundingBox = (RectangularCuboid)Visit(part);
                minPoint = new Vector3(
                    Math.Min(boundingBox.Position.X - boundingBox.SizeX / 2, minPoint.X),
                    Math.Min(boundingBox.Position.Y - boundingBox.SizeY / 2, minPoint.Y),
                    Math.Min(boundingBox.Position.Z - boundingBox.SizeZ / 2, minPoint.Z));
                maxPoint = new Vector3(
                    Math.Max(boundingBox.Position.X + boundingBox.SizeX / 2, maxPoint.X),
                    Math.Max(boundingBox.Position.Y + boundingBox.SizeY / 2, maxPoint.Y),
                    Math.Max(boundingBox.Position.Z + boundingBox.SizeZ / 2, maxPoint.Z));
            }

            var position = (minPoint + maxPoint).Divide(2);
            var size = maxPoint - minPoint;

            return new RectangularCuboid(position, size.X, size.Y, size.Z);
        }
    }

    public class BoxifyVisitor : IVisitor
    {
        public Body Visit(Body body)
        {
            if (body is Ball ball) return Visit(ball);
            if (body is RectangularCuboid rectCuboid) return Visit(rectCuboid);
            if (body is Cylinder cylinder) return Visit(cylinder);
            if (body is CompoundBody compoundBody) return Visit(compoundBody);
            throw new ArgumentException();
        }

        public Body Visit(Ball ball)
        {
            return new BoundingBoxVisitor().Visit(ball);
        }

        public Body Visit(RectangularCuboid rectCuboid)
        {
            return new BoundingBoxVisitor().Visit(rectCuboid);
        }

        public Body Visit(Cylinder cylinder)
        {
            return new BoundingBoxVisitor().Visit(cylinder);
        }

        public Body Visit(CompoundBody compoundBody)
        {
            var bodies = new List<Body>();

            foreach (var part in compoundBody.Parts)
            {
                if (part is CompoundBody cb)
                {
                    bodies.Add(Visit(cb));
                }
                else
                {
                    bodies.Add(new BoundingBoxVisitor().Visit(part));
                }
            }

            return new CompoundBody(bodies);
        }
    }
}