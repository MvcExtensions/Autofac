﻿#region Copyright
// Copyright (c) 2009 - 2011, Kazi Manzur Rashid <kazimanzurrashid@gmail.com>, hazzik <hazzik@gmail.com>.
// This source is subject to the Microsoft Public License. 
// See http://www.microsoft.com/opensource/licenses.mspx#Ms-PL. 
// All other rights reserved.
#endregion

using System;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Web;
using MvcExtensions.Autofac;

[assembly: AssemblyTitle("MvcExtensions.Autofac")]
[assembly: AssemblyProduct("MvcExtensions.Autofac")]
[assembly: CLSCompliant(true)]
[assembly: Guid("d2508795-a854-4723-81ca-dee3da291db6")]
[assembly: PreApplicationStartMethod(typeof(AutofacBootstrapper), "Run")]