using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance { get; private set; }

    [Header("Audio Sources")]
    [SerializeField] private AudioSource sfxSource;   
    [SerializeField] private AudioSource musicSource; 

    private void Awake()
    {
        

        Instance = this;
        

        if (sfxSource == null)
        {
            GameObject sfxGO = new GameObject("SFX_Source");
            sfxGO.transform.parent = transform;
            sfxSource = sfxGO.AddComponent<AudioSource>();
        }

        if (musicSource == null)
        {
            GameObject musicGO = new GameObject("Music_Source");
            musicGO.transform.parent = transform;
            musicSource = musicGO.AddComponent<AudioSource>();
            musicSource.loop = true; 
        }
    }

    
    public static void PlayClip(AudioClip clip, float volume = 1f)
    {
        if (Instance == null || clip == null) return;
        Instance.sfxSource.PlayOneShot(clip, volume);
    }

    
    public static void PlayMusic(AudioClip clip, float volume = 1f)
    {
        if (Instance == null || clip == null) return;
        if (Instance.musicSource.clip == clip && Instance.musicSource.isPlaying) return; 

        Instance.musicSource.clip = clip;
        Instance.musicSource.volume = volume;
        Instance.musicSource.Play();
    }

    
    public static void StopMusic()
    {
        if (Instance == null) return;
        Instance.musicSource.Stop();
    }
}
