using System.Collections;
using UnityEngine;

namespace UkraineVsZombies
{
public class Projectile : MonoBehaviour
{
[Header("Movement")]
[SerializeField] private float _speed = 10f;
[SerializeField] private float _lifetime = 3f;

    [Header("Animation")]
    [SerializeField] private Animator _animator;
    [SerializeField] private float _hitAnimationDuration = 0.2f;

    private static readonly int HitTrigger =
        Animator.StringToHash("Hit");

    private Enemy _target;
    private float _damage;
    private float _timer;
    private bool _isHitting;

    private Collider2D _collider;

    private void Awake()
    {
        if (_animator == null)
            _animator = GetComponent<Animator>();

        _collider = GetComponent<Collider2D>();
    }

    public void Initialize(Enemy target, float damage)
    {
        _target = target;
        _damage = damage;
        _timer = _lifetime;
        _isHitting = false;
    }

    private void Update()
    {
        if (_isHitting)
            return;

        _timer -= Time.deltaTime;

        if (_timer <= 0f)
        {
            Destroy(gameObject);
            return;
        }

        if (_target == null || !_target.IsAlive)
        {
            Destroy(gameObject);
            return;
        }

        Vector3 direction =
            (_target.transform.position - transform.position).normalized;

        transform.position +=
            direction * _speed * Time.deltaTime;

        float angle =
            Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        transform.rotation =
            Quaternion.Euler(0f, 0f, angle);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (_isHitting)
            return;

        Enemy enemy = other.GetComponent<Enemy>();

        if (enemy != null && enemy == _target)
        {
            enemy.TakeDamage(_damage);
            StartCoroutine(HitRoutine());
        }
    }

    private IEnumerator HitRoutine()
    {
        _isHitting = true;

        if (_collider != null)
            _collider.enabled = false;

        if (_animator != null)
            _animator.SetTrigger(HitTrigger);

        yield return new WaitForSeconds(
            _hitAnimationDuration
        );

        Destroy(gameObject);
    }
}

}
