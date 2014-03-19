using System.Collections.Generic;
using System.Linq;
using KA.ObjectComparator;
using NUnit.Framework;

namespace Tests
{
    [TestFixture]
    public class ObjectComparatorTest
    {
        [Test]
        public void BasicTest()
        {
            var rightItem1 = new TestObject {Id = 1, X = 1, Y = 1};
            var rightItem2 = new TestObject {Id = 2, X = 1, Y = 1};
            var rigthItem3 = new TestObject {Id = 3, X = 3, Y = 1};
            var rightItem4 = new TestObject {Id = 4, Name = "this is id 4", X = 1, Y = 3};
            var leftItem1 = new TestObject {Id = 1, X = 1, Y = 1};
            var leftItem3 = new TestObject {Id = 3, X = 1, Y = 1};
            var leftItem4 = new TestObject {Id = 4, X = 2, Y = 1};
            var leftItem6 = new TestObject {Id = 6, Name = "test", X = 2, Y = 1};

            var right = new List<TestObject> {rightItem1, rightItem2, rigthItem3, rightItem4};
            var left = new List<TestObject> {leftItem1, leftItem3, leftItem4, leftItem6};

            var differ = new ObjectComparator<TestObject>(left, right);
            var diffs = differ.Difference();

            var added = diffs.ToList().FindAll(x => x is Added<TestObject>);
            var deleted = diffs.ToList().FindAll(x => x is Deleted<TestObject>);
            var modified = diffs.ToList().FindAll(x => x is Modified<TestObject>);

            Assert.IsNotEmpty(added, "Number of added items in the collection should not be empty.");
            Assert.IsNotEmpty(deleted, "Number of deleted items in the collection should not be empty.");
            Assert.IsNotEmpty(modified, "Number of modified items in the collection should not be empty.");

            Assert.AreEqual(1, added.Count, "There should be 1 item added to the collection.");
            Assert.AreEqual(1, deleted.Count, "There should be 1 item deleted from the collection.");
            Assert.AreEqual(2, modified.Count, "There should be 2 items modified in the collection.");

            Assert.AreEqual(leftItem6, ((Added<TestObject>)added.ElementAt(0)).Item);
            Assert.AreEqual(rightItem2, ((Deleted<TestObject>)deleted.ElementAt(0)).Item);

            var modifiedItem = (Modified<TestObject>)modified.Find(x => ((Modified<TestObject>)x).Right.Key() == rigthItem3.Key());
            Assert.IsNotNull(modifiedItem, "Item3 should be in the list of modified items.");
            Assert.AreEqual(1, modifiedItem.ChangedProperties.Count, "Item3 should have one property modified.");
            Assert.IsTrue(modifiedItem.ChangedProperties.ContainsKey("X"), "Item3 should have its property X modified.");
            Assert.AreEqual(3, modifiedItem.ChangedProperties["X"].Right, "Right value of item3.X should be 3.");
            Assert.AreEqual(1, modifiedItem.ChangedProperties["X"].Left, "Left value of item3.X should be 1.");
            Assert.AreEqual(rigthItem3, modifiedItem.Right);
            Assert.AreEqual(leftItem3, modifiedItem.Left);

            modifiedItem = (Modified<TestObject>)modified.Find(x => ((Modified<TestObject>)x).Right.Key() == rightItem4.Key());
            Assert.IsNotNull(modifiedItem, "Item4 should be in the list of modified items.");
            Assert.AreEqual(3, modifiedItem.ChangedProperties.Count, "Item4 should have three properties modified.");
            Assert.IsTrue(modifiedItem.ChangedProperties.ContainsKey("X"), "Item4 should have its property X modified.");
            Assert.AreEqual(1, modifiedItem.ChangedProperties["X"].Right, "Right value of item4.X should be 1.");
            Assert.AreEqual(2, modifiedItem.ChangedProperties["X"].Left, "Left value of item4.X should be 2.");

            Assert.IsTrue(modifiedItem.ChangedProperties.ContainsKey("Y"), "Item4 should have its property Y modified.");
            Assert.AreEqual(1, modifiedItem.ChangedProperties["Y"].Left, "Left value of item4.Y should be 3.");
            Assert.AreEqual(3, modifiedItem.ChangedProperties["Y"].Right, "Right value of item4.Y should be 1.");

            Assert.IsTrue(modifiedItem.ChangedProperties.ContainsKey("Name"), "Item4 should have its property Name modified.");
            Assert.AreEqual(null, modifiedItem.ChangedProperties["Name"].Left, "left value of item4.Name should be 'this is id 4'.");
            Assert.AreEqual("this is id 4", modifiedItem.ChangedProperties["Name"].Right, "Right value of item4.Name should be NULL.");

            Assert.AreEqual(rightItem4, modifiedItem.Right);
            Assert.AreEqual(leftItem4, modifiedItem.Left);
        }

