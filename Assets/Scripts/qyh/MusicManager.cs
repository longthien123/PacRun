using UnityEngine;

public class MusicManager : MonoBehaviour
{
    public static MusicManager Instance;

    [Header("Audio Clips")]
    public AudioClip homeMenuMusic;
    public AudioClip sampleSceneMusic;
    public AudioClip christmasMusic;
    public AudioClip happynewyearMusic;

    private AudioSource audioSource;
    private bool isMusicOn = true; // trạng thái nhạc

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // giữ nhạc khi đổi scene
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.loop = true;
        audioSource.playOnAwake = false;
    }

    // Bật/Tắt nhạc từ MainMenu
    public void ToggleMusic()
    {
        isMusicOn = !isMusicOn;
        if (isMusicOn)
            audioSource.Play();
        else
            audioSource.Pause();
    }

    // Lấy trạng thái nhạc
    public bool IsMusicOn() => isMusicOn;

    // Phát nhạc theo scene
    public void PlayMusicForScene(string sceneName)
    {
        AudioClip clipToPlay = null;

        switch (sceneName)
        {
            case "HomeMenu":
                clipToPlay = homeMenuMusic;
                break;

            case "SampleScene":
                clipToPlay = sampleSceneMusic;
                break;

            case "ChristmasDay":
            case "next_level2":               // 2 scene dùng chung nhạc Christmas
                clipToPlay = christmasMusic;
                break;

            case "Happy New Year":
            case "next_level3":               // 2 scene dùng chung nhạc Happy New Year
                clipToPlay = happynewyearMusic;
                break;

            case "game_over":
                clipToPlay = homeMenuMusic;
                break;
                
            default:
                clipToPlay = homeMenuMusic;
                break;
        }


        if (clipToPlay != null && audioSource.clip != clipToPlay)
        {
            audioSource.clip = clipToPlay;
            if (isMusicOn)
                audioSource.Play();
            else
                audioSource.Pause(); // giữ trạng thái ON/OFF
        }
    }
}
