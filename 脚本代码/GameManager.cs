using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;   //静态实例
    public LayerMask m_groundlayer;   //地面的碰撞Layer
    public int m_wave = 1;    //波数
    public int m_waveMax = 10;   //最大波数
    public int m_life = 10;    //生命值
    public int m_money = 30;    //金币数

    Text m_txt_wave;       //波数UI控件
    Text m_txt_life;       //生命值UI控件
    Text m_txt_money;      //金币UI控件

    Button m_restart;   //重新开始按钮UI

    public bool m_debug = true;   //是否显示路点的开关
    public List<PathNode> m_PathNodes;   //路点，保存所有路点

    bool m_isSelectedButton = false;   //当前是否选中创建防守单位的按钮

    public List<Enemy> m_EnemyList = new List<Enemy>();   //存放敌人的数组

    void Awake()
    {
        Instance = this;    //静态初始化
    }
    void Start()
    {
        UnityAction<BaseEventData> downAction = new UnityAction<BaseEventData>(OnButCreateDefenderDown);
        //创建UnityAction,响应按钮按下事件
        UnityAction<BaseEventData> upAction = new UnityAction<BaseEventData>(OnButCreateDefenderUp);
        //创建UnityAction,响应按钮抬起事件

        EventTrigger.Entry down = new EventTrigger.Entry();  //按钮事件
        down.eventID = EventTriggerType.PointerDown;
        down.callback.AddListener(downAction);

        EventTrigger.Entry up = new EventTrigger.Entry();     //按钮抬起事件
        up.eventID = EventTriggerType.PointerUp;
        up.callback.AddListener(upAction);

        foreach (Transform t in this.GetComponentsInChildren<Transform>())   //查找所有子物体,根据名称获取UI控件
        {
            if (t.name.CompareTo("wave") == 0)    //找到波数控件
            {
                m_txt_wave = t.GetComponent<Text>();
                SetWave(1);
            }
            else if (t.name.CompareTo("life") == 0)   //找到生命值控件
            {
                m_txt_life = t.GetComponent<Text>();
                m_txt_life.text = string.Format("生命：{0}", m_life);
            }
            else if (t.name.CompareTo("money") == 0)   //找到金币控件
            {
                m_txt_money = t.GetComponent<Text>();
                m_txt_money.text = string.Format("金币：{0}", m_money);
            }
            else if (t.name.CompareTo("Button_restart") == 0)    //找到重新开始按钮
            {
                m_restart = t.GetComponent<Button>();
                m_restart.onClick.AddListener(delegate()
                {
                    SceneManager.LoadScene(SceneManager.GetActiveScene().name);   //重新开始场景
                });
                m_restart.gameObject.SetActive(false);   //默认隐藏重新开始按钮
            }
            else if (t.name.Contains("Button_player"))   //找到创建防守单位按钮     test
            {  //给防守按钮添加事件
                EventTrigger trigger = t.gameObject.AddComponent<EventTrigger>();
                trigger.triggers = new List<EventTrigger.Entry>();
                trigger.triggers.Add(down);
                trigger.triggers.Add(up);
            }
        }
        BuildPath();     //test
    }

    void Update()
    {
        if(Input.GetKey(KeyCode.Escape))
        {
            Application.Quit();
        }
        if (m_isSelectedButton)     //如果选中创建的按钮，则取消摄像机操作
            return;
#if (UNITY_IOS || UNITY_ANDROID) && !UNITY_EDITOR    //判断平台，决定执行触屏还是鼠标
        bool press = Input.touches.Length > 0 ? true : false;    //判断手指是否触屏
        float mx = 0;
        float my = 0;
        if(press)
        {
            if(Input.GetTouch(0).phase==TouchPhase.Moved)    //获得手指移动距离
            {
                mx = Input.GetTouch(0).deltaPosition.x * 0.01f;
                my = Input.GetTouch(0).deltaPosition.y * 0.01f;
            }
        }
#else
        bool press = Input.GetMouseButton(0);   //获得鼠标移动距离
        float mx = Input.GetAxis("Mouse X");
        float my = Input.GetAxis("Mouse Y");
#endif
        GameCamera.Inst.Control(press, mx, my);  //摄像机移动
    }

    public void SetWave(int wave)      //更新波数UI
    {
        m_wave = wave;
        m_txt_wave.text = string.Format("波数：{0}/{1}", m_wave, m_waveMax);
    }

    public void SetDamage(int damage)    //更新生命UI
    {
        m_life = m_life - damage;
        if (m_life <= 0)
        {
            m_life = 0;
            m_restart.gameObject.SetActive(true);    //显示重新开始按钮
        }
        m_txt_life.text = string.Format("生命：{0}", m_life);
    }

    public bool SetMoney(int money)      //更新金币UI文字
    {
        if (m_money + money < 0)
            return false;
        m_money = m_money + money;
        m_txt_money.text = string.Format("金币：{0}", m_money);
        return true;
    }


    void OnButCreateDefenderDown(BaseEventData data)   //按下“创建防守单位按钮”
    {
        m_isSelectedButton = true;
    }

    void OnButCreateDefenderUp(BaseEventData data)    //抬起“创建防守单位按钮”
    {
        print("test 144");
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);  //创建射线
        RaycastHit hitinfo;   //保存射线信息
        print("test 147");
        if (Physics.Raycast(ray, out hitinfo, 1000, m_groundlayer))    //检测是否与地面碰撞
        {
            print("test 150");
            //如果选中一个可用的格子
            if(TileObject.Instance.getDataFromPosition(hitinfo.point.x,hitinfo.point.z)==(int)Defender.TileStatus.GUARD)
            {
                print("test 154");
                Vector3 hitpos = new Vector3(hitinfo.point.x, 0, hitinfo.point.z);  //获得碰撞点位置
                Vector3 gridPos = TileObject.Instance.transform.position;   //获得Grid的坐标位置
                print(gridPos);
               float tilesize = TileObject.Instance.tileSize;   //获得格子大小
                hitpos.x = gridPos.x + (int)((hitpos.x - gridPos.x) / tilesize) * tilesize + tilesize * 0.5f;
                hitpos.z = gridPos.z + (int)((hitpos.z - gridPos.z) / tilesize) * tilesize + tilesize * 0.5f;
                //计算格子的中心位置
                GameObject go = data.selectedObject;  //获得选择按钮
                if(go.name.Contains("1"))     //如果按钮名字包括"1"
                {
                    if (SetMoney(-15))   //减少15块钱，创建近战单位
                        Defender.Create<Defender>(hitpos, new Vector3(0, 180, 0));
                }
                else if(go.name.Contains("2"))
                {
                    if (SetMoney(-20))    //减少20块，创建远程单位
                        Defender.Create<Archer>(hitpos, new Vector3(0, 180, 0));
                }
            }
        }
        print("test 174");
        m_isSelectedButton = false;
    }

    [ContextMenu("BuildPath")]
    void BuildPath()
    {
        m_PathNodes = new List<PathNode>();
        GameObject[] objs = GameObject.FindGameObjectsWithTag("pathnode");   //寻找所有路点
        for(int i=0;i<objs.Length;i++) 
        {
            m_PathNodes.Add(objs[i].GetComponent<PathNode>());   //保存路点
        }
    }

    void OnDrawGizmos()
    {
        if (!m_debug || m_PathNodes == null)
            return;
        Gizmos.color = Color.yellow;  //将路点连线设为蓝色
        foreach(PathNode node in m_PathNodes)   //遍历路点
        {
            if (node.m_next != null)
            {
                Gizmos.DrawLine(node.transform.position, node.m_next.transform.position);
            }
        }
    }
}
