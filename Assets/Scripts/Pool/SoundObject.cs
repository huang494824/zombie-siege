using UnityEngine;
using UnityEngine.Pool;

[RequireComponent(typeof(AudioSource))]
public class SoundObject : MonoBehaviour
{
    private AudioSource audioSource;
    private IObjectPool<SoundObject> pool;

    //避免刚激活但还没开始播放时被立即回收
    private bool isPlaying;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    public void InitInfo(IObjectPool<SoundObject> pool)
    {
        this.pool = pool;
    }

    public void PlaySound(AudioClip clip, float volume, bool isMute)
    {
        audioSource.clip = clip;
        audioSource.volume = volume;
        audioSource.mute = isMute;

        isPlaying = true;
        audioSource.Play();
    }

    public void StopSound()
    {
        isPlaying = false;
        audioSource.Stop();
        audioSource.clip = null;
    }

    private void Update()
    {
        if (isPlaying && !audioSource.isPlaying)
        {
            isPlaying = false;
            pool.Release(this);
        }
    }
}