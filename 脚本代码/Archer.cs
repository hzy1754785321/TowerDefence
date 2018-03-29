using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Archer : Defender {

    protected override void Init()
    {
        m_attackArea = 5.0f;    //攻击范围
        m_power = 1;    //攻击力
        m_attackInterval = 1.0f;  //攻击间隔
        print("test 12");
        CreateModel("archer");  //创建模型
        StartCoroutine(Attack());  //开始攻击
    }

    protected override IEnumerator Attack()
    {
        print("test 19");
        while (m_targetEnemy == null || !m_isFaceEnemy)
            yield return 0;
        while(!m_ani.GetCurrentAnimatorStateInfo(0).IsName("attack"))  //等待进入攻击
            yield return 0;
        float ani_length=m_ani.GetCurrentAnimatorStateInfo(0).length;  //获得攻击动画长度
        yield return new WaitForSeconds(ani_length*0.5f);  //等待完成动作     
        if(m_targetEnemy!=null)
        {
            print("test 30");
            Vector3 pos = this.m_model.transform.Find("atkpoint").position;  //寻找攻击点位置

            Projectile.Create(m_targetEnemy.transform, pos, (Enemy enemy) =>     //创建弓箭
                {   
                    enemy.SetDamage(m_power);
                    m_targetEnemy = null;
                });
       }
       yield return new WaitForSeconds(ani_length * 0.5f);  //等待剩余动画
        m_ani.CrossFade("idle", 0.8f);   //播放待机动画

        yield return new WaitForSeconds(m_attackInterval);   //设置时间间隔
        StartCoroutine(Attack());   //下一次攻击
    }
}
