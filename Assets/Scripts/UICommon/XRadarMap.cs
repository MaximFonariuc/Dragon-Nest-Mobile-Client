using UnityEngine;
using System.Collections;
using UILib;

public class XRadarMap : MonoBehaviour,IXRadarMap {

    // Use this for initialization
    public Vector3[] m_vertices;
    public Vector2[] m_uv;
    public Color[] m_color;
    public Vector3[] m_normals;
    public int[] m_triangles;
    private Mesh m_mesh;
    private MeshFilter m_meshFilter;
    private bool m_reposition = true;
    //private float m_verticeValue = 1.0f;
    void Awake()
    {
        m_mesh = new Mesh();
        m_meshFilter = GetComponent<MeshFilter>();
        m_vertices = new Vector3[4]
        {
            new Vector3(0,0,0),
            new Vector3(1,0,0),
            new Vector3(0,1,0),
            new Vector3(1,1,0)
        };
        m_uv = new Vector2[4]
        {
            new Vector2(0,0),
            new Vector2(1,0),
            new Vector2(0,1),
            new Vector2(1,1)
        };

        m_triangles = new int[6] // 两个三角面的连接
        {
            0,1,2,// 通过顶点012连接形成的三角面
            1,3,2,// 通过顶点132连接形成的三角面
        };
    }



    public void SetSite(int pos , float value )
    {
        switch (pos)
        {
            case 2:
                SetLeftSite(value);
                break;
            case 1:
                SetRightSite(value);
                break;
            case 3:
                SetUpSite(value);
                break;
            case 0:
                SetBottomSite(value);
                break;
        }
        repositionNow = true;
    }

    private void SetLeftSite(float value)
    {
        value *= 0.25f;
        m_vertices[2].y = 0.75f+value;
        m_vertices[2].x = 0.25f-value;
    }

    /// <summary>
    /// 下
    /// </summary>
    /// <param name="value"></param>
    private void SetBottomSite(float value)
    {
        value *= 0.25f;
        m_vertices[0].x = 0.25f - value;
        m_vertices[0].y = 0.25f - value;
    }

    private void SetRightSite(float value)
    {
        value *= 0.25f;
        m_vertices[1].x = 0.75f + value;
        m_vertices[1].y = 0.25f - value;
    }
    /// <summary>
    /// 上边
    /// </summary>
    /// <param name="value"></param>
    private void SetUpSite(float value)
    {
        value *= 0.25f;
        m_vertices[3].x = 0.75f + value; ;
        m_vertices[3].y = 0.75f + value;
    }
    public bool repositionNow
    {
        set { m_reposition = value; }
    }


    void Update()
    {
        if (!m_reposition) return;
        Reposition();
  
    }

    // Update is called once per frame
        [ContextMenu("Execute")]
    private void  Reposition()
    {

        m_reposition = false;

        m_mesh.Clear();


        m_mesh.vertices = m_vertices;


        m_mesh.uv = m_uv;


        m_mesh.colors = m_color;


        m_mesh.normals = m_normals;


        m_mesh.triangles = m_triangles;


        m_mesh.RecalculateNormals();
        m_meshFilter.mesh = m_mesh;
    }

    public void Refresh()
    {
        repositionNow = true;
    }
}
