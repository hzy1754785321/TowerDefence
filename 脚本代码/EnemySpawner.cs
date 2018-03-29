using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour {
    public PathNode m_startNode;   //起始节点
    private int m_liveEnemy = 0;  //存活的敌人数量
    public List<WaveDate> waves;   //战斗波数配置数组
    int enemyIndex = 0;   //生成敌人数组的下标
    int waveIndex = 0;   //战斗波数数组的下标
	// Use this for initialization
	void Start () {
        StartCoroutine(SpawnEnemies());   //开始生成敌人
	}
	
    IEnumerator SpawnEnemies()
    {
        yield return new WaitForEndOfFrame();    //保证在start之后执行
        GameManager.Instance.SetWave((waveIndex + 1));  //设置UI显示波数
        WaveDate wave = waves[waveIndex];   //获得当前波数的配置
        yield return new WaitForSeconds(wave.interval);   //生成敌人时间间隔
        while(enemyIndex<wave.ememyprefab.Count)    //如果没有生成全部敌人
        {
            Vector3 dir = m_startNode.transform.position - this.transform.position;   //初始方向
            GameObject enemyObj = (GameObject)Instantiate(wave.ememyprefab[enemyIndex], transform.position, Quaternion.LookRotation(dir)); 
             //创建敌人
            Enemy enemy = enemyObj.GetComponent<Enemy>();
            enemy.m_currentNode = m_startNode;    //设置敌人的起点

            enemy.m_life = wave.level * 200;   //设置敌人生命
            enemy.m_maxlife = enemy.m_life;   //设置敌人最大生命值

            m_liveEnemy++;   //增加敌人数量
            enemy.onDeath = new System.Action<Enemy>((Enemy e) => { m_liveEnemy--; });  //当敌人死亡事件发生，减少敌人数量
            enemyIndex++;//更新敌人数组下标
            yield return new WaitForSeconds(wave.interval);   //生成敌人时间间隔
        }

        while(m_liveEnemy>0)     //等待敌人全被消灭
        {
            yield return 0;
        }

        enemyIndex = 0;   //重置敌人数组下标
        waveIndex++;  //更新波数数组下标
        if(waveIndex<waves.Count)     //当现在波数小与最大波数
        {
            StartCoroutine(SpawnEnemies());    //继续生成敌人
        }
        else
        {
            //success
        }
    }


	void OnDrawGizmos()
    {
        Gizmos.DrawIcon(transform.position,"Spawner.tif");
    }
		
}
