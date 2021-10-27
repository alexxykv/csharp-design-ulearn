using System.Linq;
using System.Reflection;

namespace Ddd.Infrastructure
{
    public class ValueType<T>
    {
        static readonly PropertyInfo[] properties;
        static ValueType()
        {
            properties = typeof(T)
                .GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .OrderBy(prop => prop.Name)
                .ToArray();
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(this, obj)) return true;
            if (obj is T valueTypeObj)
                return Equals(valueTypeObj);
            return false;
        }

        public bool Equals(T obj)
        {
            if (obj == null) return false;
            return properties
                .Select(prop => prop.GetValue(this))
                .SequenceEqual(properties.Select(prop => prop.GetValue(obj)));
        }

        public override int GetHashCode()
        {
            var prime = 569;
            var result = 0;
            foreach (var prop in properties)
            {
                var hash = prop.GetValue(this).GetHashCode();
                result = unchecked(result * prime ^ hash);
            }
            return result;
        }

        public override string ToString()
        {
            var name = typeof(T).Name;
            var args = properties
                .Select(prop => $"{prop.Name}: {prop.GetValue(this)}")
                .ToArray();
            return $"{name}({string.Join("; ", args)})";
        }
    }
}
