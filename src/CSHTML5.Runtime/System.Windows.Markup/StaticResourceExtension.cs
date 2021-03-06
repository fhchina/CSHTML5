﻿
//===============================================================================
//
//  IMPORTANT NOTICE, PLEASE READ CAREFULLY:
//
//  => This code is licensed under the GNU General Public License (GPL v3). A copy of the license is available at:
//        https://www.gnu.org/licenses/gpl.txt
//
//  => As stated in the license text linked above, "The GNU General Public License does not permit incorporating your program into proprietary programs". It also does not permit incorporating this code into non-GPL-licensed code (such as MIT-licensed code) in such a way that results in a non-GPL-licensed work (please refer to the license text for the precise terms).
//
//  => Licenses that permit proprietary use are available at:
//        http://www.cshtml5.com
//
//  => Copyright 2019 Userware/CSHTML5. This code is part of the CSHTML5 product (cshtml5.com).
//
//===============================================================================



using CSHTML5.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
#if !MIGRATION
using Windows.UI.Xaml;
#endif

namespace System.Windows.Markup
{
    /// <summary>
    /// Class used to access elements inside the XAML code
    /// </summary>
    [System.Windows.Markup.ContentProperty("ResourceKey")]
    public class StaticResourceExtension : MarkupExtension
    {
        /// <summary>
        /// The key of the StaticResource.
        /// </summary>
        public string ResourceKey { get; set; }

        /// <summary>
        /// Initializes a new instance of StaticResource.
        /// </summary>
        public StaticResourceExtension() { }

        /// <summary>
        /// Initializes a new instance of StaticResource with the given ResourceKey.
        /// </summary>
        /// <param name="resourceKey">The ResourceKey for the StaticResource</param>
        public StaticResourceExtension(string resourceKey)
        {
            ResourceKey = resourceKey;
        }

        //public override object ProvideValue(IServiceProvider serviceProvider)
        //{
        //    serviceProvider.GetService();
        //    var staticResourceExtension = new StaticResourceExtension(ResourceKey);

        //    var resource = staticResourceExtension.ProvideValue(serviceProvider) as INTERNAL_CorrespondingItem;

        //    return resource == null ? "Invalid INTERNAL_CorrespondingItem" : String.Format("My {0} {1}", resource.Value, resource.Title);
        //}


        
        /// <summary>
        /// returns an object that is provided as the value of the target property for this StaticResource.
        /// </summary>
        /// <param name="serviceProvider">A service provider helper that can provide services for the StaticResource.</param>
        /// <returns>An object that is provided as the value of the target property for this StaticResource.</returns>
        
#if BRIDGE
        public override object ProvideValue(ServiceProvider serviceProvider)
#else
        public override object ProvideValue(IServiceProvider serviceProvider) 
#endif
        {
            
            if (serviceProvider is ServiceProvider)
            {
                ServiceProvider serviceProviderAsServiceProvider = (ServiceProvider)serviceProvider;
                bool isFirst = true;
                object elementItself = null;
                foreach (Object parentElement in serviceProviderAsServiceProvider.Parents)
                {
                    if (isFirst)
                    {
                        elementItself = parentElement;
                    }
                    else
                    {
                        ResourceDictionary resourceDictionary = null;
                        if (parentElement is ResourceDictionary)
                        {
                            resourceDictionary = (ResourceDictionary)parentElement;
                        }
                        else if (parentElement is FrameworkElement)
                        {
                            resourceDictionary = ((FrameworkElement)parentElement).Resources;
                        }
                        if (resourceDictionary != null && resourceDictionary.ContainsKey(ResourceKey))
                        {
                            object returnElement = resourceDictionary[ResourceKey];
                            if (returnElement != elementItself)
                            {
                                return returnElement;
                            }
                        }
                    }
                    isFirst = false;
                }
                if (Application.Current.Resources != null && Application.Current.Resources.ContainsKey(ResourceKey))
                {
                    return Application.Current.Resources[ResourceKey];
                }
            else
            {
                    // Look in the built-in resources (eg. "SystemAccentColor"):
                    object result = Application.Current.TryFindResource(ResourceKey);
                    if (result == null)
                    {
                        throw new XamlParseException(string.Format("StaticResource resolve failed: cannot find resource named '{0}' (Note: resource names are case sensitive)", ResourceKey));
                    }
                    return result;
                }
                throw new XamlParseException(string.Format("StaticResource resolve failed: cannot find resource named '{0}' (Note: resource names are case sensitive)", ResourceKey));
            }
            else
            {
                throw new SystemException("StaticResourceExtension.ProvideValue failed: the service provider is not of the expected type. Please contact support.");
            }
        }
    }
}
