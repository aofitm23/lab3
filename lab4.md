# Лабораторна робота №4 — Анімації в Unity

## Основна сцена

`Assets/Scenes/UkraineVsZombies.unity`

## Реалізовані анімації

1. **Enemy**

   * EnemyWalk
   * EnemyHurt
   * EnemyDeath

2. **Defender**

   * DefenderIdle
   * DefenderAttack

3. **Bullet**

   * BulletFly
   * BulletHit

4. **SpawnPoint**

   * SpawnIdle
   * SpawnEffect

5. **LivesText**

   * LivesIdle
   * LivesDamage

## Керування анімаціями з коду

Перемикання станів Animator реалізовано у файлах:

* `Enemy.cs`
* `Tower.cs`
* `Projectile.cs`
* `SpawnPoint.cs`
* `GameManager.cs`

Анімації пошкодження, смерті, атаки, влучання, появи ворога та втрати життя запускаються відповідно до стану ігрових об’єктів.
