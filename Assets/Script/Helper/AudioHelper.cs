using System;
using UnityEngine;

public class AudioControl
{
    private const string MUTE_KEY = "SystemMute";
    private static bool mute = false;

//    public static void Init()
//    {
//        mute = (PlayerPrefs.GetInt(MUTE_KEY, 0) == 1);
//        AudioListener.volume = mute ? 0f : 1f;
//    }
    
//    public static bool IsMute()
//    {
//        return mute;
//    }
    
//    public static void SetMute(bool _mute)
//    {
//        if (mute != _mute)
//        {
//            mute = _mute;
//            AudioListener.volume = mute ? 0f : 1f;
            
//            PlayerPrefs.SetInt(MUTE_KEY, mute ? 1 : 0);
//        }
//    }
    
//	public static AudioSource PlayOneShot(string path, bool isLoop, float volume)
//    {
//        path = "Sound/" + path;
//		AudioClip res = ResourceMgr.LoadAudio(path) as AudioClip;
//		if (res != null) {
//			return PlayOneShot (res, isLoop, volume);
//		}
//		return null;
//    }

//	public static AudioSource PlayOneShotFromActor(GameObject caster, string path, bool isLoop, float volume)
//    {
//        path = "Sound/" + path;
//		AudioClip res = ResourceMgr.LoadAudio(path) as AudioClip;
//        if (res != null)
//        {
//			return PlayOneShotFromActor(caster, res, isLoop, volume);
//        }
//		return null;
//    }

//	public static AudioSource PlayOneShot(AudioClip res, bool isLoop, float volume)
//	{
//		if (res == null)
//		{
//			return null;
//		}

//		AudioSource source = UIAudioCenter.GetAudioSource();
//		if (source == null)
//		{
//			return null;
//		}

//		if (Camera.main != null) {
//			source.gameObject.transform.position = Camera.main.transform.position;
//		}
//		source.clip = res;
//		source.volume = volume;
//		source.loop = isLoop;
//		source.Play();
//		//source.PlayOneShot(res);
//		return source;
//	}
	
//	public static AudioSource PlayOneShotFromActor(GameObject caster, AudioClip res, bool isLoop, float volume)
//	{
//		if (res == null)
//		{
//			return null;
//		}

//		if (null == caster)
//		{
//			return null;
//		}

//		if (!caster.activeInHierarchy)
//		{
//			return null;
//		}
		
////		if (null == caster.audio)
////		{
////			caster.AddComponent<AudioSource>();
////			
////			caster.audio.loop = false;
////			caster.audio.rolloffMode = AudioRolloffMode.Linear;
////			caster.audio.dopplerLevel = 0.0f;
////			caster.audio.minDistance = minDis;
////			caster.audio.maxDistance = maxDis;
////		}
////
////		caster.audio.PlayOneShot(res);
	//	AudioSource source = UIAudioCenter.GetAudioSource();
	//	if (source == null)
	//	{
	//		return null;
	//	}
	//	source.gameObject.transform.position = caster.transform.position;
	//	source.clip = res;
	//	source.volume = volume;
	//	source.loop = isLoop;
	//	source.Play();
	//	return source;
	//}

 //   public static bool PlayMusic(string path, bool ifloop, float volume)
 //   {
 //       AudioSource source = MusicCenter.GetAudioSource();
 //       if (source == null)
 //       {
 //           return false;
 //       }
        
 //       path = "Music/" + path;

 //       AudioClip res = ResourceMgr.LoadMusic(path) as AudioClip;
 //       if (res == null)
 //       {
 //           return false;
 //       }
        
 //       source.clip = res;
 //       source.volume = volume;
 //       source.loop = ifloop;
 //       source.Play();
 //       return true;
 //   }

 //   public static void StopMusic()
 //   {
 //       AudioSource source = MusicCenter.GetAudioSource();
 //       if (source == null)
 //       {
 //           return;
 //       }

 //       source.Stop();
	//}
	
	//public static void SetVolume(float volume) {
	//	AudioSource source = MusicCenter.GetAudioSource();
	//	if( source == null )
	//	{
	//		return;
	//	}
		
	//	source.volume = volume;
		
	//}
	
	//public static float GetVolume() {
	//	AudioSource source = MusicCenter.GetAudioSource();
	//	if( source == null )
	//	{
	//		return 0f;
	//	}
		
	//	return source.volume;
		
	//}
	
}


