using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace UkraineVsZombies
{
    public class Enemy : MonoBehaviour
    {
        [Header("Stats")]
        [SerializeField] private float _maxHealth = 50f;
        [SerializeField] private float _moveSpeed = 2f;
        [SerializeField] private float _attackDamage = 10f;
        [SerializeField] private float _attackRate = 1f;

        [Header("HP Bar")]
        [SerializeField] private Slider _hpSlider;

        [Header("Animation")]
        [SerializeField] private Animator _animator;
        [SerializeField] private float _deathAnimationDuration = 0.6f;

        private static readonly int HurtTrigger =
            Animator.StringToHash("Hurt");

        private static readonly int DeathTrigger =
            Animator.StringToHash("Death");

        private float _currentHealth;
        private float _attackTimer;
        private Tower _targetTower;
        private bool _deathStarted;

        public event Action OnDeath;

        // ќставл€ем условие таким же, как в исходной версии.
        public bool IsAlive => _currentHealth > 0f;

        private void Awake()
        {
            if (_animator == null)
                _animator = GetComponentInChildren<Animator>();
        }

        public void Initialize()
        {
            _currentHealth = _maxHealth;
            _attackTimer = 0f;
            _targetTower = null;
            _deathStarted = false;

            if (_hpSlider != null)
                _hpSlider.gameObject.SetActive(true);

            UpdateHPBar();
        }

        private void Update()
        {
            if (!IsAlive)
                return;

            if (_targetTower != null && _targetTower.IsAlive)
                Attack();
            else
                Move();
        }

        private void Move()
        {
            transform.position +=
                Vector3.left * _moveSpeed * Time.deltaTime;

            if (transform.position.x < -10f)
            {
                GameManager.Instance?.LoseLife();
                OnDeath?.Invoke();
                Destroy(gameObject);
            }
        }

        private void Attack()
        {
            _attackTimer -= Time.deltaTime;

            if (_attackTimer <= 0f)
            {
                _targetTower.TakeDamage(_attackDamage);
                _attackTimer = 1f / _attackRate;
            }
        }

        public void TakeDamage(float damage)
        {
            if (!IsAlive || _deathStarted)
                return;

            _currentHealth -= damage;
            UpdateHPBar();

            if (_currentHealth <= 0f)
            {
                _currentHealth = 0f;
                StartCoroutine(DeathRoutine());
            }
            else if (_animator != null)
            {
                _animator.SetTrigger(HurtTrigger);
            }
        }

        private IEnumerator DeathRoutine()
        {
            if (_deathStarted)
                yield break;

            _deathStarted = true;

            GameManager.Instance?.AddScore();
            OnDeath?.Invoke();

            if (_hpSlider != null)
                _hpSlider.gameObject.SetActive(false);

            if (_animator != null)
            {
                _animator.ResetTrigger(HurtTrigger);
                _animator.SetTrigger(DeathTrigger);
            }

            yield return new WaitForSeconds(_deathAnimationDuration);

            Destroy(gameObject);
        }

        private void UpdateHPBar()
        {
            if (_hpSlider != null)
                _hpSlider.value = _currentHealth / _maxHealth;
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            Tower tower = other.GetComponent<Tower>();

            if (tower != null && tower.IsAlive)
                _targetTower = tower;
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            Tower tower = other.GetComponent<Tower>();

            if (tower != null && tower == _targetTower)
                _targetTower = null;
        }
    }
}