using UnityEngine;

[CreateAssetMenu(fileName = "AudioConfig", menuName = "Audio/Audio Config")]
public class AudioConfig : ScriptableObject
{
    public AudioClip MenuMusic;
    public AudioClip LevelCompleteClip;
    public AudioClip PuzzleSolvedClip;
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
