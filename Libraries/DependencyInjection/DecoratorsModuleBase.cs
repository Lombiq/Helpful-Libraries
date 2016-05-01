using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autofac;
using Autofac.Core;

namespace Piedone.HelpfulLibraries.Libraries.DependencyInjection
{
    /// <summary>
    /// A base class for an Autofac module that registers decorators for other services.
    /// </summary>
    public abstract class DecoratorsModuleBase : Module
    {
        private IEnumerable<DecorationConfiguration> _decorationConfigurations;


        protected abstract IEnumerable<DecorationConfiguration> DescribeDecorators();

        protected override void Load(ContainerBuilder builder)
        {
            foreach (var configuration in GetDecorationConfigurations())
            {
                builder
                    .RegisterType(configuration.DecoratorType)
                    .AsSelf()
                    .InstancePerDependency()
                    .WithMetadata("IsDecorator", true);
            }
        }

        protected override void AttachToComponentRegistration(
            IComponentRegistry componentRegistry,
            IComponentRegistration registration)
        {
            foreach (var configuration in GetDecorationConfigurations())
            {
                if (configuration.DecoratedType.IsAssignableFrom(registration.Activator.LimitType) &&
                    registration.Activator.LimitType != configuration.DecoratorType)
                {
                    registration.Activating += (sender, e) =>
                    {
                        if (e.Component.Metadata.ContainsKey("IsDecorator")) return;

                        // This is needed so e.g. the Localizer and Logger can get registered.
                        registration.RaiseActivated(e.Context, e.Parameters, e.Instance);

                        e.Instance = e.Context
                            .Resolve(configuration.DecoratorType, new TypedParameter(configuration.DecoratedType, e.Instance));
                    };
                }
            }
        }


        private IEnumerable<DecorationConfiguration> GetDecorationConfigurations()
        {
            if (_decorationConfigurations == null) _decorationConfigurations = DescribeDecorators();
            return _decorationConfigurations;
        }


        protected class DecorationConfiguration
        {
            public Type DecoratedType { get; private set; }
            public Type DecoratorType { get; private set; }


            public DecorationConfiguration(Type decoratedType, Type decoratorType)
            {
                DecoratedType = decoratedType;
                DecoratorType = decoratorType;
            }


            public static DecorationConfiguration Create<TDecorated, TDecorator>()
            {
                return new DecorationConfiguration(typeof(TDecorated), typeof(TDecorator));
            }
        }
    }
}
