using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager instance;

    private AudioSource audioBgm;
    private AudioSource audioSfx;

    [SerializeField] private Dictionary<string, AudioClip> bgmContainer = new Dictionary<string, AudioClip>();
    [SerializeField] private Dictionary<string, AudioClip> sfxContainer = new Dictionary<string, AudioClip>();

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
            
            InitSoundChannels(); // 여기서 오디오 기계를 자동으로 만들고 세팅합니다.
            LoadAllSounds(); 
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void InitSoundChannels()
    {
        // 스크립트가 붙은 오브젝트에 오디오 소스 기계 2개를 자동으로 생성합니다.
        if (audioBgm == null) audioBgm = gameObject.AddComponent<AudioSource>();
        if (audioSfx == null) audioSfx = gameObject.AddComponent<AudioSource>();

        audioBgm.loop = true; 

        // ★ 2D 게임 전용 세팅 (소리가 멀어져도 작아지지 않고 화면 전체에 똑바로 들리게 함)
        audioBgm.spatialBlend = 0f;
        audioSfx.spatialBlend = 0f;
    }

    private void LoadAllSounds()
    {
        // Resources/Sounds/BGM 폴더 안의 mp3들을 싹 긁어 모읍니다.
        AudioClip[] bgmClips = Resources.LoadAll<AudioClip>("Sounds/BGM");
        foreach (AudioClip clip in bgmClips)
        {
            bgmContainer[clip.name] = clip;
        }

        // Resources/Sounds/SFX 폴더 안의 mp3들을 싹 긁어 모읍니다.
        AudioClip[] sfxClips = Resources.LoadAll<AudioClip>("Sounds/SFX");
        foreach (AudioClip clip in sfxClips)
        {
            sfxContainer[clip.name] = clip;
        }

        Debug.Log($"[SoundManager] 로드 완료 - BGM: {bgmContainer.Count}개, SFX: {sfxContainer.Count}개");
    }

    public void PlayBGM(string clipName)
    {
        if (!bgmContainer.ContainsKey(clipName))
        {
            Debug.LogWarning($"[SoundManager] BGM 파일을 찾을 수 없음: {clipName}");
            return;
        }

        if (audioBgm.isPlaying && audioBgm.clip == bgmContainer[clipName]) return;

        audioBgm.clip = bgmContainer[clipName];
        audioBgm.Play();
    }

    public void StopBGM()
    {
        audioBgm.Stop();
    }

    public void PlaySFX(string clipName)
    {
        if (!sfxContainer.ContainsKey(clipName))
        {
            Debug.LogWarning($"[SoundManager] SFX 파일을 찾을 수 없음: {clipName}");
            return;
        }

        audioSfx.PlayOneShot(sfxContainer[clipName]);
    }
}