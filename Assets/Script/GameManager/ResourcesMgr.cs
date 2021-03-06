﻿#if UNITY_EDITOR
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEditor;
    using Code.Core;
    using System;
    using UObject = UnityEngine.Object;
namespace GManager {
    public class ResourcesMgr : ManagerBase<ResourcesMgr> {
        public string a = "";
        static string[] editorExts = new string[]{
            ".prefab",
            ".jpg",
            ".png",
            ".ttf",

            };

        static public UnityEngine.Object EditorLoad(string url)
        {
            string _url = url.Split('.')[0];
            foreach (var ext in editorExts)
            {

                string path = string.Format("Assets/Assetbundle/{0}{1}", _url, ext);
                UnityEngine.Object obj = AssetDatabase.LoadAssetAtPath(path, typeof(UnityEngine.Object));
                if (obj == null)
                {
                    obj = Resources.Load(_url);
                }
                if (obj)
                {
                    return obj;
                }
            }
            Log.Error(string.Format("Obj Is Null . Path = {0}", url));
            return null;
        }

        public UnityEngine.Object[] LoadUIPrefab(string assetName, ParametricMethod func = null) {
            return this.LoadPrefab("uiassetbundle", assetName, func);

        }

        public UnityEngine.Object[] LoadPrefab(string abName, string assetName, ParametricMethod func)
        {
            List<UObject> result = new List<UObject>();
            UObject go = EditorLoad(assetName);
            if (go != null) result.Add(go);
            if (func != null) func(result.ToArray());
            return result.ToArray();
        }

        public UnityEngine.Object[] LoadPrefab(string abName, string[] assetNames, ParametricMethod func)
        {
            List<UObject> result = new List<UObject>();
            for (int i = 0; i < assetNames.Length; i++)
            {
                UObject go = EditorLoad(assetNames[i]);
                if (go != null) result.Add(go);
            }
            if (func != null) func(result.ToArray());
            return result.ToArray();
        }


    }
}

#elif ASYNC_MODE
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Code.Core;
using System;
using UObject = UnityEngine.Object;
public class AssetBundleInfo
{
    public AssetBundle assetBundle;
    public int referencedCount;   ///引用计数

    public AssetBundleInfo(AssetBundle _assetbundle){
        this.assetBundle = _assetbundle;
        this.referencedCount = 0;
    }
}
namespace GManager
{
    public class ResourcesMgr : ManagerBase<ResourcesMgr>
    {
        private string m_BaseDownLoadLoadingURL = "";   //下载根目录
        private string[] m_AllManifest = null;         //AssetBundle清单
        private AssetBundleManifest m_AssetBundleManifest = null;
        Dictionary<string, string[]> m_Dependencies = new Dictionary<string, string[]>();
        private Dictionary<string, AssetBundleInfo> m_LoadedAssetBundles = new Dictionary<string, AssetBundleInfo>(); //加载Bundles字典
        private Dictionary<string, List<LoadAssetRequest>> m_LoadRequests = new Dictionary<string, List<LoadAssetRequest>>(); //加载请求队列

        Dictionary<string, string> m_ReadAssetPaths = new Dictionary<string, string>(); //缓存文件名对应AssetBundle路径 防止重复查找产生多余GC

        class LoadAssetRequest{
            public Type assetType;
            public string[] assetNames;
            public Action<UObject[]> sharpFunc;
        }
        public void Initialize(string manifestName , Action initOk){
            this.m_BaseDownLoadLoadingURL = Util.GetRelativePath();
        }

        string GetRealAssetPath(string abName) {
            if (abName.Equals(AppConst.AssetDir)) {
                return abName;
            }
            abName = abName.ToLower();
            //如果没有格式扩展名添加扩展名
            if (!abName.EndsWith(AppConst.ExtName)) {
                abName += AppConst.ExtName;
            }
            var mainfest = "";
            if (this.m_ReadAssetPaths.TryGetValue(abName, out mainfest)) {
                return mainfest;
            } else {
                for (int i = 0; i < this.m_AllManifest.Length; i++) {
                    int index = this.m_AllManifest[i].LastIndexOf('/');
                    string path = this.m_AllManifest[i].Remove(0, index + 1); //字符串操作函数都会产生GC
                    if (path.Equals(abName)) {
                        this.m_ReadAssetPaths.Add(abName, this.m_AllManifest[i]);
                        return this.m_AllManifest[i];
                    }
                }
            }
            Debug.LogError("GetRealAssetPath Error:>>" + abName);
            return null;
        }
        
