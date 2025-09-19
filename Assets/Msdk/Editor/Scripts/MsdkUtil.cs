using UnityEngine;
using System;
using System.Collections;
using System.IO;
using System.Xml;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

public class MsdkUtil
{

    #region 工具接口
    // 检查Unity3D IDE版本是否低于 version
    public static bool isUnityEarlierThan(string version)
    {
        string unityVersion = Application.unityVersion;
        if (unityVersion.Length > version.Length) {
            unityVersion = unityVersion.Substring(0, version.Length);
        } else {
            version = version.Substring(0, unityVersion.Length);
        }
        if (unityVersion.CompareTo(version) < 0) {
            return true;
        } else {
            return false;
        }
    }
    #endregion

    #region 文本操作
    // 读取 key = vaule 类的配置
    public static Dictionary<string, string> ReadConfigs(string filePath)
    {
        Debug.Log("filePath:"+ filePath);
        Dictionary<string, string> configs = new Dictionary<string, string>();
        string[] lines = File.ReadAllLines(filePath);
        string pattern = @"^([(^;)\w\?\.\\:/]+)\s*=\s*([\w\?\.\\:=/]+)\s*";
        foreach (var line in lines) {
            foreach (Match m in Regex.Matches(line, pattern)) {
                if (!m.Groups[0].Success) {
                    continue;
                }
                string key = m.Groups[1].Value;
                string value = m.Groups[2].Value;
                Debug.Log("key:" + key + ",value:" + value);
                configs.Add(key, value);
            }
        }
        return configs;
    }

    // 将 below 的下一行替换为 text
	public static void ReplaceBelow(string fileFullPath, string below, string text)
	{
		StreamReader streamReader = new StreamReader(fileFullPath);
		string text_all = streamReader.ReadToEnd();
		streamReader.Close();

		int firstIndex = text_all.IndexOf(below);
		if(firstIndex == -1){
			Debug.LogError(fileFullPath +"中没有找到标志"+below);
			return;
		}

		int beginIndex = text_all.IndexOf("\n", firstIndex + below.Length);
		int endIndex = text_all.IndexOf("\n", beginIndex + 1);

		text_all = text_all.Substring (0, beginIndex) + "\n" + text + "\n" + text_all.Substring (endIndex + 1);

		StreamWriter streamWriter = new StreamWriter(fileFullPath);
		streamWriter.Write(text_all);
		streamWriter.Close();
	}

    // 在 below 下行添加 text
    public static void WriteBelow(string filePath, string below, string text)
    {
        StreamReader streamReader = new StreamReader(filePath);
        string text_all = streamReader.ReadToEnd();
        streamReader.Close();

        int beginIndex = text_all.IndexOf(below);
        if (beginIndex == -1) {
            Debug.LogError(filePath + "中没有找到标志" + below);
            return;
        }

        int endIndex = text_all.LastIndexOf("\n", beginIndex + below.Length);

        if (!text_all.Substring(endIndex, text.Length + 2).Contains(text)) {
            text_all = text_all.Substring(0, endIndex) + "\n" + text + "\n" + text_all.Substring(endIndex);
        }
        StreamWriter streamWriter = new StreamWriter(filePath);
        streamWriter.Write(text_all);
        streamWriter.Close();
    }

    // 在 below 下将 第一个oldString行 替换为 newString行
    public static void ReplaceLineBelow(string filePath, string below, string oldString, string newString)
    {
        string[] lines = File.ReadAllLines(filePath);
        for (int i = 0; i < lines.Length; i++) {
            if (lines[i].IndexOf(below) != -1) {
                for (int j = i; j < lines.Length; j++) {
                    if (lines[j].IndexOf(oldString) != -1) {
                        lines[j] = newString;
                        break;
                    }
                    if (j == lines.Length - 1) {
                        Debug.LogError(filePath + "中没有找到标志" + oldString);
                    }
                }
                break;
            }
            if (i == lines.Length - 1) {
                Debug.LogError(filePath + "中没有找到标志" + below);
            }
        }
        File.WriteAllLines(filePath, lines);
    }

    //替换文本中的含有oldText的整行为newText
    public static void ReplaceSpecialText(string fielFullPath, string oldText, string newText) {
		string[] lines = File.ReadAllLines (fielFullPath);
		for (int i=0; i<lines.Length; i++) {
			if (lines[i].IndexOf(oldText) != -1) {
				lines[i] = newText;
                break;
			}
            if (i == lines.Length - 1) {
                Debug.LogError(fielFullPath + "中没有找到需要替换的内容" + oldText);
            }
		}
		File.WriteAllLines (fielFullPath, lines);
	}

	// 替换文件的匹配的文本
	public static void ReplaceText(string fielFullPath, string oldText, string newText) {
		string[] lines = File.ReadAllLines (fielFullPath);
		for (int i=0; i<lines.Length; i++) {
			if (Regex.IsMatch(lines[i], oldText)) {
				lines[i] = lines[i].Replace(oldText, newText);
			}
		}
		File.WriteAllLines (fielFullPath, lines);
	}


	// 正则表达式替换文件的行
	public static void ReplaceTextWithRegex(string fielFullPath, string regexString, string replaceString) {
		string[] lines = File.ReadAllLines (fielFullPath);
		for (int i=0; i<lines.Length; i++) {
			if (Regex.IsMatch(lines[i], regexString)) {
				lines[i] = replaceString;
			}
		}
		File.WriteAllLines (fielFullPath, lines);
	}

