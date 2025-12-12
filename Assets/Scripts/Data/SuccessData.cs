using UnityEngine;


namespace Data
{
    [CreateAssetMenu(fileName = "NewSuccessData", menuName = "Succès VR/Données de Succès")]
    public class SuccessData : ScriptableObject
    {
        [Header("Name")]
        public string successName;
    
        [Header("Descriptions")]
        [TextArea]
        public string descriptionSuccess;
        public string descriptionObjet;
    }
}