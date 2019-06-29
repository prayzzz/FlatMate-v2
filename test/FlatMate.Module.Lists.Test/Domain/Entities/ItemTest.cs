using System;
using FlatMate.Module.Lists.Domain.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using prayzzz.Common.Results;

namespace FlatMate.Module.Lists.Test.Domain.Entities
{
    [TestClass]
    public class ItemTest
    {
        [TestMethod]
        public void Test_Constructor_For_Existing_Item()
        {
            const int id = 1;
            const string name = "MyItem";
            const int ownerId = 1;

            var (_, itemList) = ItemList.Create("MyList", ownerId);
            var (result, item) = Item.Create(id, name, ownerId, itemList, DateTime.UtcNow);

            Assert.IsTrue(result.IsSuccess);
            Assert.IsNotNull(item);
            Assert.AreEqual(id, item.Id);
            Assert.IsTrue(item.IsSaved);
            Assert.AreSame(name, item.Name);
            Assert.AreEqual(ownerId, item.OwnerId);
            Assert.IsTrue(item.Created > DateTime.UtcNow.AddSeconds(-1));
            Assert.IsTrue(item.Modified > DateTime.UtcNow.AddSeconds(-1));
        }

        [TestMethod]
        public void Test_Constructor_For_New_Item()
        {
            const string name = "MyItem";
            const int ownerId = 1;

            var (_, itemList) = ItemList.Create("MyList", ownerId);
            var (result, item) = Item.Create(name, ownerId, itemList);

            Assert.IsTrue(result.IsSuccess);
            Assert.IsNotNull(item);
            Assert.AreEqual(0, item.Id);
            Assert.AreSame(name, item.Name);
            Assert.AreEqual(ownerId, item.OwnerId);
            Assert.IsTrue(item.Created > DateTime.UtcNow.AddSeconds(-1));
            Assert.IsTrue(item.Modified > DateTime.UtcNow.AddSeconds(-1));
        }

        [TestMethod]
        public void Test_Constructor_Invalid_Name_Empty()
        {
            const string name = "";

            var (_, itemList) = ItemList.Create("MyList", 1);

            var (result1, _) = Item.Create(name, 1, itemList);
            Assert.IsInstanceOfType(result1, typeof(Result));
            Assert.IsTrue(result1.IsError);

            var (result2, _) = Item.Create(1, name, 1, itemList, DateTime.UtcNow);
            Assert.IsInstanceOfType(result2, typeof(Result));
            Assert.IsTrue(result2.IsError);
        }

        [TestMethod]
        public void Test_Constructor_Invalid_Name_Null()
        {
            const string name = null;

            var (_, itemList) = ItemList.Create("MyList", 1);

            var (result1, _) = Item.Create(name, 1, itemList);
            Assert.IsInstanceOfType(result1, typeof(Result));
            Assert.IsTrue(result1.IsError);

            var (result2, _) = Item.Create(1, name, 1, itemList, DateTime.UtcNow);
            Assert.IsInstanceOfType(result2, typeof(Result));
            Assert.IsTrue(result2.IsError);
        }

        [TestMethod]
        public void Test_Rename()
        {
            const string initialName = "MyItem";
            const string newName = "MyAwesomeItem";

            var (_, itemList) = ItemList.Create("MyList", 1);

            var (_, item) = Item.Create(1, initialName, 1, itemList, DateTime.UtcNow);
            Assert.AreSame(initialName, item.Name);

            var result = item.Rename(newName);
            Assert.IsInstanceOfType(result, typeof(Result));
            Assert.IsTrue(result.IsSuccess);
            Assert.AreSame(newName, item.Name);
        }
    }
}