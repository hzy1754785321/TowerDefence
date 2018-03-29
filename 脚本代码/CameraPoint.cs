using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraPoint : MonoBehaviour {
    public static CameraPoint Instance = null;
	void Awake()
    {
        Instance = this;
    }

    void OnDrawGizmos()
    {
        Gizmos.DrawIcon(transform.position, "CameraPoint.tif");
    }
}
