using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class MonsterObject : MonoBehaviour
{
    //动画相关
    private Animator animator;
    //位移相关 寻路组件
    private NavMeshAgent agent;
    //一些不变的基础数据
    private MonsterInfo monsterInfo;

    //当前血量
    private int hp;
    //怪物是否死亡
    public bool isDead = false;

    //上一次攻击的时间
    private float frontTime = 0;

    //攻击检测结果缓存 主塔层通常只有少量碰撞体
    private Collider[] atkColliders = new Collider[4];


    // Start is called before the first frame update
    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
    }


    public void InitInfo(MonsterInfo info)
    {
        monsterInfo = info;

        //每次从池中取出时重置怪物状态
        isDead = false;
        frontTime = 0;

        //状态机加载和重置
        animator.runtimeAnimatorController =
            Resources.Load<RuntimeAnimatorController>(info.animator);
        animator.Rebind();
        animator.Update(0);

        //重置血量
        hp = info.hp;

        //死亡时关闭过寻路组件 复用时需要重新开启
        if (!agent.enabled)
            agent.enabled = true;

        agent.speed = agent.acceleration = info.moveSpeed;
        agent.angularSpeed = info.roundSpeed;
        agent.isStopped = false;
    }

    //受伤
    public void Wound(int dmg)
    {
        if (isDead)
            return;
        
        //减少血量
        hp -= dmg;
        //播放受伤动画
        animator.SetTrigger("Wound");

        if (hp <= 0)
        {
            //死亡
            Dead();
        }
        else
        {
            //播放音效
            GameDataMgr.Instance.PlaySound("Music/Wound");
        }
    }

    //死亡
    public void Dead()
    {
        isDead = true;
        //停止移动
        //agent.isStopped = true;
        agent.enabled = false;

        //播放死亡动画
        animator.SetBool("Dead", true);

        //播放音效
        GameDataMgr.Instance.PlaySound("Music/dead");
        //加钱
        GameLevelMgr.Instance.player.AddMoney(10);
    }

    public void DeadEvent()
    {
        //从怪物列表中移除
        GameLevelMgr.Instance.RemoveMonster(this);

        //先判断游戏是否结束 再回收怪物
        bool isOver = GameLevelMgr.Instance.CheckOver();

        //放回对象池 不再销毁
        PoolMgr.Instance.PushObj(gameObject);

        if (isOver)
        {
            GameOverPanel panel = UIManager.Instance.ShowPanel<GameOverPanel>();
            panel.InitInfo(GameLevelMgr.Instance.player.money, true);
        }
    }

    //出生过后再移动
    //移动-寻路组件
    public void BornOver()
    {
        //出生结束后 再让怪物朝目标点移动
        agent.SetDestination(MainTowerObject.Instance.transform.position);
        //播放移动动画
        animator.SetBool("Run", true);
    }

    //攻击
    void Update()
    {
        //检测什么时候停下来攻击
        if (isDead)
            return;
        //根据速度 来决定动画播放什么
        animator.SetBool("Run", agent.velocity != Vector3.zero);
        //检测和目标点达到移动条件时 就攻击
        if (Vector3.Distance(this.transform.position, MainTowerObject.Instance.transform.position) < 5 &&
            Time.time - frontTime >= monsterInfo.atkOffset)
        {
            //记录这次攻击时的时间
            frontTime = Time.time;
            animator.SetTrigger("Atk");
        }
    }

    public void AtkEvent()
    {
        //使用缓存数组进行范围检测 避免每次创建新数组
        int colliderNum = Physics.OverlapSphereNonAlloc(
            this.transform.position + transform.forward + transform.up,
            1,
            atkColliders,
            1 << LayerMask.NameToLayer("MainTower")
        );

        //播放音效
        GameDataMgr.Instance.PlaySound("Music/Eat");

        for (int i = 0; i < colliderNum; i++)
        {
            if (atkColliders[i].gameObject == MainTowerObject.Instance.gameObject)
            {
                MainTowerObject.Instance.Wound(monsterInfo.atk);
            }
        }
    }
}
