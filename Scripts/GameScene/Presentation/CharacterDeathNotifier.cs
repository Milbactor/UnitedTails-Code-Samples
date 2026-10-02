using System.Collections.Generic;

namespace WhiteKNight
{
    public class CharacterDeathNotifier
    {
        private bool _isNotified = false;
        private List<ICharacterLifeCycle> _lifeCycles = new List<ICharacterLifeCycle>();
 
        public void Register(ICharacterLifeCycle characterLifeCycle)
        {
            _lifeCycles.Add(characterLifeCycle);
        }

        public void NotifyDead() 
        { 
            if(_isNotified) return;
            _isNotified = true;
            foreach (var lifeCycle in _lifeCycles) 
            { 
                lifeCycle.OnDead();
            } 
        }
    }
}