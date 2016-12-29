using System;
using Newtonsoft.Json.Serialization;

namespace FlatMate.Web.Mvc.Json
{
    public class FlatMateContractResolver : DefaultContractResolver
    {
        private static Type _dateTimeType;
        private static Type _nullableDateTimeType;
        private static FlatMateContractResolver _instance;

        private FlatMateContractResolver()
        {
            _dateTimeType = typeof(DateTime);
            _nullableDateTimeType = typeof(DateTime?);

            NamingStrategy = new CamelCaseNamingStrategy();
        }

        public static FlatMateContractResolver Instance => _instance ?? (_instance = new FlatMateContractResolver());

        protected override JsonContract CreateContract(Type objectType)
        {
            var contract = base.CreateContract(objectType);

            if (objectType == _dateTimeType || objectType == _nullableDateTimeType)
            {
                contract.Converter = new DateTimeConverter();
            }

            return contract;
        }
    }
}