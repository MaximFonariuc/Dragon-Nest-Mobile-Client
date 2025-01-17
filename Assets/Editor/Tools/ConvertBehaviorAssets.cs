using System.IO;
using System.Collections.Generic;
using System.Text;
using UnityEditor;
using UnityEngine;
using XEditor;
using XUtliPoolLib;
using System.Xml;
using System.Collections;


public class ConvertBehaviorAssets 
{
    enum PARSE_STEP
    {
        Begin,
        ParsingClass,
        ParsingData,
        End
    }

    class NodeData
    {
        public int nodeindex;
        public string nodename;
        public List<NodeData> child = new List<NodeData>();
    }

    static string _behavior_folder_name = "Assets/Behavior Designer/Behavior Data";

    [MenuItem("Tools/ConvertBehaviorAssetsDebug")]
    private static void DoConvertAssetsToXMLDebug()
    {
        bIsDebug = true;
        DoConvertAssetsToXML();
        bIsDebug = false;
    }

    static bool bIsDebug = false;
    [MenuItem("Tools/ConvertBehaviorAssets")]
    private static void DoConvertAssetsToXML()
    {
        Debug.Log("Start Convert AssetsToXML");
        System.Threading.Thread.CurrentThread.CurrentCulture = System.Globalization.CultureInfo.InvariantCulture;

        DirectoryInfo TheFolder = new DirectoryInfo(_behavior_folder_name);

        foreach (FileInfo NextFile in TheFolder.GetFiles())
        {
            if (NextFile.FullName.Contains(".asset") && !NextFile.FullName.Contains(".meta"))
            {
			
				/*
				if(!NextFile.FullName.Contains("Global"))
				{
					continue;
				}
				*/

				ConvertFileToXML(NextFile.FullName);
				//break;
			}
        }

        EditorUtility.DisplayDialog("Export Behavior Tree", "Export successfully!", "OK");
    }

    private static void BuildTestNodeData(NodeData root)
    {
        root.nodename = "BehaviorName";

        for (int i=0; i<10; i++)
        {
            NodeData childNode = new NodeData();
            childNode.nodeindex = i;
            childNode.nodename = "Node" + i.ToString();
            root.child.Add(childNode);

            for (int j=0; j<5; j++)
            {
                NodeData subNode = new NodeData();
                subNode.nodeindex = i * 10 + j;
                subNode.nodename = "subNode" + j.ToString();
                childNode.child.Add(subNode);
            }
        }
    }

