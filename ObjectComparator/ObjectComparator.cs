using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Xml.Schema;

namespace KA.ObjectComparator
{
    /// <summary>
    /// The goal of this class is to compare and report the difference between two collections
    /// of complex objects. To be able to compare the objects they must implement the IComparable interface.
    /// </summary>
    /// <typeparam name="T">A class of type T that implements the IComparable interface.</typeparam>
    public class ObjectComparator<T> where T : IObjectComparatorKey
    {
        private IEnumerable<T> Left { get; set; }
        private IEnumerable<T> Right { get; set; }

        /// <summary>
        /// Construct an ObjectComparator object with a left and right collection.
        /// </summary>
        /// <param name="left">A collection of objects implementing the IComparable interface</param>
        /// <param name="right">A collection of objects implementing the IComparable interface</param>
        public ObjectComparator(IEnumerable<T> left, IEnumerable<T> right)
        {
            Left = left;
            Right = right;
        }

        /// <summary>
        /// Compares two collection of objects.
        /// Currently the comparison is made by comparing left to right. That means that an item found in left but not right is considered
        /// added, an item found in right but not left is considered deleted.
        /// 
        /// TODO: The result of a comparison should return the difference from both the left AND right side view (ie. left -> right AND right -> left)
        /// 
        /// </summary>
        /// <returns>A collection of Difference objects describing the difference</returns>
        public IList<Difference> Difference()
        {
            Validate.NotNull(Left);
            Validate.NotNull(Right);

            var onlyInList1 = Only(Left);
            var onlyInList2 = Only(Right);
            var inBothList1AndList2 = Intersection();

            var differences = onlyInList1.Select(item => new Added<T> { Item = item}).Cast<Difference>().ToList();
            differences.AddRange(onlyInList2.Select(item => new Deleted<T> { Item =  item }));
            differences.AddRange((from item in inBothList1AndList2
                let left = Left.ToList().Find(y => y.Key() == item.Key())
                let right = Right.ToList().Find(y => y.Key() == item.Key())
                let propDiff = PropertiesDiff(left, right)
                where propDiff.Count != 0
                select new Modified<T>
                {
                    ChangedProperties = propDiff,
                    Left = left,
                    Right = right
                }));
            return differences;
        }

        /// <summary>
        /// Compare the property values of the two objects and return the difference as a dictionary.
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns>Dictionary where key is the attribute name and the value is an instance of PropertyValues</returns>
        private static IDictionary<string, PropertyValues> PropertiesDiff(T left, T right)
        {
            Validate.NotNull(left);
            Validate.NotNull(right);

            var diff = new Dictionary<string, PropertyValues>();
            if (left.Equals(right))
            {
                return diff;
            }

            PropertyInfo[] properties = left.GetType().GetProperties();
            if (left is IObjectComparatorIgnoreProperties)
            {
                var propertiesToIgnore = (left as IObjectComparatorIgnoreProperties).IgnoreProperties();
                properties = properties.Where(property => !propertiesToIgnore.Contains(property.Name)).ToArray();
            }
            foreach (var property in properties)
            {
                var leftValue = property.GetValue(left, null);
                var rightValue = property.GetValue(right, null);
                if (leftValue == null && rightValue != null || leftValue != null && rightValue == null)
                {
                    diff.Add(property.Name, new PropertyValues { Left = leftValue, Right = rightValue });
                }
                else if (leftValue != null && !leftValue.Equals(rightValue))
                {
                    diff.Add(property.Name, new PropertyValues { Left = leftValue, Right = rightValue });
                }
            }
            return diff;
        }

        /// <summary>
        /// Find the intersection of the two collections held in the private instance variables Left and Right.
        /// The intersection is computed using the Key method of the objects contained in the collections.
        /// The objects compared must implement the interface IComparable.
        /// </summary>
        /// <returns>A collection of items that exists in both collections.</returns>
        private IEnumerable<T> Intersection()
        {
            var leftDict = Left.ToDictionary(item => item.Key());
            var rightDict = Right.ToDictionary(item => item.Key());
            return leftDict.Keys.Intersect(rightDict.Keys).Select(key => leftDict[key]).ToList();
        }

        /// <summary>
        /// Compares two collection of objects and returns the objects that only exists in parameter list.
        /// </summary>
        /// <param name="list">A collection of objects implementing the IComparable interface</param>
        /// <returns>The items only in list</returns>
        private IEnumerable<T> Only(IEnumerable<T> list)
        {
            Validate.NotNull(list);
            var a = (list == Left ? Left : Right).ToDictionary(item => item.Key());
            var b = (list == Left ? Right : Left).ToDictionary(item => item.Key());
            return a.Keys.Except(b.Keys).Select(key => a[key]).ToList();
        }
    }
}