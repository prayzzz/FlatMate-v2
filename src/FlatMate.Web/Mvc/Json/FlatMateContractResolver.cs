using System;
using Newtonsoft.Json.Serialization;
using prayzzz.Common.Mvc.Json;

namespace FlatMate.Web.Mvc.Json
{
    public class FlatMateContractResolver : DefaultContractResolver
    {
        private static FlatMateContractResolver _instance;

        private readonly DateTimeConverter _dateTimeConverter;
        private readonly ResultConverter _resultConverter;

        private FlatMateContractResolver()
        {
            NamingStrategy = new CamelCaseNamingStrategy();

            _dateTimeConverter = new DateTimeConverter();
            _resultConverter = new ResultConverter(NamingStrategy);
        }

        public static FlatMateContractResolver Instance => _instance ?? (_instance = new FlatMateContractResolver());

        protected override JsonContract CreateContract(Type objectType)
        {
            var contract = base.CreateContract(objectType);

            if (_dateTimeConverter.CanConvert(objectType))
            {
                contract.Converter = _dateTimeConverter;
            }

            if (_resultConverter.CanConvert(objectType))
            {
                contract.Converter = _resultConverter;
            }

            return contract;
        }
    }
}