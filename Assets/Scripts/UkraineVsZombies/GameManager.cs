using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

namespace UkraineVsZombies
{
public class GameManager : MonoBehaviour
{
[Header("Spawn Points")]
[SerializeField] private SpawnPoint[] _spawnPoints;

    [Header("Spawn Settings")]
    [SerializeField] private float _minSpawnTime = 2f;
    [SerializeField] private float _maxSpawnTime = 4f;
    [SerializeField] private int _maxEnemies = 20;

    [Header("Lanes")]
    [SerializeField] private int _laneCount = 5;

    [Header("Game Stats")]
    [SerializeField] private int _lives = 3;
    [SerializeField] private int _score = 0;

    [Header("UI")]
    [SerializeField] private GameObject _gameOverPanel;
    [SerializeField] private TextMeshProUGUI _livesText;
    [SerializeField] private TextMeshProUGUI _scoreText;

    [Header("UI Animation")]
    [SerializeField] private Animator _livesAnimator;

    private static readonly int DamageTrigger =
        Animator.StringToHash("Damage");

    private readonly Dictionary<int, List<Enemy>>
        _enemiesByLane = new();

    private readonly Dictionary<int, List<Tower>>
        _towersByLane = new();

    private float _spawnTimer;
    private bool _isGameOver;

    public static GameManager Instance { get; private set; }

    private void Awake()
    {
        Instance = this;

        if (_livesAnimator == null && _livesText != null)
            _livesAnimator = _livesText.GetComponent<Animator>();

        for (int i = 0; i < _laneCount; i++)
        {
            _enemiesByLane[i] = new List<Enemy>();
            _towersByLane[i] = new List<Tower>();
        }
    }

    private void Start()
    {
        if (_gameOverPanel != null)
            _gameOverPanel.SetActive(false);

        _spawnTimer =
            Random.Range(_minSpawnTime, _maxSpawnTime);

        UpdateUI();
    }

    private void Update()
    {
        if (_isGameOver)
            return;

        UpdateSpawning();
        CleanupLists();
        UpdateTargets();
    }

    private void UpdateSpawning()
    {
        int totalEnemies = 0;

        foreach (List<Enemy> list in _enemiesByLane.Values)
            totalEnemies += list.Count;

        _spawnTimer -= Time.deltaTime;

        if (_spawnTimer <= 0f &&
            totalEnemies < _maxEnemies)
        {
            SpawnEnemy();

            _spawnTimer =
                Random.Range(_minSpawnTime, _maxSpawnTime);
        }
    }

    private void SpawnEnemy()
    {
        if (_spawnPoints == null ||
            _spawnPoints.Length == 0)
        {
            return;
        }

        int index =
            Random.Range(0, _spawnPoints.Length);

        SpawnPoint spawnPoint = _spawnPoints[index];

        if (spawnPoint == null)
            return;

        Enemy enemy = spawnPoint.Spawn();

        if (enemy != null)
            RegisterEnemy(enemy, index);
    }

    public void RegisterEnemy(Enemy enemy, int lane)
    {
        if (lane < 0 || lane >= _laneCount)
            return;

        _enemiesByLane[lane].Add(enemy);
    }

    public void RegisterTower(Tower tower, int lane)
    {
        if (lane < 0 || lane >= _laneCount)
            return;

        _towersByLane[lane].Add(tower);
    }

    private void CleanupLists()
    {
        foreach (List<Enemy> list in _enemiesByLane.Values)
        {
            for (int i = list.Count - 1; i >= 0; i--)
            {
                if (list[i] == null)
                    list.RemoveAt(i);
            }
        }

        foreach (List<Tower> list in _towersByLane.Values)
        {
            for (int i = list.Count - 1; i >= 0; i--)
            {
                if (list[i] == null)
                    list.RemoveAt(i);
            }
        }
    }

    private void UpdateTargets()
    {
        for (int lane = 0; lane < _laneCount; lane++)
        {
            List<Tower> towers = _towersByLane[lane];
            List<Enemy> enemies = _enemiesByLane[lane];

            foreach (Tower tower in towers)
            {
                if (tower == null || !tower.IsAlive)
                    continue;

                Enemy bestTarget = null;
                float closestDistance = float.MaxValue;

                foreach (Enemy enemy in enemies)
                {
                    if (enemy == null || !enemy.IsAlive)
                        continue;

                    float distance =
                        enemy.transform.position.x -
                        tower.transform.position.x;

                    if (distance > 0f &&
                        distance <= tower.Range &&
                        distance < closestDistance)
                    {
                        closestDistance = distance;
                        bestTarget = enemy;
                    }
                }

                tower.SetTarget(bestTarget);
            }
        }
    }

    public void LoseLife()
    {
        if (_isGameOver)
            return;

        _lives--;
        UpdateUI();

        if (_livesAnimator != null)
        {
            _livesAnimator.ResetTrigger(DamageTrigger);
            _livesAnimator.SetTrigger(DamageTrigger);
        }

        if (_lives <= 0)
        {
            GameOver();
            Invoke(nameof(RestartScene), 2f);
        }
    }

    public void AddScore()
    {
        if (_isGameOver)
            return;

        _score++;
        UpdateUI();
    }

    private void UpdateUI()
    {
        if (_livesText != null)
            _livesText.text = "Lives: " + _lives;

        if (_scoreText != null)
            _scoreText.text = "Score: " + _score;
    }

    public void GameOver()
    {
        if (_isGameOver)
            return;

        _isGameOver = true;

        if (_gameOverPanel != null)
            _gameOverPanel.SetActive(true);
    }

    private void RestartScene()
    {
        SceneManager.LoadScene(
            SceneManager.GetActiveScene().buildIndex
        );
    }
}

}
