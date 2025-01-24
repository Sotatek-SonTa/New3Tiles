using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : Singleton<SoundManager>
{
    [SerializeField] List<AudioClip> audioClips;
    [SerializeField] AudioSource bgmSound;
    [SerializeField] AudioSource sfxSound;
     public void PlaySound(SoundName audioClipName, float volumne)
        {
            var audioClip =  audioClips.Find(x => x.name == audioClipName.ToString());
            if(audioClip)
            {
              sfxSound.clip = audioClip;
              sfxSound.volume = volumne;
              sfxSound.Play();
            }
        }
        public void ChangeBGMSound(SoundName audioClipName,float volume)
        {
            var audioClip =  audioClips.Find(x => x.name == audioClipName.ToString());
            if(audioClip)
            {
              bgmSound.clip = audioClip;
              bgmSound.volume = volume;
              bgmSound.Play();
            }
        }
        public void StopBGM()
        {
           bgmSound.Stop();
        }
}
public enum SoundName
{
    BGM,
    WIN,
    SELECTEDTILE,
    MATCHINGTILE,
    BUTTONCLICK,
}
