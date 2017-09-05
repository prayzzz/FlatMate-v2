﻿using FlatMate.Web.Mvc.Json;
using prayzzz.Common.Attributes;

namespace FlatMate.Web.Mvc.Base
{
    public interface IMvcControllerService
    {
        IJsonService JsonService { get; }
    }

    [Inject]
    public class MvcControllerService : IMvcControllerService
    {
        public MvcControllerService(IJsonService jsonService)
        {
            JsonService = jsonService;
        }

        public IJsonService JsonService { get; }
    }
}