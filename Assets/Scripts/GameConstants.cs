using UnityEngine;

[CreateAssetMenu(fileName = "GameConstants", menuName = "ScriptableObjects/GameConstants", order = 1)]
public class GameConstants : ScriptableObject
{
    // #-------------------- PLAYER ---------------------#
    public int playerMaxLives = 3;

    // Movement
    public float playerMoveSpeed = 5f;

    // Attack
    public float playerBulletSpeed = 2.5f;
    public float playerFireDelay = 1f;

    // Deflect
    public float playerDeflectArcAngle = 95f;
    public float playerDeflectCooldown = 0.5f;
    public float playerDeflectRange = 2f;

    // #-------------------- ENEMIES ---------------------#
    public int enemyMaxLives = 1;

    // Movement
    public float enemyMoveSpeed = 1f;
    public float enemyChaseRange = 5f;

    // Attack
    public float enemyBulletSpeed = 5f;
    public float enemyFireDelay = 0.7f;
    public float enemyShootRange = 0.2f;

    // #-------------------- HOTWIRES ---------------------#
    public float hotwireDetectionRadius = 2.0f;
}