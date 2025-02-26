﻿using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEditor;
using UnityEngine;
using Directory = UnityEngine.Windows.Directory;

namespace BM
{
    public static class BuildAssetsTools
    {
        /// <summary>
        /// 获取一个目录下所有的子文件
        /// </summary>
        public static void GetChildFiles(string basePath, HashSet<string> files)
        {
            DirectoryInfo basefolder = new DirectoryInfo(basePath);
            FileInfo[] basefil = basefolder.GetFiles();
            for (int i = 0; i < basefil.Length; i++)
            {
                    
                if (CantLoadType(basefil[i].FullName))
                {
                    files.Add(basePath + "/" + basefil[i].Name);
                }
            }
            Er(basePath);
            void Er(string subPath)
            {
                string[] subfolders = AssetDatabase.GetSubFolders(subPath);
                for (int i = 0; i < subfolders.Length; i++)
                {
                    DirectoryInfo subfolder = new DirectoryInfo(subfolders[i]);
                    FileInfo[] fil = subfolder.GetFiles();
                    for (int j = 0; j < fil.Length; j++)
                    {
                    
                        if (CantLoadType(fil[j].FullName))
                        {
                            files.Add(subfolders[i] + "/" + fil[j].Name);
                        }
                    }
                    Er(subfolders[i]);
                }
            }
        }
        
        /// <summary>
        /// 创建加密的AssetBundle
        /// </summary>
        public static void CreateEncryptAssets(string bundlePackagePath, string encryptAssetPath, AssetBundleManifest manifest)
        {
            string[] assetBundles = manifest.GetAllAssetBundles();
            foreach (string assetBundle in assetBundles)
            {
                string bundlePath = Path.Combine(bundlePackagePath, assetBundle);
                if (!Directory.Exists(encryptAssetPath))
                {
                    Directory.CreateDirectory(encryptAssetPath);
                }
                using (FileStream fs = new FileStream(Path.Combine(encryptAssetPath, assetBundle), FileMode.OpenOrCreate))
                {
                    byte[] encryptBytes = VerifyHelper.CreateEncryptData(bundlePath, "mister91jiao");
                    fs.Write(encryptBytes, 0, encryptBytes.Length);
                }
            }
        }
        
        /// <summary>
        /// 需要忽略加载的格式
        /// </summary>
        public static bool CantLoadType(string fileFullName)
        {
            string suffix = Path.GetExtension(fileFullName);
            switch (suffix)
            {
                case ".dll":
                    return false;
                case ".cs":
                    return false;
                case ".meta":
                    return false;
                case ".js":
                    return false;
                case ".boo":
                    return false;
            }
            return true;
        }
        
        /// <summary>
        /// 是Shader资源
        /// </summary>
        public static bool IsShaderAsset(string fileFullName)
        {
            string suffix = Path.GetExtension(fileFullName);
            switch (suffix)
            {
                case ".shader":
                    return true;
                case ".shadervariants":
                    return true;
            }
            return false;
        }
        
    }
}