    private static void ConvertFileToXML(string filepath)
    {
        //PARSE_STEP step = PARSE_STEP.Begin;


        Debug.Log("FileName:" + filepath);
        StreamReader sr = new StreamReader(filepath, Encoding.Default);
        string line;
		
		
		bool isnode = false; //节点 范围标志
		bool ispara = false; //参数 范围标志
		
		XmlDocument xmlDoc = new XmlDocument();
		XmlNode xmlDec = xmlDoc.CreateXmlDeclaration("1.0", "utf-8", "yes");
		xmlDoc.AppendChild (xmlDec);
	
		string[] rns = filepath.Split(new char[]{'\\','.'});
		if (rns.Length < 2) 
		{
			Debug.Log("filepath error: " + filepath);
			return ;
		}
		XmlNode rootNode = xmlDoc.CreateElement(rns[rns.Length - 2]);//文件名就是根节点名称
		ArrayList nodeList = new ArrayList();

		ArrayList paraList = new ArrayList ();
		ArrayList dataPosList = new ArrayList ();
		ArrayList disabledList = new ArrayList ();



        while ((line = sr.ReadLine()) != null)
        {
          	if(line.Contains("types:"))
			{
				isnode = true; //节点 开始
			}else if(isnode && line.Contains("-"))
			{
				string[] nnss = line.Replace("-","").Trim().Split('.');
				if(nnss.Length >= 1)
				{
					XmlNode subNode = xmlDoc.CreateElement(nnss[nnss.Length-1]);
					nodeList.Add(subNode);//存储  节点
				}
				
				
			}
			else if(line.Contains("parentIndex:"))
			{
				//确认节点 关系
				string[] strs = line.Split(':');
				string preIndexStr = strs[strs.Length-1].Trim();
				if(preIndexStr.Length != nodeList.Count * 8)
				{
					Debug.Log("DATA ERROR");
					break;
				}
		
				for(int i = 0 , index = 0; i + 8 <= preIndexStr.Length && index < nodeList.Count; i = i + 8 , index ++)
				{
					string s = preIndexStr.Substring(i,8);
					int pre = ConvertStrsToInt(s);
				
					if(0 == index)
					{
						if(null != rootNode)
						{
							rootNode.AppendChild((XmlNode)nodeList[index]);
						}

					}else if(pre < nodeList.Count && pre >= 0)
					{
						XmlNode tmp = (XmlNode)nodeList[pre];
						tmp.AppendChild((XmlNode)nodeList[index]);
					}
					//Debug.Log("num str: " + s + " " + pre);
				}

			

				isnode = false; //节点结束

				
			}else if(line.Contains("typeName:"))
			{
				ispara = true;//参数 开始
			}else if(ispara && line.Contains("-") && dataPosList.Count == 0)
			{
				paraList.Add(line.Replace("-","").Trim()); //参数 说明

			}else if(ispara && line.Contains("dataPosition:"))
			{
				//参数位置
				string str = line.Replace("dataPosition:","").Trim();
				//Debug.Log("dataPositing len = " + str.Length);
				for(int i = 0; i + 8 <= str.Length; i = i + 8)
				{
					dataPosList.Add(ConvertStrsToInt(str.Substring(i,8))*2);
				}
			}else if(ispara && line.Contains("byteData:"))
			{
				string str = line.Replace("byteData:","").Trim();
				 
				if(dataPosList.Count <= paraList.Count)
				{
					int nodeDataIndex = -1;
					string sharedType = "";
					string sharedName = "";
					string prefix = "";
					for(int i = 0 , j = 0; i < paraList.Count && j < dataPosList.Count; ++i , ++j)
					{
						string tmpstr = (string)paraList[i];
						if(tmpstr.Contains("Int32ID"))
						{
							++ nodeDataIndex;
						}

						 
						if((tmpstr.Contains("SharedTransform") && tmpstr.Substring(0,15) == "SharedTransform")
						   ||(tmpstr.Contains("SharedFloat") && tmpstr.Substring(0,11) == "SharedFloat")
						   ||(tmpstr.Contains("SharedInt") && tmpstr.Substring(0,9) == "SharedInt")
						   ||(tmpstr.Contains("SharedString") && tmpstr.Substring(0,12) == "SharedString")
						   ||(tmpstr.Contains("SharedBool") && tmpstr.Substring(0,10) == "SharedBool")
						   ||(tmpstr.Contains("SharedVector3")&&tmpstr.Substring(0,13)=="SharedVector3"))
						{
							 
							--j;
							continue;
						}
						 
						int l = (int)dataPosList[j];
						 
						int r = l;
						if( i + 1 == paraList.Count)
						{
							r = str.Length;
						}else if(j + 1 < dataPosList.Count)
						{
							r = (int)dataPosList[j + 1];
						}
						if(r >= l)
						{
							string strvalue = str.Substring(l,r-l);
							string sstmp = ""; //true value

							
							if(tmpstr.Contains("String"))
							{
								sstmp = ConvertStrToStr(strvalue);
							}else if(tmpstr.Contains("Int32"))
							{
								sstmp = System.Convert.ToInt32(strvalue,16).ToString();
							}else if(tmpstr.Contains("Boolean"))
							{

								sstmp =  System.Convert.ToInt32(strvalue,16).ToString();
							}else if(tmpstr.Contains("Single"))
							{
								float ftmp = ConvertStrToFloat(strvalue);
								sstmp = ftmp.ToString();
								 
							}else if(tmpstr.Contains("Vector3"))
							{
								 
								if(strvalue.Length == 24)
								{
									sstmp = ConvertStrToVecFloat(strvalue.Substring(0,8)).ToString();
									sstmp = sstmp + ":" + ConvertStrToVecFloat(strvalue.Substring(8,8)).ToString();
									sstmp = sstmp + ":" + ConvertStrToVecFloat(strvalue.Substring(16,8)).ToString();
								}
								 
							}else
							{
								if("ConditionalconditionalTask" == tmpstr)
								{
									sstmp = ConvertStrToStr(strvalue);
								}
								else if(strvalue.Length == 8)//枚举 （暂时 需修改）
								{
									sstmp = ConvertStrsToEnumInt(strvalue).ToString();
								}
							}
							  
						
							if(-1 == nodeDataIndex)//全局变量
							{
								 
								if(tmpstr.Contains("Type"))
								{
									sharedType = sstmp;


									//加前缀方便C++读取
									if(sstmp.Contains("SharedTransform"))
									{
										prefix = "T_";
									}else if(sstmp.Contains("SharedFloat"))
									{
										prefix = "F_";
									}else if(sstmp.Contains("SharedBool"))
									{
										prefix = "B_";
									}else if(sstmp.Contains("SharedInt"))
									{
										prefix = "I_";
									}else if(sstmp.Contains("SharedString"))
									{
										prefix = "S_";
									}else if(sstmp.Contains("SharedVector3"))
									{
										prefix = "V_";
									}
									else
									{
										prefix = "";
									}
								 

								}else if(tmpstr.Contains("Name"))
								{
									if(sharedType.Contains("Transform"))
									{
										 
										XmlAttribute attr = xmlDoc.CreateAttribute(prefix + sstmp);
										attr.Value = "";
										((XmlElement)nodeList[0]).SetAttributeNode(attr);
										sharedType = "";
									}else
									{
										sharedName = sstmp;
									}
								}else if(tmpstr.Contains("IsShared"))
								{
								}else if(tmpstr.Contains("mValue"))
								{
									if(sharedName != "")
									{
										XmlAttribute attr = xmlDoc.CreateAttribute(prefix + sharedName);
										attr.Value = sstmp;
										((XmlElement)nodeList[0]).SetAttributeNode(attr);
									}
									sharedName = "";
								}
								continue;
							}

							if("BooleanNodeDataDisabled" == tmpstr)
							{
								disabledList.Add(sstmp);
							}
							 
							//Debug.Log( "  " + i + " : " + tmpstr + " " + strvalue + " " + sstmp); 
							if(tmpstr.Contains("mAIArg") && !(tmpstr.EndsWith("Type")&&tmpstr.Contains("String")) && !tmpstr.Contains("IsShared") && !(tmpstr.Contains("Value")&&!tmpstr.Contains("Shared")))
							{
								 
								if(nodeDataIndex < nodeList.Count && nodeDataIndex >=0 )
								{

									string[] ts = tmpstr.Replace("mAIArg",".").Split('.');
									XmlNode tmp = (XmlNode)nodeList[nodeDataIndex];

									if(tmpstr.Contains("Shared"))
									{
										ts[ts.Length-1] = "Shared_" + ts[ts.Length-1];
									}
									XmlAttribute attr = xmlDoc.CreateAttribute(ts[ts.Length-1]);
									attr.Value = sstmp;

									((XmlElement)tmp).SetAttributeNode(attr);
								}else
								{
									Debug.Log("ERROR : nodeindex = " + nodeDataIndex + "   " + "para = " +tmpstr); 
								}
							}
                            else if (tmpstr == ("BooleanNodeDataIsBreakpoint"))
                            {
                                if (bIsDebug && "1" == sstmp && nodeDataIndex < nodeList.Count && nodeDataIndex >= 0)
                                {
                                    XmlNode tmp = (XmlNode)nodeList[nodeDataIndex];
                                    XmlAttribute attr = xmlDoc.CreateAttribute("IsBreakPoint");
                                    attr.Value = sstmp;
                                    ((XmlElement)tmp).SetAttributeNode(attr);
                                }
                            }
                            else if (tmpstr == ("StringFriendlyName"))
                            {
                                if (bIsDebug && "" != sstmp && "Entry" != sstmp && nodeDataIndex < nodeList.Count && nodeDataIndex >= 0)
                                {
                                    sstmp = sstmp.Replace(" ", "");
                                    XmlNode tmp = (XmlNode)nodeList[nodeDataIndex];
                                    if (tmp.Name != sstmp)
                                    {
                                        XmlAttribute attr = xmlDoc.CreateAttribute("FriendlyName");
                                        attr.Value = sstmp;
                                        ((XmlElement)tmp).SetAttributeNode(attr);
                                    }
                                }
                            }
                            else
							{
								XmlElement tmp = (XmlElement)nodeList[nodeDataIndex];
							 
								string speName = tmp.Name;

								if("ConditionalEvaluator" == tmp.Name)
								{

									if("ConditionalconditionalTask" == tmpstr)
									{
										string[] ts = sstmp.Split('.');
										XmlAttribute attr = xmlDoc.CreateAttribute("ConditionalTask");
										attr.Value = ts[ts.Length-1];
										tmp.SetAttributeNode(attr);

									}else if(tmpstr.Contains("ConditionalconditionalTask"))
									{
										tmpstr = tmpstr.Replace("ConditionalconditionalTask","");
										string constr = tmp.GetAttributeNode("ConditionalTask").Value;
										speName = constr;
									}
								}

								//系统的node
								//Debug.Log( "  " + i + " : " + tmpstr + " " + strvalue + " " + sstmp); 
								if("RandomFloat" == speName)
								{
									ParseRandomFloat(tmp,xmlDoc,tmpstr,sstmp);
								}else if("FloatOperator" == speName)
								{
									ParseFloatOperator(tmp,xmlDoc,tmpstr,sstmp);
								}else if("FloatComparison" == speName)
								{
									ParseFloatComparison(tmp,xmlDoc,tmpstr,sstmp);
								}else if("SetFloat" == speName)
								{
									ParseSetFloat(tmp,xmlDoc,tmpstr,sstmp);
								}else if("BoolComparison" == speName)
								{
									ParseBoolComparison(tmp,xmlDoc,tmpstr,sstmp);
								}else if("SetBool" == speName)
								{ 
									ParseSetBool(tmp,xmlDoc,tmpstr,sstmp);

								}else if("CompareTo" == speName)
								{
									ParseCompareTo(tmp,xmlDoc,tmpstr,sstmp);
								}else if("SetInt" == speName)
								{
									ParseSetInt(tmp,xmlDoc,tmpstr,sstmp);
								}else if("IntOperator" == speName)
								{
									ParseIntOperator(tmp,xmlDoc,tmpstr,sstmp);
								}else if("IntComparison" == speName)
								{
									ParseIntComparison(tmp,xmlDoc,tmpstr,sstmp);
								}else if ("GetRealtimeSinceStartup" == speName)
                                {
                                    ParseRealtimeSinceStartup(tmp, xmlDoc, tmpstr, sstmp);
                                }
							}

						}
					}
				}else
				{
					Debug.Log("ERROR para num = " + paraList.Count + " pos num = " + dataPosList.Count);
					Debug.Log("ERROR : " + paraList[0] + " " + paraList[paraList.Count - 1]);
				}
				ispara = false;//参数结束

			}


        }



		//disable调试
		if (disabledList.Count == nodeList.Count) {
			for (int i = disabledList.Count-1; i >= 0 ; --i) {
				if("1" == disabledList[i].ToString().Trim())
				{
					XmlElement tmp = (XmlElement)nodeList[i];
					XmlNode parent = tmp.ParentNode;
                    if (parent != null)
                        parent.RemoveChild(tmp);
				}

			}
		}

		nodeList.Clear();
		paraList.Clear();
		dataPosList.Clear ();
		disabledList.Clear ();


		xmlDoc.AppendChild(rootNode);
		/*
		string serverpath = System.Environment.GetEnvironmentVariable ("XProjectPath") + "/server/exe/gsconf/serveronly/AITree/";
		Debug.Log ("servercode path = " + serverpath);
		if(Directory.Exists(serverpath))
		{
			xmlDoc.Save(serverpath + rootNode.Name + ".xml");
		}

		string restpath = System.Environment.GetEnvironmentVariable ("XResourcePath") + "/server/exe/gsconf/serveronly/AITree/";
		if(Directory.Exists(restpath))
		{
			xmlDoc.Save(restpath + rootNode.Name + ".xml");
		}
		*/
        //string sss = Application.unityVersion;
        //string restpath = 
        //    System.Environment.GetEnvironmentVariable(Application.unityVersion.StartsWith("5") ? "U5XResourcePath" : "XResourcePath")
        //    + "/XProject/Assets/Resources/Table/AITree/";
        string restpath = "Assets/Resources/Table/AITree/";
        if (Directory.Exists(restpath))
		{
			xmlDoc.Save(restpath + rootNode.Name + ".xml");
		}

        //string clientPath =
        //    System.Environment.GetEnvironmentVariable(Application.unityVersion.StartsWith("5") ? "U5XResourcePath" : "XResourcePath")
        //    + "/XProject/Assets/Resources/Behavior/Runtime/";
        //if (Directory.Exists(clientPath))
        //{
        //    xmlDoc.Save(clientPath + rootNode.Name + ".xml");
        //}
      
    }

