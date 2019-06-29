using System.Threading.Tasks;
using FlatMate.Module.Lists.Api;
using FlatMate.Module.Lists.Api.Jso;
using FlatMate.Web.Areas.Lists.Data;
using FlatMate.Web.Mvc;
using FlatMate.Web.Mvc.Base;
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
                                  IMvcControllerServices controllerService) : base(logger, controllerService)
        {
            _listApi = listApi;
        }

        [HttpGet]
        public async Task<IActionResult> Browse()
        {
            var model = ApplyTempResult(new ItemListBrowseVm());

            var allLists = _listApi.GetAllLists(new GetAllListsQuery());
            var favorites = _listApi.GetFavorites();

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
                model.Result = new Result(ErrorType.ValidationError, "Bitte füll das Formular korrekt aus");
                return View(model);
            }

            var (result, list) = await _listApi.CreateList(new ItemListJso { Description = model.Description, IsPublic = model.IsPublic, Name = model.Name });
            if (!result.IsSuccess)
            {
                model.Result = result;
                return View(model);
            }

            return RedirectToAction("View", new { id = list.Id });
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            var (result, list) = await _listApi.GetList(id);
            if (result.IsError)
            {
                TempData[Constants.TempData.Result] = JsonService.Serialize(result);
                return RedirectToAction("My");
            }

            var deleteList = await _listApi.DeleteListAsync(id);
            if (deleteList.IsError)
            {
                TempData[Constants.TempData.Result] = JsonService.Serialize(new Result(deleteList.ErrorType, $"Die Liste '{list.Name}' konnte nicht gelöscht werden."));
                return RedirectToAction("My");
            }

            TempData[Constants.TempData.Result] = JsonService.Serialize(new Result(ErrorType.None, $"Die Liste '{list.Name}' wurde gelöscht."));
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
                TempData[Constants.TempData.Result] = JsonService.Serialize(new Result(ErrorType.None, "Als Favorit hinzugefügt"));
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
            model.Favorites = await _listApi.GetFavorites();
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
                TempData[Constants.TempData.Result] = JsonService.Serialize(new Result(ErrorType.None, "Als Favorit entfernt"));
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
            var (result, itemList) = await _listApi.GetList(id);
            if (result.IsError)
            {
                return RedirectToAction("My");
            }

            if (itemList.Owner.Id != CurrentUserId)
            {
                return TryRedirectToReferer(RedirectToAction("My"));
            }

            var model = new ItemListUpdateVm { Description = itemList.Description, Id = itemList.Id.Value, IsPublic = itemList.IsPublic, Name = itemList.Name };
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Update(int id, [FromForm] ItemListUpdateVm updateModel)
        {
            if (!ModelState.IsValid)
            {
                updateModel.Result = new Result(ErrorType.ValidationError, "Bitte füll das Formular korrekt aus");
                return View(updateModel);
            }

            var (result, itemList) = await _listApi.Update(id, new ItemListJso { Description = updateModel.Description, Id = updateModel.Id, IsPublic = updateModel.IsPublic, Name = updateModel.Name });
            if (result.IsError)
            {
                updateModel.Result = result;
                return View(updateModel);
            }

            ModelState.Clear();
            var model = new ItemListUpdateVm
            {
                Description = itemList.Description,
                Id = itemList.Id.Value,
                IsPublic = itemList.IsPublic,
                Name = itemList.Name,
                Result = new Result(ErrorType.None, "Änderungen gespeichert")
            };
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> View(int id)
        {
            var (result, itemList) = await _listApi.GetList(id, true);
            if (result.IsError)
            {
                return RedirectToAction("My");
            }

            var model = new ItemListViewVm { List = itemList };
            return View(model);
        }
    }
}