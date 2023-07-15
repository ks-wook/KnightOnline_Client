using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * 효과음 및 Bgm 처리담당 매니저 스크립트
 * 
 * 오디오 리소스 로드 및 재생을 담당
 */


public class SoundManager 
{
    // 사운드 종류별로 오디오 리소스 관리
    AudioSource[] _audioSources = new AudioSource[(int)Define.Sound.MaxCount];

    // 오디오 클립 캐싱
    Dictionary<string, AudioClip> _audioClips = new Dictionary<string, AudioClip>();

    public void Init()
    {
        GameObject soundRoot = GameObject.Find("@Sound");

        if (soundRoot == null) // 사운드 매니저 오브젝트가 없다면 생성
        {
            soundRoot = new GameObject { name = "@Sound" };
            Object.DontDestroyOnLoad(soundRoot);

            string[] soundNames = System.Enum.GetNames(typeof(Define.Sound));

            for(int i = 0; i < soundNames.Length - 1; i++)
            {
                GameObject go = new GameObject { name = soundNames[i] };
                _audioSources[i] = go.AddComponent<AudioSource>();
                go.transform.parent = soundRoot.transform;
            }

            // BGM은 반복해서 재생
            _audioSources[(int)Define.Sound.Bgm].loop = true;
        }


    }

    // 사운드 재생
    public void Play(string path, Define.Sound type = Define.Sound.Effect, float pitch = 1.0f)
    {
        if(path.Contains("Sounds/") == false) // 모든 오디오 리소스는 Sounds 폴더 아래에 있으므로
        {
            path = $"Sounds/{path}";
        }

        if(type == Define.Sound.Bgm) // Bgm 재생
        {
            AudioClip audioClip = GetOrAddAudioClip(path); // 오디오 클립 로드
            if(audioClip == null)
            {
                Debug.Log($"적합한 오디오가 존재하지 않습니다. : {path}");
                return;
            }

            AudioSource audioSource = _audioSources[(int)Define.Sound.Bgm];

            if (audioSource.isPlaying) // 재생 중인 오디오 리소스가 있었던 경우 재생을 중단
                audioSource.Stop();

            audioSource.pitch = pitch;
            audioSource.clip = audioClip;
            audioSource.Play();
        }
        else // 이펙트 음원 재생
        {
            AudioClip audioClip = GetOrAddAudioClip(path);
            if(audioClip == null)
            {
                Debug.Log($"적합한 오디오가 존재하지 않습니다. : {path}");
                return;
            }

            AudioSource audioSource = _audioSources[(int)Define.Sound.Effect];
            audioSource.pitch = pitch;
            audioSource.PlayOneShot(audioClip);
        }

    }

    public void StopBgm()
    {
        AudioSource audioSource = _audioSources[(int)Define.Sound.Bgm];

        if (audioSource.isPlaying) // 재생 중인 오디오 리소스가 있었던 경우 재생을 중단
            audioSource.Stop();
    }

    // 오디오 파일 로드 및 캐싱 처리 함수
    AudioClip GetOrAddAudioClip(string path)
    {

        AudioClip audioClip = null;
        if(!_audioClips.TryGetValue(path, out audioClip)) // 오디오 클립을 탐색
        {
            // 재생된 적이 없는 오디오 파일 이라면 로드
            audioClip = Managers.Resource.Load<AudioClip>(path);

            // 로드된 클립을 캐싱
            _audioClips.Add(path, audioClip);
        }

        return audioClip;
    }


    public void Clear()
    {
        foreach (AudioSource audioSource in _audioSources)
        {
            audioSource.clip = null;
            audioSource.Stop();
        }

        _audioClips.Clear();
    }
}
