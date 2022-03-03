using MCIB.Libs;
using System;
using System.Globalization;
using System.Linq;
using System.Windows;

namespace MCIB.Lang
{
    static class Ext
    {
        public static string FindChar(this Window _, string key)
        {
            var obj = LangManager.Instance.Resource[key];
            return obj == null ? string.Empty : obj.ToString();
        }
    }
    public sealed class LangManager
    {
        private static LangManager instance;
        public static LangManager Instance
        {
            get
            {
                if (instance == null)
                    instance = new LangManager();
                return instance;
            }
        }
        private LangManager() { }
        public ResourceDictionary Resource { get; private set; }
        public LangType Current { get; private set; } = LangType.Chinese;
        public void Init()
        {
            var cultureName = CultureInfo.CurrentCulture.Name.ToLower();
            if (!cultureName.Contains("zh"))
            {
                var langType = (cultureName.Contains("ru") || cultureName.Contains("be"))
                    ? LangType.Russian : LangType.English;
                Switch(langType);
            }
            else
                Resource = Application.Current.Resources.MergedDictionaries.FirstOrDefault(x =>
                x.Source.ToString().ToLower().Contains(Current.ToString().ToLower()));
        }
        public void Switch(LangType langType)
        {
            if (langType != Current)
            {
                Current = langType;
                try
                {
                    Uri uri = new Uri(string.Format("Lang/{0}.xaml", Current), UriKind.Relative);
                    Resource = Application.LoadComponent(uri) as ResourceDictionary;
                }
                catch (Exception ex)
                {
                    LogManager.Instance.LogError("Switch", ex);
                    throw;
                }
                if (Resource != null)
                {
                    var resourceDictionary = Application.Current.Resources.MergedDictionaries.FirstOrDefault(x =>
                      x.Source.ToString().ToLower().Contains("chinese") ||
                     x.Source.ToString().ToLower().Contains("Russian") ||
                     x.Source.ToString().ToLower().Contains("English"));
                    if (resourceDictionary != null)
                        Application.Current.Resources.MergedDictionaries.Remove(resourceDictionary);
                    Application.Current.Resources.MergedDictionaries.Add(Resource);
                }
            }
        }
    }
}