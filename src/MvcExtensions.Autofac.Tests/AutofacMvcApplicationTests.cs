#region Copyright
// Copyright (c) 2009 - 2010, Kazi Manzur Rashid <kazimanzurrashid@gmail.com>.
// This source is subject to the Microsoft Public License. 
// See http://www.microsoft.com/opensource/licenses.mspx#Ms-PL. 
// All other rights reserved.
#endregion

namespace MvcExtensions.Autofac.Tests
{
    using Xunit;

    public class AutofacMvcApplicationTests
    {
        [Fact]
        public void Should_be_able_to_create_bootstrapper()
        {
            var application = new AutofacMvcApplication();

            Assert.NotNull(application.Bootstrapper);
            Assert.IsType<AutofacBootstrapper>(application.Bootstrapper);
        }
    }
}