using System;
using FlatMate.Module.Account.Dtos;
using FlatMate.Module.Common.Domain.Entities;
using FlatMate.Module.Lists.Domain.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using prayzzz.Common.Result;

namespace FlatMate.Module.Lists.Test.Domain.Entities
{
    [TestClass]
    public class ItemListTest
    {
        [TestMethod]
        public void Test_AddGroup()
        {
            var itemList = ItemList.Create(1, "MyList", new UserDto()).Data;
            var result = itemList.AddGroup("MyGroup", new UserDto());

            Assert.IsInstanceOfType(result, typeof(SuccessResult<ItemGroup>));
        }

        [TestMethod]
        public void Test_Constructor_For_Existing_ItemList()
        {
            const int id = 1;
            const string name = "MyList";
            var owner = new UserDto();

            var itemList = ItemList.Create(id, name, owner).Data;

            Assert.IsNotNull(itemList);
            Assert.AreEqual(id, itemList.Id);
            Assert.IsTrue(itemList.IsSaved);
            Assert.AreSame(name, itemList.Name);
            Assert.AreSame(owner, itemList.Owner);
            Assert.IsTrue(itemList.CreationDate > DateTime.Now.AddSeconds(-1));
            Assert.IsTrue(itemList.ModifiedDate > DateTime.Now.AddSeconds(-1));
        }

        [TestMethod]
        public void Test_Constructor_For_New_ItemList()
        {
            const string name = "MyList";
            var owner = new UserDto();

            var itemList = ItemList.Create(name, owner).Data;

            Assert.IsNotNull(itemList);
            Assert.AreEqual(Entity.DefaultId, itemList.Id);
            Assert.AreSame(name, itemList.Name);
            Assert.AreSame(owner, itemList.Owner);
            Assert.IsTrue(itemList.CreationDate > DateTime.Now.AddSeconds(-1));
            Assert.IsTrue(itemList.ModifiedDate > DateTime.Now.AddSeconds(-1));
        }

        [TestMethod]
        public void Test_Constructor_Invalid_Name_Empty()
        {
            const string name = "";

            var result1 = ItemList.Create(name, new UserDto());
            Assert.IsInstanceOfType(result1, typeof(ErrorResult<ItemList>));

            var result2 = ItemList.Create(1, name, new UserDto());
            Assert.IsInstanceOfType(result2, typeof(ErrorResult<ItemList>));
        }

        [TestMethod]
        public void Test_Constructor_Invalid_Name_Null()
        {
            const string name = null;

            var result1 = ItemList.Create(name, new UserDto()).Data;
            Assert.IsInstanceOfType(result1, typeof(ErrorResult<ItemList>));

            var result2 = ItemList.Create(1, name, new UserDto()).Data;
            Assert.IsInstanceOfType(result2, typeof(ErrorResult<ItemList>));
        }

        [TestMethod]
        public void Test_Rename()
        {
            const string initialName = "MyList";
            const string newName = "MyAwesomeList";

            var itemList = ItemList.Create(1, initialName, new UserDto()).Data;

            Assert.AreSame(initialName, itemList.Name);

            var result = itemList.Rename(newName);

            Assert.IsInstanceOfType(result, typeof(SuccessResult));
            Assert.AreSame(newName, itemList.Name);
        }
    }
}