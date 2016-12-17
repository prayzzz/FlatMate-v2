using System;
using FlatMate.Module.Account.Dtos;
using FlatMate.Module.Common.Domain.Entities;
using FlatMate.Module.Lists.Domain.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using prayzzz.Common.Result;

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
            var owner = new UserDto();

            var itemList = ItemList.Create("MyList", owner).Data;
            var itemGroup = ItemGroup.Create("MyGroup", owner, itemList).Data;
            var item = Item.Create(id, name, owner, itemGroup).Data;

            Assert.IsNotNull(item);
            Assert.AreEqual(id, item.Id);
            Assert.IsTrue(item.IsSaved);
            Assert.AreSame(name, item.Name);
            Assert.AreSame(owner, item.Owner);
            Assert.IsTrue(item.CreationDate > DateTime.Now.AddSeconds(-1));
            Assert.IsTrue(item.ModifiedDate > DateTime.Now.AddSeconds(-1));
        }

        [TestMethod]
        public void Test_Constructor_For_New_Item()
        {
            const string name = "MyItem";
            var owner = new UserDto();

            var itemList = ItemList.Create("MyList", owner).Data;
            var itemGroup = ItemGroup.Create("MyGroup", owner, itemList).Data;
            var item = Item.Create(name, owner, itemGroup).Data;

            Assert.IsNotNull(item);
            Assert.AreEqual(Entity.DefaultId, item.Id);
            Assert.AreSame(name, item.Name);
            Assert.AreSame(owner, item.Owner);
            Assert.IsTrue(item.CreationDate > DateTime.Now.AddSeconds(-1));
            Assert.IsTrue(item.ModifiedDate > DateTime.Now.AddSeconds(-1));
        }

        [TestMethod]
        public void Test_Constructor_Invalid_Name_Empty()
        {
            const string name = "";

            var itemList = ItemList.Create("MyList", new UserDto()).Data;
            var itemGroup = ItemGroup.Create("MyGroup", new UserDto(), itemList).Data;

            var result1 = Item.Create(name, new UserDto(), itemGroup);
            Assert.IsInstanceOfType(result1, typeof(ErrorResult<Item>));

            var result2 = Item.Create(1, name, new UserDto(), itemGroup);
            Assert.IsInstanceOfType(result2, typeof(ErrorResult<Item>));
        }

        [TestMethod]
        public void Test_Constructor_Invalid_Name_Null()
        {
            const string name = null;

            var itemList = ItemList.Create("MyList", new UserDto()).Data;
            var itemGroup = ItemGroup.Create("MyGroup", new UserDto(), itemList).Data;

            var result1 = Item.Create(name, new UserDto(), itemGroup);
            Assert.IsInstanceOfType(result1, typeof(ErrorResult<Item>));

            var result2 = Item.Create(1, name, new UserDto(), itemGroup);
            Assert.IsInstanceOfType(result2, typeof(ErrorResult<Item>));
        }

        [TestMethod]
        public void Test_Rename()
        {
            const string initialName = "MyItem";
            const string newName = "MyAwesomeItem";

            var itemList = ItemList.Create("MyList", new UserDto()).Data;
            var itemGroup = ItemGroup.Create("MyGroup", new UserDto(), itemList).Data;

            var item = Item.Create(1, initialName, new UserDto(), itemGroup).Data;
            Assert.AreSame(initialName, item.Name);

            var result = item.Rename(newName);

            Assert.IsInstanceOfType(result, typeof(SuccessResult));
            Assert.AreSame(newName, item.Name);
        }
    }
}