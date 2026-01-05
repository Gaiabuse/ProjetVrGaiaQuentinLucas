using UnityEngine;

[CreateAssetMenu(fileName = "TutoData", menuName = "Scriptable Objects/TutoData")]
public class TutoData : ScriptableObject
{
    public Sprite Illustration;
    public string Text;
    public float TimeBeforeNextWindow;
}
