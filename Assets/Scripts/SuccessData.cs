using UnityEngine;

// Permet de créer l'asset via le menu Unity: Assets -> Create -> Succès VR
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