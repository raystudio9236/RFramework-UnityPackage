using System.Collections.Generic;
using UnityEngine;
using RFramework.Common.Singleton;

namespace RFramework.Runtime.Module.Resource
{
    public delegate void LoadSuccessDelegate(string path, object resource, object userData);
    public delegate void LoadFailedDelegate(string path, string errorInfo, object userData);

    public class ResourceManager : MonoSingleton<ResourceManager>
    {
        private struct LoadData
        {
            public ResourceRequest Request;
            public string Path;
            public object UserData;

            public LoadSuccessDelegate LoadSuccessDelegate;
            public LoadFailedDelegate LoadFailedDelegate;
        }

        private List<LoadData> _loadReqs = new List<LoadData>();

        #region Public API

        /// <summary>
        /// 同步加载某个资源
        /// </summary>
        /// <param name="path">资源路径</param>
        /// <typeparam name="T">资源类型</typeparam>
        /// <returns>对应资源</returns>
        public T LoadSync<T>(string path) where T : Object
        {
            return Resources.Load<T>(path);
        }

        /// <summary>
        /// 同步加载某个资源
        /// </summary>
        /// <param name="path">资源路径</param>
        /// <typeparam name="T">资源类型</typeparam>
        /// <returns>对应资源</returns>
        public T Load<T>(string path) where T : Object
        {
            return LoadSync<T>(path);
        }

        /// <summary>
        /// 异步加载某个资源
        /// </summary>
        /// <param name="path">资源路径</param>
        /// <param name="loadSuccessDelegate">资源加载成功回调</param>
        /// <param name="loadFailedDelegate">资源加载失败回调</param>
        /// <param name="userData">用户数据</param>
        /// <typeparam name="T">资源类型</typeparam>
        public void LoadAsync<T>(string path,
            LoadSuccessDelegate loadSuccessDelegate,
            LoadFailedDelegate loadFailedDelegate,
            object userData) where T : Object
        {
            var loadData = new LoadData
            {
                Path = path,
                UserData = userData,
                LoadSuccessDelegate = loadSuccessDelegate,
                LoadFailedDelegate = loadFailedDelegate,
                Request = Resources.LoadAsync<T>(path)
            };
            _loadReqs.Add(loadData);
        }

        /// <summary>
        /// 从内存中卸载某个资源
        /// </summary>
        /// <param name="asset">资源引用</param>
        public void Unload(Object asset)
        {
            Resources.UnloadAsset(asset);
        }

        /// <summary>
        /// 从内存中卸载所有没有用到的资源
        /// </summary>
        public void UnloadUnusedAssets()
        {
            Resources.UnloadUnusedAssets();
        }

        #endregion

        private void Update()
        {
            for (var i = _loadReqs.Count - 1; i >= 0; i--)
            {
                var req = _loadReqs[i];
                if (req.Request.isDone)
                {
                    _loadReqs.RemoveAt(i);

                    if (req.Request.asset == null)
                    {
                        req.LoadFailedDelegate?.Invoke(req.Path, "not found!", req.UserData);
                    }
                    else
                    {
                        req.LoadSuccessDelegate.Invoke(req.Path, req.Request.asset, req.UserData);
                    }
                }
            }
        }
    }
}