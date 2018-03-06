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
        private Dictionary<string, AssetBundle> m_LoadedAssetBundles = new Dictionary<string, AssetBundle>(); //加载Bundles字典
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

        //IEnumerator OnLoadAsset<T>(string abName) where T : UObject {
        //    //AssetBundle budleInfo = 
        //}

        //IEnumerator LoadAssetBundle(string abName, Type type) {
        //    string url = this.m_BaseDownLoadLoadingURL + abName;
        //    WWW downlload = null;
        //    if (type == typeof(AssetBundleManifest)) {
        //        downlload = new WWW(url);
        //    } else {
        //        string[] dependencies = this.m_AssetBundleManifest.GetAllDependencies(abName);
        //        if(dependencies.Length > 0) {
                    
        //        }
        //    }
        //}
    }
}
