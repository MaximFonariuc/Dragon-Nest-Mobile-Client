using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace com.tencent.pandora
{
    public abstract class AbstractLoader : MonoBehaviour
    {
        //加载出错的队列，会静默加载（不断的尝试加载），为避免过于频繁设置一个间隔
        protected Queue<string> _errorQueue;
        //重试间隔，以帧数为单位
        protected int _errorRetryMaxSpan = 300;
        protected int _errorRetrySpan = 0;
        //Key为资源路径，不含?seed=的
        protected Dictionary<string, int> _errorRetryCountDict;
        //Key为资源路径，不含?seed=的
        protected Dictionary<string, int> _errorMaxRetryCountDict;

        //下载并发数
        protected int _concurrentCount = 1;
        protected Queue<string> _toLoadQueue;
        protected Dictionary<string, WWW> _loadingDict;
        protected HashSet<string> _loadingSet;
        protected HashSet<string> _loadedSet;
        protected HashSet<string> _failedSet;
        protected List<Task> _taskList;
        public bool PauseDownloading { get; set; }

        public virtual void Initialize()
        {
            _toLoadQueue = new Queue<string>();
            _loadingDict = new Dictionary<string, WWW>();
            _loadingSet = new HashSet<string>();
            _loadedSet = new HashSet<string>();
            _failedSet = new HashSet<string>();
            _taskList = new List<Task>();
            _errorQueue = new Queue<string>();
            _errorRetryCountDict = new Dictionary<string, int>();
            _errorMaxRetryCountDict = new Dictionary<string, int>();
            StartCoroutine(DaemonHandleTaskLoaded());
            for (int i = 0; i < _concurrentCount; i++)
            {
                StartCoroutine(DaemonLoad());
            }
            StartCoroutine(DaemonHandleError());
        }

        protected virtual IEnumerator DaemonLoad()
        {
            while (true)
            {
                if (_toLoadQueue.Count > 0 && PauseDownloading == false)
                {
                    string url = _toLoadQueue.Dequeue();
                    string originalUrl = GetOriginalUrl(url);
                    WWW www = new WWW(ReplaceHttpToken(url));
                    _loadingDict.Add(originalUrl, www);
                    _loadingSet.Add(originalUrl);
                    yield return www;
                    _loadingDict.Remove(originalUrl);
                    _loadingSet.Remove(originalUrl);

                    if(string.IsNullOrEmpty(www.error) == false)
                    {
                        //错误处理，放入_errorQueue, 稍后重新加载
                        _errorQueue.Enqueue(GetRandomSeedUrl(url));
                    }
                    else
                    {
                        if(HandleLoadedContent(originalUrl, www) == true)
                        {
                            _errorMaxRetryCountDict.Remove(originalUrl);
                            _errorRetryCountDict.Remove(originalUrl);
                            _loadedSet.Add(originalUrl);
                        }
                        else
                        {
                            _errorQueue.Enqueue(GetRandomSeedUrl(url));
                        }
                    }

                }
                yield return null;
            }
        }

        public string ReplaceHttpToken(string url)
        {
            if(Pandora.Instance.UseHttps == true)
            {
                return url.Replace("http://", "https://");
            }
            return url;
        }

        //发生错误重连时需要加上?seed=xxx以消除cdn节点缓存的影响
        public string GetRandomSeedUrl(string url)
        {
            return GetOriginalUrl(url) + "?seed=" + UnityEngine.Random.Range(0, 10000).ToString();
        }

        public string GetOriginalUrl(string url)
        {
            if(url.Contains("?") == false)
            {
                return url;
            }
            int index = url.IndexOf("?");
            return url.Substring(0, index);
        }


        /// <summary>
        /// 检查加载任务是否完成
        /// </summary>
        /// <returns></returns>
        private IEnumerator DaemonHandleTaskLoaded()
        {
            while(true)
            {
                for(int i = _taskList.Count - 1; i >= 0; i--)
                {
                    Task task = _taskList[i];
                    if(task.IsFinished() == true)
                    {
                        _taskList.RemoveAt(i);
                        task.ExecuteLoadedCallback();
                    }
                }
                yield return null;
            }
        }

        private IEnumerator DaemonHandleError()
        {
            while(true)
            {
                if(_errorQueue.Count > 0)
                {
                    if(_errorRetrySpan < _errorRetryMaxSpan)
                    {
                        _errorRetrySpan += 1;
                    }
                    else
                    {
                        _errorRetrySpan = 0;
                        string url = _errorQueue.Dequeue();
                        string originalUrl = GetOriginalUrl(url);
                        if((_errorRetryCountDict.ContainsKey(originalUrl) && _errorMaxRetryCountDict.ContainsKey(originalUrl))
                            && _errorRetryCountDict[originalUrl] < _errorMaxRetryCountDict[originalUrl])
                        {
                            _errorRetryCountDict[originalUrl] = _errorRetryCountDict[originalUrl] + 1;
                            Logger.Log("重连： " + url);
                            _toLoadQueue.Enqueue(url);
                        }
                        else
                        {
                            _errorMaxRetryCountDict.Remove(originalUrl);
                            _errorRetryCountDict.Remove(originalUrl);
                            _loadingDict.Remove(originalUrl);
                            _loadingSet.Remove(originalUrl);
                            _failedSet.Add(originalUrl);
                            HandleError(originalUrl);
                        }
                    }
                }
                yield return null;
            }
        }

        /// <summary>
        /// 资源加载后的操作
        /// </summary>
        /// <param name="url"></param>
        /// <param name="www"></param>
        protected virtual bool HandleLoadedContent(string url, WWW www)
        {
            return false;
        }
        /// <summary>
        /// 重试次数用完后依然没有获取到数据时的处理
        /// </summary>
        /// <param name="url"></param>
        protected abstract void HandleError(string url);

        public bool IsLoaded(string url)
        {
            return _loadedSet.Contains(GetOriginalUrl(url));
        }

        public bool IsFailed(string url)
        {
            return _failedSet.Contains(GetOriginalUrl(url));
        }

        public bool RemoveFormLoadedSet(string url)
        {
            return _loadedSet.Remove(GetOriginalUrl(url));
        }

        public virtual void LoadAsset(string url, Action<string> callback, int maxRetryCount)
        {
            AddAsset(url, maxRetryCount);
            SingleTask task = new SingleTask(url, callback, this);
            _taskList.Insert(0, task);
        }

        /// <summary>
        /// 将资源加载存放到
        /// </summary>
        /// <param name="urlList"></param>
        /// <param name="callback"></param>
        public virtual void LoadAssetList(List<string> urlList, Action<List<string>> callback, int maxRetryCount)
        {
            for(int i = 0; i < urlList.Count; i++)
            {
                AddAsset(urlList[i], maxRetryCount);
            }
            BatchTask task = new BatchTask(urlList, callback, this);
            _taskList.Insert(0, task);
        }

        private void AddAsset(string url, int maxRetryCount)
        {
            if (_loadingSet.Contains(GetOriginalUrl(url)) || _loadedSet.Contains(GetOriginalUrl(url)) || _toLoadQueue.Contains(url))
            {
                return;
            }

            _failedSet.Remove(url);
            _toLoadQueue.Enqueue(url);
            _errorRetryCountDict[GetOriginalUrl(url)] = 1;
            _errorMaxRetryCountDict[GetOriginalUrl(url)] = maxRetryCount;
        }

        public virtual void Clear()
        {
            _toLoadQueue.Clear();
            _loadingDict.Clear();
            _loadingSet.Clear();
            _loadedSet.Clear();
            _failedSet.Clear();
            _taskList.Clear();
            _errorQueue.Clear();
            _errorRetryCountDict.Clear();
            _errorMaxRetryCountDict.Clear();
        }

        public abstract class Task
        {
            protected AbstractLoader _loader;
            protected bool _isSuccessful = true;
            public abstract bool IsFinished();
            public abstract void ExecuteLoadedCallback();
        }

        /// <summary>
        /// 批量加载模式
        /// </summary>
        public class BatchTask : Task
        {
            private List<string> _urlList;
            private Action<List<string>> _callback;

            public BatchTask(List<string> urlList, Action<List<string>> callback, AbstractLoader loadedr)
            {
                _urlList = urlList;
                _callback = callback;
                _loader = loadedr;
            }

            public override bool IsFinished()
            {
                bool finished = true;
                for (int j = 0; j < _urlList.Count; j++)
                {
                    string url = _urlList[j];

                    if (_loader.IsLoaded(url) == false && _loader.IsFailed(url) == false)
                    {
                        finished = false;
                        break;
                    }
                }
                return finished;
            }

            public override void ExecuteLoadedCallback()
            {
                for (int i = 0; i < _urlList.Count; i++)
                {
                    if(_loader.IsFailed(_urlList[i]) == true)
                    {
                        _isSuccessful = false;
                        break;
                    }
                }
                if(_callback != null && _isSuccessful == true)
                {
                    _callback(_urlList);
                    _callback = null;
                }
            }
        }

        internal class SingleTask : Task
        {
            private string _url;
            private Action<string> _callback;

            public SingleTask(string url, Action<string> callback, AbstractLoader loader)
            {
                _url = url;
                _callback = callback;
                _loader = loader;
            }

            public override bool IsFinished()
            {
                return _loader.IsLoaded(_url) || _loader.IsFailed(_url);
            }

            public override void ExecuteLoadedCallback()
            {
                _isSuccessful = !_loader.IsFailed(_url);
                if(_callback != null && _isSuccessful == true)
                {
                    _callback(_url);
                    _callback = null;
                }
            }
        }

    }
}
