using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Dispatcher;
using System.Web.Http.Routing;

namespace WEB.API.App_Start
{
    public class VersionHttpControllerSelector : IHttpControllerSelector
    {
        private const string VersionKey = "version";
        private const string ControllerKey = "controller";

        private readonly HttpConfiguration _configuration;
        private readonly Lazy<Dictionary<string, HttpControllerDescriptor>> _controllers;
        private readonly HashSet<string> _duplicates;

        public VersionHttpControllerSelector(HttpConfiguration config)
        {
            _configuration = config;
            _duplicates = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            _controllers = new Lazy<Dictionary<string, HttpControllerDescriptor>>(InitializeControllerDictionary);
        }

        private Dictionary<string, HttpControllerDescriptor> InitializeControllerDictionary()
        {
            var dictionary = new Dictionary<string, HttpControllerDescriptor>(StringComparer.OrdinalIgnoreCase);

            // Create a lookup table where key is "namespace.controller". The value of "namespace" is the last
            // segment of the full namespace. For example:
            // MyApplication.Controllers.V1.ProductsController => "V1.Products"
            IAssembliesResolver assembliesResolver = _configuration.Services.GetAssembliesResolver();
            IHttpControllerTypeResolver controllersResolver = _configuration.Services.GetHttpControllerTypeResolver();

            ICollection<Type> controllerTypes = controllersResolver.GetControllerTypes(assembliesResolver);

            foreach (Type t in controllerTypes)
            {
                var segments = t.Namespace.Split(Type.Delimiter);

                // For the dictionary key, strip "Controller" from the end of the type name.
                // This matches the behavior of DefaultHttpControllerSelector.
                var controllerName = t.Name.Remove(t.Name.Length - DefaultHttpControllerSelector.ControllerSuffix.Length);
                string version = segments[segments.Length - 1];
                var key = String.Format(CultureInfo.InvariantCulture, "{0}.{1}", version, controllerName);
                if (version == "Controllers")
                {
                    key = String.Format(CultureInfo.InvariantCulture, "{0}", controllerName);
                }
                // Check for duplicate keys.
                if (dictionary.Keys.Contains(key))
                {
                    _duplicates.Add(key);
                }
                else
                {
                    dictionary[key] = new HttpControllerDescriptor(_configuration, t.Name, t);
                }
            }

            // Remove any duplicates from the dictionary, because these create ambiguous matches. 
            // For example, "Foo.V1.ProductsController" and "Bar.V1.ProductsController" both map to "v1.products".
            foreach (string s in _duplicates)
            {
                dictionary.Remove(s);
            }
            return dictionary;
        }

        // Get a value from the route data, if present.
        private static T GetRouteVariable<T>(IHttpRouteData routeData, string name)
        {
            object result = null;
            if (routeData.Values.TryGetValue(name, out result))
            {
                return (T)result;
            }
            return default(T);
        }

        public HttpControllerDescriptor SelectController(HttpRequestMessage request)
        {
            IHttpRouteData routeData = request.GetRouteData();
            if (routeData == null)
            {
                throw new HttpResponseException(HttpStatusCode.NotFound);
            }

            // Get the version and controller variables from the route data.
            string version = GetRouteVariable<string>(routeData, VersionKey);
            if (string.IsNullOrEmpty(version))
            {
                version = GetVersionFromHTTPHeaderAndAcceptHeader(request);
            }
            string controllerName = GetRouteVariable<string>(routeData, ControllerKey);
            if (controllerName == null)
            {
                throw new HttpResponseException(HttpStatusCode.NotFound);
            }

            // Find a matching controller.
            string key = String.Format(CultureInfo.InvariantCulture, "{0}", controllerName);
            if (!string.IsNullOrEmpty(version))
            {
                if (controllerName.Contains(version))
                {
                    key = String.Format(CultureInfo.InvariantCulture, "{0}", controllerName);
                }
                else
                {
                    key = String.Format(CultureInfo.InvariantCulture, "{0}.{1}", version, controllerName);
                }


            }

            HttpControllerDescriptor controllerDescriptor;
            if (_controllers.Value.TryGetValue(key, out controllerDescriptor))
            {
                return controllerDescriptor;
            }
            else if (_duplicates.Contains(key))
            {
                throw new HttpResponseException(
                    request.CreateErrorResponse(HttpStatusCode.InternalServerError,
                    "Multiple controllers were found that match this request."));
            }
            else
            {
                throw new HttpResponseException(HttpStatusCode.NotFound);
            }
        }

        public IDictionary<string, HttpControllerDescriptor> GetControllerMapping()
        {
            return _controllers.Value;
        }
        private string GetVersionFromHTTPHeaderAndAcceptHeader(HttpRequestMessage request)
        {
            if (request.Headers.Contains(VersionKey))
            {
                var versionHeader = request.Headers.GetValues(VersionKey).FirstOrDefault();
                if (versionHeader != null)
                {
                    return versionHeader;
                }
            }
            var acceptHeader = request.Headers.Accept;
            foreach (var mime in acceptHeader)
            {
                if (mime.MediaType == "application/json" || mime.MediaType == "text/html")
                {
                    var version = mime.Parameters
                                     .Where(v => v.Name.Equals(VersionKey, StringComparison.OrdinalIgnoreCase))
                                      .FirstOrDefault();

                    if (version != null)
                    {
                        return version.Value;
                    }
                    return string.Empty;
                }
            }
            return string.Empty;
        }
    }
}
