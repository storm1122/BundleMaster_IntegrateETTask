﻿using ET;

namespace BM
{
    public abstract class LoadHandlerBase
    {
        /// <summary>
        /// 对应的加载的资源的路径
        /// </summary>
        protected string AssetPath;
        
        /// <summary>
        /// 所属分包的名称
        /// </summary>
        protected string BundlePackageName;
        
        /// <summary>
        /// 加载完成的计数
        /// </summary>
        protected int RefLoadFinishCount = 0;
        
        /// <summary>
        /// 是否卸载标记位
        /// </summary>
        private bool _unloadFinish = false;
        
        /// <summary>
        /// 加载计数器(负责完成所有依赖的Bundle加载完成)
        /// </summary>
        protected async ETTask LoadAsyncLoader(LoadBase loadBase, ETTask baseTcs)
        {
            ETTask tcs = ETTask.Create(true);
            loadBase.LoadAssetBundleAsync(tcs, BundlePackageName);
            await tcs;
            RefLoadFinishCount--;
            if (RefLoadFinishCount == 0)
            {
                baseTcs.SetResult();
            }
            if (RefLoadFinishCount < 0)
            {
                AssetLogHelper.LogError("资源加载引用计数不正确: " + RefLoadFinishCount);
            }
        }
        
        public void UnLoad()
        {
            if (AssetComponentConfig.AssetLoadMode == AssetLoadMode.Develop)
            {
                return;
            }
            if (_unloadFinish)
            {
                AssetLogHelper.LogError(AssetPath + "已经卸载完了");
                return;
            }
            //减少引用数量
            ClearAsset();
            _unloadFinish = true;
        }

        /// <summary>
        /// 子类需要实现清理资源引用的逻辑
        /// </summary>
        protected abstract void ClearAsset();
    }
}