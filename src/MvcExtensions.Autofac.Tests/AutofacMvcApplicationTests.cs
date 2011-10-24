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
            Assert.NotNull(new AutofacMvcApplicationTestDouble(null).PublicCreateBootstrapper());
        }

        #region Nested type: AutofacMvcApplicationTestDouble
        public class AutofacMvcApplicationTestDouble : AutofacMvcApplication
        {
            private readonly IBootstrapper bootstrapper;

            public AutofacMvcApplicationTestDouble(IBootstrapper bootstrapper)
            {
                this.bootstrapper = bootstrapper;
            }

            public IBootstrapper PublicCreateBootstrapper()
            {
                return base.CreateBootstrapper();
            }

            protected override IBootstrapper CreateBootstrapper()
            {
                return bootstrapper;
            }
        }
        #endregion
    }
}