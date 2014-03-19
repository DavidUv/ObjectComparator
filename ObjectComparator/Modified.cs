using System.Collections.Generic;

namespace KA.ObjectComparator
{
    /// <summary>
    /// A class that holds the right and left values of a modified property.
    /// </summary>
    public class PropertyValues
    {
        public object Left { get; set; }
        public object Right { get; set; }
    }

    /// <summary>
    /// A class that represent a modified item. An item is considered modified if
    /// it exists in both the left and right collection and some of the properties differ.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class Modified<T> : Difference where T : IObjectComparatorKey
    {
        /// <summary>
        /// Dictionary where key is the name of the property that was changed and value
        /// is an instance of PropertyValues.
        /// </summary>
        public IDictionary<string, PropertyValues> ChangedProperties { get; set; }

        /// <summary>
        /// This is the actual item in the Left collection
        /// </summary>
        public T Left { get; set; }

        /// <summary>
        /// This is the actual item in the Right collection
        /// </summary>
        public T Right { get; set; }
    }
}