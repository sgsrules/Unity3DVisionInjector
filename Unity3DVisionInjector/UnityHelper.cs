using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEngine;

    public static class UnityHelper
    {
#if !UNITY_4_5
        private static AssetBundle _SteamVR;
        private static IDictionary<string, AssetBundle> _AssetBundles = new Dictionary<string, AssetBundle>();
#endif
        private static readonly MethodInfo _LoadFromMemory = typeof(AssetBundle).GetMethod("LoadFromMemory", new Type[] { typeof(byte[]) });
        private static readonly MethodInfo _CreateFromMemory = typeof(AssetBundle).GetMethod("CreateFromMemoryImmediate", new Type[] { typeof(byte[]) });


  

        internal static Shader GetShader(string name)
        {
            string filePath = System.IO.Path.Combine(Application.streamingAssetsPath, "stereoassetbundle");
        byte[] d =    File.ReadAllBytes(filePath);
            return LoadFromAssetBundle<Shader>(

                d,

                name);
        }


        public static T LoadFromAssetBundle<T>(byte[] assetBundleBytes, string name) where T : UnityEngine.Object
        {
#if UNITY_4_5
            var assetBundle = AssetBundle.CreateFromMemoryImmediate(assetBundleBytes);
            //foreach(var asset in assetBundle.LoadAll())
            //{
            //     Console.WriteLine(asset.name);
            //}
             Console.WriteLine("Getting {0} from {1}", name, assetBundle.name);
            var obj = GameObject.Instantiate(assetBundle.Load(name)) as T;
            assetBundle.Unload(false);
            return obj;
#else
            var key = GetKey(assetBundleBytes);
            if (!_AssetBundles.ContainsKey(key))
            {
                _AssetBundles[key] = LoadAssetBundle(assetBundleBytes);
                if (_AssetBundles[key] == null)
                {
                     Console.WriteLine("Looks like the asset bundle failed to load?");
                }
            }

            try
            {
                 Console.WriteLine("Loading: {0} ({1})", name, key);
                //foreach (var asset in _AssetBundles[key].LoadAllAssets())
                //{
                //     Console.WriteLine(asset.name);
                //}

                name = name.Replace("Custom/", "");
                var loadedAsset = _AssetBundles[key].LoadAsset<T>(name);
                if (!loadedAsset)
                {
                     Console.WriteLine("Failed to load {0}", name);
                }

                return !typeof(Shader).IsAssignableFrom(typeof(T)) && !typeof(ComputeShader).IsAssignableFrom(typeof(T)) ? UnityEngine.Object.Instantiate<T>(loadedAsset) : loadedAsset;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return null;
            }
#endif
        }

        private static AssetBundle LoadAssetBundle(byte[] bytes)
        {
            if (_LoadFromMemory != null)
            {
                return _LoadFromMemory.Invoke(null, new object[] { bytes }) as AssetBundle;
            }
            else if (_CreateFromMemory != null)
            {
                return _CreateFromMemory.Invoke(null, new object[] { bytes }) as AssetBundle;
            }
            else
            {
                Console.WriteLine("Could not find a way to load AssetBundles!");
                return null;
            }
        }

        private static string CalculateChecksum(byte[] byteToCalculate)
        {
            int checksum = 0;
            foreach (byte chData in byteToCalculate)
            {
                checksum += chData;
            }
            checksum &= 0xff;
            return checksum.ToString("X2");
        }

        private static string GetKey(byte[] assetBundleBytes)
        {
            return CalculateChecksum(assetBundleBytes);
        }

        private static Dictionary<string, Transform> _DebugBalls = new Dictionary<string, Transform>();
        public static Transform GetDebugBall(string name)
        {
            Transform debugBall;
            if (!_DebugBalls.TryGetValue(name, out debugBall) || !debugBall)
            {
                debugBall = GameObject.CreatePrimitive(PrimitiveType.Sphere).transform;
                debugBall.transform.localScale *= 0.03f;
                _DebugBalls[name] = debugBall;
            }

            return debugBall;
        }


        public static Transform CreateGameObjectAsChild(string name, Transform parent, bool dontDestroy = false)
        {
            var go = new GameObject(name);
            go.transform.SetParent(parent, false);

            if (dontDestroy)
            {
                GameObject.DontDestroyOnLoad(go);
            }

            return go.transform;
        }

        /// <summary>
        /// Loads an image from the images folder.
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public static Texture2D LoadImage(string filePath)
        {
            string ovrDirectory = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "Images");
            filePath = Path.Combine(ovrDirectory, filePath);

            Texture2D tex = null;
            byte[] fileData;

            if (File.Exists(filePath))
            {
                fileData = File.ReadAllBytes(filePath);
                tex = new Texture2D(2, 2);
                tex.LoadImage(fileData); //..this will auto-resize the texture dimensions.
            }
            else
            {
             //   VRLog.Warn("File " + filePath + " does not exist");
            }
            return tex;
        }

        public static string[] GetLayerNames(int mask)
        {
            List<string> masks = new List<string>();
            for (int i = 0; i <= 31; i++) //user defined layers start with layer 8 and unity supports 31 layers
            {
                if ((mask & (1 << i)) != 0) masks.Add(LayerMask.LayerToName(i));
            }
            return masks.Select(m => m.Trim()).Where(m => m.Length > 0).ToArray();
        }


        public static T CopyComponent<T>(T original, GameObject destination) where T : Component
        {
            System.Type type = original.GetType();
            Component copy = destination.AddComponent(type);
            System.Reflection.FieldInfo[] fields = type.GetFields();
            foreach (System.Reflection.FieldInfo field in fields)
            {
                field.SetValue(copy, field.GetValue(original));
            }
            return copy as T;
        }

  
        public static IEnumerable<GameObject> GetRootNodes()
        {
            return UnityEngine.Object.FindObjectsOfType<GameObject>().Where(go => go.transform.parent == null);
        }

     

        // -- COMPATIBILITY --
  
    
    }
