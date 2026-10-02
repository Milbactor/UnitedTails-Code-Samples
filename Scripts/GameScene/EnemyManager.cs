using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Zenject;

namespace WhiteKNight
{
    public class EnemyManager : MonoBehaviour, ICharacterRegistry
    {
        public static EnemyManager Instance { get; private set; }

        private List<IEnemyDeathListener> _player = null;
        public List<IEnemyDeathListener> Player => _player;

        private List<IEnemyDeathListener> _npc = null;
        public List<IEnemyDeathListener> Npc => _npc;

        [SerializeField] private ResultUIView _resultUIView;

        private int _deadEnemyCount = 0;

        private void Awake()
        {
            Instance = this;
        }

        [SerializeField] private int totalEnemyCount = 0;
        [SerializeField] private IEnemyView[] enemies;
        [SerializeField] private GameObject[] enemyGameObjects;
        [SerializeField] private GameObject[] balls;
        private ILeftEnemiesUseCase _leftEnemiesUseCase; 

        [Inject]
        public void Construct(ILeftEnemiesUseCase leftEnemiesUseCase)
        {
            _leftEnemiesUseCase = leftEnemiesUseCase;
            totalEnemyCount = enemyGameObjects.Length;
            foreach (var ball in balls)
            {
                ball.SetActive(false);
            }
            _leftEnemiesUseCase.InitializeEnemiesCounters(totalEnemyCount);
        }

        public void Initialize( List<IEnemyDeathListener> npc)
        {      
            _npc = npc;
        }

        public void OnEnemyDied(IEnemyView enemy)
        {
            _leftEnemiesUseCase.OnEnemyDied();
            NotifyEnemyDied(enemy);
            _deadEnemyCount++;
            if (_deadEnemyCount == totalEnemyCount)
            {
                _resultUIView.gameObject.SetActive(true);
                _resultUIView.OnWin();
            }

            if (_deadEnemyCount == totalEnemyCount-2)
            {
                foreach (var ball in balls)
                {
                    ball.SetActive(true);
                }
            }
        }

        public void NotifyEnemyDied(IEnemyView enemy)
        {
            foreach (var listener in _npc)
            {
                listener.OnEnemyDied(enemy);
            }
        }
    }

    public interface IEnemyDeathListener
    {
        void OnEnemyDied(IEnemyView enemy);
    }

    public class EnemyDeathListener : MonoBehaviour, IEnemyDeathListener
    {
        public void OnEnemyDied(IEnemyView enemy)
        {
           //TODO you must write tasks to do when enemy dies
        }
    }

    public interface ICharacterRegistry
    {
        List<IEnemyDeathListener> Player { get; }
        List<IEnemyDeathListener> Npc { get; }

        void Initialize(
         //  List<IEnemyDeathListener> player,
           List<IEnemyDeathListener> npc);

        void NotifyEnemyDied(IEnemyView enemy);
    }

    public class CharacterRegistry : ICharacterRegistry
    {
        private List<IEnemyDeathListener> _player;
        private List<IEnemyDeathListener> _npc;

        public List<IEnemyDeathListener> Player => _player;

        public List<IEnemyDeathListener> Npc => _npc;

        public void Initialize(
         // List<IEnemyDeathListener> player,
          List<IEnemyDeathListener> npc)
        {
           // _player = player;
            _npc = npc;
        }

        public void NotifyEnemyDied(IEnemyView enemy)
        {
            foreach(var listener  in _player)
            {
                listener.OnEnemyDied(enemy);
            }
            foreach(var listener in _npc)
            {
                listener.OnEnemyDied(enemy);
            }
        }
    }
}