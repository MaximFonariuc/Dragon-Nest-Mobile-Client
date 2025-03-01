﻿#if UNITY_4_7
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using XUtliPoolLib;

namespace ABSystem
{
    public class AssetBundleBuilder4x : ABBuilder
    {
        static BuildAssetBundleOptions options =
            BuildAssetBundleOptions.DeterministicAssetBundle |
            BuildAssetBundleOptions.CollectDependencies |
            BuildAssetBundleOptions.UncompressedAssetBundle |
            BuildAssetBundleOptions.CompleteAssets;

        /// <summary>
        /// 本次增量更新的
        /// </summary>
        protected List<AssetTarget> newBuildTargets = new List<AssetTarget>();

        public AssetBundleBuilder4x(AssetBundlePathResolver pathResolver) : base(pathResolver)
        {

        }

        public override void Export()
        {
            try
            {
                base.Export();

                //Build Export Tree
                var all = AssetBundleUtils.GetAll();
                List<List<AssetTarget>> tree = new List<List<AssetTarget>>();
                float total = all.Count;
                float count = 0;
                foreach (AssetTarget target in all)
                {
                    target.AnalyzeIfDepTreeChanged();
                    BuildExportTree(target, tree, 0);

                    EditorUtility.DisplayProgressBar(string.Format("BuildExportTree...({0}/{1})", count, total), target.assetPath, ++count / total);
                }

                //Export
                this.Export(tree, 0);
                this.SaveDepAll(all);
                this.RemoveUnused(all);
                AssetDatabase.Refresh();
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }
        }

        void BuildExportTree(AssetTarget parent, List<List<AssetTarget>> tree, int currentLevel)
        {
            if (parent.level == -1 && parent.type != AssetType.Builtin)
            {
                List<AssetTarget> levelList = null;
                if (tree.Count > currentLevel)
                {
                    levelList = tree[currentLevel];
                }
                else
                {
                    levelList = new List<AssetTarget>();
                    tree.Add(levelList);
                }
                levelList.Add(parent);
                parent.UpdateLevel(currentLevel + 1, levelList);

                foreach (AssetTarget ei in parent.dependsChildren)
                {
                    if (ei.level != -1 && ei.level <= parent.level)
                    {
                        ei.UpdateLevel(-1, null);
                    }
                    BuildExportTree(ei, tree, currentLevel + 1);
                }
            }
        }

        void Export(List<List<AssetTarget>> tree, int currentLevel)
        {
            if (currentLevel >= tree.Count)
                return;

            BuildPipeline.PushAssetDependencies();
            List<AssetTarget> levelList = tree[currentLevel];

            //把Child个数多的放在前面
            levelList.Sort();

            float total = levelList.Count;
            float count = 0;
            foreach (AssetTarget ei in levelList)
            {
                Export(ei);
                EditorUtility.DisplayProgressBar(string.Format("Export...({0}/{1})", count, total), ei.assetPath, ++count / total);
            }
            if (currentLevel < tree.Count)
            {
                Export(tree, currentLevel + 1);
            }
            BuildPipeline.PopAssetDependencies();
        }

        void Export(AssetTarget target)
        {
            if (target.needExport)
            {
                //写入 .assetbundle 包
                target.BuildBundle(options);

                if (target.isNewBuild)
                    newBuildTargets.Add(target);
            }
        }
    }
}
#endif