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
