using UnityEngine;
using System.Collections.Generic;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance;

    [System.Serializable]
    public class Sound
    {
        public string name;
        public AudioClip clip;
        public bool loop;
        public float volume = 1f;
    }

    public List<Sound> sounds;
    private Dictionary<string, AudioSource> soundDict = new Dictionary<string, AudioSource>();

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            InitializeSounds();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void InitializeSounds()
    {
        foreach (Sound s in sounds)
        {
            AudioSource source = gameObject.AddComponent<AudioSource>();
            source.clip = s.clip;
            source.loop = s.loop;
            source.volume = s.volume;
            soundDict[s.name] = source;
        }
    }

    public void Play(string name)
    {
        if (soundDict.ContainsKey(name))
            soundDict[name].Play();
        else
            Debug.LogWarning("Sound not found: " + name);
    }

    public void Stop(string name)
    {
        if (soundDict.ContainsKey(name))
            soundDict[name].Stop();
    }
}