	private static void ParseIntComparison(XmlElement elem,XmlDocument xmlDoc,string tmpstr,string sstmp)
	{
		string attrName = "";

		if ("StringSharedIntinteger1Name" == tmpstr) {
			attrName = "Shared_Int1Name";
			
		} else if ("Int32SharedIntinteger1mValue" == tmpstr) {
			attrName = "int1Value";
			
		} else if ("StringSharedIntinteger2Name" == tmpstr) {
			attrName = "Shared_Int2Name";
			
		} else if ("Int32SharedIntinteger2mValue" == tmpstr) {
			attrName = "int2Value";
			
		} else if ("Operationoperation" == tmpstr) 
		{
			attrName = "type";
		}
		if(attrName != "")
		{
			XmlAttribute attr = xmlDoc.CreateAttribute(attrName);
			attr.Value = sstmp;
			elem.SetAttributeNode(attr);
		}
	}

    private static void ParseRealtimeSinceStartup(XmlElement elem, XmlDocument xmlDoc, string tmpstr, string sstmp)
    {
        string attrName = "";
        if ("StringSharedFloatstoreResultName" == tmpstr)
        {
            attrName = "Shared_FloatstoreResultName";
        }
        else if ("SingleSharedFloatstoreResultmValue" == tmpstr)
        {
            attrName = "Shared_FloatstoreResultmValue";
        }

        if (!string.IsNullOrEmpty(attrName))
        {
            XmlAttribute attr = xmlDoc.CreateAttribute(attrName);
            attr.Value = sstmp;
            elem.SetAttributeNode(attr);
        }
    }


