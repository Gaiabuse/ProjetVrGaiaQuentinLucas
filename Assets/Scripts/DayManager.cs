
using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class DayManager : MonoBehaviour
{
    
    public static DayManager instance;
    [SerializeField] List<DayData> days = new List<DayData>();
    [SerializeField] DialogueRunner dialogueRunner;
    [SerializeField] XRGrabInteractable[] doors;
    private PlayerConditionManager playerCondition;
    [SerializeField] private Image fadeImage;
    [SerializeField] private int currentDay;
    private int currentDayIndex = -1;
    [SerializeField] private bool autoStart = true;
    [SerializeField] private float durationFade = 0.5f;
    private bool timerFinished = false;
    private float duration = 0f;

    private void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
        }
        else
        {
            instance = this;
        }
    }

    void Start()
    {
        playerCondition = PlayerConditionManager.instance;
        if (autoStart)
        {
            StartCoroutine(DayLoop());
        }
    }

   
    public IEnumerator DayLoop()
    {
        for (int i = 0; i < days.Count; i++)
        {
            timerFinished = false;
            currentDayIndex = i;
            DayData day = days[i];
            Tween fade = null;
            if (day.fadeAtStart)
            {
                fade = fadeImage.DoFadeInAndOut(durationFade);
            }

            if (day.doorsLockedIndex.Length > 0)
            {
                foreach (var door in doors)
                {
                    door.enabled = true;
                }
                foreach (int index in day.doorsLockedIndex)
                {
                    if(doors.Length > index)
                        doors[index].enabled = false;
                }
            }
            if (fade != null)
            {
                yield return new WaitUntil(() => fade.IsComplete());
            }

           
            
            DialogueGraph dialogue = ChooseDialogueForDay(day);

            if (dialogue != null)
            {
                dialogueRunner.StartDialogue(dialogue);
            }
            else
            {
                Debug.LogWarning("Aucun dialogue trouvé pour le jour " + day.dayName);
            }
            
            duration = day.durationInMinutes * 60f;
            yield return new WaitUntil(() => timerFinished);

            Debug.Log("Fin du jour : " + day.dayName);
        }
    }
    
    

    DialogueGraph ChooseDialogueForDay(DayData day)
    {
        DialogueOption fallback = null;

        foreach (var option in day.dialogueOptions)
        {
            if (option.isFallback)
            {
                fallback = option;
                continue;
            }
            
            bool conditionsComplete = true;
            foreach (var cond in option.conditions)
            {
                if (cond == null) continue;

                if (!cond.isComplete(playerCondition))
                {
                    conditionsComplete = false;
                    break;
                }
            }

            if (conditionsComplete)
            {
                Debug.Log("condition complete");
                return option.dialogue;
            }
        }
        
        if (fallback != null)
        {
            Debug.Log("no condition for dialogue complete");
            return fallback.dialogue;
        }
        
        return null;
    }
    public IEnumerator StartDayTimer()
    {
        timerFinished = false;
        yield return new WaitForSeconds(duration);
        timerFinished = true;
    }
}


[Serializable]
public class DayData
{
    public string dayName;
    [Tooltip("duration of the day")]
    public float durationInMinutes = 1f;

    [Tooltip(" one index > at max doors  for open all doors")]
    public int[] doorsLockedIndex;

    public bool fadeAtStart = false;

    [Tooltip("All dialogues possibility")] public DialogueOption[] dialogueOptions;
}
