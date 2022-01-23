using System;
using System.Collections.Generic;
using System.Reflection;
using Xamarin.CommunityToolkit.ObjectModel;

namespace Insurance_app.ViewModels
{
    public class ShellViewModel
    {
        private static ShellViewModel _shellViewModel = null;
        private Dictionary<string , object>viewModels = new Dictionary<string , object>();
        private ShellViewModel() { }
        
        public static ShellViewModel GetInstance()
        {
            if (_shellViewModel is null)
            {
                return _shellViewModel = new ShellViewModel();
            }
            return _shellViewModel;
        }

        public void AddViewModel(string name,object viewModel)
        {
            if (viewModels.ContainsKey(name)) return;
            viewModels.Add(name,viewModel);
        }
//dictionary  as key Activator.CreateInstance(Type.GetType("namespace."+ nameof(Class)));
        public object GetViewModel(string viewModelName)
        {
            if (viewModels.ContainsKey(viewModelName))
            {
                object obj;
                viewModels.TryGetValue(viewModelName, out obj);
                return obj;
            }
            else
            {
                try
                {
                    Type t = Type.GetType(viewModelName);
                    var obj = Activator.CreateInstance(t);
                    
                    viewModels.Add(viewModelName,obj);
                    return obj;
                    
                }catch (Exception e)
                {
                    Console.WriteLine(e);
                }
            }

            return null;
/*
            try
            {
                if (viewModels.ContainsKey(name))
                {
                    object viewModel = null;
                    viewModels.TryGetValue(name, out viewModel);
                    return viewModel;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);

            }
            return null;
            */
        }
    }
}