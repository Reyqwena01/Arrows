using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{

    [SerializeField] private AudioSource _audioSource = null;

    private static AudioManager _instance = null;

    public static AudioManager Instance { get => _instance; set => _instance = value; }

    public void PlaySound(AudioClip audioClip)
    {
        _audioSource.PlayOneShot(audioClip);
    }

    // Start is called before the first frame update
    void Start()
    {
        _instance = FindObjectOfType<AudioManager>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
