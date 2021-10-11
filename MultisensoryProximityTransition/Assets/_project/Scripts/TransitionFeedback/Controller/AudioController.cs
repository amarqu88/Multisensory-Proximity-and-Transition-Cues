using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioController : FeedbackController
{
    public AudioSource audioSource;

    public override void emit(float intensity)
    {
        if (!audioSource.isPlaying)
            audioSource.Play();
            //audioSource.PlayOneShot(audioSource.clip);
        if(intensity == 0)
        {
          audioSource.Stop();
          //  StartCoroutine(AudioFadeOut.StartFade(audioSource, 0.1f, 0));
        //    audioSource.time = 0;
        }
        audioSource.volume = intensity;
    }

    public override void prepareController()
    {
        audioSource.Stop();
        audioSource.time = 0;
    }

    public void setAudioClip(AudioClip audioClip)
    {
        audioSource.clip = audioClip;
    }

    public void setLoop(bool loop)
    {
        audioSource.loop = loop;
    }
    public override void disableController()
    {
        audioSource.Stop();
        audioSource.time = 0;
    }
}
