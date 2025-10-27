using UnityEngine;

[CreateAssetMenu(fileName = "LevelData", menuName = "Scriptable Objects/LevelData")]
public class LevelData : ScriptableObject, ISerializationCallbackReceiver
{
    public AudioClip audioClip;
    public AudioClip noteSound;
    public int[,,] sheetMusic;
    public Vector2[,,] spawnPositions;
    public int bpm;
    public int beat;
    public int division;

    #region 3d Array serialization
    [SerializeField] private Vector3Int sheetMusicSize;
    [SerializeField] private Vector3Int spawnPositionSize;
    [SerializeField] private int[] sheetMusicData;
    [SerializeField] private Vector2[] spawnPositionData;
    public void OnBeforeSerialize()
    {
        if (sheetMusic != null)
        {
            sheetMusicSize = new Vector3Int(sheetMusic.GetLength(0), sheetMusic.GetLength(1), sheetMusic.GetLength(2));
            sheetMusicData = new int[sheetMusicSize.x * sheetMusicSize.y * sheetMusicSize.z];

            int index = 0;
            for (int x = 0; x < sheetMusicSize.x; x++)
                for (int y = 0; y < sheetMusicSize.y; y++)
                    for (int z = 0; z < sheetMusicSize.z; z++)
                        sheetMusicData[index++] = sheetMusic[x, y, z];
        }
        
        if (spawnPositions != null)
        {
            spawnPositionSize = new Vector3Int(spawnPositions.GetLength(0), spawnPositions.GetLength(1), spawnPositions.GetLength(2));
            spawnPositionData = new Vector2[spawnPositionSize.x * spawnPositionSize.y * spawnPositionSize.z];

            int index = 0;
            for (int x = 0; x < spawnPositionSize.x; x++)
                for (int y = 0; y < spawnPositionSize.y; y++)
                    for (int z = 0; z < spawnPositionSize.z; z++)
                        spawnPositionData[index++] = spawnPositions[x, y, z];
        }
    }

    public void OnAfterDeserialize()
    {
        if (sheetMusicData != null && sheetMusicSize != Vector3Int.zero)
        {
            sheetMusic = new int[sheetMusicSize.x, sheetMusicSize.y, sheetMusicSize.z];
            int index = 0;
            for (int x = 0; x < sheetMusicSize.x; x++)
                for (int y = 0; y < sheetMusicSize.y; y++)
                    for (int z = 0; z < sheetMusicSize.z; z++)
                        sheetMusic[x, y, z] = sheetMusicData[index++];
        }
        
        if (spawnPositionData != null && spawnPositionSize != Vector3Int.zero)
        {
            spawnPositions = new Vector2[spawnPositionSize.x, spawnPositionSize.y, spawnPositionSize.z];
            int index = 0;
            for (int x = 0; x < spawnPositionSize.x; x++)
                for (int y = 0; y < spawnPositionSize.y; y++)
                    for (int z = 0; z < spawnPositionSize.z; z++)
                        spawnPositions[x, y, z] = spawnPositionData[index++];
        }
    }
    #endregion
}
