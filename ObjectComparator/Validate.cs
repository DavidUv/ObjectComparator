namespace KA.ObjectComparator
{
    public class Validate
    {
        public static void NotNull(object obj, string msg = @"Object must not be null.")
        {
            if (obj == null)
            {
                throw new ObjectComparatorException(msg);
            }
        }
    }
}