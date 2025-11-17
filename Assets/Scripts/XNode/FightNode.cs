using UnityEngine;

public class FightNode : BaseNode
{
    [Input] public string Entry;
    [Output] public string AsWin;
    [Output] public string AsLoose;

    public LevelData level;

    public override string GetString()
    {
        return "Fight";
    }
}
