namespace MvcExtensions.Autofac
{
    using System;
    using System.Web;
    using global::Autofac;

    internal class RequestLifetimeScopeProvider : ILifetimeScopeProvider
    {
        public static readonly object HttpRequestTag = "PerHttpRequest";
        private static readonly Type lifettymeScopeType = typeof(ILifetimeScope);
        private readonly ILifetimeScope container;

        public RequestLifetimeScopeProvider(ILifetimeScope container)
        {
            Invariant.IsNotNull(container, "container");

            this.container = container;
            
            AutofacMvcApplication.SetLifetimeScopeProvider(this);
        }

        private ILifetimeScope LifetimeScope
        {
            get { return (ILifetimeScope)container.Resolve<HttpContextBase>().Items[lifettymeScopeType]; }
            set { container.Resolve<HttpContextBase>().Items[lifettymeScopeType] = value; }
        }

        #region ILifetimeScopeProvider Members

        public ILifetimeScope GetLifetimeScope()
        {
            if (LifetimeScope == null)
            {
                var lifetimeScope = container.BeginLifetimeScope(HttpRequestTag);
                if (lifetimeScope == null)
                {
                    throw new InvalidOperationException("Could not create lifetimeScope");
                }

                LifetimeScope = lifetimeScope;
            }

            return LifetimeScope;
        }

        public void EndLifetimeScope()
        {
            ILifetimeScope lifetimeScope = LifetimeScope;
            if (lifetimeScope != null)
            {
                lifetimeScope.Dispose();
            }
        }

        #endregion
    }
}