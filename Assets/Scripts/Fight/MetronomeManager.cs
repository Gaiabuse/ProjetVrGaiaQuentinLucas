using UnityEngine;
using UnityEngine.Serialization;

namespace Fight
{
    public class MetronomeManager : MonoBehaviour 
    {
        public AudioSource AudioSourceMusic;
    
        [SerializeField] private AudioSource audioSourceMetronome;
        [SerializeField] private AudioClip firstMetronome; 
        [SerializeField] private AudioClip otherMetronome;
        [SerializeField] private int timeSignature = 4;
        [SerializeField] private float initialBpm = 120;
        [SerializeField] private float endMusicCutTime = 0.5f;
   
    
        private int _division = 1;
        private int _beatCpt = 0;
        private int _divisionCpt = 0;
    
        private float _actualTimeBetweenBeat;
        private float _audioClipLength;

        private bool _isFirstMeasure = true;
        private int _actualMeasure = 0;
        private int _actualBeat = 0;
        private int _actualDivision = 0;

        private float _secondsPerBeat;
        private float _secondsPerDivision;
        private int _currentBeat;
        private int _currentDivision;
        
        // évite au maximum les grands bouts de logiques dans l'update, tu ne devrais avoir que quelques appels de fonction
        // là c'est une énorme poubelle contenant une tonne de choses dont on n'a aucune idée de ce qu'elles font
        private void Update()
        {
            if (!AudioSourceMusic.isPlaying)
            {
                return;
            }

            if (_audioClipLength - endMusicCutTime <= AudioSourceMusic.time)
            {
                FightManager.INSTANCE.EndFight(true);
                return;
            }
       
            // attention aux magic numbers
            initialBpm = Mathf.Clamp(initialBpm, 40, 260);
            _secondsPerBeat = 60f / initialBpm;
            _secondsPerDivision = (60f / initialBpm)/_division;
            _currentBeat = Mathf.FloorToInt(AudioSourceMusic.time / _secondsPerBeat) % timeSignature + 1;
            _currentDivision= Mathf.FloorToInt(AudioSourceMusic.time / _secondsPerDivision) % timeSignature + 1;
            if (_currentBeat != _beatCpt)
            {
            
                _beatCpt = _currentBeat;
                if (_beatCpt == 1)
                {
                    if (!_isFirstMeasure)
                    {
                        _actualMeasure++;
                    }
                    else
                    {
                        _isFirstMeasure = false;
                    }
                    _actualBeat = 0;
                    audioSourceMetronome.PlayOneShot(firstMetronome);
                }
                else
                {
                    _actualBeat++;
                    audioSourceMetronome.PlayOneShot(otherMetronome);
                }
                _actualDivision = 0;
            }
            else if (_currentDivision != _divisionCpt )
            {
                _divisionCpt = _currentDivision;
                audioSourceMetronome.PlayOneShot(otherMetronome);
                _actualDivision++;
                FightManager.INSTANCE.NoteSpawn(_actualMeasure, _actualBeat, _actualDivision - 1);
                FightManager.INSTANCE.NotePrevisualisation(_actualMeasure, _actualBeat, _actualDivision - 1);
            }
        }

        public void ChangeValues(AudioClip newSong, float newBpm = 120, int newTimeSignature = 4, int newDivision = 1)
        {
            _audioClipLength = newSong.length;
            AudioSourceMusic.clip = newSong;
            initialBpm = newBpm;
            timeSignature = newTimeSignature;
            _division = newDivision;
        }

        // ta fonction EndFight, au final, elle endfight pas, elle fait un reset, gaffe au naming
        public void EndFight()
        {
            AudioSourceMusic.Stop();
            audioSourceMetronome.Stop();
            _actualBeat = 0;
            _actualDivision = 0;
            _actualMeasure = 0;
        }
    
    }
}

