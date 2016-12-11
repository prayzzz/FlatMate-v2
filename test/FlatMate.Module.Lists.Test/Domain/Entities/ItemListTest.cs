using System;
using FlatMate.Module.Account.Domain.Entities;
using FlatMate.Module.Lists.Domain.Entities;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using prayzzz.Common.Result;

namespace FlatMate.Module.Lists.Test.Domain.Entities
{
    [TestClass]
    public class ItemListTest
    {
        [TestMethod]
        public void Test_Constructor_For_New_ItemList()
        {
            const string name = "MyList";
            var owner = new User();

            var itemList = new ItemList(name, owner);

            Assert.IsNotNull(itemList);
            Assert.AreEqual(Entity.DefaultId, itemList.Id);
            Assert.AreSame(name, itemList.Name);
            Assert.AreSame(owner, itemList.Owner);
            Assert.IsTrue(itemList.CreationDate > DateTime.Now.AddSeconds(-1));
            Assert.IsTrue(itemList.ModifiedDate > DateTime.Now.AddSeconds(-1));
        }

        [TestMethod]
        public void Test_Constructor_For_Existing_ItemList()
        {
            const int id = 1;
            const string name = "MyList";
            var owner = new User();
            var creationDate = DateTime.Now.AddDays(-1);
            var modifiedDate = DateTime.Now;

            var itemList = new ItemList(id, name, owner, creationDate, modifiedDate);

            Assert.IsNotNull(itemList);
            Assert.AreEqual(id, itemList.Id);
            Assert.IsTrue(itemList.IsSaved);
            Assert.AreSame(name, itemList.Name);
            Assert.AreSame(owner, itemList.Owner);
            Assert.AreEqual(creationDate, itemList.CreationDate);
            Assert.AreEqual(modifiedDate, itemList.ModifiedDate);
        }

        [TestMethod]
        public void Test_Constructor_Invalid_Id()
        {
            const int id = -1;

            Assert.ThrowsException<ArgumentException>(() => new ItemList(id, "MyList", new User(), DateTime.Now, DateTime.Now));
        }

        [TestMethod]
        public void Test_Constructor_Invalid_Name_Empty()
        {
            const string name = "";

            Assert.ThrowsException<ArgumentException>(() => new ItemList(name, new User()));
            Assert.ThrowsException<ArgumentException>(() => new ItemList(1, name, new User(), DateTime.Now, DateTime.Now));
        }

        [TestMethod]
        public void Test_Constructor_Invalid_Name_Null()
        {
            const string name = null;

            Assert.ThrowsException<ArgumentException>(() => new ItemList(name, new User()));
            Assert.ThrowsException<ArgumentException>(() => new ItemList(1, name, new User(), DateTime.Now, DateTime.Now));
        }

        [TestMethod]
        public void Test_Constructor_Invalid_User()
        {
            User owner = null;

            Assert.ThrowsException<ArgumentNullException>(() => new ItemList("MyList", owner));
            Assert.ThrowsException<ArgumentNullException>(() => new ItemList(1, "MyList", owner, DateTime.Now, DateTime.Now));
        }

        [TestMethod]
        public void Test_Rename()
        {
            const string initialName = "MyList";
            const string newName = "MyAwesomeList";
            var modifiedDate = DateTime.Now.AddMinutes(-1);
            
            var itemList = new ItemList(1, initialName, new User(), DateTime.Now, modifiedDate);

            Assert.AreSame(initialName, itemList.Name);

            var result = itemList.Rename(newName);

            Assert.IsInstanceOfType(result, typeof(SuccessResult));
            Assert.AreSame(newName, itemList.Name);
            Assert.IsTrue(itemList.ModifiedDate > modifiedDate);
        }

        [TestMethod]
        public void Test_AddGroup()
        {
            var modifiedDate = DateTime.Now.AddMinutes(-1);

            var itemList = new ItemList(1, "MyList", new User(), DateTime.Now, modifiedDate);

            Assert.AreEqual(0, itemList.Groups.Count);

            var group = new ItemGroup("MyGroup", new User());
            var result = itemList.AddGroup(group);

            Assert.IsInstanceOfType(result, typeof(SuccessResult));
            Assert.AreEqual(1, itemList.Groups.Count);
            Assert.AreSame(group, itemList.Groups[0]);
            Assert.IsTrue(itemList.ModifiedDate > modifiedDate);
        }
    }
}
