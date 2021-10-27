using System;
using System.Collections.Generic;
using System.Linq;

namespace SRP.ControlDigit
{
    public static class Extensions
    {
        public static IEnumerable<int> GetDigitsReverse(this long number)
        {
            while (number > 0)
            {
                yield return (int)(number % 10);
                number /= 10;
            }
        }

        public static IEnumerable<int> GetDigitsReverse(this int number) => GetDigitsReverse((long)number);

        public static int Sum(this IEnumerable<int> collection, Func<int, int> selector, Func<int, bool> indexSelector)
        {
            return collection
                .Select((value, index) => indexSelector(index) ? selector(value) : value)
                .Sum();
        }
    }

    public static class ControlDigitAlgo
    {
        public static int Upc(long number)
        {
            var factor = 3;
            var sum = number
                .GetDigitsReverse()
                .Sum(digit => factor * digit, index => index % 2 == 0);
            var result = sum % 10;
            return result == 0 ? result : 10 - result;
        }

        public static int Isbn10(long number)
        {
            var factor = 1;
            var sum = number
                .GetDigitsReverse()
                .Sum(digit => digit * ++factor, index => true);
            var result = sum % 11 == 0 ? sum : 11 - sum % 11;
            return result == 10 ? 'X' : result.ToString().First();
        }

        public static int Luhn(long number)
        {
            var factor = 2;
            var sum = number
                .GetDigitsReverse()
                .Sum(digit => (digit * factor).GetDigitsReverse().Sum(), index => index % 2 == 0);
            return sum * 9 % 10;
        }
    }
}
