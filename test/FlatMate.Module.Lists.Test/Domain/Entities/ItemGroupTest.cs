using System;
using FlatMate.Module.Account.Domain.Entities;
using FlatMate.Module.Lists.Domain.Entities;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using prayzzz.Common.Result;

namespace FlatMate.Module.Lists.Test.Domain.Entities
{
    [TestClass]
    public class ItemGroupTest
    {
        [TestMethod]
        public void Test_Constructor_For_New_ItemGroup()
        {
            const string name = "MyGroup";
            var owner = new User();

            var itemGroup = new ItemGroup(name, owner);

            Assert.IsNotNull(itemGroup);
            Assert.AreEqual(Entity.DefaultId, itemGroup.Id);
            Assert.AreSame(name, itemGroup.Name);
            Assert.AreSame(owner, itemGroup.Owner);
            Assert.IsTrue(itemGroup.CreationDate > DateTime.Now.AddSeconds(-1));
            Assert.IsTrue(itemGroup.ModifiedDate > DateTime.Now.AddSeconds(-1));
        }

        [TestMethod]
        public void Test_Constructor_For_Existing_ItemGroup()
        {
            const int id = 1;
            const string name = "MyGroup";
            var owner = new User();
            var creationDate = DateTime.Now.AddDays(-1);
            var modifiedDate = DateTime.Now;

            var itemGroup = new ItemGroup(id, name, owner, creationDate, modifiedDate);

            Assert.IsNotNull(itemGroup);
            Assert.AreEqual(id, itemGroup.Id);
            Assert.IsTrue(itemGroup.IsSaved);
            Assert.AreSame(name, itemGroup.Name);
            Assert.AreSame(owner, itemGroup.Owner);
            Assert.AreEqual(creationDate, itemGroup.CreationDate);
            Assert.AreEqual(modifiedDate, itemGroup.ModifiedDate);
        }

        [TestMethod]
        public void Test_Constructor_Invalid_Id()
        {
            const int id = -1;

            Assert.ThrowsException<ArgumentException>(() => new ItemGroup(id, "MyGroup", new User(), DateTime.Now, DateTime.Now));
        }

        [TestMethod]
        public void Test_Constructor_Invalid_Name_Empty()
        {
            const string name = "";

            Assert.ThrowsException<ArgumentException>(() => new ItemGroup(name, new User()));
            Assert.ThrowsException<ArgumentException>(() => new ItemGroup(1, name, new User(), DateTime.Now, DateTime.Now));
        }

        [TestMethod]
        public void Test_Constructor_Invalid_Name_Null()
        {
            const string name = null;

            Assert.ThrowsException<ArgumentException>(() => new ItemGroup(name, new User()));
            Assert.ThrowsException<ArgumentException>(() => new ItemGroup(1, name, new User(), DateTime.Now, DateTime.Now));
        }

        [TestMethod]
        public void Test_Constructor_Invalid_User()
        {
            User owner = null;

            Assert.ThrowsException<ArgumentNullException>(() => new ItemGroup("MyGroup", owner));
            Assert.ThrowsException<ArgumentNullException>(() => new ItemGroup(1, "MyGroup", owner, DateTime.Now, DateTime.Now));
        }

        [TestMethod]
        public void Test_Rename()
        {
            const string initialName = "MyGroup";
            const string newName = "MyAwesomeGroup";
            var modifiedDate = DateTime.Now.AddMinutes(-1);

            var itemGroup = new ItemGroup(1, initialName, new User(), DateTime.Now, modifiedDate);

            Assert.AreSame(initialName, itemGroup.Name);

            var result = itemGroup.Rename(newName);

            Assert.IsInstanceOfType(result, typeof(SuccessResult));
            Assert.AreSame(newName, itemGroup.Name);
            Assert.IsTrue(itemGroup.ModifiedDate > modifiedDate);
        }

        [TestMethod]
        public void Test_AddItem()
        {
            var modifiedDate = DateTime.Now.AddMinutes(-1);
            var itemGroup = new ItemGroup(1, "MyGroup", new User(), DateTime.Now, modifiedDate);

            Assert.AreEqual(0, itemGroup.Items.Count);

            var item = new Item("MyItem", new User());
            var result = itemGroup.AddItem(item);

            Assert.IsInstanceOfType(result, typeof(SuccessResult));
            Assert.AreEqual(1, itemGroup.Items.Count);
            Assert.AreSame(item, itemGroup.Items[0]);
            Assert.IsTrue(itemGroup.ModifiedDate > modifiedDate);
        }
    }
}