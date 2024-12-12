using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SFXManager : MonoBehaviour
{
    public static SFXManager instance;

    [SerializeField] private AudioSource sfxObject;
    [SerializeField] private AudioSource continuousSFXObject;
    // TODO: Remove????
    private Dictionary<string, AudioSource> audioSources;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }

        audioSources = new Dictionary<string, AudioSource>();
    }

    public void PlaySFXClip(AudioClip audioClip, Transform spawnTransform, float volume = 1f)
    {
        AudioSource audioSource = CreateAudioSource(sfxObject, audioClip, spawnTransform, volume);

        audioSource.Play();

        float clipLength = audioSource.clip.length;
        Destroy(audioSource.gameObject, clipLength);
    }

    public void AddAndPlayContinuousSFXClip(AudioClip audioClip, string clipName, GameObject parent, float volume = 1f)
    {
        AudioSource audioSource = CreateAudioSource(continuousSFXObject, audioClip, parent.transform, volume);
        audioSource.gameObject.transform.SetParent(parent.transform, true);
    }

    private AudioSource CreateAudioSource(AudioSource sfxObject, AudioClip audioClip, Transform spawnTransform, float volume)
    {
        AudioSource audioSource = Instantiate(sfxObject, spawnTransform.position, Quaternion.identity);
        audioSource.clip = audioClip;
        audioSource.volume = volume;

        return audioSource;
    }
}
