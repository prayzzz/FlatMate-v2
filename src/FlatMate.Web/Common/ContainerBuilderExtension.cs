using System;
using System.Linq;
using System.Reflection;
using Autofac;
using FlatMate.Module.Common;
using prayzzz.Common.Attributes;
using prayzzz.Common.Enums;

namespace FlatMate.Web.Common
{
    public static class ContainerBuilderExtension
    {
        public static void InjectDependencies(this ContainerBuilder builder, FlatMateModule module)
        {
            builder.InjectDependencies(module.GetType());
        }

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

                if (serviceTypes.Length == 0)
                {
                    serviceTypes = type.GetInterfaces();
                }

                var registrationBuilder = builder.RegisterType(type)
                                                 .AsSelf()
                                                 .As(serviceTypes);

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