using UnityEngine;

[CreateAssetMenu(fileName = "AudioConfig", menuName = "Audio/Audio Config")]
public class AudioConfig : ScriptableObject
{
    public AudioClip MenuMusic;
    public AudioClip LevelCompleteClip;
    public AudioClip PuzzleSolvedClip;
    public AudioClip Footsteps;
    public AudioClip Wind;
    public AudioClip Door_open;
    public AudioClip Door_close;
    public AudioClip Pressure_plate_down;
    public AudioClip Pressure_plate_up;
    public AudioClip Echo_area;
    public AudioClip Echo_area_activated;
    public AudioClip Echo_playback;
    public AudioClip Echo_recording; 
    public SceneMusicEntry[] SceneMusic;

    public AudioClip GetSceneClip(string sceneName)
    {
        if (SceneMusic == null)
            return null;

        for (int i = 0; i < SceneMusic.Length; i++)
        {
            if (string.Equals(SceneMusic[i].SceneName, sceneName))
                return SceneMusic[i].Clip;
        }

        return null;
    }
}

[System.Serializable]
public class SceneMusicEntry
{
    public string SceneName;
    public AudioClip Clip;
}
