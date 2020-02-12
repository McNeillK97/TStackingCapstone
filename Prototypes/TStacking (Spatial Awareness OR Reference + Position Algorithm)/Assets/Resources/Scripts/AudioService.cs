using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioService : MonoBehaviour 
{
	public AudioSource audioBg;
	public AudioSource audioUI;
	public AudioSource audioScanning;
	
	public void InitService()
    {
		Debug.Log("Init audio service");
    }

	public void PlayBgMusic(string audioName, bool isLoop)
	{
		AudioClip audioClip = LoadAudio("Audios/" + audioName, true);

		if(audioBg.clip == null || audioBg.clip.name != audioClip.name)
		{
			audioBg.clip = audioClip;
			audioBg.loop = isLoop;
			audioBg.Play();
		}
	}

	public void PlayUIAudio(string audioName, bool isLoop)
	{
		AudioClip audioClip = LoadAudio("Audios/" + audioName, true);
		audioUI.clip = audioClip;
		audioUI.loop = isLoop;
		audioUI.Play(); 
	}

	public void SetScanningAudio(bool isActive)
	{
		audioScanning.gameObject.SetActive(isActive);
	}

	private Dictionary<string, AudioClip> audioDictionary = new Dictionary<string, AudioClip>();
	public AudioClip LoadAudio(string path, bool isCache)
	{
		AudioClip audioClip = null;

		if (!audioDictionary.TryGetValue(path, out audioClip))
		{
			audioClip = Resources.Load<AudioClip>(path);
			
			if (isCache)
			{
				audioDictionary.Add(path, audioClip);
			}
		}

		return audioClip;
	}
}
