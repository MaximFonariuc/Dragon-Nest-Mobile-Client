using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace com.tencent.pandora.tools
{
    /// <summary>
    /// 切换平台打包时，资源（主要是图片）需要重新导入时，因为图片需要根据不同平台
    /// 重新压缩，该过程比较耗时。
    /// 解决方法：将不需要打包的图片导入设置为不压缩，真彩色
    /// </summary>
    public class TexturePostprocessor : AssetPostprocessor
    {
        /// <summary>
        /// 此处加入导入图片为不压缩、真彩色的文件夹列表
        /// </summary>
        private static List<string> UNCOMPRESS_FOLDER_LIST = new List<string>() { "Assets/CACHE/", "Assets/NGUI/Editor" };

        /// <summary>
        /// 面板资源的原始贴图以真32导入
        /// </summary>
        private static Regex PANEL_TEXTURE_PATH = new Regex(@"Assets/Actions/Resources/.*?/Textures", RegexOptions.IgnoreCase);

        private static Regex PANEL_ATLAS_PATH = new Regex(@"Assets/Actions/Resources/.*?/Atlas", RegexOptions.IgnoreCase);

        private void OnPreprocessTexture()
        {
            if(PANEL_TEXTURE_PATH.IsMatch(this.assetPath) == true || IsUncompressTexture(this.assetPath) == true)
            {
                //以真32保存图片
                TextureImporter textureImporter = (TextureImporter)assetImporter;
                textureImporter.textureFormat = TextureImporterFormat.RGBA32;
            }
            else if(PANEL_ATLAS_PATH.IsMatch(this.assetPath) == true)
            {
                //关闭mipmap
                TextureImporter textureImporter = (TextureImporter)assetImporter;
                textureImporter.mipmapEnabled = false;
            }
        }

        private bool IsUncompressTexture(string path)
        {
            foreach(string s in UNCOMPRESS_FOLDER_LIST)
            {
                if(path.Contains(s) == true)
                {
                    return true;
                }
            }
            return false;
        }
    }
}

