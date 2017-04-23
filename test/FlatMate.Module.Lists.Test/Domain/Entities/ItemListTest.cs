using System;
using FlatMate.Module.Lists.Domain.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using prayzzz.Common.Results;

namespace FlatMate.Module.Lists.Test.Domain.Entities
{
    [TestClass]
    public class ItemListTest
    {
        [TestMethod]
        public void Test_Constructor_For_Existing_ItemList()
        {
            const int id = 1;
            const string name = "MyList";
            const int ownerId = 1;

            var itemList = ItemList.Create(id, name, ownerId).Data;

            Assert.IsNotNull(itemList);
            Assert.AreEqual(id, itemList.Id);
            Assert.IsTrue(itemList.IsSaved);
            Assert.AreSame(name, itemList.Name);
            Assert.AreEqual(ownerId, itemList.OwnerId);
            Assert.IsTrue(itemList.Created > DateTime.Now.AddSeconds(-1));
            Assert.IsTrue(itemList.Modified > DateTime.Now.AddSeconds(-1));
        }

        [TestMethod]
        public void Test_Constructor_For_New_ItemList()
        {
            const string name = "MyList";
            const int ownerId = 1;

            var itemList = ItemList.Create(name, ownerId).Data;

            Assert.IsNotNull(itemList);
            Assert.IsNull(itemList.Id);
            Assert.AreSame(name, itemList.Name);
            Assert.AreEqual(ownerId, itemList.OwnerId);
            Assert.IsTrue(itemList.Created > DateTime.Now.AddSeconds(-1));
            Assert.IsTrue(itemList.Modified > DateTime.Now.AddSeconds(-1));
        }

        [TestMethod]
        public void Test_Constructor_Invalid_Name_Empty()
        {
            const string name = "";

            var result1 = ItemList.Create(name, 1);
            Assert.IsInstanceOfType(result1, typeof(ErrorResult<ItemList>));

            var result2 = ItemList.Create(1, name, 1);
            Assert.IsInstanceOfType(result2, typeof(ErrorResult<ItemList>));
        }

        [TestMethod]
        public void Test_Constructor_Invalid_Name_Null()
        {
            const string name = null;

            var result1 = ItemList.Create(name, 1).Data;
            Assert.IsInstanceOfType(result1, typeof(ErrorResult<ItemList>));

            var result2 = ItemList.Create(1, name, 1).Data;
            Assert.IsInstanceOfType(result2, typeof(ErrorResult<ItemList>));
        }

        [TestMethod]
        public void Test_Rename()
        {
            const string initialName = "MyList";
            const string newName = "MyAwesomeList";

            var itemList = ItemList.Create(1, initialName, 1).Data;

            Assert.AreSame(initialName, itemList.Name);

            var result = itemList.Rename(newName);

            Assert.IsInstanceOfType(result, typeof(SuccessResult));
            Assert.AreSame(newName, itemList.Name);
        }
    }
}