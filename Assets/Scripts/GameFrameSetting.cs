using UnityEngine;

public class GameFrameSetting : MonoBehaviour
{
    private void Awake()
    {
        // 限制最高帧率为 60
        Application.targetFrameRate = 60;
    }
}