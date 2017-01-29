using FlatMate.Api.Areas.Lists.ItemList;
using FlatMate.Web.Areas.Lists.Data;
using FlatMate.Web.Mvc.Base;
using Microsoft.AspNetCore.Mvc;

namespace FlatMate.Web.Areas.Lists.Controllers
{
    [Area("Lists")]
    public class ItemListController : MvcController
    {
        private readonly ItemListApiController _listApi;

        public ItemListController(ItemListApiController listApi)
        {
            _listApi = listApi;
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View(new ItemListCreateVm());
        }

        [HttpPost]
        public IActionResult Create([FromForm] ItemListCreateVm model)
        {
            if (!ModelState.IsValid)
            {
                model.ErrorMessage = "Bitte füll das Formular korrekt aus";
                return View(model);
            }

            var result = _listApi.Create(new ItemListCreateJso {Description = model.Description, IsPublic = model.IsPublic, Name = model.Name});
            if (!result.IsSuccess)
            {
                model.ErrorResult = result;
                return View(model);
            }

            return RedirectToAction("View", new {id = result.Data.Id});
        }

        [HttpGet]
        public IActionResult Delete(int id)
        {
            var delete = _listApi.Delete(id);

            return RedirectToAction("My");
        }

        [HttpGet]
        public IActionResult My()
        {
            var listDtos = _listApi.GetAll(CurrentUserId);

            var model = new ItemListMyVm();
            model.Lists = listDtos;

            return View(model);
        }

        [HttpGet]
        public IActionResult Update(int id)
        {
            var result = _listApi.GetById(id);

            if (!result.IsSuccess)
            {
                return RedirectToAction("My");
            }

            var list = result.Data;
            var model = new ItemListUpdateVm {Description = list.Description, Id = list.Id, IsPublic = list.IsPublic, Name = list.Name};
            return View(model);
        }

        [HttpPost]
        public IActionResult Update(int id, [FromForm] ItemListUpdateVm model)
        {
            if (!ModelState.IsValid)
            {
                model.ErrorMessage = "Bitte füll das Formular korrekt aus";
                return View(model);
            }

            var result = _listApi.Update(id, new ItemListUpdateJso {Description = model.Description, IsPublic = model.IsPublic, Name = model.Name});
            if (!result.IsSuccess)
            {
                model.ErrorResult = result;
                return View(model);
            }

            ModelState.Clear();
            model.SuccessMessage = "Änderungen gespeichert";
            return View(model);
        }

        [HttpGet]
        public IActionResult View(int id)
        {
            var result = _listApi.GetById(id, true);

            if (!result.IsSuccess)
            {
                return RedirectToAction("My");
            }

            var model = new ItemListViewVm {List = result.Data};
            return View(model);
        }
    }
}