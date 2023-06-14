using System.Collections.Generic;
using System.IO;
using Core.Util;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using UnityEngine.U2D;

namespace Common
{
    /// <summary>
    /// 资源加载
    /// 遗弃，使用新的Addressables组件
    /// </summary>
    public class ResourceManager : SingleInstance<ResourceManager>
    {
        /// <summary>
        /// 资源对应列表
        /// </summary>
        Dictionary<string, string> resourcePaths;


        /**
         * 图集列表
         */
        private Dictionary<string, SpriteAtlas> _atlasMap;

        private AssetBundleManifest assetBundleManifest;

        /// <summary>
        /// 资源加载器
        /// </summary>
        private Dictionary<string, AssetBundleLoader> _assetBundleLoaders;

        public delegate void UISpriteDelegate(Sprite sprite);

        public ResourceManager()
        {
            Init();
        }

        /// <summary>
        /// 初始化，加载资源列表
        /// </summary>
        public void Init()
        {
            _atlasMap = new Dictionary<string, SpriteAtlas>();
            resourcePaths = new Dictionary<string, string>();
#if UNITY_EDITOR
            string[] names = AssetDatabase.GetAllAssetBundleNames();
            int length = names.Length;
            for (int i = 0; i < length; i++)
            {
                var name = names[i];
                var deps = AssetDatabase.GetAssetPathsFromAssetBundle(name);
                if (deps.Length <= 0) continue;
                string str = string.Empty;
                foreach (var s in deps)
                {
                    string[] paths = s.Split('/');
                    string fileName = paths[^1];
                    if (resourcePaths.ContainsKey(fileName))
                    {
                        Debug.Log(
                            $"have same file name in resource {fileName} , path 1 = {s} , path2 = {resourcePaths[fileName]}");
                        continue;
                    }

                    resourcePaths.Add(fileName, s);
                }
            }
# endif


            //windows平台
            if (Application.platform == RuntimePlatform.WindowsPlayer /* ||
                Application.platform == RuntimePlatform.WindowsEditor*/)
            {
                var path = Path.Combine(Application.streamingAssetsPath, "StreamingAssets");
                var myLoadedAssetBundle = AssetBundle.LoadFromFile(path);
                if (myLoadedAssetBundle == null)
                {
                    var message = $"路径：{path} StreamingAssets 加载失败";
                    Log.Println(message);
                    Debug.LogError(message);
                }

                assetBundleManifest = myLoadedAssetBundle.LoadAsset<AssetBundleManifest>("AssetBundleManifest");

                Log.Println($"资源路径{Application.streamingAssetsPath}");
                Log.Println($"平台{Application.platform}");
                _assetBundleLoaders = new Dictionary<string, AssetBundleLoader>();
                foreach (var name in assetBundleManifest.GetAllAssetBundles())
                {
                    Debug.Log($"创建资源加载器：{name}");
                    _assetBundleLoaders.Add(name, new AssetBundleLoader(name));
                }
            }
        }


        /// <summary>
        ///  加载prefab 游戏对象 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="callback"></param>
        /// <param name="async"></param>
        public void LoadPrefabGameObject(string name, System.Action<GameObject> callback, bool async = true)
        {
            if (string.IsNullOrEmpty(name))
                return;

            LoadAsset<GameObject>(name, "prefab", callback);
        }

        /// <summary>
        ///  加载 UI
        /// </summary>
        /// <param name="name"></param>
        /// <param name="callback"></param>
        /// <param name="async"></param>
        public void LoadUI(string name, System.Action<GameObject> callback)
        {
            if (string.IsNullOrEmpty(name))
                return;

            LoadAsset<GameObject>(name, "prefab", callback, "ui");
        }

        /// <summary>
        /// 加载音频文件 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="callback"></param>
        /// <param name="suffix">默认MP3</param>
        public void LoadAudioClip(string name, System.Action<AudioClip> callback,string suffix="mp3")
        {
            LoadAsset<AudioClip>(name, suffix, callback, "audio");
        }

        public void LoadTextAsset(string name, System.Action<TextAsset> callback, bool async = true)
        {
            LoadAsset<TextAsset>(name, "bytes", callback);
        }

        public void LoadText(string name, System.Action<TextAsset> callback, bool async = true)
        {
            LoadAsset<TextAsset>(name, "txt", callback);
        }

