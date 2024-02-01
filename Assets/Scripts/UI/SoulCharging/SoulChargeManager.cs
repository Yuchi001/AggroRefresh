using System.Collections.Generic;
using System.Linq;
using PlayerPack.PlayerOngoingStatsPack;
using SoulPack.Enum;
using UnityEngine;

namespace UI.SoulCharging
{
    public class SoulChargeManager : MonoBehaviour
    {
        [SerializeField] private int soulPointsMax;
        [SerializeField] private GameObject slotQ;
        [SerializeField] private GameObject slotE;

        private Dictionary<ESoulType, int> _soulPointsDict = new();
        private int _currentSoulPoints = 0;
        private bool _soulReady = false;

        private ESoulType? _firstPickedSoul = null;
        private ESoulType? _secondPickedSoul = null;

        public void PickSoulPoints(ESoulType soulType, int points)
        {
            _currentSoulPoints += points;
            if (_soulPointsDict.TryAdd(soulType, points) && !_soulReady) return;

            _soulPointsDict[soulType] += points;

            if (!_soulReady) return;
            
            var sortedDict = from entry in _soulPointsDict orderby entry.Value ascending select entry;
            _firstPickedSoul = sortedDict.ElementAt(0).Key;
            _secondPickedSoul = sortedDict.ElementAt(1).Key;
        }
        
        private void Update()
        {
            if (!_soulReady && _currentSoulPoints >= soulPointsMax)
            {
                _soulReady = true;
                var sortedDict = from entry in _soulPointsDict orderby entry.Value ascending select entry;
                _firstPickedSoul = sortedDict.ElementAt(0).Key;
                _secondPickedSoul = sortedDict.ElementAt(1).Key;
                // todo: set slotQ to first dict element
                // todo: set slotE to second dict element
            }
            
            slotQ.SetActive(_soulReady);   
            slotE.SetActive(_soulReady);   
            
            if (Input.GetKeyDown(KeyCode.Q) && _soulReady && _firstPickedSoul is not null)
            {
                PickSoul(_firstPickedSoul.Value);
            }

            if (Input.GetKeyDown(KeyCode.E) && _soulReady && _secondPickedSoul is not null)
            {
                PickSoul(_secondPickedSoul.Value);
            }
        }

        private void PickSoul(ESoulType soulType)
        {
            PlayerOngoingStats.Instance.PickSoul(soulType);
            _soulReady = false;
            _firstPickedSoul = null;
            _secondPickedSoul = null;
            _soulPointsDict.Clear();
            _currentSoulPoints = 0;
        }
    }
}