        [Test]
        public void TestWithIObjectComparatorPropertiesImplementation()
        {
            var rightItem = new TestObject2 { Id = 4, Name = "this is id 4", X = 1, Y = 3 };
            var leftItem = new TestObject2 { Id = 4, Name = "test", X = 2, Y = 1 };

            var right = new List<TestObject2> { rightItem };
            var left = new List<TestObject2> { leftItem };

            var differ = new ObjectComparator<TestObject2>(left, right);
            var diffs = differ.Difference();

            var added = diffs.ToList().FindAll(x => x is Added<TestObject2>);
            var deleted = diffs.ToList().FindAll(x => x is Deleted<TestObject2>);
            var modified = diffs.ToList().FindAll(x => x is Modified<TestObject2>);

            Assert.IsEmpty(added, "Collection should not be empty.");
            Assert.IsEmpty(deleted, "Collection should not be empty.");
            Assert.IsNotEmpty(modified, "Collection should not be empty.");

            Assert.AreEqual(1, modified.Count, "There should be 1 modified item in the collection.");

            var modifiedItem = (Modified<TestObject2>)modified.Find(x => ((Modified<TestObject2>)x).Right.Key() == rightItem.Key());
            Assert.IsNotNull(modifiedItem, "Should be in the list of modified items.");
            Assert.AreEqual(2, modifiedItem.ChangedProperties.Count, "Should have two properties modified (name property should be disregarded).");
            Assert.IsTrue(modifiedItem.ChangedProperties.ContainsKey("X"), "Should have its property X modified.");
            Assert.AreEqual(2, modifiedItem.ChangedProperties["X"].Left, "Left value of X should be 1.");
            Assert.AreEqual(1, modifiedItem.ChangedProperties["X"].Right, "Right value of X should be 2.");

            Assert.IsTrue(modifiedItem.ChangedProperties.ContainsKey("Y"), "Should have its property Y modified.");
            Assert.AreEqual(1, modifiedItem.ChangedProperties["Y"].Left, "Left value of Y should be 3.");
            Assert.AreEqual(3, modifiedItem.ChangedProperties["Y"].Right, "Right value of Y should be 1.");

            Assert.IsFalse(modifiedItem.ChangedProperties.ContainsKey("Name"), "Should not have its Name property modified.");

            Assert.AreEqual(rightItem, modifiedItem.Right);
            Assert.AreEqual(leftItem, modifiedItem.Left);
        }

        [Test]
        public void TestWithEmptyListOnRightSide()
        {
            var rightItem1 = new TestObject {Id = 1, X = 1, Y = 1};
            var rightItem2 = new TestObject {Id = 2, X = 1, Y = 1};
            var rigthItem3 = new TestObject {Id = 3, X = 3, Y = 1};
            var rightItem4 = new TestObject {Id = 4, X = 1, Y = 3};

            var right = new List<TestObject>();
            var left = new List<TestObject> {rightItem1, rightItem2, rigthItem3, rightItem4};

            var differ = new ObjectComparator<TestObject>(left, right);
            var diffs = differ.Difference();

            var added = diffs.ToList().FindAll(x => x is Added<TestObject>);
            var deleted = diffs.ToList().FindAll(x => x is Deleted<TestObject>);
            var modified = diffs.ToList().FindAll(x => x is Modified<TestObject>);

            Assert.IsNotEmpty(added, "Number of added items in the collection should not be empty.");
            Assert.IsEmpty(deleted, "Number of deleted items in the collection should be empty.");
            Assert.IsEmpty(modified, "Number of modified items in the collection should be empty.");

            Assert.AreEqual(4, added.Count, "There should be 4 item added to the collection.");
        }

        [Test]
        public void TestListWithSameObjects()
        {
            var item = new TestObject { Id = 1, X = 1, Y = 1 };

            var right = new List<TestObject> {item};
            var left = new List<TestObject> {item};

            var differ = new ObjectComparator<TestObject>(left, right);
            var diffs = differ.Difference();

            Assert.IsTrue(diffs.Count == 0, "There should be no difference between the right and left collection.");
        }

        [Test]
        public void TestException()
        {
            var item = new TestObject { Id = 1, X = 1, Y = 1 };

            var right = new List<TestObject> { item };

            var differ = new ObjectComparator<TestObject>(null, right);

            Assert.Throws<ObjectComparatorException>(() => differ.Difference());

            var left = new List<TestObject> { item };

            differ = new ObjectComparator<TestObject>(left, null);

            Assert.Throws<ObjectComparatorException>(() => differ.Difference());

            differ = new ObjectComparator<TestObject>(null, null);

            Assert.Throws<ObjectComparatorException>(() => differ.Difference());
        }
    }
}
