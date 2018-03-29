using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameCamera : MonoBehaviour {
    public static GameCamera Inst = null;   //静态实例
    protected float m_distance = 14;    //摄像机距离地面的距离
    protected Vector3 m_rot = new Vector3(-55, 90, 0);   //摄像机的角度
    protected float m_moveSpeed = 60;   //摄像机移动速度
    protected float m_vx = 0;    
    protected float m_vy = 0;    //摄像机移动数值

    protected Transform m_transform;     //transform组件
    protected Transform m_cameraPoint;   //摄像机焦点

   

    void Awake()
    {
        Inst = this;
        m_transform = this.transform;
    }
	// Use this for initialization
	void Start () {
        m_cameraPoint = CameraPoint.Instance.transform;   //获取摄像机焦点
        Follow();
	}
	
	// Update is called once per frame
	void LateUpdate () {      //LateUpdate是在update之后执行,确保所有操作完成后移动摄像机
        Follow();
	}

    void Follow()
    {
        m_cameraPoint.eulerAngles = m_rot;   //设置旋转角度
        m_transform.position=m_cameraPoint.TransformPoint(new Vector3(0,0,m_distance));  //将摄像机移动至指定位置
        transform.LookAt(m_cameraPoint);   //将摄像机镜头对准目标点
    }

    public void Control(bool mouse,float mx,float my)    //控制摄像机移动
    {
        if (!mouse)
            return;
        m_cameraPoint.eulerAngles = Vector3.zero; 
        m_cameraPoint.Translate(-mx, 0, -my);     //平移摄像机目标点
    }
}
