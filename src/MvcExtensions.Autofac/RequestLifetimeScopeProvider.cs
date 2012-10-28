﻿#region Copyright
// Copyright (c) 2009 - 2011, Kazi Manzur Rashid <kazimanzurrashid@gmail.com>, hazzik <hazzik@gmail.com>.
// This source is subject to the Microsoft Public License. 
// See http://www.microsoft.com/opensource/licenses.mspx#Ms-PL. 
// All other rights reserved.
#endregion

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

            PerWebRequestLifestyleModule.SetLifetimeScopeProvider(this);
        }

        private ILifetimeScope LifetimeScope
        {
            get
            {
                var httpContext = HttpContext.Current;
                if (httpContext == null)
                {
                    return container;
                }

                return (ILifetimeScope)httpContext.Items[lifettymeScopeType];
            }

            set
            {
                HttpContext.Current.Items[lifettymeScopeType] = value;
            }
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
            var lifetimeScope = LifetimeScope;
            if (lifetimeScope != null)
            {
                lifetimeScope.Dispose();
            }
        }
        #endregion
    }
}