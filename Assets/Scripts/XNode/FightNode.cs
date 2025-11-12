using UnityEngine;

public class FightNode : BaseNode
{
    [Input] public string Entry;
    [Output(dynamicPortList = true)] public string Choices;

    public string SpeakerName;

    public override string GetString()
    {
        return "Fight";
    }
}
