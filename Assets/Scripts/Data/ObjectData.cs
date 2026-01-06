using UnityEngine;
using UnityEngine.Serialization;


namespace Data
{
    [CreateAssetMenu(fileName = "ObjectData", menuName = "ObjectData")]
    public class ObjectData : ScriptableObject
    {
        [TextArea]
        public string ObjectName;
        public string DescriptionObject;
    }
}