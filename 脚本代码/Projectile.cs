using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour {
    System.Action<Enemy> onAttack;  //当攻击到目标
    Transform m_target;  //目标对象
    Bounds m_targetCenter;  //目标对象模型的边框

    public static void Create(Transform target,Vector3 spawnPos,System.Action<Enemy> onAttack)  //静态函数，创建实体
    {
        GameObject prefab = Resources.Load<GameObject>("arrow");
        GameObject go = (GameObject)Instantiate(prefab, spawnPos, Quaternion.LookRotation(target.position - spawnPos));

        Projectile arrowmodel = go.AddComponent<Projectile>();  //添加弓箭组件
        arrowmodel.m_target = target;   //设置弓箭目标
        arrowmodel.m_targetCenter = target.GetComponentInChildren<SkinnedMeshRenderer>().bounds;  //获得模型边框
        arrowmodel.onAttack = onAttack;  //获得Action
        Destroy(go, 3.0f);  //3秒后销毁
    }
	
	// Update is called once per frame
	void Update () {
        if (m_target != null)
            this.transform.LookAt(m_targetCenter.center);   //瞄准目标中心
        this.transform.Translate(new Vector3(0, 0, 10 * Time.deltaTime));   //向着目标前进
        if(m_target!=null)
        {
            if(Vector3.Distance(this.transform.position,m_targetCenter.center)<0.5f)   //检测是否打击到目标
            {
                onAttack(m_target.GetComponent<Enemy>());  //攻击发射者
                Destroy(this.gameObject);  //销毁自己
            }
        }
	}
}
