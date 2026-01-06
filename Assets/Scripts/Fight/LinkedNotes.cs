using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using CommonUsages = UnityEngine.XR.CommonUsages;

namespace Fight
{
    public class LinkedNotes : NoteScript
    {
        [SerializeField] private int nextNoteMaxDistance = 6;
        [SerializeField] private float counterDamages = -2f;
        [SerializeField] private MeshRenderer meshRenderer;
        [SerializeField] private GameObject particles;
        [SerializeField] private float maxLineTime = 1.5f;
        [SerializeField] private Link link;
        [SerializeField] private Collider collider;
    
        private Vector3 _nextNotePos;
        private Vector3Int _sheetMusicPosition;
        private bool _isFirstLinked = false;
        private int _maxMeasure;
        private int _maxBeat;
        private int _maxDivision;
        private bool _canHavePoints = true;

        private const int _linkedNoteIndex = 2;

        protected override void Awake()
        {
            base.Awake();
            _maxMeasure = FightManager.INSTANCE.GetLevel().SheetMusic.GetLength(0);
            _maxBeat = FightManager.INSTANCE.GetLevel().SheetMusic.GetLength(1);
            _maxDivision = FightManager.INSTANCE.GetLevel().SheetMusic.GetLength(2);
        }

        public void ChangeSheetMusicPosition(Vector3Int newPosition)
        {
            _sheetMusicPosition = newPosition;
            int divisionToNextNote = CountDivisionsToNextNote(_sheetMusicPosition.x,_sheetMusicPosition.y,_sheetMusicPosition.z);
            if (divisionToNextNote <= nextNoteMaxDistance)
            {
                FightManager.INSTANCE.CanLinkState(true);
                if (FightManager.INSTANCE.TakeFirstLinked())
                {
                    _isFirstLinked = true;
                }
                CheckNextNote(_sheetMusicPosition.x,_sheetMusicPosition.y,_sheetMusicPosition.z);
            }
            else
            {
                FightManager.INSTANCE.CanLinkState(false);
                FightManager.INSTANCE.ReleaseFirstLinked();
                Destroy(link.gameObject);
            }
        }

        void CheckNextNote(int measure , int beat , int division)
        {
            int remainingDistance = nextNoteMaxDistance;
            while (remainingDistance > 0)
            {
                division++;
                if (division >= _maxDivision)
                {
                    division = 0;
                    beat++;
                    if (beat >= _maxBeat)
                    {
                        beat = 0;
                        measure++;
                        if (measure >= _maxMeasure)
                            break;
                    }
                }

                int noteType = FightManager.INSTANCE.GetNote(measure, beat, division);
                if (noteType == _linkedNoteIndex)
                {
                    _nextNotePos = FightManager.INSTANCE.GetPos(measure, beat, division);
                    LinkNote();
                
                    break;
                }

                remainingDistance--;
            }
    
        }
        int CountDivisionsToNextNote(int measure, int beat, int division)
        {
            int divisionsCounted = 0;
            while (divisionsCounted < nextNoteMaxDistance + 1)
            {
                division++;
                if (division >= _maxDivision)
                {
                    division = 0;
                    beat++;
                    if (beat >= _maxBeat)
                    {
                        beat = 0;
                        measure++;
                        if (measure >= _maxMeasure)
                            break;
                    }
                }

                divisionsCounted++;
                int noteType = FightManager.INSTANCE.GetNote(measure, beat, division);
                if (noteType == _linkedNoteIndex)
                {
                    return divisionsCounted;
                }
            }
            return divisionsCounted;
        }

        void LinkNote()
        {
            int divisionsToNextNote = CountDivisionsToNextNote(_sheetMusicPosition.x,_sheetMusicPosition.y,_sheetMusicPosition.z);
            float timePerDivision = 60f / FightManager.INSTANCE.GetLevel().Bpm / FightManager.INSTANCE.GetLevel().Beat;
            float duration = timePerDivision * divisionsToNextNote;
            link.Move(_nextNotePos, duration);
        }

        protected override void OnTriggerEnter(Collider other)
        {
            if (FightManager.INSTANCE.IsInLine())
            {
                base.OnTriggerEnter(other);
                StartCoroutine(PlayerInTrigger());
            }
        }

        private IEnumerator PlayerInTrigger()
        {
            while (_inTrigger)
            {
                bool rightPressed = _rightHand.TryGetFeatureValue(CommonUsages.trigger, out float triggerValueRight) && triggerValueRight > 0.1f;
                bool leftPressed  = _leftHand.TryGetFeatureValue(CommonUsages.trigger, out float triggerValueLeft) && triggerValueLeft > 0.1f;
                if (rightPressed || leftPressed)
                {
                    if (_canHavePoints)
                    {
                        if (Time.time - spawnTime < timeForPerfect)
                        {
                            FightManager.INSTANCE.AddAnxiety(counterDamages * perfectMultiplier);
                            GameObject perfectGO = Instantiate(perfectEffectPrefab);
                            perfectGO.transform.position = transform.position;
                            Debug.Log("Perfect");
                        }
                        else
                        {
                            FightManager.INSTANCE.AddAnxiety(counterDamages);
                        }
                        if (rightPressed)
                        {
                            _rightHand.SendHapticImpulse(0, 0.5f, maxLineTime);
                        }
                        else
                        {
                            _leftHand.SendHapticImpulse(0, 0.5f, maxLineTime);
                        }
                        StartCoroutine(WaitForDestroy());
                    }
                }
                else if (!_isFirstLinked)
                {
                    FightManager.INSTANCE.ChangeInLineState(false);
                    yield break;
                }

                yield return null;
            
            }
        }
    

        IEnumerator WaitForDestroy()
        {
            FightManager.INSTANCE.AddAnxiety(counterDamages);
            Destroy(meshRenderer);
            Destroy(particles);
            _canHavePoints = false;
            yield return new WaitForSeconds(maxLineTime);
            Destroy(gameObject);
        }

        protected override void OnRightPressed(InputAction.CallbackContext ctx)
        {
            return;
        }

        protected override void OnLeftPressed(InputAction.CallbackContext ctx)
        {
            return;
        }
    }
}