    private static void ParseIntOperator(XmlElement elem,XmlDocument xmlDoc,string tmpstr,string sstmp)
	{
		string attrName = "";
		//Debug.Log ("Parse IntOperator : " + tmpstr + " " + sstmp);
		if ("StringSharedIntinteger1Name" == tmpstr) {
			attrName = "Shared_Int1Name";
			
		} else if ("Int32SharedIntinteger1mValue" == tmpstr) {
			attrName = "int1Value";
			
		} else if ("StringSharedIntinteger2Name" == tmpstr) {
			attrName = "Shared_Int2Name";
			
		} else if ("Int32SharedIntinteger2mValue" == tmpstr) {
			attrName = "int2Value";
			
		} else if ("StringSharedIntstoreResultName" == tmpstr) {
			attrName = "Shared_StoredResultName";
			
		}else if ("Operationoperation" == tmpstr) 
		{
			attrName = "type";
		}
		if(attrName != "")
		{
			XmlAttribute attr = xmlDoc.CreateAttribute(attrName);
			attr.Value = sstmp;
			elem.SetAttributeNode(attr);
		}
	}

	private static void ParseSetInt(XmlElement elem,XmlDocument xmlDoc,string tmpstr,string sstmp)
	{
		//Debug.Log ("Parse SetInt : " + tmpstr + " " + sstmp);
		string attrName = "";
		if ("StringSharedIntintValueName" == tmpstr) {
			attrName = "Shared_ValueName";
			
		} else if ("Int32SharedIntintValuemValue" == tmpstr) {
			attrName = "value";
		} else if ("StringSharedIntstoreResultName" == tmpstr) {
			attrName = "Shared_StoredResultName";
			
		}
		if(attrName != "")
		{
			XmlAttribute attr = xmlDoc.CreateAttribute(attrName);
			attr.Value = sstmp;
			elem.SetAttributeNode(attr);
		}
	}

