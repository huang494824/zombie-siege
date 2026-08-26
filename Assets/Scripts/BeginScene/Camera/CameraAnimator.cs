using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class CameraAnimator : MonoBehaviour
{
    private Animator animator;
    //用于记录 动画播放完成后 你想要做的事情的函数
    private UnityAction overAction;

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
    }

    //左转
    public void TurnLeft(UnityAction action)
    {
        animator.SetTrigger("Left");
        overAction = action;
    }
    //右转
    public void TurnRight(UnityAction action)
    {
        animator.SetTrigger("Right");
        overAction = action;
    }

    //当动画播放完时 会调用的方法
    public void PlayerOver()
    {
        overAction?.Invoke();
        overAction = null;
    }
    
}
