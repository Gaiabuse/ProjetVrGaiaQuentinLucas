using UnityEngine;

// Permet de créer l'asset via le menu Unity: Assets -> Create -> Succès VR
[CreateAssetMenu(fileName = "NewSuccessData", menuName = "Succès VR/Données de Succès")]
public class SuccessData : ScriptableObject
{
    [Header("Identification")]
    [Tooltip("L'ID unique pour PlayerPrefs (ex: 'CLE_SECRETE_GRAB').")]
    public string successName;
    
    [Header("Contenu")]
    [Tooltip("La description de l'objet ou du succès qui sera affichée.")]
    [TextArea]
    public string descriptionSuccess;
    public string descriptionObjet;
}