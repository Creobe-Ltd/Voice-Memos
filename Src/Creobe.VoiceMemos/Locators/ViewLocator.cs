using System;
using System.Collections.Generic;
using System.Linq;

namespace Creobe.VoiceMemos.Locators
{
    public class ViewLocator
    {
        private const string _viewUriTemplate = "/Views/{0}View.xaml";
        private const string _viewParamUriTemplate = "/Views/{0}View.xaml?{1}";

        public Uri View(string name)
        {
            return View(name, null);
        }

        public Uri View(string name, object values)
        {
            if (values != null)
            {
                var valueDictionary = values.GetType().GetProperties()
                    .ToDictionary(p => p.Name, p => p.GetValue(values, null));

                return View(name, valueDictionary);
            }

            return View(name, null);
        }

        public Uri View(string name, Dictionary<string, object> valueDictionary)
        {
            if (valueDictionary != null && valueDictionary.Count > 0)
            {
                var queryStrings = string.Join("&", valueDictionary.Select(v => v.Key + "=" + v.Value));
                return new Uri(string.Format(_viewParamUriTemplate, name, queryStrings), UriKind.RelativeOrAbsolute);
            }

            return new Uri(string.Format(_viewUriTemplate, name), UriKind.RelativeOrAbsolute);
        }
    }
}
