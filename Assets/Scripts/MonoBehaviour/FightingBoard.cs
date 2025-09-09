using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using Priority_Queue;
using Scriptables;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class FightingBoard : MonoBehaviour
{
    private const float SpawnRadius = .5f;
    private const int StartCapacity = 20;

    #region Serialized Fields

    [SerializeField] private GameObject enemyPrefab;

    #endregion

    private readonly FastPriorityQueue<EnemyQueueNode> _displayedEnemies =
        new FastPriorityQueue<EnemyQueueNode>(StartCapacity + 3);

    private readonly List<ICard.Buff> _enemyBuffs = new List<ICard.Buff>();

    private readonly List<Enemy> _enemyPool = new List<Enemy>(StartCapacity);

    private readonly ConcurrentDictionary<EnemyQueueNode, float> _queueUpdates =
        new ConcurrentDictionary<EnemyQueueNode, float>();

    public Rect Bounds { get; private set; }

    public Player Player { get; private set; }

    #region Event Functions

    private void Awake()
    {
        if (FindObjectsOfType<FightingBoard>().Length > 1)
            Destroy(gameObject);
    }

    private void Start()
    {
        // #inefficient but it's start so idc
        Player = FindObjectOfType<Player>();

        // bounds calculation
        var position = transform.position;
        var size = GetComponent<SpriteRenderer>().size;
        Bounds = new Rect(position.x - size.x / 2, position.y - size.y / 2, size.x, size.y);

        // propagate myself
        Enemy.SetFightingBoard(this);
        Player.SetFightingBoard(this);

        // fill enemy pool
        for (var i = 0; i < _enemyPool.Capacity; i++)
        {
            var enemy = InstantiateEnemy();
            enemy.SetActive(false);
            _enemyPool.Add(enemy.GetComponent<Enemy>());
        }
    }

    private void LateUpdate()
    {
        foreach (var update in _queueUpdates)
            if (update.Value == 0)
                _displayedEnemies.Remove(update.Key);
            else
                _displayedEnemies.UpdatePriority(update.Key, update.Value);

        _queueUpdates.Clear();

        UpdatePlayerRotation();
    }

    #endregion

    private void UpdatePlayerRotation()
    {
        Player.LookAt(_displayedEnemies.Count == 0 ? null : _displayedEnemies.First.Enemy);
    }

    /**
     * a distance of 0 means it gets removed from the queue
     * 
     * it might have been more efficient to just use a list since adding takes O(1) and deleting O(n)
     * 
     * every frame the player searches for the closest enemy which would be O(n) the enemies do not have to
     * implement Update in this case.
     * 
     * In the current case it's for every enemy update it's priority which takes O(log n) worst.
     * 
     * worst case therefore is O(n log n).
     * 
     * But i think in the average case there is not very many sift up operations performed therefore mine should be a
     * bit faster in an average case: additional feature: getting the closest enemy is O(1)
     */
    public void QueueUpdate(EnemyQueueNode node, float distance)
    {
        if (_queueUpdates.ContainsKey(node))
            if (_queueUpdates[node] < distance)
                return;
            else
                _queueUpdates[node] = distance;
        while (!_queueUpdates.TryAdd(node, distance))
        {
        }
    }

    public void ReturnToPool(Enemy enemy)
    {
        _enemyPool.Add(enemy);
        QueueUpdate(enemy.Node, 0);
    }

    public void SpawnEnemy(EnemyCard enemyCard, Vector2 at, bool multiple = false)
    {
        var enemy = GetFromPool();
        var position = new Vector3(at.x + Random.Range(-SpawnRadius, SpawnRadius),
            at.y + Random.Range(-SpawnRadius, SpawnRadius), 0);
        enemy.Spawn(enemyCard, position);
        if (_enemyBuffs.Count == 0) goto Enqueue;
        foreach (var buff in _enemyBuffs)
            enemy.AddBuff(buff);
        if (multiple) _enemyBuffs.Clear();
        Enqueue:
        _displayedEnemies.Enqueue(enemy.Node, enemy.GetPriority());

        if (enemyCard is Boss)
            StartCoroutine(WatchBoss(enemy.gameObject));
    }

    private IEnumerator WatchBoss(GameObject boss)
    {
        while (boss.activeSelf)
            yield return null;
        Player.EndGame();
    }

    public void AddBuff(ICard.Buff buff)
    {
        _enemyBuffs.Add(buff);
    }

    private Enemy GetFromPool()
    {
        if (_enemyPool.Count == 0) return InstantiateEnemy().GetComponent<Enemy>();
        var returnValue = _enemyPool[0];
        _enemyPool.RemoveAt(0);
        return returnValue;
    }

    public bool HasEnemies()
    {
        return _displayedEnemies.Count > 0;
    }

    private GameObject InstantiateEnemy()
    {
        if (_displayedEnemies.Count >= _displayedEnemies.MaxSize - 2)
            _displayedEnemies.Resize(_displayedEnemies.MaxSize * 2);
        return Instantiate(enemyPrefab, transform.position, Quaternion.identity);
    }

    public ICard.PlayOutData GetPlayOutData(Vector2 playedPosition, CardHand hand)
    {
        return new ICard.PlayOutData(Player, playedPosition, hand);
    }

    #region Nested type: ${0}

    public class EnemyQueueNode : FastPriorityQueueNode
    {
        public readonly Enemy Enemy;

        public EnemyQueueNode(Enemy enemy)
        {
            Enemy = enemy;
        }
    }

    #endregion
}