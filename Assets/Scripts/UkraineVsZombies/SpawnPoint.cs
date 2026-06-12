using UnityEngine;

namespace UkraineVsZombies
{
    public class SpawnPoint : MonoBehaviour
    {
        [Header("Enemy Settings")]
        [SerializeField] private GameObject[] _enemyPrefabs;

        [Header("Animation")]
        [SerializeField] private Animator _animator;

        private static readonly int SpawnTrigger =
            Animator.StringToHash("Spawn");

        private void Awake()
        {
            // Animator находится на дочернем SpawnVisual.
            if (_animator == null)
                _animator = GetComponentInChildren<Animator>();
        }

        public Enemy Spawn()
        {
            if (_enemyPrefabs == null || _enemyPrefabs.Length == 0)
                return null;

            int index = Random.Range(0, _enemyPrefabs.Length);
            GameObject prefab = _enemyPrefabs[index];

            if (prefab == null)
                return null;

            if (_animator != null)
            {
                _animator.ResetTrigger(SpawnTrigger);
                _animator.SetTrigger(SpawnTrigger);
            }

            GameObject obj = Instantiate(
                prefab,
                transform.position,
                Quaternion.identity
            );

            Enemy enemy = obj.GetComponent<Enemy>();

            if (enemy != null)
                enemy.Initialize();

            return enemy;
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.red;

            Gizmos.DrawWireSphere(
                transform.position,
                0.3f
            );

            Gizmos.DrawLine(
                transform.position,
                transform.position + Vector3.left * 2f
            );
        }
    }
}