	private static void ParseCompareTo(XmlElement elem,XmlDocument xmlDoc,string tmpstr,string sstmp)
	{
		string attrName = "";
		//Debug.Log ("parse CompareTo : " + tmpstr + " " + sstmp);
		if ("StringSharedStringfirstStringName" == tmpstr) {
			attrName = "Shared_FirstStringName";
			
		} else if ("StringSharedStringfirstStringmValue" == tmpstr) {
			attrName = "firstString";
			
		} else if ("StringSharedStringsecondStringName" == tmpstr) {
			attrName = "Shared_SecondStringName";
			
		} else if ("StringSharedStringsecondStringmValue" == tmpstr) {
			attrName = "secondString";
			
		} else if ("StringSharedIntstoreResultName" == tmpstr) 
		{
			attrName = "Shared_ResultName";
		}
		if(attrName != "")
		{
			XmlAttribute attr = xmlDoc.CreateAttribute(attrName);
			attr.Value = sstmp;
			elem.SetAttributeNode(attr);
		}
	}

	private static void ParseSetBool(XmlElement elem,XmlDocument xmlDoc,string tmpstr,string sstmp)
	{
		//Debug.Log ("parse set bool : " + tmpstr + " " + sstmp);
		string attrName = "";
		if ("StringSharedBoolboolValueName" == tmpstr) {
			attrName = "Shared_ValueName";
		} else if("BooleanSharedBoolboolValuemValue" == tmpstr) {
			attrName = "value";

		} else if ("StringSharedBoolstoreResultName" == tmpstr) {
			attrName = "Shared_StoredResultName";
		}

		if(attrName != "")
		{
			XmlAttribute attr = xmlDoc.CreateAttribute(attrName);
			attr.Value = sstmp;
			elem.SetAttributeNode(attr);
		}
	}

