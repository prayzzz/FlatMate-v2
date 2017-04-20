using System.Threading.Tasks;
using FlatMate.Api.Areas.Lists.ItemList;
using FlatMate.Web.Areas.Lists.Data;
using FlatMate.Web.Mvc;
using FlatMate.Web.Mvc.Base;
using FlatMate.Web.Mvc.Json;
using Microsoft.AspNetCore.Mvc;
using prayzzz.Common.Results;

namespace FlatMate.Web.Areas.Lists.Controllers
{
    [Area("Lists")]
    public class ItemListController : MvcController
    {
        private readonly IJsonService _jsonService;
        private readonly ItemListApiController _listApi;

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
                model.Result = new ErrorResult(ErrorType.ValidationError, "Bitte füll das Formular korrekt aus");
                return View(model);
            }

            var result = await _listApi.Create(new ItemListJso { Description = model.Description, IsPublic = model.IsPublic, Name = model.Name });
            if (!result.IsSuccess)
            {
                model.Result = result;
                return View(model);
            }

            return RedirectToAction("View", new { id = result.Data.Id });
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            var getResult = await _listApi.GetList(id);
            if (getResult.IsError)
            {
                TempData[Constants.TempData.Result] = _jsonService.Serialize(getResult);
                return RedirectToAction("My");
            }

            var result = await _listApi.DeleteListAsync(id);
            if (result.IsError)
            {
                TempData[Constants.TempData.Result] = _jsonService.Serialize(new ErrorResult(result.ErrorType, $"Die Liste '{getResult.Data.Name}' konnte nicht gelöscht werden."));
                return RedirectToAction("My");
            }

            TempData[Constants.TempData.Result] = _jsonService.Serialize(new SuccessResult($"Die Liste '{getResult.Data.Name}' wurde gelöscht."));
            return RedirectToAction("My");
        }

        [HttpGet]
        public async Task<IActionResult> My()
        {
            var model = new ItemListMyVm();

            // check for passed result from redirect
            if (TempData.TryGetValue(Constants.TempData.Result, out var data))
            {
                model.Result = _jsonService.Deserialize<Result>(data as string);
                TempData.Remove(Constants.TempData.Result);
            }

            model.Lists = await _listApi.GetAllLists(CurrentUserId);
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Update(int id)
        {
            var result = await _listApi.GetList(id);

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
                model.Result = new ErrorResult(ErrorType.ValidationError, "Bitte füll das Formular korrekt aus");
                return View(model);
            }

            var result = await _listApi.Update(id, new ItemListJso { Description = model.Description, Id = model.Id, IsPublic = model.IsPublic, Name = model.Name });
            if (!result.IsSuccess)
            {
                model.Result = result;
                return View(model);
            }

            ModelState.Clear();
            model.Result = new SuccessResult("Änderungen gespeichert");
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> View(int id)
        {
            var result = await _listApi.GetList(id, true);

            if (!result.IsSuccess)
            {
                return RedirectToAction("My");
            }

            var model = new ItemListViewVm { List = result.Data };
            return View(model);
        }
    }
}