	// 正则表达式批量替换文件的行
	public static void ReplaceTextWithRegex(string fielFullPath, Dictionary<string , string> regexRules) {
		string[] lines = File.ReadAllLines (fielFullPath);
		for (int i=0; i<lines.Length; i++) {
			foreach(KeyValuePair<string, string> rule in regexRules) {
				if (Regex.IsMatch(lines[i], rule.Key)) {
					lines[i] = rule.Value;
				}
			}
		}
		File.WriteAllLines (fielFullPath, lines);
	}
    #endregion

    #region 文件操作
    // 替换文件夹，若目标目录已存在此文件夹则删除后复制
	public static void ReplaceDir(string srcPath, string destPath) {
		if (Directory.Exists(destPath)) {
			try {
				Directory.Delete(destPath, true);
			} catch(IOException e) {
				Debug.LogException(e);
				return;
			}
		}
		CopyDir (srcPath, destPath, true);
	}

	// 复制文件夹，包括子目录和子文件，cover为true时覆盖已有文件(不包括meta文件)
	public static void CopyDir(string srcPath, string destPath, bool cover) {
		if (Directory.Exists (srcPath)) {
			if (!Directory.Exists(destPath)) {
				Directory.CreateDirectory(destPath);
			}
			string[] files = Directory.GetFiles(srcPath);
			foreach(string file in files) {
				string fileName = Path.GetFileName(file);
				string destFile = Path.Combine(destPath, fileName);
				string extention = Path.GetExtension(file);
				if (extention != ".meta") {
					try {
						File.Copy(file, destFile, cover);
					} catch (IOException e) {
						Debug.Log (e.Message);
					}
				}
			}
			string[] dirs = Directory.GetDirectories(srcPath);
			foreach(string dir in dirs) {
				string dirName = Path.GetFileName(dir);
                if (".svn".Equals(dirName)) {
                    continue;
                }
				string destDir = Path.Combine(destPath, dirName);
				CopyDir(dir, destDir, cover);
			}
		}
	}

	//复制文件. 若目标已存在, duplicate为true时则创建副本, 否则不进行复制. 返回目标文件路径.
	public static string CopyFile(string srcFile, string destFile, bool duplicate) {
		string fileName = Path.GetFileName(destFile);
		if (File.Exists(destFile)) {
			if (duplicate) {
				string dirPath = Path.GetDirectoryName(destFile);
				fileName = "Copy_" + fileName;
				destFile = Path.Combine(dirPath, fileName);
			} else {
				return destFile;
			}
		}
		File.Copy (srcFile, destFile, true);
		return destFile;
	}

	// 计算文件MD5
	public static string GetFileMd5(string filePath) {
		try
		{
			FileStream file = new FileStream(filePath, FileMode.Open);
			System.Security.Cryptography.MD5 md5 = new System.Security.Cryptography.MD5CryptoServiceProvider();
			byte[] retVal = md5.ComputeHash(file);
			file.Close();

			StringBuilder sb = new StringBuilder();
			for (int i = 0; i < retVal.Length; i++)
			{
				sb.Append(retVal[i].ToString("x2"));
			}
			return sb.ToString();
		}
		catch(Exception e)
		{
            Debug.LogException(e);
			return null;
		}
    }
    #endregion

    #region Xml操作
    // 节点或其子节点是否包含属性
    public static bool XmlInclue(XmlNode parent, string attributes, string value = "", bool recursivity = false)
    {
        if (parent == null || parent.Attributes == null) {
            return false;
        }
        XmlAttribute attr = parent.Attributes[attributes];
        if (attr != null) {
            if (String.IsNullOrEmpty(value) || attr.Value.Equals(value)) {
                return true;
            }
        }
        if (!recursivity) {
            return false;
        }
        if (!parent.HasChildNodes) {
            return false;
        }
        XmlNodeList childs = parent.ChildNodes;
        foreach (XmlNode child in childs) {
            if (XmlInclue(child, attributes, value, recursivity)) {
                return true;
            }
        }
        return false;
    }


    // 字点中包含子节点 <keyType>key</keyType>，且此子节点下一节点为 <valueType>value</valueType>
    public static bool XmlMath(XmlNode parent, string keyType, string key, string valueType, string value = "")
    {
        if (parent == null) {
            return false;
        }
        XmlNode keyChild = parent.SelectSingleNode(keyType + "[. = '" + key + "']");
        if (keyChild == null || keyChild.NextSibling == null) {
            return false;
        }
        if (!keyChild.NextSibling.Name.Equals(valueType)) {
            return false;
        }
        if (String.IsNullOrEmpty(value)) {
            return true;
        }
        if (keyChild.NextSibling.InnerText.Equals(value)) {
            return true;
        } else {
            return false;
        }
    }

    // 读取父节点某子节点下一节点的节点名
    public static string XmlNextName(XmlNode parent, string keyType, string key)
    {
        if (parent == null) {
            return "";
        }
        XmlNode keyChild = parent.SelectSingleNode(keyType + "[. = '" + key + "']");
        if (keyChild == null || keyChild.NextSibling == null) {
            return "";
        }
        return keyChild.NextSibling.Name;
    }

    // 读取父节点某子节点下一节点的值
    public static string XmlNextValue(XmlNode parent, string keyType, string key, string valueType)
    {
        if (parent == null) {
            return null;
        }
        XmlNode keyChild = parent.SelectSingleNode(keyType + "[. = '" + key + "']");
        if (keyChild == null || keyChild.NextSibling == null) {
            return null;
        }
        if (!keyChild.NextSibling.Name.Equals(valueType)) {
            return null;
        }
        return keyChild.NextSibling.InnerText;
    }


    #endregion
}
