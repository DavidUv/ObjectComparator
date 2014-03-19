using KA.ObjectComparator;

namespace Tests
{
    public class TestObject2 : IObjectComparatorKey, IObjectComparatorIgnoreProperties
    {
        public int Id { get; set; }
        public int X { get; set; }
        public int Y { get; set; }
        public string Name { get; set; }

        public int Key()
        {
            return Id.GetHashCode();
        }

        //public override bool Equals(object o)
        //{
        //    var e = o as TestObject2;
        //    if (e == null) return false;
        //    return X == e.X && Y == e.Y && Id == e.Id;
        //}

        //public override int GetHashCode()
        //{
        //    return X.GetHashCode() ^ Y.GetHashCode() ^ Id.GetHashCode();
        //}

        public string[] IgnoreProperties()
        {
            return new []{"Name"};
        }
    }
}