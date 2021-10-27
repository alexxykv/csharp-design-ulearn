using System;
using System.Linq;
using System.Reflection;

namespace Reflection.Randomness
{
    public class Generator<T>
        where T : class, new()
    {
        static readonly PropertyInfo[] properties;
        static readonly FromDistributionAttribute[] attributes;
        static Generator()
        {
            properties = typeof(T)
                .GetProperties()
                .Where(z => Attribute.IsDefined(z, typeof(FromDistributionAttribute)))
                .ToArray();
            attributes = properties
                .Select(z => z.GetCustomAttribute<FromDistributionAttribute>())
                .ToArray();
        }

        static ConstructorInfo GetDistributionConstructor(FromDistributionAttribute attr)
        {
            if (!attr.Distribution.GetInterfaces().Contains(typeof(IContinuousDistribution)))
                throw new ArgumentException(
                    $"{attr.Distribution.Name} doesn't implement " +
                    $"interface 'IContinuousDistribution'");

            var types = attr.Parameters
                .Select(z => z.GetType())
                .ToArray();
            var constructor = attr.Distribution.GetConstructor(types);

            if (constructor == null)
                throw new ArgumentException(
                    $"{attr.Distribution.Name} doesn't have a constructor " +
                    $"that takes {attr.Parameters.Length} arguments");

            return constructor;
        }

        public T Generate(Random rnd)
        {
            var obj = new T();

            for (var i = 0; i < properties.Length; i++)
            {
                var constructor = GetDistributionConstructor(attributes[i]);
                var parameters = attributes[i].Parameters
                    .Select(z => (object)z)
                    .ToArray();
                var distribution = (IContinuousDistribution)constructor.Invoke(parameters);
                properties[i].SetValue(obj, distribution.Generate(rnd));
            }

            return obj;
        }
    }

    public class FromDistributionAttribute : Attribute
    {
        public Type Distribution { get; }
        public double[] Parameters { get; }

        public FromDistributionAttribute(Type distribution, params double[] parameters)
        {
            Distribution = distribution;
            Parameters = parameters;
        }
    }
}
