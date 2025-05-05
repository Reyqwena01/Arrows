using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{

    [SerializeField] private AudioSource _audioSource = null;
    [SerializeField] private AudioSource _windAudioSource = null;
    [SerializeField] private AudioSource _timeAudioSource = null;
    [SerializeField] private Sound[] _sounds = null;

    private bool _wind = false;

    private Dictionary<string, AudioClip> _soundsDic = new Dictionary<string, AudioClip>();

    private static AudioManager _instance = null;

    public static AudioManager Instance { get => _instance; set => _instance = value; }

    public void PlaySound(AudioClip audioClip)
    {
        _audioSource.PlayOneShot(audioClip);
    }

    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < _sounds.Length; i++)
        {
            _soundsDic.Add(_sounds[i].ID, _sounds[i].Clip);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (_wind)
        {
            _windAudioSource.pitch += 1 * Time.deltaTime;
        }

        _timeAudioSource.pitch = Mathf.Lerp(_timeAudioSource.pitch, Time.timeScale, Time.deltaTime * 1.75f);
    }


    public void Init()
    {
        _instance = FindObjectOfType<AudioManager>();
        Object.DontDestroyOnLoad(gameObject);
    }

    public void PlaySound(string soundID)
    {
        _audioSource.pitch = Random.Range(0.9f, 1.1f);
        _audioSource.PlayOneShot(_soundsDic[soundID]);
    }

    public void PlayTimeSound(string soundID)
    {
        _timeAudioSource.PlayOneShot(_soundsDic[soundID]);
    }

    public void StartPlayingWind()
    {
        _wind = true;
        _windAudioSource.Play();
    }

    public void PauseWind()
    {
        _wind = false;
        _windAudioSource.Pause();
    }

    public void UnpauseWind()
    {
        _wind = true;
        _windAudioSource.UnPause();
    }

    public void StopPlayingWind()
    {
        _wind = false;
        _windAudioSource.pitch = 1.5f;
        _windAudioSource.Stop();
    }
}


[System.Serializable]
public struct Sound
{
    public string ID;
    public AudioClip Clip;
}
