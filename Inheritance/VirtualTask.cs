using System;
using System.Collections.Generic;
using System.Linq;

namespace Inheritance.Geometry.Virtual
{
    public abstract class Body
    {
        public Vector3 Position { get; }

        protected Body(Vector3 position)
        {
            Position = position;
        }

        public abstract bool ContainsPoint(Vector3 point);

        public abstract RectangularCuboid GetBoundingBox();
    }

    public class Ball : Body
    {
        public double Radius { get; }

        public Ball(Vector3 position, double radius) : base(position)
        {
            Radius = radius;
        }

        public override bool ContainsPoint(Vector3 point)
        {
            var vector = point - Position;
            var length2 = vector.GetLength2();
            return length2 <= Radius * Radius;
        }

        public override RectangularCuboid GetBoundingBox()
        {
            var diameter = Radius * 2;
            return new RectangularCuboid(Position, diameter, diameter, diameter);
        }
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

        public override bool ContainsPoint(Vector3 point)
        {
            var minPoint = new Vector3(
                Position.X - SizeX / 2,
                Position.Y - SizeY / 2,
                Position.Z - SizeZ / 2);
            var maxPoint = new Vector3(
                Position.X + SizeX / 2,
                Position.Y + SizeY / 2,
                Position.Z + SizeZ / 2);

            return point >= minPoint && point <= maxPoint;
        }

        public override RectangularCuboid GetBoundingBox()
        {
            return new RectangularCuboid(Position, SizeX, SizeY, SizeZ);
        }
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

        public override bool ContainsPoint(Vector3 point)
        {
            var vectorX = point.X - Position.X;
            var vectorY = point.Y - Position.Y;
            var length2 = vectorX * vectorX + vectorY * vectorY;
            var minZ = Position.Z - SizeZ / 2;
            var maxZ = minZ + SizeZ;

            return length2 <= Radius * Radius && point.Z >= minZ && point.Z <= maxZ;
        }

        public override RectangularCuboid GetBoundingBox()
        {
            var diameter = Radius * 2;
            return new RectangularCuboid(Position, diameter, diameter, SizeZ);
        }
    }

    public class CompoundBody : Body
    {
        public IReadOnlyList<Body> Parts { get; }

        public CompoundBody(IReadOnlyList<Body> parts) : base(parts[0].Position)
        {
            Parts = parts;
        }

        public override bool ContainsPoint(Vector3 point)
        {
            return Parts.Any(body => body.ContainsPoint(point));
        }

        public override RectangularCuboid GetBoundingBox()
        {
            var minPoint = new Vector3(double.MaxValue, double.MaxValue, double.MaxValue);
            var maxPoint = new Vector3(double.MinValue, double.MinValue, double.MinValue);

            foreach (var part in Parts)
            {
                var boundingBox = part.GetBoundingBox();
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

    public static class VectorExtension
    {
        public static Vector3 Divide(this Vector3 v, int divisor)
        {
            return new Vector3(v.X / divisor, v.Y / divisor, v.Z / divisor);
        }
    }
}