        public void LoadXMLAsset(string name, System.Action<TextAsset> callback, bool async = true)
        {
            LoadAsset<TextAsset>(name, "xml", callback);
        }

        /// <summary>
        /// 加载精灵图片 @
        /// </summary>
        /// <param name="atlasName"></param>
        /// <param name="spriteName"></param>
        /// <param name="callback"></param>
        /// <param name="async"></param>
        public void LoadSpriteAsset(string atlasName, string spriteName, UISpriteDelegate callback)
        {
            SpriteAtlas runningAtlas;
            if (!_atlasMap.TryGetValue(atlasName, out runningAtlas))
            {
                LoadAsset<SpriteAtlas>(atlasName, "spriteatlas", (atlas) =>
                {
                    if (atlas != null)
                    {
                        runningAtlas = atlas;
                        _atlasMap.Add(atlasName, atlas);
                        callback(atlas.GetSprite(spriteName));
                    }
                    else
                    {
                        callback(null);
                    }
                });
            }
            else
            {
                callback(runningAtlas.GetSprite(spriteName));
            }
        }

        /// <summary>
        /// 加载单个贴图，生成精灵
        /// </summary>
        /// <param name="name"></param>
        /// <param name="callback"></param>
        /// <param name="async"></param>
        public void LoadSpriteBySingleAsset(string name, UISpriteDelegate callback, bool async = true)
        {
            LoadAsset<Texture2D>(name, "png", (texture) =>
            {
                if (texture != null)
                {
                    callback(Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), Vector2.zero));
                }
                else
                {
                    callback(null);
                }
            });
        }


        /// <summary>
        /// 加载资源 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="suffix"></param>
        /// <param name="callback"></param>
        /// <param name="async"></param>
        /// <param name="bundleName"></param>
        /// <typeparam name="T"></typeparam>
        private void LoadAsset<T>(string name, string suffix, System.Action<T> callback,
            string bundleName = "ui")
            where T : class
        {
            string trueName = $"{name}.{suffix}";
#if UNITY_EDITOR
            string path = GetBundleName(trueName);
            UnityEngine.Object o = AssetDatabase.LoadAssetAtPath(path, typeof(T));
            if (o != null)
            {
                T clip = o as T;
                callback(clip);
            }
            else
            {
                Debug.LogWarning($"资源：{name}.{suffix} 获取失败");
                callback(null);
            }
#endif

            //windows平台
            if (Application.platform == RuntimePlatform.WindowsPlayer /*||
                Application.platform == RuntimePlatform.WindowsEditor*/)
            {
                if (_assetBundleLoaders.TryGetValue(bundleName, out var assetBundleLoader))
                {
                    assetBundleLoader.LoadResource<T>(trueName, obj =>
                        {
                            if (obj == null)
                            {
                                Log.Println($"{trueName} 在 {bundleName} 中获取失败");
                            }

                            callback(obj);
                        }
                    );
                }
                else
                {
                    Log.Println($"资源加载器：{bundleName} 不存在");
                }
            }
        }


        /// <summary>
        /// 获取资源加载器名字 
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        private string GetBundleName(string name)
        {
            if (resourcePaths.ContainsKey(name))
            {
                return resourcePaths[name];
            }

            return string.Empty;
        }
    }

    /// <summary>
    /// 资源加载器
    /// </summary>
    public class AssetBundleLoader
    {
        AssetBundle _assetBundle = null;

        //加载缓存的文件
        Dictionary<string, UnityEngine.Object> objects;

        //bundle 名字
        private string name;

        public AssetBundleLoader(string name)
        {
            objects = new Dictionary<string, Object>();
            this.name = name;
            _assetBundle = AssetBundle.LoadFromFile(Path.Combine(Application.streamingAssetsPath, name));
        }


        /// <summary>
        /// 加载网络资源 
        /// </summary>
        /// <param name="resourceName"></param>
        /// <param name="async"></param>
        /// <param name="complete"></param>
        /// <typeparam name="T"></typeparam>
        public void LoadResource<T>(string resourceName, System.Action<T> complete) where T : class
        {
            UnityEngine.Object o = null;

            if (!objects.TryGetValue(resourceName, out o))
            {
                o = _assetBundle.LoadAsset(resourceName);
                objects.Add(resourceName, o);
                complete(o as T);
            }
            else
            {
                complete(o as T);
            }
        }
    }
}