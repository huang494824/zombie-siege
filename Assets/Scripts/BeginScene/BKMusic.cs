using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class BKMusic : MonoBehaviour
{
    private static BKMusic instance;
    public static BKMusic Instance => instance;

    AudioSource bkSource;

    private void Awake()
    {
        instance = this;

        bkSource = this.GetComponent<AudioSource>();

        //通过数据 来设置 音乐的大小和开关
        MusicData data = GameDataMgr.Instance.musicData;
        SetIsOpen(data.musicOpen);
        ChangeValue(data.musicValue);
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    //开关背景音乐的方法
    public void SetIsOpen(bool isOpen)
    {
        bkSource.mute = !isOpen;
    }

    //调整背景音乐大小的方法
    public void ChangeValue(float v)
    {
        bkSource.volume= v;
    }

}    