        public UnityEngine.Object[] LoadPrefab(string abName, string assetName, ParametricMethod func = null)
        {
            List<UObject> result = new List<UObject>();
            UObject go = LoadAsset<UObject>(abName, assetName);
            if (go != null) result.Add(go);
            if (func != null) func(result.ToArray());
            return result.ToArray();
        }

        public UnityEngine.Object[] LoadPrefab(string abName, string[] assetNames, ParametricMethod func = null)
        {
            abName = abName.ToLower();
            List<UObject> result = new List<UObject>();
            for (int i = 0; i < assetNames.Length; i++)
            {
                UObject go = LoadAsset<UObject>(abName, assetNames[i]);
                if (go != null) result.Add(go);
            }
            if (func != null) func(result.ToArray());
            return result.ToArray();
        }

        //载入素材
        void LoadAsset<T>(string abName , string[] assetNames, Action<UObject[]> action = null) where T : UObject {
            abName = GetRealAssetPath(abName);
            LoadAssetRequest request = new LoadAssetRequest();
            request.assetType = typeof(T);
            request.assetNames = assetNames;
            request.sharpFunc = action;

            List<LoadAssetRequest> requests = null;
            if(!this.m_LoadRequests.TryGetValue(abName , out requests)) {
                requests = new List<LoadAssetRequest>();
                requests.Add(request);
                this.m_LoadRequests.Add(abName, requests);
                
            } else {
                requests.Add(request);
            }
        }

        IEnumerator OnLoadAsset<T>(string abName) where T : UObject {
            AssetBundleInfo bundleInfo = GetLoadedAssetBundle(abName);
            if(bundleInfo == null){
                yield return StartCoroutine((OnLoadAssetBundle(abName, typeof(T))));

                bundleInfo = GetLoadedAssetBundle(abName);

                if(bundleInfo == null){
                    this.m_LoadRequests.Remove(abName);
                    Log.Error("OnLoadAsset ---->>>"+abName);
                    yield break;
                }

                List<LoadAssetRequest> list = null;
                if(!this.m_LoadRequests.TryGetValue(abName,out list)){
                    this.m_LoadRequests.Remove(abName);
                    yield break;
                }

                for (int i = 0; i < list.Count; i ++){
                    string[] assetNames = list[i].assetNames;
                    List<UObject> result = new List<UObject>();

                    AssetBundle ab = bundleInfo.assetBundle;
                    for (int j = 0; j < assetNames.Length; j ++){
                        string assetPath = assetNames[j];
                        AssetBundleRequest request = ab.LoadAssetAsync(assetPath, list[i].assetType);
                        yield return request;
                        result.Add(request.asset);
                    }
                    if(list[i].sharpFunc != null){
                        list[i].sharpFunc(result.ToArray());
                        list[i].sharpFunc = null;
                    }
                    bundleInfo.referencedCount++;
                }
            }
            this.m_LoadRequests.Remove(abName);
        }

        IEnumerator OnLoadAssetBundle(string abName, Type type) {
            string url = this.m_BaseDownLoadLoadingURL + abName;
            WWW download = null;
            if (type == typeof(AssetBundleManifest)) {
                download = new WWW(url);
            } else {
                string[] dependencies = this.m_AssetBundleManifest.GetAllDependencies(abName);
                if(dependencies.Length > 0) {     
                    this.m_Dependencies.Add(abName,dependencies);
                    for (int i = 0; i < dependencies.Length; i++)
                    {
                        string depName = dependencies[i];
                        AssetBundleInfo bundleInfo = null;
                        if (this.m_LoadedAssetBundles.TryGetValue(depName , out bundleInfo)){
                            bundleInfo.referencedCount++;
                        }else if(!this.m_LoadRequests.ContainsKey(abName)){
                            yield return StartCoroutine(OnLoadAssetBundle(abName , type));
                        }
                    }
                }

                download = WWW.LoadFromCacheOrDownload(url, this.m_AssetBundleManifest.GetAssetBundleHash(abName), 0);

                yield return download;

                AssetBundle assetObj = download.assetBundle;
                if(assetObj != null){
                    this.m_LoadedAssetBundles.Add(abName , new AssetBundleInfo(assetObj));
                }
            }
        }

