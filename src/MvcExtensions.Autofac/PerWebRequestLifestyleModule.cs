#region Copyright
// Copyright (c) 2009 - 2011, Kazi Manzur Rashid <kazimanzurrashid@gmail.com>, hazzik <hazzik@gmail.com>.
// This source is subject to the Microsoft Public License. 
// See http://www.microsoft.com/opensource/licenses.mspx#Ms-PL. 
// All other rights reserved.
#endregion
namespace MvcExtensions.Autofac
{
    using System;
    using System.Web;

    /// <summary>
    /// PerWebRequestLifestyle module
    /// </summary>
    public class PerWebRequestLifestyleModule : IHttpModule
    {
        private static ILifetimeScopeProvider lifetimeScopeProvider;

        /// <summary>
        /// Init http modole
        /// </summary>
        /// <param name="context"></param>
        public void Init(HttpApplication context)
        {
            context.EndRequest += OnEndRequest;
        }

        /// <summary>
        /// Dispose object
        /// </summary>
        public void Dispose()
        {
        }

        internal static void SetLifetimeScopeProvider(ILifetimeScopeProvider scopeProvider)
        {
            Invariant.IsNotNull(scopeProvider, "scopeProvider");

            lifetimeScopeProvider = scopeProvider;
        }

        private static void OnEndRequest(object sender, EventArgs e)
        {
            if (lifetimeScopeProvider != null)
            {
                lifetimeScopeProvider.EndLifetimeScope();
            }
        }
    }
}