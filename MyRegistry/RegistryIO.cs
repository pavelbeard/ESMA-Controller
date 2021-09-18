using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MyLibrary
{
    public static class RegistryIO
    {
        private static RegistryKey currentUser;
        private static RegistryKey registryKey;
        
        [Required]
        public static string NameRegistry { get; set; }

        //[RequiredKeyName(KeyName = NameRegistry)]
        public static void SetData<T>(List<string> names, List<T> data)
        {
            currentUser = Registry.CurrentUser;
            registryKey = currentUser?.OpenSubKey(NameRegistry, true) ?? null;

            if (registryKey == null)
                registryKey = currentUser.CreateSubKey(NameRegistry, true);

            for (int i = 0; i < names.Count; i++)
            {
                registryKey.SetValue(names[i], data[i]);
            }

            registryKey.Close();
        }
        public static void SetData<T>(string name, T data)
        {
            currentUser = Registry.CurrentUser;
            registryKey = currentUser?.OpenSubKey(NameRegistry, true) ?? null;

            if (registryKey == null)
                registryKey = currentUser.CreateSubKey(NameRegistry, true);

            registryKey.SetValue(name, data);

            registryKey.Close();
        }
        public static IEnumerable<object> GetData(params string[] values)
        {
            currentUser = Registry.CurrentUser;
            try
            {
                registryKey = currentUser?.OpenSubKey(NameRegistry) ?? null;
            }
            catch (Exception)
            {
                registryKey = currentUser.CreateSubKey(NameRegistry);
            }
            foreach (var v in values)
            {
                yield return registryKey?.GetValue(v);
            }
        }
        public static object GetData(string value)
        {
            currentUser = Registry.CurrentUser;
            try
            {
                registryKey = currentUser?.OpenSubKey(NameRegistry) ?? null;
            }
            catch (Exception)
            {
                registryKey = currentUser.CreateSubKey(NameRegistry);
            }
            return registryKey?.GetValue(value) ?? default;
        }
        public static void ResetData(params string[] values)
        {
            currentUser = Registry.CurrentUser;
            registryKey = currentUser?.OpenSubKey(NameRegistry, true) ?? null;

            if (values != null && registryKey != null)
            {
                foreach (var v in values)
                {
                    registryKey.DeleteValue(v, false);
                }
            }

            registryKey.Close();
        }
        public static void RemoveSubKey()
        {
            Registry.CurrentUser.DeleteSubKey(NameRegistry);
        }
    }
}
