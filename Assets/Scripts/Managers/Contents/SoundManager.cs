using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * ȿ���� �� Bgm ó����� �Ŵ��� ��ũ��Ʈ
 * 
 * ����� ���ҽ� �ε� �� ����� ���
 */


public class SoundManager 
{
    // ���� �������� ����� ���ҽ� ����
    AudioSource[] _audioSources = new AudioSource[(int)Define.Sound.MaxCount];

    // ����� Ŭ�� ĳ��
    Dictionary<string, AudioClip> _audioClips = new Dictionary<string, AudioClip>();

    public void Init()
    {
        GameObject soundRoot = GameObject.Find("@Sound");

        if (soundRoot == null) // ���� �Ŵ��� ������Ʈ�� ���ٸ� ����
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

            // BGM�� �ݺ��ؼ� ���
            _audioSources[(int)Define.Sound.Bgm].loop = true;
        }


    }

    // ���� ���
    public void Play(string path, Define.Sound type = Define.Sound.Effect, float pitch = 1.0f)
    {
        if(path.Contains("Sounds/") == false) // ��� ����� ���ҽ��� Sounds ���� �Ʒ��� �����Ƿ�
        {
            path = $"Sounds/{path}";
        }

        if(type == Define.Sound.Bgm) // Bgm ���
        {
            AudioClip audioClip = GetOrAddAudioClip(path); // ����� Ŭ�� �ε�
            if(audioClip == null)
            {
                Debug.Log($"������ ������� �������� �ʽ��ϴ�. : {path}");
                return;
            }

            AudioSource audioSource = _audioSources[(int)Define.Sound.Bgm];

            if (audioSource.isPlaying) // ��� ���� ����� ���ҽ��� �־��� ��� ����� �ߴ�
                audioSource.Stop();

            audioSource.pitch = pitch;
            audioSource.clip = audioClip;
            audioSource.Play();
        }
        else // ����Ʈ ���� ���
        {
            AudioClip audioClip = GetOrAddAudioClip(path);
            if(audioClip == null)
            {
                Debug.Log($"������ ������� �������� �ʽ��ϴ�. : {path}");
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

        if (audioSource.isPlaying) // ��� ���� ����� ���ҽ��� �־��� ��� ����� �ߴ�
            audioSource.Stop();
    }

    // ����� ���� �ε� �� ĳ�� ó�� �Լ�
    AudioClip GetOrAddAudioClip(string path)
    {

        AudioClip audioClip = null;
        if(!_audioClips.TryGetValue(path, out audioClip)) // ����� Ŭ���� Ž��
        {
            // ����� ���� ���� ����� ���� �̶�� �ε�
            audioClip = Managers.Resource.Load<AudioClip>(path);

            // �ε�� Ŭ���� ĳ��
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
