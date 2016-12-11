using System;
using FlatMate.Module.Account.Domain.Entities;
using FlatMate.Module.Lists.Domain.Entities;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using prayzzz.Common.Result;

namespace FlatMate.Module.Lists.Test.Domain.Entities
{
    [TestClass]
    public class ItemTest
    {
        [TestMethod]
        public void Test_Constructor_For_New_Item()
        {
            const string name = "MyItem";
            var owner = new User();

            var item = new Item(name, owner);

            Assert.IsNotNull(item);
            Assert.AreEqual(Entity.DefaultId, item.Id);
            Assert.AreSame(name, item.Name);
            Assert.AreSame(owner, item.Owner);
            Assert.IsTrue(item.CreationDate > DateTime.Now.AddSeconds(-1));
            Assert.IsTrue(item.ModifiedDate > DateTime.Now.AddSeconds(-1));
        }

        [TestMethod]
        public void Test_Constructor_For_Existing_Item()
        {
            const int id = 1;
            const string name = "MyItem";
            var owner = new User();
            var creationDate = DateTime.Now.AddDays(-1);
            var modifiedDate = DateTime.Now;

            var item = new Item(id, name, owner, creationDate, modifiedDate);

            Assert.IsNotNull(item);
            Assert.AreEqual(id, item.Id);
            Assert.IsTrue(item.IsSaved);
            Assert.AreSame(name, item.Name);
            Assert.AreSame(owner, item.Owner);
            Assert.AreEqual(creationDate, item.CreationDate);
            Assert.AreEqual(modifiedDate, item.ModifiedDate);
        }

        [TestMethod]
        public void Test_Constructor_Invalid_Id()
        {
            const int id = -1;

            Assert.ThrowsException<ArgumentException>(() => new Item(id, "MyItem", new User(), DateTime.Now, DateTime.Now));
        }

        [TestMethod]
        public void Test_Constructor_Invalid_Name_Empty()
        {
            const string name = "";

            Assert.ThrowsException<ArgumentException>(() => new Item(name, new User()));
            Assert.ThrowsException<ArgumentException>(() => new Item(1, name, new User(), DateTime.Now, DateTime.Now));
        }

        [TestMethod]
        public void Test_Constructor_Invalid_Name_Null()
        {
            const string name = null;

            Assert.ThrowsException<ArgumentException>(() => new Item(name, new User()));
            Assert.ThrowsException<ArgumentException>(() => new Item(1, name, new User(), DateTime.Now, DateTime.Now));
        }

        [TestMethod]
        public void Test_Constructor_Invalid_User()
        {
            User owner = null;

            Assert.ThrowsException<ArgumentNullException>(() => new Item("MyItem", owner));
            Assert.ThrowsException<ArgumentNullException>(() => new Item(1, "MyItem", owner, DateTime.Now, DateTime.Now));
        }

        [TestMethod]
        public void Test_Rename()
        {
            const string initialName = "MyItem";
            const string newName = "MyAwesomeItem";
            var modifiedDate = DateTime.Now.AddMinutes(-1);

            var item = new Item(1, initialName, new User(), DateTime.Now, modifiedDate);

            Assert.AreSame(initialName, item.Name);

            var result = item.Rename(newName);

            Assert.IsInstanceOfType(result, typeof(SuccessResult));
            Assert.AreSame(newName, item.Name);
            Assert.IsTrue(item.ModifiedDate > modifiedDate);
        }
    }
}