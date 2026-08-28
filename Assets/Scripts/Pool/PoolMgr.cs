using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class PoolMgr : MonoBehaviour
{
    private static PoolMgr instance;

    public static PoolMgr Instance
    {
        get
        {
            if (instance == null)
            {
                GameObject obj = new GameObject("PoolMgr");
                instance = obj.AddComponent<PoolMgr>();
            }

            return instance;
        }
    }

    //根据Resources路径记录不同的对象池
    private Dictionary<string, ObjectPool<GameObject>> poolDic =
        new Dictionary<string, ObjectPool<GameObject>>();

    //缓存已经加载过的音频资源
    private Dictionary<string, AudioClip> audioClipDic =
        new Dictionary<string, AudioClip>();

    //临时音效对象池
    private ObjectPool<SoundObject> soundPool;

    private void Awake()
    {
        soundPool = new ObjectPool<SoundObject>(
            CreateSoundObject,
            GetSoundObject,
            PushSoundObject,
            DestroySoundObject,
            true,
            10,
            30
        );
    }

    /// <summary>
    /// 创建指定资源对应的对象池
    /// </summary>
    private ObjectPool<GameObject> CreatePool(string resName)
    {
        GameObject prefab = Resources.Load<GameObject>(resName);

        ObjectPool<GameObject> pool = new ObjectPool<GameObject>(
            () =>
            {
                GameObject obj = Instantiate(prefab, transform);

                PoolObject poolObj = obj.GetComponent<PoolObject>();
                if (poolObj == null)
                    poolObj = obj.AddComponent<PoolObject>();

                poolObj.InitInfo(resName);
                obj.SetActive(false);

                return obj;
            },
            null,
            (obj) =>
            {
                obj.transform.SetParent(transform);
                obj.GetComponent<PoolObject>().PushToPool();
            },
            (obj) =>
            {
                Destroy(obj);
            },
            true,
            10,
            50
        );

        return pool;
    }

    /// <summary>
    /// 从对象池中获取对象
    /// </summary>
    public GameObject GetObj(string resName, Vector3 position, Quaternion rotation)
    {
        ObjectPool<GameObject> pool;

        if (!poolDic.TryGetValue(resName, out pool))
        {
            pool = CreatePool(resName);
            poolDic.Add(resName, pool);
        }

        GameObject obj = pool.Get();

        obj.transform.SetParent(transform);
        obj.transform.SetPositionAndRotation(position, rotation);
        obj.GetComponent<PoolObject>().GetFromPool();

        return obj;
    }

    /// <summary>
    /// 立即回收对象
    /// </summary>
    public void PushObj(GameObject obj)
    {
        if (obj == null)
            return;

        PoolObject poolObj = obj.GetComponent<PoolObject>();

        if (poolObj == null)
        {
            Destroy(obj);
            return;
        }

        //防止同一个对象被重复回收
        if (poolObj.isPush)
            return;

        ObjectPool<GameObject> pool;

        if (poolDic.TryGetValue(poolObj.poolName, out pool))
            pool.Release(obj);
        else
            Destroy(obj);
    }

    /// <summary>
    /// 延迟回收对象
    /// </summary>
    public void PushObj(GameObject obj, float time)
    {
        if (obj == null)
            return;

        PoolObject poolObj = obj.GetComponent<PoolObject>();

        if (poolObj == null)
        {
            Destroy(obj, time);
            return;
        }

        poolObj.DelayPush(time);
    }

    /// <summary>
    /// 播放临时音效
    /// </summary>
    public void PlaySound(string resName, float volume, bool isMute)
    {
        AudioClip clip;

        if (!audioClipDic.TryGetValue(resName, out clip))
        {
            clip = Resources.Load<AudioClip>(resName);
            audioClipDic.Add(resName, clip);
        }

        SoundObject soundObj = soundPool.Get();
        soundObj.PlaySound(clip, volume, isMute);
    }

    private SoundObject CreateSoundObject()
    {
        GameObject obj = new GameObject("SoundObject");
        obj.transform.SetParent(transform);

        SoundObject soundObj = obj.AddComponent<SoundObject>();
        soundObj.InitInfo(soundPool);

        obj.SetActive(false);

        return soundObj;
    }

    private void GetSoundObject(SoundObject soundObj)
    {
        soundObj.gameObject.SetActive(true);
    }

    private void PushSoundObject(SoundObject soundObj)
    {
        soundObj.StopSound();
        soundObj.transform.SetParent(transform);
        soundObj.gameObject.SetActive(false);
    }

    private void DestroySoundObject(SoundObject soundObj)
    {
        Destroy(soundObj.gameObject);
    }

    private void OnDestroy()
    {
        if (instance == this)
            instance = null;
    }
}