	private static void ParseBoolComparison(XmlElement elem,XmlDocument xmlDoc,string tmpstr,string sstmp)
	{
		string attrName = "";
		if ("StringSharedBoolbool1Name" == tmpstr) {
			attrName = "Shared_Bool1Name";
		}else if ("BooleanSharedBoolbool1mValue" == tmpstr) {
			attrName = "bool1Value";
		}else if ("StringSharedBoolbool2Name" == tmpstr) {
			attrName = "Shared_Bool2Name";
		}else if ("BooleanSharedBoolbool2mValue" == tmpstr) {
			attrName = "bool2Value";
		}

		if("" != attrName)
		{
			XmlAttribute attr = xmlDoc.CreateAttribute(attrName);
			attr.Value = sstmp;
			elem.SetAttributeNode(attr);
		}
	}

	private static void ParseSetFloat(XmlElement elem,XmlDocument xmlDoc,string tmpstr,string sstmp)
	{
		string attrName = "";
		if ("StringSharedFloatfloatValueName" == tmpstr) {
			attrName = "Shared_ValueName";

		} else if ("SingleSharedFloatfloatValuemValue" == tmpstr) {
			attrName = "value";
		} else if ("StringSharedFloatstoreResultName" == tmpstr) {
			attrName = "Shared_StoredResultName";
			
		}
		if(attrName != "")
		{
			XmlAttribute attr = xmlDoc.CreateAttribute(attrName);
			attr.Value = sstmp;
			elem.SetAttributeNode(attr);
		}
	}

	private static void ParseFloatComparison(XmlElement elem,XmlDocument xmlDoc,string tmpstr,string sstmp)
	{
		string attrName = "";
		
		if ("StringSharedFloatfloat1Name" == tmpstr) {
			attrName = "Shared_Float1Name";
			
		} else if ("SingleSharedFloatfloat1mValue" == tmpstr) {
			attrName = "float1Value";
			
		} else if ("StringSharedFloatfloat2Name" == tmpstr) {
			attrName = "Shared_Float2Name";
			
		} else if ("SingleSharedFloatfloat2mValue" == tmpstr) {
			attrName = "float2Value";
			
		} else if ("Operationoperation" == tmpstr) 
		{
			attrName = "type";
		}
		if(attrName != "")
		{
			XmlAttribute attr = xmlDoc.CreateAttribute(attrName);
			attr.Value = sstmp;
			elem.SetAttributeNode(attr);
		}
	}

