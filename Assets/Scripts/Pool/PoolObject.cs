using UnityEngine;

public class PoolObject : MonoBehaviour
{
    //记录自己属于哪个对象池
    public string poolName;

    //是否已经回收到池中
    public bool isPush;

    //需要延迟回收的时间点
    private float pushTime;

    //缓存粒子和预设体自带的音效 避免每次获取组件产生额外开销
    private ParticleSystem[] particleSystems;
    private AudioSource[] audioSources;

    private void Awake()
    {
        particleSystems = GetComponentsInChildren<ParticleSystem>(true);
        audioSources = GetComponentsInChildren<AudioSource>(true);

        //默认不执行Update 只有延迟回收时才开启
        enabled = false;
    }

    public void InitInfo(string poolName)
    {
        this.poolName = poolName;
    }

    /// <summary>
    /// 从池中取出时调用
    /// </summary>
    public void GetFromPool()
    {
        isPush = false;
        enabled = false;

        gameObject.SetActive(true);

        //重新播放特效
        for (int i = 0; i < particleSystems.Length; i++)
        {
            particleSystems[i].Clear(true);
            particleSystems[i].Play(true);
        }

        //重新播放预设体上设置为自动播放的音效
        for (int i = 0; i < audioSources.Length; i++)
        {
            if (audioSources[i].playOnAwake && audioSources[i].clip != null)
            {
                audioSources[i].Stop();
                audioSources[i].Play();
            }
        }
    }

    /// <summary>
    /// 回收到池中时调用
    /// </summary>
    public void PushToPool()
    {
        enabled = false;

        for (int i = 0; i < particleSystems.Length; i++)
        {
            particleSystems[i].Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
        }

        for (int i = 0; i < audioSources.Length; i++)
        {
            audioSources[i].Stop();
        }

        gameObject.SetActive(false);
        isPush = true;
    }

    /// <summary>
    /// 设置延迟回收
    /// </summary>
    public void DelayPush(float time)
    {
        pushTime = Time.time + time;
        enabled = true;
    }

    private void Update()
    {
        if (Time.time >= pushTime)
        {
            enabled = false;
            PoolMgr.Instance.PushObj(gameObject);
        }
    }
}