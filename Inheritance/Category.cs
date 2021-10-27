using System;

namespace Inheritance.DataStructure
{
    public class Category : IComparable
    {
        public string ProductName { get; }
        public MessageType MessageType { get; }
        public MessageTopic MessageTopic { get; }

        public Category(
            string productName,
            MessageType messageType,
            MessageTopic messageTopic)
        {
            ProductName = productName;
            MessageType = messageType;
            MessageTopic = messageTopic;
        }

        public int CompareTo(object obj)
        {
            var category = obj as Category;
            if (category == null) return -1;

            var value = string.Compare(ProductName, category.ProductName);
            if (value != 0) return value;

            value = MessageType.CompareTo(category.MessageType);
            if (value != 0) return value;

            return MessageTopic.CompareTo(category.MessageTopic);
        }

        public override bool Equals(object obj)
        {
            return obj is Category && CompareTo(obj) == 0;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override string ToString()
        {
            return $"{ProductName}.{MessageType}.{MessageTopic}";
        }

        public static bool operator >(Category c1, Category c2)
        {
            return c1.CompareTo(c2) == 1;
        }

        public static bool operator <(Category c1, Category c2)
        {
            return c1.CompareTo(c2) == -1;
        }

        public static bool operator >=(Category c1, Category c2)
        {
            return !(c1 < c2);
        }

        public static bool operator <=(Category c1, Category c2)
        {
            return !(c1 > c2);
        }
    }
}
