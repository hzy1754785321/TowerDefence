using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathNode : MonoBehaviour {
    public PathNode m_parenet;
    public PathNode m_next;
	
    public void SetNext(PathNode node)     //设置节点
    {
        if(m_next!=null)
            m_next.m_parenet = null;
        m_next = node;
        node.m_parenet = this;
    }

    void OnDrawGizmos()
    {
        Gizmos.DrawIcon(this.transform.position, "Node.tif");
    }
}
