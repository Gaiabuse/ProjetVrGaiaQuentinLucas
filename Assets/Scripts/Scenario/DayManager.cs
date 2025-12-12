using System;
using System.Collections;
using System.Collections.Generic;
using Data.Conditions;
using DG.Tweening;
using Exploration;
using Nodes;
using UnityEngine;

namespace Scenario
{
    public class DayManager : MonoBehaviour
    {
    
        public static DayManager INSTANCE;
        [SerializeField] List<DayData> days = new List<DayData>();
        [SerializeField] DialogueRunner dialogueRunner;
        [SerializeField] HingeJoint[] doors;
        [SerializeField] private CanvasGroup cvg;
        [SerializeField] private int currentDay;
        [SerializeField] private bool autoStart = true;
        [SerializeField] private float durationFade = 0.5f;
        [SerializeField] private float maxValueDoor = 130f;
    
        private bool _dayTimerFinished;
        private float _durationOfDay;
    
        private readonly PlayerManager _player = PlayerManager.INSTANCE;
        private void Awake()
        {
            if (INSTANCE != null)
            {
                Destroy(gameObject);
            }
            else
            {
                INSTANCE = this;
            }
        }

        void Start()
        {
            if (autoStart)
            {
                StartCoroutine(DayLoop());
            }
        }

   
        public IEnumerator DayLoop()
        {
            foreach (DayData dayData in days)
            {
                _dayTimerFinished = false;
                DayData day = dayData;
                bool fadeIsFinish = true;
                if (day.fadeAtStart)
                {
                    fadeIsFinish = false;
                    cvg.DOKill();
                    cvg.alpha = 1f;
                    cvg.DOFade(0f, durationFade).SetEase(Ease.OutBounce).OnComplete(() =>fadeIsFinish = true);
                }
                yield return new WaitUntil(() => fadeIsFinish);
                if (day.doorsLockedIndex.Length > 0)
                {
                    foreach (var door in doors)
                    {
                        var limits = door.limits;
                        limits.max = maxValueDoor;
                        door.limits = limits;
                    }
                    foreach (int index in day.doorsLockedIndex)
                    {
                        if(doors.Length > index)
                        {
                            var limits = doors[index].limits;
                            limits.max = 0;
                            doors[index].limits = limits;
                        }
                    }
                }
                DialogueGraph dialogue = SelectDialogueOfTheDay(day);

                if (dialogue != null)
                {
                    dialogueRunner.StartDialogue(dialogue);
                }
                else
                {
                    Debug.LogWarning("Aucun dialogue trouvé pour le jour " + day.dayName);
                }
            
                _durationOfDay = day.durationInMinutes * 60f;
                yield return new WaitUntil(() => _dayTimerFinished);// Wait Day is Finished and go to next day
            }
        }
    
        DialogueGraph SelectDialogueOfTheDay(DayData day)
        {
            DialogueOption fallback = null;// dialogue option if no dialogue with condition is complete

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

                    if (cond.IsComplete(_player)) continue;
                    conditionsComplete = false;
                    break;
                }

                if (conditionsComplete)
                {
                    return option.dialogue;
                }
            }
        
            return fallback?.dialogue;
        }
        public IEnumerator StartDayTimer()
        {
            _dayTimerFinished = false;
            yield return new WaitForSeconds(_durationOfDay);
            _dayTimerFinished = true;
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

    [Serializable]
    public class DialogueOption 
    {
        public DialogueGraph dialogue;
        [Tooltip("if true, it's default dialogue with no conditions")]
        [SerializeField]public bool isFallback = false;
        public Condition[] conditions;
    }
}