        AssetBundleInfo GetLoadedAssetBundle(string abName){
            AssetBundleInfo bundle = null;
            this.m_LoadedAssetBundles.TryGetValue(abName , out bundle);
            if(bundle == null){
                return null;
            }

            //没有记录依赖项，只有bundle本身。
            string[] dependencies = null;
            if (!this.m_Dependencies.TryGetValue(abName, out dependencies)){
                return bundle;
            }

            //确保加载所有依赖项。需要判断依赖是否存在
            foreach(var dependency in dependencies){
                AssetBundleInfo dependentBunle;
                this.m_LoadedAssetBundles.TryGetValue(dependency , out dependentBunle);
                if (dependentBunle == null) return null;
            }
            return bundle;
        }


        /// <summary>
        /// 此函数交给外部卸载专用，自己调整是否需要彻底清除AB
        /// </summary>
        /// <param name="abName"></param>
        /// <param name="isThorough"></param>
        public void UnloadAssetBundle(string abName, bool isThorough = false)
        {
            abName = GetRealAssetPath(abName);
            Debug.Log(m_LoadedAssetBundles.Count + " assetbundle(s) in memory before unloading " + abName);
            UnloadAssetBundleInternal(abName, isThorough);
            UnloadDependencies(abName, isThorough);
            Debug.Log(m_LoadedAssetBundles.Count + " assetbundle(s) in memory after unloading " + abName);
        }

        void UnloadDependencies(string abName, bool isThorough)
        {
            string[] dependencies = null;
            if (!m_Dependencies.TryGetValue(abName, out dependencies))
                return;

            // Loop dependencies.
            foreach (var dependency in dependencies)
            {
                UnloadAssetBundleInternal(dependency, isThorough);
            }
            m_Dependencies.Remove(abName);
        }

        void UnloadAssetBundleInternal(string abName, bool isThorough)
        {
            AssetBundleInfo bundle = GetLoadedAssetBundle(abName);
            if (bundle == null) return;

            if (--bundle.referencedCount <= 0)
            {
                if (m_LoadRequests.ContainsKey(abName))
                {
                    return;     //如果当前AB处于Async Loading过程中，卸载会崩溃，只减去引用计数即可
                }
                bundle.assetBundle.Unload(isThorough);
                m_LoadedAssetBundles.Remove(abName);
                Debug.Log(abName + " has been unloaded successfully");
            }
        }
    }
}
#else
using System.Collections;
using UnityEngine;
using System;
using System.Collections.Generic;
using System.IO;
using UObject = UnityEngine.Object;
using Code.Core;
namespace GManager
{
    public class ResourcesMgr : ManagerBase<ResourcesMgr>
    {
        private string[] m_Variants = { };
        private AssetBundleManifest manifest;
        private AssetBundle shared, assetbundle;
        private Dictionary<string, AssetBundle> bundles;

        void Awake(){
        }

        /// <summary>
        /// 初始化
        /// </summary>
        public void Initialize(){
            byte[] stream = null;
            string uri = string.Empty;
            bundles = new Dictionary<string, AssetBundle>();
            uri = Util.DataPath + AppConst.AssetDir;
            if (!File.Exists(uri)) return;
            stream = File.ReadAllBytes(uri);
            assetbundle = AssetBundle.LoadFromMemory(stream);//AssetBundle.CreateFromMemoryImmediate(stream);
            manifest = assetbundle.LoadAsset<AssetBundleManifest>("AssetBundleManifest");
        }

