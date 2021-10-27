using System;
using System.Numerics;

namespace Incapsulation.RationalNumbers
{
    public class Rational
    {
        public readonly int Numerator;
        public readonly int Denominator;
        public bool IsNan => Denominator == 0;

        public Rational(int numerator, int denominator = 1)
        {
            var gcd = (int)BigInteger.GreatestCommonDivisor(
                Math.Abs(numerator),
                Math.Abs(denominator));

            if (gcd == 0)
            {
                Numerator = Denominator = 0;
            }
            else
            {
                Numerator = numerator * denominator < 0 
                    ? -Math.Abs(numerator / gcd)
                    : Math.Abs(numerator / gcd);
                Denominator = Math.Abs(denominator / gcd);
            }
        }

        public static Rational operator +(Rational r1, Rational r2)
        {
            return new Rational(
                r1.Numerator * r2.Denominator + r2.Numerator * r1.Denominator,
                r1.Denominator * r2.Denominator);
        }

        public static Rational operator -(Rational r1, Rational r2)
        {
            return new Rational(
                r1.Numerator * r2.Denominator - r2.Numerator * r1.Denominator,
                r1.Denominator * r2.Denominator);
        }

        public static Rational operator *(Rational r1, Rational r2)
        {
            return new Rational(r1.Numerator * r2.Numerator, r1.Denominator * r2.Denominator);
        }

        public static Rational operator /(Rational r1, Rational r2)
        {
            if (r1.IsNan || r2.IsNan)
                return new Rational(0, 0);
            return new Rational(r1.Numerator * r2.Denominator, r2.Numerator * r1.Denominator);
        }

        public static implicit operator double(Rational r)
        {
            if (r.IsNan) 
                return double.NaN;
            return (double)r.Numerator / r.Denominator;
        }

        public static implicit operator Rational(int n)
        {
            return new Rational(n);
        }

        public static explicit operator int(Rational r)
        {
            if (r.Numerator % r.Denominator != 0)
                throw new Exception();
            return r.Numerator / r.Denominator;
        }
    }
}
