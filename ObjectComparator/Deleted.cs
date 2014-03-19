namespace KA.ObjectComparator
{
    /// <summary>
    /// A class that represent a deleted item.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class Deleted<T> : Difference where T : IObjectComparatorKey
    {
        public T Item { get; set; }
    }
}