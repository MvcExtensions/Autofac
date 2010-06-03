#region Copyright
// Copyright (c) 2009 - 2010, Kazi Manzur Rashid <kazimanzurrashid@gmail.com>.
// This source is subject to the Microsoft Public License. 
// See http://www.microsoft.com/opensource/licenses.mspx#Ms-PL. 
// All other rights reserved.
#endregion

namespace MvcExtensions.Autofac
{
    using System;
    using System.Linq;
    using System.Web;

    using ContainerBuilder = global::Autofac.ContainerBuilder;
    using ILifetimeScope = global::Autofac.ILifetimeScope;
    using IModule = global::Autofac.Core.IModule;
    using RegisterExtension = global::Autofac.RegistrationExtensions;

    /// <summary>
    /// Defines a <seealso cref="Bootstrapper">Bootstrapper</seealso> which is backed by <seealso cref="AutofacAdapter"/>.
    /// </summary>
    public class AutofacBootstrapper : Bootstrapper
    {
        private static readonly Type moduleType = typeof(IModule);

        /// <summary>
        /// Initializes a new instance of the <see cref="AutofacBootstrapper"/> class.
        /// </summary>
        /// <param name="buildManager">The build manager.</param>
        public AutofacBootstrapper(IBuildManager buildManager) : base(buildManager)
        {
        }

        /// <summary>
        /// Creates the container adapter.
        /// </summary>
        /// <returns></returns>
        protected override ContainerAdapter CreateAdapter()
        {
            ContainerBuilder builder = new ContainerBuilder();

            RegisterExtension.Register(builder, c => new HttpContextWrapper(HttpContext.Current)).As<HttpContextBase>().InstancePerDependency();

            AutofacAdapter adapter = new AutofacAdapter(builder.Build());

            return adapter;
        }

        /// <summary>
        /// Loads the container specific modules.
        /// </summary>
        protected override void LoadModules()
        {
            ContainerBuilder builder = new ContainerBuilder();

            BuildManager.ConcreteTypes
                        .Where(type => moduleType.IsAssignableFrom(type) && type.HasDefaultConstructor())
                        .Each(type => RegisterExtension.RegisterModule(builder, Activator.CreateInstance(type) as IModule));

            ILifetimeScope container = ((AutofacAdapter)Adapter).Container;

            builder.Update(container.ComponentRegistry);
        }
    }
}