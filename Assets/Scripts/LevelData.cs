using UnityEngine;

[CreateAssetMenu(fileName = "LevelData", menuName = "Scriptable Objects/LevelData")]
public class LevelData : ScriptableObject
{
    public AudioClip audioClip;
    public AudioClip noteSound;
    public bool[,,] sheetMusic;
    public Vector2[,,] spawnPositions;
    public int bpm;
    public int beat;
    public int division;
}
