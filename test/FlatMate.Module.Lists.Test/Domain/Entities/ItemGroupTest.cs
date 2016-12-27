using System;
using FlatMate.Module.Account.Shared.Dtos;
using FlatMate.Module.Common.Domain.Entities;
using FlatMate.Module.Lists.Domain.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using prayzzz.Common.Result;

namespace FlatMate.Module.Lists.Test.Domain.Entities
{
    [TestClass]
    public class ItemGroupTest
    {
        [TestMethod]
        public void Test_AddItem()
        {
            var itemList = ItemList.Create("itemlist", new UserDto()).Data;
            var itemGroup = ItemGroup.Create(1, "MyGroup", new UserDto(), itemList).Data;

            var result = itemGroup.AddItem("item", new UserDto());

            Assert.IsInstanceOfType(result, typeof(SuccessResult<Item>));
        }

        [TestMethod]
        public void Test_Constructor_For_Existing_ItemGroup()
        {
            const int id = 1;
            const string name = "MyGroup";
            var owner = new UserDto();

            var itemList = ItemList.Create("itemlist", owner).Data;
            var itemGroup = ItemGroup.Create(id, name, owner, itemList).Data;

            Assert.IsNotNull(itemGroup);
            Assert.AreEqual(id, itemGroup.Id);
            Assert.IsTrue(itemGroup.IsSaved);
            Assert.AreSame(name, itemGroup.Name);
            Assert.AreSame(owner, itemGroup.Owner);
            Assert.IsTrue(itemGroup.CreationDate > DateTime.Now.AddSeconds(-1));
            Assert.IsTrue(itemGroup.ModifiedDate > DateTime.Now.AddSeconds(-1));
        }

        [TestMethod]
        public void Test_Constructor_For_New_ItemGroup()
        {
            const string name = "MyGroup";
            var owner = new UserDto();

            var itemList = ItemList.Create("itemlist", owner).Data;
            var itemGroup = ItemGroup.Create(name, owner, itemList).Data;

            Assert.IsNotNull(itemGroup);
            Assert.AreEqual(Entity.DefaultId, itemGroup.Id);
            Assert.AreSame(name, itemGroup.Name);
            Assert.AreSame(owner, itemGroup.Owner);
            Assert.IsTrue(itemGroup.CreationDate > DateTime.Now.AddSeconds(-1));
            Assert.IsTrue(itemGroup.ModifiedDate > DateTime.Now.AddSeconds(-1));
        }

        [TestMethod]
        public void Test_Constructor_Invalid_Name_Empty()
        {
            const string name = "";

            var itemList = ItemList.Create("itemlist", new UserDto()).Data;

            var createResult1 = ItemGroup.Create(name, new UserDto(), itemList);
            Assert.IsInstanceOfType(createResult1, typeof(ErrorResult<ItemGroup>));

            var createResult2 = ItemGroup.Create(1, name, new UserDto(), itemList);
            Assert.IsInstanceOfType(createResult2, typeof(ErrorResult<ItemGroup>));
        }

        [TestMethod]
        public void Test_Constructor_Invalid_Name_Null()
        {
            const string name = null;

            var itemList = ItemList.Create("itemlist", new UserDto()).Data;

            var createResult1 = ItemGroup.Create(name, new UserDto(), itemList);
            Assert.IsInstanceOfType(createResult1, typeof(ErrorResult<ItemGroup>));

            var createResult2 = ItemGroup.Create(1, name, new UserDto(), itemList);
            Assert.IsInstanceOfType(createResult2, typeof(ErrorResult<ItemGroup>));
        }

        [TestMethod]
        public void Test_Rename()
        {
            const string initialName = "MyGroup";
            const string newName = "MyAwesomeGroup";

            var itemList = ItemList.Create("itemlist", new UserDto()).Data;
            var itemGroup = ItemGroup.Create(1, initialName, new UserDto(), itemList).Data;

            Assert.AreSame(initialName, itemGroup.Name);

            var result = itemGroup.Rename(newName);

            Assert.IsInstanceOfType(result, typeof(SuccessResult));
            Assert.AreSame(newName, itemGroup.Name);
        }
    }
}