using System;
using System.Collections;
using System.Collections.Generic;
using Custom.Logic.UI;
using Engine;
using Engine.DI;
using example1;
using Main;
using Main.Level;
using Unity.Mathematics;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Custom.Logic
{
    public class EnemySpawner : MonoBehaviour,ILevelStarted,ILevelCompleted, ILevelFailed
    {
        [SerializeField] private GameObject _enemyPrefab;
        [SerializeField] private int _count=10;
        [SerializeField] private float _delay=3f;
        [SerializeField] private float _spawnDistance = 30f;
        private ILevel _levelInfo;
        [SerializeField] private Collider _floorColider;
        [SerializeField] private Transform[] _spawnPoints;


        public void OnEnable()
        {
            
            LevelStatueStarted.Subscribe(this);
            LevelStatueFailed.Subscribe(this);
      
        }
        public void OnDisable()
        {
            LevelStatueStarted.Unsubscribe(this);
            LevelStatueFailed.Unsubscribe(this);
        }

        public void SetUp(int count, float delay)
        {
            _count = count;
            _delay = delay;
            
        }
        

        public void LevelStarted()
        {
            _levelInfo = DIContainer.GetAsSingle<ILevelsManager>().level;
            SetUp(_levelInfo.WaveInfos[_levelInfo.CurrentWave].NumberOfEnemys,_levelInfo.WaveInfos[_levelInfo.CurrentWave].DelayBetweenSpawn);
            StartCoroutine(SpawningEnemy());
           
        }

        private IEnumerator SpawningEnemy()
        {
            while (_count>0)
            {
                _count--;
                /*Vector2 p = Random.insideUnitCircle.normalized * _spawnDistance;
                Vector3 newPoint =
                    _floorColider.ClosestPoint(_levelInfo.PlayerInformation.CameraPoint.position +
                                               new Vector3(p.x, 0, p.y));*/
                bool canSpawn=false;
                Vector3 spawnPosition=_spawnPoints[Random.Range(0, _spawnPoints.Length)].position;
                while (!canSpawn)
                {
                    spawnPosition = _spawnPoints[Random.Range(0, _spawnPoints.Length)].position;
                    Collider[] hitColliders = Physics.OverlapSphere(spawnPosition, 3);
                    foreach (var hitCollider in hitColliders)
                    {
                        if (hitCollider.TryGetComponent(out Player player))
                        {
                            spawnPosition = _spawnPoints[Random.Range(0, _spawnPoints.Length)].position;
                            canSpawn = false;
                            break;
                        }
                        else
                        {
                            canSpawn = true;
                        }
                    }
                }
                GameObject _enemy=Instantiate(_enemyPrefab,spawnPosition , Quaternion.identity);
                _enemy.GetComponent<EnemyNavMeshDestination>().Init();
                _levelInfo.SetCount(_levelInfo.CurrentWave,_count);

                if (_count <= 0)
                {
                    if (_levelInfo.WaveInfos.Count-1 > _levelInfo.CurrentWave)
                    {
                        _levelInfo.CurrentWave++;
                        SetUp(_levelInfo.WaveInfos[_levelInfo.CurrentWave].NumberOfEnemys,_levelInfo.WaveInfos[_levelInfo.CurrentWave].DelayBetweenSpawn);
                        Debug.Log("NewWave");
                        DIContainer.GetAsSingle<IBufsContainer>().ShowPanel();
                    }
                    else
                    {
                        DIContainer.GetAsSingle<IMakeCompleted>().MakeCompleted();
                    }
                    
                    
                }
                yield return new WaitForSeconds(_delay);
            }
            
        }

        public void LevelCompleted()
        {
           StopAllCoroutines();
        }

        public void LevelFailed()
        {
            StopAllCoroutines();
        }
    }
}
