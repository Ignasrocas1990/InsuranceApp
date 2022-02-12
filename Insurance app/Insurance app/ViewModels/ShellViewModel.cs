using System;
using System.Collections.Generic;
using System.Reflection;
using Xamarin.CommunityToolkit.ObjectModel;

namespace Insurance_app.ViewModels
{
    public class ShellViewModel : IDisposable
    {
        private static ShellViewModel _shellViewModel = null;
        private Dictionary<string , object>viewModels = new Dictionary<string , object>();
        private ShellViewModel() { }
        
        public static ShellViewModel GetInstance()
        {
            if (_shellViewModel is null)
            { 
                _shellViewModel = new ShellViewModel();
            }
            return _shellViewModel;
        }
//dictionary  as key Activator.CreateInstance(Type.GetType("namespace."+ nameof(Class)));
        public object GetViewModel(string viewModelName)
        {
            if (viewModels.ContainsKey(viewModelName))
            {
                viewModels.TryGetValue(viewModelName, out var obj);
                return obj;
            }

            try
            {
                Type t = Type.GetType(viewModelName);
                if (t != null)
                {
                    var obj = Activator.CreateInstance(t);
                    
                    viewModels.Add(viewModelName,obj);
                    return obj;
                }
            }catch (Exception e)
            {
                Console.WriteLine(e);
            }
            return null;
        }

        public void Dispose()
        {
            viewModels = null;
            _shellViewModel = null;
        }
    }
}