        /// <summary>
        /// 载入素材
        /// </summary>
        public T LoadAsset<T>(string abname, string assetname) where T : UnityEngine.Object{
            abname = abname.ToLower();
            AssetBundle bundle = LoadAssetBundle(abname);
            return bundle.LoadAsset<T>(assetname);
        }

        public UnityEngine.Object[] LoadPrefab(string abName, string assetName, ParametricMethod func = null)
        {
            List<UObject> result = new List<UObject>();
            UObject go = LoadAsset<UObject>(abName, assetName);
            if (go != null) result.Add(go);
            if (func != null) func(result.ToArray());
            return result.ToArray();
        }

        public UnityEngine.Object[] LoadPrefab(string abName, string[] assetNames, ParametricMethod func = null)
        {
            abName = abName.ToLower();
            List<UObject> result = new List<UObject>();
            for (int i = 0; i < assetNames.Length; i++)
            {
                UObject go = LoadAsset<UObject>(abName, assetNames[i]);
                if (go != null) result.Add(go);
            }
            if (func != null) func(result.ToArray());
            return result.ToArray();
        }

        /// <summary>
        /// 载入AssetBundle
        /// </summary>
        /// <param name="abname"></param>
        /// <returns></returns>
        public AssetBundle LoadAssetBundle(string abname)
        {
            if (!abname.EndsWith(AppConst.ExtName))
            {
                abname += AppConst.ExtName;
            }
            AssetBundle bundle = null;
            if (!bundles.ContainsKey(abname))
            {
                byte[] stream = null;
                string uri = Util.DataPath + abname;
                Debug.LogWarning("LoadFile::>> " + uri);
                LoadDependencies(abname);

                stream = File.ReadAllBytes(uri);
                bundle = AssetBundle.LoadFromMemory(stream); //关联数据的素材绑定
                bundles.Add(abname, bundle);
            }
            else
            {
                bundles.TryGetValue(abname, out bundle);
            }
            return bundle;
        }

        /// <summary>
        /// 载入依赖
        /// </summary>
        /// <param name="name"></param>
        void LoadDependencies(string name)
        {
            if (manifest == null)
            {
                Debug.LogError("Please initialize AssetBundleManifest by calling AssetBundleManager.Initialize()");
                return;
            }
            // Get dependecies from the AssetBundleManifest object..
            string[] dependencies = manifest.GetAllDependencies(name);
            if (dependencies.Length == 0) return;

            for (int i = 0; i < dependencies.Length; i++)
                dependencies[i] = RemapVariantName(dependencies[i]);

            // Record and load all dependencies.
            for (int i = 0; i < dependencies.Length; i++)
            {
                LoadAssetBundle(dependencies[i]);
            }
        }

        // Remaps the asset bundle name to the best fitting asset bundle variant.
        string RemapVariantName(string assetBundleName)
        {
            string[] bundlesWithVariant = manifest.GetAllAssetBundlesWithVariant();

            // If the asset bundle doesn't have variant, simply return.
            if (System.Array.IndexOf(bundlesWithVariant, assetBundleName) < 0)
                return assetBundleName;

            string[] split = assetBundleName.Split('.');

            int bestFit = int.MaxValue;
            int bestFitIndex = -1;
            // Loop all the assetBundles with variant to find the best fit variant assetBundle.
            for (int i = 0; i < bundlesWithVariant.Length; i++)
            {
                string[] curSplit = bundlesWithVariant[i].Split('.');
                if (curSplit[0] != split[0])
                    continue;

                int found = System.Array.IndexOf(m_Variants, curSplit[1]);
                if (found != -1 && found < bestFit)
                {
                    bestFit = found;
                    bestFitIndex = i;
                }
            }
            if (bestFitIndex != -1)
                return bundlesWithVariant[bestFitIndex];
            else
                return assetBundleName;
        }

        /// <summary>
        /// 销毁资源
        /// </summary>
        void OnDestroy()
        {
            if (shared != null) shared.Unload(true);
            if (manifest != null) manifest = null;
            Debug.Log("~ResourceManager was destroy!");
        }
    }
}
#endif