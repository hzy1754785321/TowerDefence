using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class WaveDate {
    public int wave = 0;
    public List<GameObject> ememyprefab;   //每波敌人的prefab
    public int level = 1;   //敌人的等级
    public float interval = 3;   //每3秒创建一个敌人
}
