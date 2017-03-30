using System.Threading.Tasks;
using FlatMate.Api.Areas.Lists.ItemList;
using FlatMate.Web.Areas.Lists.Data;
using FlatMate.Web.Mvc;
using FlatMate.Web.Mvc.Base;
using FlatMate.Web.Mvc.Json;
using Microsoft.AspNetCore.Mvc;
using prayzzz.Common.Result;

namespace FlatMate.Web.Areas.Lists.Controllers
{
    [Area("Lists")]
    public class ItemListController : MvcController
    {
        private readonly ItemListApiController _listApi;
        private readonly IJsonService _jsonService;

        public ItemListController(ItemListApiController listApi, IJsonService jsonService)
        {
            _listApi = listApi;
            _jsonService = jsonService;
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View(new ItemListCreateVm());
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromForm] ItemListCreateVm model)
        {
            if (!ModelState.IsValid)
            {
                model.ErrorMessage = "Bitte füll das Formular korrekt aus";
                return View(model);
            }

            var result = await _listApi.Create(new ItemListJso { Description = model.Description, IsPublic = model.IsPublic, Name = model.Name });
            if (!result.IsSuccess)
            {
                model.ErrorResult = result;
                return View(model);
            }

            return RedirectToAction("View", new { id = result.Data.Id });
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _listApi.DeleteAsync(id);

            TempData[Constants.TempData.Result] = _jsonService.Serialize(result);

            return RedirectToAction("My", new { IsSuccess = true });
        }

        [HttpGet]
        public async Task<IActionResult> My()
        {
            var model = new ItemListMyVm();

            // check for passed result from redirect
            if (TempData.TryGetValue(Constants.TempData.Result, out var data))
            {
                var result = _jsonService.Deserialize<Result>(data as string);

                if (result != null && result.IsSuccess)
                {
                    // TODO refactor Result to allow success messages
                    model.SuccessMessage = "Aktion erfolgreich";
                }

                TempData.Remove(Constants.TempData.Result);
            }

            model.Lists = await _listApi.GetAll(CurrentUserId);
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Update(int id)
        {
            var result = await _listApi.GetById(id);

            if (!result.IsSuccess)
            {
                return RedirectToAction("My");
            }

            var itemList = result.Data;
            var model = new ItemListUpdateVm { Description = itemList.Description, Id = itemList.Id.Value, IsPublic = itemList.IsPublic, Name = itemList.Name };
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Update(int id, [FromForm] ItemListUpdateVm model)
        {
            if (!ModelState.IsValid)
            {
                model.ErrorMessage = "Bitte füll das Formular korrekt aus";
                return View(model);
            }

            var result = await _listApi.Update(id, new ItemListJso { Description = model.Description, Id = model.Id, IsPublic = model.IsPublic, Name = model.Name });
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
        public async Task<IActionResult> View(int id)
        {
            var result = await _listApi.GetById(id, true);

            if (!result.IsSuccess)
            {
                return RedirectToAction("My");
            }

            var model = new ItemListViewVm { List = result.Data };
            return View(model);
        }
    }
}