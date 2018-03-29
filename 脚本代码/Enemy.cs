using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour {
    public PathNode m_currentNode;   //敌人当前路点
    public int m_life = 15;     //敌人当前生命
    public int m_maxlife = 15;    //敌人最大生命
    public float m_speed = 2;    //敌人速度
    public System.Action<Enemy> onDeath;   //敌人的死亡事件
    Transform m_lifebarObj;    //敌人的UI生命体
    UnityEngine.UI.Slider m_lifebar;   //控制生命体显示

	// Use this for initialization
	void Start () {
        GameManager.Instance.m_EnemyList.Add(this);     //添加至敌人数组

        GameObject prefab = (GameObject)Resources.Load("Canvas3D");   //读取生命条
        m_lifebarObj = ((GameObject)Instantiate(prefab, Vector3.zero, Camera.main.transform.rotation, this.transform)).transform;
        //创建生命条，设当前transform为父物体
       
        if (this.tag.CompareTo("boar")==0)
        {
            m_lifebarObj.localPosition = new Vector3(-25f, 170f, 125f);   //将生命条放在头上
            m_lifebarObj.localScale = new Vector3(1.6f, 1.6f, 1.6f);

        }
        else
        {
            m_lifebarObj.localPosition = new Vector3(0, 2.0f, 0);   //将生命条放在头上
            m_lifebarObj.localScale = new Vector3(0.02f, 0.02f, 0.02f);
        }
        m_lifebar = m_lifebarObj.GetComponentInChildren<UnityEngine.UI.Slider>();

        StartCoroutine(UpdateLifebar());
	}
	

    IEnumerator UpdateLifebar()
    {
        m_lifebar.value = (float)m_life / (float)m_maxlife;   //更新生命值
        m_lifebarObj.transform.eulerAngles = Camera.main.transform.eulerAngles;   //更新角度，使其始终面向摄像机
        yield return 0;
        StartCoroutine(UpdateLifebar());   //循环
    }

	// Update is called once per frame
	void Update () {
        RotateTo();
        MoveTo();
	}

    public void RotateTo()   //转向目标
    {
        var position = m_currentNode.transform.position - transform.position;
        position.y = 0;   //保持仅旋转Y轴
        var targetRotate = Quaternion.LookRotation(position);  //四元数计算旋转
        float next = Mathf.MoveTowardsAngle(transform.eulerAngles.y, targetRotate.eulerAngles.y, 120 * Time.deltaTime);
        //获得中间旋转角度
        this.transform.eulerAngles=new Vector3(0,next,0);  //旋转
    }

    public void MoveTo()
    {
        Vector3 pos1 = this.transform.position;
        Vector3 pos2 = m_currentNode.transform.position;
        float dist = Vector2.Distance(new Vector2(pos1.x,pos1.z), new Vector2(pos2.x, pos2.z));
        if(dist<1.0f)    //如果到路点的位置
        {
            if (m_currentNode.m_next == null)    //下一个路点为空，说明已经到终点
            {
                GameManager.Instance.SetDamage(1);   //受到1点伤害
                DestroyMe();
            }
            else
            {
                m_currentNode = m_currentNode.m_next;   //更新到下一个路点
            }
        }
        this.transform.Translate(new Vector3(0, 0, m_speed * Time.deltaTime));  //移动
    }

    public void DestroyMe()
    {
        GameManager.Instance.m_EnemyList.Remove(this);   //在数组中移除
        onDeath(this);     //发布死亡信息
        Destroy(this.gameObject);
    }

    public void SetDamage(int damage)
    {
        print("test attack");
        m_life = m_life - damage;
        if(m_life<=0)
        {
            print("test death");
            m_life = 0;
            GameManager.Instance.SetMoney(5);   //消灭敌人获得金币
            DestroyMe();
        }
    }
}
