using System;
using System.Collections.Generic;
using UnityEngine;
using static Define;

public class SoundManager
{
	private AudioSource[] _audioSources = new AudioSource[(int)ESound.Max];
	private Dictionary<string, AudioClip> _audioClips = new Dictionary<string, AudioClip>();
	private GameObject _soundRoot = null;

	public void Init()
	{
		if (_soundRoot == null)
		{
			_soundRoot = GameObject.Find("@SoundRoot");

			if (_soundRoot == null)
			{
				_soundRoot = new GameObject { name = "@SoundRoot" };
				UnityEngine.Object.DontDestroyOnLoad(_soundRoot);

				string[] soundTypeNames = System.Enum.GetNames(typeof(ESound));
				for (int count = 0; count < soundTypeNames.Length - 1; count++)
				{
					GameObject go = new GameObject { name = soundTypeNames[count] };
					_audioSources[count] = go.AddComponent<AudioSource>();
					go.transform.parent = _soundRoot.transform;
				}

				_audioSources[(int)ESound.Bgm].loop = true;
			}
		}
	}

	public void Clear()
	{
		foreach (AudioSource audioSource in _audioSources)
			audioSource.Stop();

		_audioClips.Clear();
	}

	public void Play(ESound type)
	{
		AudioSource audioSource = _audioSources[(int)type];
		audioSource.Play();
	}

	public void Play(Define.ESound soundType, string key, float volume = 0.5f, float pitch = 1.0f)
	{
		AudioSource audioSource = _audioSources[(int)soundType];
		audioSource.volume = volume;  
		audioSource.pitch = pitch;    

		LoadAudioClip(key, (audioClip) =>
		{
			if (soundType == Define.ESound.Bgm)
			{
				if (audioSource.isPlaying)
					audioSource.Stop();

				audioSource.clip = audioClip;
				if (Managers.Game.BGMOn)
					audioSource.Play();
			}
			else
			{
				if (Managers.Game.EffectSoundOn)
					audioSource.PlayOneShot(audioClip, volume); 
			}
		});
	}

	public void Play(ESound type, AudioClip audioClip, float volume = 0.5f, float pitch = 1.0f)
	{
		AudioSource audioSource = _audioSources[(int)type];
		audioSource.volume = volume;

		if (type == ESound.Bgm)
		{
			if (audioSource.isPlaying)
				audioSource.Stop();

			audioSource.clip = audioClip;
			if (Managers.Game.BGMOn)
				audioSource.Play();
		}
		else
		{
			audioSource.pitch = pitch;
			if (Managers.Game.EffectSoundOn)
				audioSource.PlayOneShot(audioClip);
		}
	}

	public void Stop(ESound type)
	{
		AudioSource audioSource = _audioSources[(int)type];
		audioSource.Stop();
	}

	// 여기에다가 자주 쓰이는 버튼 클릭이라던가 팝업 관련 사운드 넣기!!
	
	public void PlayButtonClick()
	{
		Play(Define.ESound.Effect, "Click_CommonButton");
	}
	
	private void LoadAudioClip(string key, Action<AudioClip> callback)
	{
		AudioClip audioClip = null;
		if (_audioClips.TryGetValue(key, out audioClip))
		{
			callback?.Invoke(audioClip);
			return;
		}

		audioClip = Managers.Resource.Load<AudioClip>(key);

		if (_audioClips.ContainsKey(key) == false)
			_audioClips.Add(key, audioClip);

		callback?.Invoke(audioClip);
	}
}
