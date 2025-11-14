using System;
using UnityEngine;

public class LinkedNotes : NoteScript
{
    [SerializeField] private int nextNoteMaxDistance = 6;
    
    private Vector3 _nextNotePos;
    private bool _isReleased = false;
    private Vector3Int _sheetMusicPosition;

    private const int LinkedNoteIndex = 2;
    // il faudrait que la preview des notes la viennent que pour la premiere et qu'il faille suivre la ligne après


    public void ChangeSheetMusicPosition(Vector3Int newPosition)
    {
        _sheetMusicPosition = newPosition;
    }
    private void OnEnable() 
    {
        CheckNextNote();
    }
    
    public void Released()
    {
        _isReleased = true;
    }

    void CheckNextNote()
    {
        for (int i = 0; i < nextNoteMaxDistance; i++)
        {
            if (FightManager.INSTANCE.GetNote(_sheetMusicPosition.x, _sheetMusicPosition.y, _sheetMusicPosition.z) ==
                LinkedNoteIndex)
            {
                _nextNotePos = FightManager.INSTANCE.GetPos(_sheetMusicPosition.x, _sheetMusicPosition.y,
                    _sheetMusicPosition.z);
                LinkNote();
                break;
            }
        }
    }
    
    void LinkNote()
    {
        // mettre une ligne (ça existe un component line) entre cette note et la nextNote position et il faut check si on est tjr appuyé
        // si possible la ligne apparait au fur et a mesure qu'on la suit
    }
    // peut être mettre un script qui check si on est toujours sur la ligne tracée et si on en sort on prend anxiété
    
}
