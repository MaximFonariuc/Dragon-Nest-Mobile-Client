using System.Collections;
using System.Xml;
using UnityEditor;
using UnityEngine;

class ExportServerWayPoint
{
    static string serverWayPointDir = System.Environment.GetEnvironmentVariable(Application.unityVersion.StartsWith("5") ? "U5XResourcePath" : "XResourcePath") + "/XProject/Assets/Resources/Table/WayPoint/";
    static int MAX_WAY_POINT = 1000;

    [MenuItem("Tools/ExportServerWayPoint")]
    public static void DoExportWayPoint()
    {
        GameObject [] objects = GameObject.FindGameObjectsWithTag("LevelDynamic");

        for (int i=0; i<objects.Length; i++)
        {
            string filename = objects[i].name;
            string xmlpath = serverWayPointDir + filename + ".xml";

            XmlDocument xmlDoc = new XmlDocument();
            XmlNode xmlDec = xmlDoc.CreateXmlDeclaration("1.0", "utf-8", "yes");
            xmlDoc.AppendChild(xmlDec);
            XmlNode rootNode = xmlDoc.CreateElement("WayPoint");
            //ArrayList nodeList = new ArrayList();

            Transform servernav = objects[i].transform.Find("servernav");

            if (servernav == null)
            {
                Debug.LogError("Export level error: " + filename);
                continue;
            }

            for (int j = 0; j < MAX_WAY_POINT; j++)
            {
                Transform pathTrans = servernav.Find("path" + j.ToString());

                if (pathTrans == null)
                    break;

                XmlNode subNode = xmlDoc.CreateElement("point");

                XmlAttribute attr = xmlDoc.CreateAttribute("index");
                attr.Value = j.ToString();
                ((XmlElement)subNode).SetAttributeNode(attr);

                attr = xmlDoc.CreateAttribute("pos");
                attr.Value = string.Format("{0}:{1}:{2}", pathTrans.position.x, pathTrans.position.y, pathTrans.position.z);
                ((XmlElement)subNode).SetAttributeNode(attr);
                
                rootNode.AppendChild(subNode);
            }




            xmlDoc.AppendChild(rootNode);
            xmlDoc.Save(xmlpath);
        }

        EditorUtility.DisplayDialog("Export Server Way Point", "Export successfully!", "OK");
    }
}