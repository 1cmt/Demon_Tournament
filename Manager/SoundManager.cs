using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Audio;

public enum Sound
{
    Bgm,
    Effect,
}

public enum AttackSound
{
    None,
    Attack,
    Attack2,
    Attack3,
    Attack4
}

public class SoundManager : Singleton<SoundManager>
{
    private Dictionary<string, AudioClip> _audioClips = new Dictionary<string, AudioClip>();

    [SerializeField] 
    private AudioMixer audioMixer;
    [SerializeField]
    private AudioClip[] bgmaudioClips;
    [SerializeField]
    private AudioClip[] sfxaudioClips;
    [SerializeField]
    private AudioClip[] attackClip;

    [SerializeField]
    private AudioMixerGroup bgmMixerGroup;
    [SerializeField]
    private AudioMixerGroup sfxMixerGroup;

    private AudioSource bgmSource;
    private AudioSource sfxSource;

    public override void Awake()
    {
        base.Awake();
        bgmSource = gameObject.AddComponent<AudioSource>();
        sfxSource = gameObject.AddComponent<AudioSource>();

        bgmSource.outputAudioMixerGroup = bgmMixerGroup;
        sfxSource.outputAudioMixerGroup = sfxMixerGroup;

        bgmSource.volume = 0.3f;

        foreach (var clip in bgmaudioClips)
        {
            _audioClips[clip.name] = clip;
        }

        foreach (var clip in sfxaudioClips)
        {
            _audioClips[clip.name] = clip;
        }

        foreach (var clip in attackClip)
        {
            _audioClips[clip.name] = clip;
        }
    }

    public void PlayAudio(Sound soundType, string clipName)
    {
        if (!_audioClips.TryGetValue(clipName, out var clip))
        {
            Debug.Log("sss");
            return;
        }

        if (soundType == Sound.Bgm)
        {
            bgmSource.clip = clip;
            bgmSource.loop = true;
            bgmSource.Play();
        }
        else if (soundType == Sound.Effect)
        {
            sfxSource.PlayOneShot(clip);
        }
    }

    //public void PlayAttackSound()
    //{ 
    //   int randomIndex = Random.Range(0,4);

    //    AttackSound randomAttackSound = (AttackSound)randomIndex;
    //    AudioClip clipToPlay = attackClip[(int)randomAttackSound];

    //    sfxSource.PlayOneShot(clipToPlay);
    //}

    public void StopBGM()
    {
        bgmSource.Stop();
    }

    public void StopSFX()
    {
        sfxSource.Stop();
    }

}
