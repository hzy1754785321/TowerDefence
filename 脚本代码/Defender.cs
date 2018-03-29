using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Defender : MonoBehaviour
{

    public enum TileStatus   //枚举出格子的状态
    {
        DEAD = 0,      //禁止放置
        ROAD = 1,      //敌人行走格
        GUARD = 2,     //防守单位格
    }

    public float m_attackArea = 2.0f;   //攻击范围
    public int m_power = 1;    //攻击力
    public float m_attackInterval = 5.0f;   //攻击间隔
    protected Enemy m_targetEnemy;   //目标敌人
    protected bool m_isFaceEnemy;    //是否面向敌人
    protected GameObject m_model;    //模型的perfab
    protected Animator m_ani;    //动画播放器

    public static T Create<T>(Vector3 pos, Vector3 angle) where T : Defender    //静态函数，创建防守单位实例
    {
        print("create defender");
        GameObject go = new GameObject("defender");
        go.transform.position = pos;
        go.transform.eulerAngles = angle;
        T d = go.AddComponent<T>();
        d.Init();

        TileObject.Instance.setDataFromPosition(d.transform.position.x, d.transform.position.z, (int)TileStatus.DEAD);
        //使自己当前所在格设为禁止放置
        return d;
    }

    protected virtual void Init()
    {
        m_attackArea = 2.0f;
        m_power = 1;
        m_attackInterval = 5.0f;
        print("test 12");
        CreateModel("saber");      //读取模型
        StartCoroutine(Attack());   //开始攻击
    }

    protected virtual void CreateModel(string name)
    {
        GameObject model = Resources.Load<GameObject>(name);
        m_model = (GameObject)Instantiate(model, this.transform.position, this.transform.rotation, this.transform);
        m_ani = m_model.GetComponent<Animator>();
    }
    void Update()
    {
        FindEnemy();    //寻找
        RotateTo();     //转向敌人
        StartCoroutine(Attack());
     //   Attack();       //攻击敌人
    }

    public void RotateTo()
    {
        if (m_targetEnemy == null)
            return;
        var targetdir = m_targetEnemy.transform.position - transform.position;
        targetdir.y = 0;   //保持仅旋转Y轴

        Vector3 rot_delta = Vector3.RotateTowards(this.transform.forward, targetdir, 20.0f * Time.deltaTime, 0.0f);  //获取旋转方向
        Quaternion targetrotation = Quaternion.LookRotation(rot_delta);

        float angle = Vector3.Angle(targetdir, transform.forward);   //计算当前方向与目标之间的角度

        if (angle < 1.0f)   //如果已经面向敌人
        {
            m_isFaceEnemy = true;
        }
        else
            m_isFaceEnemy = false;
        transform.rotation = targetrotation;
    }

    public void FindEnemy()
    {
        if (m_targetEnemy != null)
            return;
        m_targetEnemy = null;
        int minlife = 0;   //最低生命值
        foreach (Enemy enemy in GameManager.Instance.m_EnemyList)  //遍历敌人
        {
            if (enemy.m_life == 0)
                continue;
            Vector3 pos1 = this.transform.position;
            pos1.y = 0;
            Vector3 pos2 = enemy.transform.position;
            pos2.y = 0;
            float dist = Vector3.Distance(pos1, pos2);  //计算与敌人的距离
            if (dist > m_attackArea)     //如果距离超过攻击范围
                continue;
            if (minlife == 0 || minlife > enemy.m_life)
            {
                m_targetEnemy = enemy;
                minlife = enemy.m_life;
            }
        }
    }

    protected virtual IEnumerator Attack()
    {
        print("test  107");
       while (m_targetEnemy == null || !m_isFaceEnemy)    //没有目标
            yield return 0;    //等待
      /*  m_ani.CrossFade("attack", 0.1f);   //播放攻击动画
        while (!m_ani.GetCurrentAnimatorStateInfo(0).IsName("attack")) //等待进入攻击动画
            yield return 0;
        float ani_length = m_ani.GetCurrentAnimatorStateInfo(0).length;  //获得动画长度
        yield return new WaitForSeconds(ani_length * 0.5f);   //等待完成动画  */
        if (m_targetEnemy != null)
            m_targetEnemy.SetDamage(m_power);   //攻击
    /*    yield return new WaitForSeconds(ani_length * 0.5f);  //等待播放剩余的攻击动画
        m_ani.CrossFade("idle", 0.1f);  //播放待机动画   */
        yield return new WaitForSeconds(m_attackInterval);  //等待攻击间隔
        StartCoroutine(Attack());   //开始下一轮攻击   
    }
}
