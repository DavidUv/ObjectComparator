namespace KA.ObjectComparator
{
    /// <summary>
    /// A class that represent an added item.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class Added<T> : Difference where T : IObjectComparatorKey
    {
        public T Item { get; set; }
    }
}