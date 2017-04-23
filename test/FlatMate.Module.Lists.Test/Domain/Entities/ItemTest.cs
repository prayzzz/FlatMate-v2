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

            var itemList = ItemList.Create("MyList", ownerId).Data;
            var item = Item.Create(id, name, ownerId, itemList).Data;

            Assert.IsNotNull(item);
            Assert.AreEqual(id, item.Id);
            Assert.IsTrue(item.IsSaved);
            Assert.AreSame(name, item.Name);
            Assert.AreEqual(ownerId, item.OwnerId);
            Assert.IsTrue(item.Created > DateTime.Now.AddSeconds(-1));
            Assert.IsTrue(item.Modified > DateTime.Now.AddSeconds(-1));
        }

        [TestMethod]
        public void Test_Constructor_For_New_Item()
        {
            const string name = "MyItem";
            const int ownerId = 1;

            var itemList = ItemList.Create("MyList", ownerId).Data;
            var item = Item.Create(name, ownerId, itemList).Data;

            Assert.IsNotNull(item);
            Assert.IsNull(item.Id);
            Assert.AreSame(name, item.Name);
            Assert.AreEqual(ownerId, item.OwnerId);
            Assert.IsTrue(item.Created > DateTime.Now.AddSeconds(-1));
            Assert.IsTrue(item.Modified > DateTime.Now.AddSeconds(-1));
        }

        [TestMethod]
        public void Test_Constructor_Invalid_Name_Empty()
        {
            const string name = "";

            var itemList = ItemList.Create("MyList", 1).Data;

            var result1 = Item.Create(name, 1, itemList);
            Assert.IsInstanceOfType(result1, typeof(ErrorResult<Item>));

            var result2 = Item.Create(1, name, 1, itemList);
            Assert.IsInstanceOfType(result2, typeof(ErrorResult<Item>));
        }

        [TestMethod]
        public void Test_Constructor_Invalid_Name_Null()
        {
            const string name = null;

            var itemList = ItemList.Create("MyList", 1).Data;

            var result1 = Item.Create(name, 1, itemList);
            Assert.IsInstanceOfType(result1, typeof(ErrorResult<Item>));

            var result2 = Item.Create(1, name, 1, itemList);
            Assert.IsInstanceOfType(result2, typeof(ErrorResult<Item>));
        }

        [TestMethod]
        public void Test_Rename()
        {
            const string initialName = "MyItem";
            const string newName = "MyAwesomeItem";

            var itemList = ItemList.Create("MyList", 1).Data;

            var item = Item.Create(1, initialName, 1, itemList).Data;
            Assert.AreSame(initialName, item.Name);

            var result = item.Rename(newName);

            Assert.IsInstanceOfType(result, typeof(SuccessResult));
            Assert.AreSame(newName, item.Name);
        }
    }
}