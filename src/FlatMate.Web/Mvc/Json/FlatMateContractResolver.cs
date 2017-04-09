using System;
using Newtonsoft.Json.Serialization;
using prayzzz.Common.Mvc.Json;

namespace FlatMate.Web.Mvc.Json
{
    public class FlatMateContractResolver : DefaultContractResolver
    {
        private static readonly DateTimeConverter DateTimeConverter;
        private static readonly ResultConverter ResultConverter;

        static FlatMateContractResolver()
        {
            Instance = new FlatMateContractResolver();
            DateTimeConverter = new DateTimeConverter();
            ResultConverter = new ResultConverter();
        }

        private FlatMateContractResolver()
        {
            NamingStrategy = new CamelCaseNamingStrategy();
        }

        public static FlatMateContractResolver Instance { get; }

        protected override JsonContract CreateContract(Type objectType)
        {
            var contract = base.CreateContract(objectType);

            if (DateTimeConverter.CanConvert(objectType))
            {
                contract.Converter = DateTimeConverter;
            }

            if (ResultConverter.CanConvert(objectType))
            {
                contract.Converter = ResultConverter;
            }

            return contract;
        }
    }
}