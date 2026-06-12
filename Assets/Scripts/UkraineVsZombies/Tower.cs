using UnityEngine;
using UnityEngine.UI;

namespace UkraineVsZombies
{
public class Tower : MonoBehaviour
{
[Header("Stats")]
[SerializeField] private float _maxHealth = 100f;
[SerializeField] private float _range = 5f;
[SerializeField] private float _fireRate = 1f;
[SerializeField] private float _damage = 10f;

    [Header("Projectile")]
    [SerializeField] private GameObject _projectilePrefab;
    [SerializeField] private Transform _firePoint;

    [Header("HP Bar")]
    [SerializeField] private Slider _hpSlider;

    [Header("Animation")]
    [SerializeField] private Animator _animator;

    private static readonly int AttackTrigger =
        Animator.StringToHash("Attack");

    private float _currentHealth;
    private float _fireTimer;
    private Enemy _target;

    public bool IsAlive => _currentHealth > 0f;
    public float Range => _range;

    private void Awake()
    {
        if (_firePoint == null)
            _firePoint = transform;

        // Animator знаходиться на тому ж об'єкті Graph.
        if (_animator == null)
    _animator = GetComponentInChildren<Animator>();;

        _currentHealth = _maxHealth;
        UpdateHpBar();
    }

    private void Update()
    {
        if (!IsAlive)
            return;

        TryFire();
    }

    public void SetTarget(Enemy target)
    {
        _target = target;
    }

    private void TryFire()
    {
        _fireTimer -= Time.deltaTime;

        if (_target == null ||
            !_target.IsAlive ||
            _fireTimer > 0f)
        {
            return;
        }

        Fire();
        _fireTimer = 1f / _fireRate;
    }

    private void Fire()
{
    if (_animator == null)
        _animator = GetComponent<Animator>();


    if (_animator != null)
    {
        _animator.ResetTrigger(AttackTrigger);
        _animator.SetTrigger(AttackTrigger);
    }

    if (_projectilePrefab != null)
    {
        GameObject projectileObject = Instantiate(
            _projectilePrefab,
            _firePoint.position,
            Quaternion.identity
        );

        Projectile projectile =
            projectileObject.GetComponent<Projectile>();

        if (projectile != null)
            projectile.Initialize(_target, _damage);
    }
    else
    {
        _target.TakeDamage(_damage);
    }
}

    public void TakeDamage(float damage)
    {
        if (!IsAlive)
            return;

        _currentHealth -= damage;
        UpdateHpBar();

        if (_currentHealth <= 0f)
            Destroy(gameObject);
    }

    private void UpdateHpBar()
    {
        if (_hpSlider != null)
        {
            _hpSlider.value =
                _currentHealth / _maxHealth;
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;

        Gizmos.DrawLine(
            transform.position,
            transform.position +
            Vector3.right * _range
        );
    }
}

}
