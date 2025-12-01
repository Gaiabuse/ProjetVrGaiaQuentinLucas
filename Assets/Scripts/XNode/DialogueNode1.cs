using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XNode;

public class WaitNode : BaseNode
{
    [Input] public string Entry;
    [Output] public string continued;
    
    public int timeWaitingInMinutes;

    public override string GetString()
    {
        return "Wait";
    }
}