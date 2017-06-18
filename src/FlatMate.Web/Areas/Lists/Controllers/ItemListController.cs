using System.Threading.Tasks;
using FlatMate.Api.Areas.Lists;
using FlatMate.Api.Areas.Lists.Jso;
using FlatMate.Web.Areas.Lists.Data;
using FlatMate.Web.Mvc;
using FlatMate.Web.Mvc.Base;
using FlatMate.Web.Mvc.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using prayzzz.Common.Results;

namespace FlatMate.Web.Areas.Lists.Controllers
{
    [Area("Lists")]
    public class ItemListController : MvcController
    {
        private readonly ItemListApiController _listApi;

        public ItemListController(ItemListApiController listApi, 
                                  ILogger<ItemListController> logger,
                                  IMvcControllerService controllerService) : base(logger, controllerService)
        {
            _listApi = listApi;
        }

        [HttpGet]
        public async Task<IActionResult> Browse()
        {
            var model = ApplyTempResult(new ItemListBrowseVm());

            var allLists = _listApi.GetAllLists(new GetAllListsQuery());
            var favorites = _listApi.GetAllLists(new GetAllListsQuery { FavoritesOnly = true });

            model.Lists = await allLists;
            model.Favorites = await favorites;
            return View(model);
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

            var result = await _listApi.CreateList(new ItemListJso { Description = model.Description, IsPublic = model.IsPublic, Name = model.Name });
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
            var getList = await _listApi.GetList(id);
            if (getList.IsError)
            {
                TempData[Constants.TempData.Result] = JsonService.Serialize(getList);
                return RedirectToAction("My");
            }

            var deleteList = await _listApi.DeleteListAsync(id);
            if (deleteList.IsError)
            {
                TempData[Constants.TempData.Result] = JsonService.Serialize(new ErrorResult(deleteList.ErrorType, $"Die Liste '{getList.Data.Name}' konnte nicht gelöscht werden."));
                return RedirectToAction("My");
            }

            TempData[Constants.TempData.Result] = JsonService.Serialize(new SuccessResult($"Die Liste '{getList.Data.Name}' wurde gelöscht."));
            return RedirectToAction("My");
        }

        [HttpGet]
        public async Task<IActionResult> Favorite(int id)
        {
            var createFavorite = await _listApi.CreateFavorite(new ItemListFavoriteJso { ItemListId = id });
            if (createFavorite.IsError)
            {
                TempData[Constants.TempData.Result] = JsonService.Serialize(createFavorite);
            }
            else
            {
                TempData[Constants.TempData.Result] = JsonService.Serialize(new SuccessResult("Als Favorit hinzugefügt"));
            }

            var referer = HttpContext.Request.Headers["Referer"].ToString();
            if (!string.IsNullOrEmpty(referer))
            {
                return Redirect(referer);
            }

            return RedirectToAction("Browse");
        }

        [HttpGet]
        public async Task<IActionResult> My()
        {
            var model = ApplyTempResult(new ItemListMyVm());

            model.MyLists = await _listApi.GetAllLists(new GetAllListsQuery { OwnerId = CurrentUserId });
            model.Favorites = await _listApi.GetAllLists(new GetAllListsQuery { FavoritesOnly = true });
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Unfavorite(int id)
        {
            var deleteFavorite = await _listApi.DeleteFavorite(new ItemListFavoriteJso { ItemListId = id });
            if (deleteFavorite.IsError)
            {
                TempData[Constants.TempData.Result] = JsonService.Serialize(deleteFavorite);
            }
            else
            {
                TempData[Constants.TempData.Result] = JsonService.Serialize(new SuccessResult("Als Favorit entfernt"));
            }
            
            var referer = HttpContext.Request.Headers["Referer"].ToString();
            if (!string.IsNullOrEmpty(referer))
            {
                return Redirect(referer);
            }
            
            return RedirectToAction("Browse");
        }

        [HttpGet]
        public async Task<IActionResult> Update(int id)
        {
            var getList = await _listApi.GetList(id);
            if (getList.IsError)
            {
                return RedirectToAction("My");
            }

            var itemList = getList.Data;
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

            var updateList = await _listApi.Update(id, new ItemListJso { Description = model.Description, Id = model.Id, IsPublic = model.IsPublic, Name = model.Name });
            if (updateList.IsError)
            {
                model.Result = updateList;
                return View(model);
            }

            ModelState.Clear();
            model.Result = new SuccessResult("Änderungen gespeichert");
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> View(int id)
        {
            var getList = await _listApi.GetList(id, true);
            if (getList.IsError)
            {
                return RedirectToAction("My");
            }

            var model = new ItemListViewVm { List = getList.Data };
            return View(model);
        }
    }
}