	private static void ParseFloatOperator(XmlElement elem,XmlDocument xmlDoc,string tmpstr,string sstmp)
	{
		string attrName = "";
		
		if ("StringSharedFloatfloat1Name" == tmpstr) {
			attrName = "Shared_Float1Name";
			
		} else if ("SingleSharedFloatfloat1mValue" == tmpstr) {
			attrName = "float1Value";
			
		} else if ("StringSharedFloatfloat2Name" == tmpstr) {
			attrName = "Shared_Float2Name";
			
		} else if ("SingleSharedFloatfloat2mValue" == tmpstr) {
			attrName = "float2Value";
			
		} else if ("StringSharedFloatstoreResultName" == tmpstr) {
			attrName = "Shared_StoredResultName";
			
		}else if ("Operationoperation" == tmpstr) 
		{
			attrName = "type";
		}
		if(attrName != "")
		{
			XmlAttribute attr = xmlDoc.CreateAttribute(attrName);
			attr.Value = sstmp;
			elem.SetAttributeNode(attr);
		}
	}

	private static void ParseRandomFloat(XmlElement elem,XmlDocument xmlDoc,string tmpstr,string sstmp)
	{
		string attrName = "";

		if ("StringSharedFloatminName" == tmpstr) {
			attrName = "Shared_MinName";

		} else if ("SingleSharedFloatminmValue" == tmpstr) {
			attrName = "minValue";

		} else if ("StringSharedFloatmaxName" == tmpstr) {
			attrName = "Shared_MaxName";

		} else if ("SingleSharedFloatmaxmValue" == tmpstr) {
			attrName = "maxValue";

		} else if ("StringSharedFloatstoreResultName" == tmpstr) {
			attrName = "Shared_StoredResultName";

		}else if ("Booleaninclusive" == tmpstr) 
		{
			attrName = "inclusive";
		}
		if(attrName != "")
		{
			XmlAttribute attr = xmlDoc.CreateAttribute(attrName);
			attr.Value = sstmp;
			elem.SetAttributeNode(attr);
		}
	}

	private static string ConvertStrToStr(string strvaule)
	{
		string sstmp = "";
		for(int index = 0; index + 2 <= strvaule.Length; index = index + 2)
		{
			int ascillint = ConvertStrsToInt(strvaule.Substring(index, 2));
			sstmp = sstmp + System.Convert.ToChar(ascillint).ToString();
		}
		return sstmp;
	}

	private static float ConvertStrToFloat(string s)
	{
		float tmp = 0;
		byte[] bs = new byte[4];
		for(int j = s.Length-2 , i = 0; i < 4 && j >= 0; j = j - 2 , i++)
		{
			int s16 = System.Convert.ToInt32(s.Substring(j,2),16);
			bs[i] = System.BitConverter.GetBytes(s16)[0];
		}
		tmp = System.BitConverter.ToSingle (bs, 0);
		//Debug.Log ("s = " + s + "float = " + tmp);
		return tmp;
	}

	private static float ConvertStrToVecFloat(string s)
	{
		float tmp = 0;
		byte[] bs = new byte[4];
		for(int j = 0 , i = 0; i < 4 && j < s.Length ; j = j + 2 , i++)
		{
			int s16 = System.Convert.ToInt32(s.Substring(j,2),16);
			bs[i] = System.BitConverter.GetBytes(s16)[0];
		}
		tmp = System.BitConverter.ToSingle (bs, 0);
		//Debug.Log ("s = " + s + "float = " + tmp);
		return tmp;
	}

	private static int ConvertStrsToInt(string s)
	{
		//Debug.Log ("str = " + s);
		int tmp = 0;
		for(int j = s.Length-2; j >= 0; j = j - 2)
		{
			tmp = tmp * 256 + System.Convert.ToInt32(s.Substring(j,2),16);
		}
		return tmp;
	}

	private static int ConvertStrsToEnumInt(string s)
	{
		//Debug.Log ("str = " + s);
		int tmp = 0;
		for(int j = 0; j < s.Length; j = j + 2)
		{
			tmp = tmp * 256 + System.Convert.ToInt32(s.Substring(j,2),16);
		}
		return tmp;
	}

    private static void BuildXmlElement(XmlDocument doc, NodeData data, XmlElement element)
    {
        for (int i=0; i<data.child.Count; i++)
        {
            NodeData childNode = data.child[i];
            XmlElement childElement = doc.CreateElement(childNode.nodename);
            element.AppendChild(childElement);

            BuildXmlElement(doc, childNode, childElement);
        }
    }
}