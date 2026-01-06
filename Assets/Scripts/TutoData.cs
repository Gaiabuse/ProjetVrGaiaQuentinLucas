using UnityEngine;

[CreateAssetMenu(fileName = "TutoData", menuName = "Scriptable Objects/TutoData")]
public class TutoData : ScriptableObject
{
    public Sprite Illustration;
    [field:TextArea] public string Text;
    public float TimeBeforeNextWindow;
}
