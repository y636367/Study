using System.Collections;
using UnityEngine;

public class AudioPause : MonoBehaviour
{
    AudioSource[] audio;

    private void Awake()
    {
        audio = GetComponents<AudioSource>();
    }
    private void Start()
    {
        StartCoroutine(nameof(Sound_LC));
    }
    private IEnumerator Sound_LC()
    {
        while(true)
        {
            foreach(var sound in audio)
            {
                if (GameManager.instance.isPause && sound.isPlaying)
                    sound.Pause();
                else if(!GameManager.instance.isPause && !sound.isPlaying)
                    sound.UnPause();
            }

            yield return null;
        }
    }
}
