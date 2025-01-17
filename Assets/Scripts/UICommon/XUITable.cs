using UILib;
using UnityEngine;

public class XUITable : MonoBehaviour, IXUITable
{
    private void Awake()
    {
        m_table = GetComponent<UITable>();

        if (null == m_table)
        {
            Debug.LogError("null == m_table");
        }
    }

    public void RePositionNow()
    {
        m_table.repositionNow = true;
    }

    public void Reposition()
    {
        m_table.Reposition();
    }

    public void RePositionOnlyOneLevel()
    {
        m_table.RepositionOnlyOneLevel();
    }

    private UITable m_table;
}
