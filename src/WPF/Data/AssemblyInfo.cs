﻿using System;
using System.IO;
using System.Reflection;

namespace Phony.WPF.Data
{
    public class AssemblyInfo
    {
        /// <summary>
        /// Get the app info from the Assembly file
        /// </summary>
        /// <param name="assembly">Assembly.GetEntryAssembly()</param>
        public AssemblyInfo(Assembly assembly)
        {
            this.assembly = assembly ?? throw new ArgumentNullException("assembly");
        }

        private readonly Assembly assembly;

        /// <summary>
        /// Gets the title property
        /// </summary>
        public string AppTitle => GetAttributeValue<AssemblyTitleAttribute>(a => a.Title, Path.GetFileNameWithoutExtension(assembly.CodeBase));

        /// <summary>
        /// Gets the application's version
        /// </summary>
        public string Version
        {
            get
            {
                string result = string.Empty;
                Version version = assembly.GetName().Version;
                if (version != null)
                {
                    return version.ToString();
                }
                else
                {
                    return "1.0.0.0";
                }
            }
        }

        /// <summary>
        /// Gets the description about the application.
        /// </summary>
        public string Description
        {
            get { return GetAttributeValue<AssemblyDescriptionAttribute>(a => a.Description); }
        }

        /// <summary>
        ///  Gets the product's full name.
        /// </summary>
        public string Product
        {
            get { return GetAttributeValue<AssemblyProductAttribute>(a => a.Product); }
        }

        /// <summary>
        /// Gets the copyright information for the product.
        /// </summary>
        public string Copyright
        {
            get { return GetAttributeValue<AssemblyCopyrightAttribute>(a => a.Copyright); }
        }

        /// <summary>
        /// Gets the company information for the product.
        /// </summary>
        public string Company
        {
            get { return GetAttributeValue<AssemblyCompanyAttribute>(a => a.Company); }
        }

        protected string GetAttributeValue<TAttr>(Func<TAttr, string> resolveFunc, string defaultResult = null) where TAttr : Attribute
        {
            object[] attributes = assembly.GetCustomAttributes(typeof(TAttr), false);
            if (attributes.Length > 0)
            {
                return resolveFunc((TAttr)attributes[0]);
            }
            else
            {
                return defaultResult;
            }
        }
    }
}