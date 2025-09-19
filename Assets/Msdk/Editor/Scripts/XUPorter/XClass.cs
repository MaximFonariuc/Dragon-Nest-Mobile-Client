using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace UnityEditor.XCodeEditor
{
	public partial class XClass : System.IDisposable
	{
		
		private string filePath;
		
		public XClass(string fPath)
		{
			filePath = fPath;
			if( !System.IO.File.Exists( filePath ) ) {
				Debug.LogError( filePath +"路径下文件不存在" );
				return;
			}
		}
		
		public void ReplaceLineBelow(string below, string oldString, string newString) {
			string[] lines = File.ReadAllLines (filePath);
			for (int i=0; i<lines.Length; i++) {
				if (lines[i].IndexOf(below) != -1) {
					for (int j=i; j<lines.Length; j++) {
						if (lines[j].IndexOf(oldString) != -1) {
							lines[j] = newString;
							break;
						}
						if (j == lines.Length -1) {
							Debug.LogError(filePath +"中没有找到标志"+oldString);
						}
					}
					break;
				}
				if (i == lines.Length -1) {
					Debug.LogError(filePath +"中没有找到标志"+below);
				}
			}
			File.WriteAllLines (filePath, lines);
		}
		
		public void WriteBelow(string below, string text)
		{
			StreamReader streamReader = new StreamReader(filePath);
			string text_all = streamReader.ReadToEnd();
			streamReader.Close();
			
			int beginIndex = text_all.IndexOf(below);
			if(beginIndex == -1){
				Debug.LogError(filePath +"中没有找到标志"+below);
				return; 
			}
			
			int endIndex = text_all.LastIndexOf("\n", beginIndex + below.Length);

			if (!text_all.Substring (endIndex, text.Length + 2).Contains (text)) {
				text_all = text_all.Substring (0, endIndex) + "\n" + text + "\n" + text_all.Substring (endIndex);
			}
			StreamWriter streamWriter = new StreamWriter(filePath);
			streamWriter.Write(text_all);
			streamWriter.Close();
		}
		
		public void Replace(string below, string newText)
		{
			StreamReader streamReader = new StreamReader(filePath);
			string text_all = streamReader.ReadToEnd();
			streamReader.Close();
			
			int beginIndex = text_all.IndexOf(below);
			if(beginIndex == -1){
				Debug.LogError(filePath +"中没有找到标志"+below);
				return; 
			}
			
			text_all =  text_all.Replace(below,newText);
			StreamWriter streamWriter = new StreamWriter(filePath);
			streamWriter.Write(text_all);
			streamWriter.Close();
			
		}
		
		public void Dispose()
		{
			
		}
	}
}