using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace ScriptableResourceLoader
{
    public sealed class ResourceLoader
    {
        private static readonly Dictionary<Type, ResourceLoader> Instances = new();
        private readonly ScriptableObject _asset;

        private ResourceLoader(ScriptableObject asset)
        {
            this._asset = asset;
        }

        public static T GetResource<T>() where T : ScriptableObject
        {
            if (Instances.TryGetValue(typeof(T), out var @object))
            {
                return @object._asset as T;
            }
        
            var newInstance = new ResourceLoader(Resources.Load(typeof(T).Name) as T);

#if UNITY_EDITOR
            if (!newInstance._asset)
            {
                var asset = ScriptableObject.CreateInstance<T>();

                AssetDatabase.CreateAsset(asset, "Assets/Resources/" + typeof(T).Name + ".asset");
                AssetDatabase.SaveAssets();
                newInstance = new ResourceLoader(asset);
            }
#endif
            if (newInstance._asset)
            {
                Instances.Add(typeof(T), newInstance);
                return newInstance._asset as T;
            }
            newInstance = null;
            throw new Exception($"No database found for {typeof(T).Name}");
        }
    }
}