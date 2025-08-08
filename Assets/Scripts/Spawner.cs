using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class Spawner : MonoBehaviour
{
    [SerializeField, Range(.01f, 10)] private float _spawnRate = 1f;
    [SerializeField, Min(1)] private int _poolCapacity = 10;
    [SerializeField, Min(1)] private int _poolMaxSize = 100;
    [SerializeField] private Enemy _enemy;
    [SerializeField] private List<Transform> _spawnpoints;

    private ObjectPool<Enemy> _enemyPool;
    private bool _canSpawn;

    private void Awake()
    {
        _enemyPool = new ObjectPool<Enemy>(
            createFunc: CreateEnemy,
            actionOnGet: GetEnemy,
            actionOnRelease: ReleaseEnemy,
            actionOnDestroy: DestroyEnemy,
            collectionCheck: true,
            defaultCapacity: _poolCapacity,
            maxSize: _poolMaxSize
            );

        _canSpawn = true;
        StartCoroutine(Spawn());
    }

    private IEnumerator Spawn()
    {
        while (_canSpawn)
        {
            _enemyPool.Get();

            yield return new WaitForSeconds(_spawnRate);
        }
    }

    private void ReleaseEnemy(Enemy enemy)
    {
        enemy.gameObject.SetActive(false);
    }

    private void DestroyEnemy(Enemy enemy)
    {
        Destroy(enemy.gameObject);
    }

    private void GetEnemy(Enemy enemy)
    {
        enemy.transform.position = GetRandomSpawnPosition();
        enemy.transform.rotation = GetRandomRotation();

        enemy.gameObject.SetActive(true);
    }

    private Enemy CreateEnemy()
    {
        var postion = GetRandomSpawnPosition();
        var rotation = GetRandomRotation();
        var enemy = Instantiate(_enemy, postion, rotation);

        return enemy;
    }

    private Quaternion GetRandomRotation()
    {
        var minAngle = 0f;
        var maxAngle = 360f;
        var angle = Random.Range(minAngle, maxAngle);

        return Quaternion.Euler(minAngle, angle, minAngle);
    }

    private Vector3 GetRandomSpawnPosition()
    {
        var minIndex = 0;
        var index = Random.Range(minIndex, _spawnpoints.Count);

        return _spawnpoints[index].position;
    }
}
