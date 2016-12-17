using System;
using System.Linq;
using System.Reflection;
using Autofac;
using prayzzz.Common.Attributes;
using prayzzz.Common.Enums;

namespace FlatMate.Api.Extensions
{
    public static class ContainerBuilderExtension
    {
        public static void InjectDependencies(this ContainerBuilder builder, Type assemblyType)
        {
            var injectableTypes = assemblyType.GetTypeInfo()
                                              .Assembly
                                              .ExportedTypes
                                              .Where(t => t.GetTypeInfo().GetCustomAttribute<InjectAttribute>() != null);

            foreach (var type in injectableTypes)
            {
                var attribute = type.GetTypeInfo().GetCustomAttribute<InjectAttribute>();
                var serviceTypes = attribute.ServiceTypes;

                if (!serviceTypes.Any())
                {
                    serviceTypes = type.GetInterfaces();
                }

                var registrationBuilder = builder.RegisterType(type).As(serviceTypes);

                switch (attribute.Lifetime)
                {
                    case DependencyLifetime.Transient:
                        registrationBuilder.InstancePerDependency();
                        break;
                    case DependencyLifetime.Scoped:
                        registrationBuilder.InstancePerLifetimeScope();
                        break;
                    case DependencyLifetime.Singleton:
                        registrationBuilder.SingleInstance();
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }
    }
}