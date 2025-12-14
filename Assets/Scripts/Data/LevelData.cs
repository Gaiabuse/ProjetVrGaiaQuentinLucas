using UnityEngine;
using UnityEngine.Serialization;

namespace Data
{
    [CreateAssetMenu(fileName = "LevelData", menuName = "Scriptable Objects/LevelData")]
    public class LevelData : ScriptableObject, ISerializationCallbackReceiver
    {
        public AudioClip AudioClip;
        public AudioClip NoteSound;
        // ok j'crois que je commence à comprendre apres relecture, x c'est genre measure, y c'est bpm, et z division? 
        // c'est pas incroyable niveau maintien et compréhension
        // ca aurait pu être pas mal d'utiliser par exemple une struct ou une class comme ca:
      //  struct note
      //  {
      //      int measure, beat, division
      //  }
      //  
      //  public note[] SheetMusic
        
        public int[,,] SheetMusic;
        public Vector2[,,] SpawnPositions;
        public int Bpm;
        public int Beat;
        public int Division;

        #region 3d Array serialization
    
        [SerializeField] private Vector3Int sheetMusicSize;
        [SerializeField] private Vector3Int spawnPositionSize;
        [SerializeField] private int[] sheetMusicData;
        [SerializeField] private Vector2[] spawnPositionData;
        public void OnBeforeSerialize()
        {
            if (SheetMusic != null)
            {
                sheetMusicSize = new Vector3Int(SheetMusic.GetLength(0), SheetMusic.GetLength(1), SheetMusic.GetLength(2));
                sheetMusicData = new int[sheetMusicSize.x * sheetMusicSize.y * sheetMusicSize.z];

                int index = 0;
                for (int x = 0; x < sheetMusicSize.x; x++)
                for (int y = 0; y < sheetMusicSize.y; y++)
                for (int z = 0; z < sheetMusicSize.z; z++)
                    sheetMusicData[index++] = SheetMusic[x, y, z];
            }
        
            if (SpawnPositions != null)
            {
                spawnPositionSize = new Vector3Int(SpawnPositions.GetLength(0), SpawnPositions.GetLength(1), SpawnPositions.GetLength(2));
                spawnPositionData = new Vector2[spawnPositionSize.x * spawnPositionSize.y * spawnPositionSize.z];

                int index = 0;
                // waaaa premiere fois de ma vie que je vois le triple for comme ca, ca fait peur, mais si ca marche tant mieux
                // parcontre, aucune idée de ce que ca doit faire, mais ca fait tres peur la triple imbrication
                for (int x = 0; x < spawnPositionSize.x; x++)
                for (int y = 0; y < spawnPositionSize.y; y++)
                for (int z = 0; z < spawnPositionSize.z; z++)
                    spawnPositionData[index++] = SpawnPositions[x, y, z];
            }
        }

        public void OnAfterDeserialize()
        {
            if (sheetMusicData != null && sheetMusicSize != Vector3Int.zero)
            {
                SheetMusic = new int[sheetMusicSize.x, sheetMusicSize.y, sheetMusicSize.z];
                int index = 0;
                for (int x = 0; x < sheetMusicSize.x; x++)
                for (int y = 0; y < sheetMusicSize.y; y++)
                for (int z = 0; z < sheetMusicSize.z; z++)
                    SheetMusic[x, y, z] = sheetMusicData[index++];
            }
        
            if (spawnPositionData != null && spawnPositionSize != Vector3Int.zero)
            {
                SpawnPositions = new Vector2[spawnPositionSize.x, spawnPositionSize.y, spawnPositionSize.z];
                int index = 0;
                for (int x = 0; x < spawnPositionSize.x; x++)
                for (int y = 0; y < spawnPositionSize.y; y++)
                for (int z = 0; z < spawnPositionSize.z; z++)
                    SpawnPositions[x, y, z] = spawnPositionData[index++];
            }
        }
        #endregion
    }
}
