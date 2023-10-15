using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public Rigidbody2D rb;
    private SpriteRenderer sr;
    public bool isEnemyProjectile;
    public float damage;
    public float moveSpeed;
    public Vector3 moveDirection;
    public Sprite playerSprite;
    public Sprite enemySprite;
    public ProjectileType type;
    public float homingTurnSpeed;
    public GameObject target;
    private void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();
       
        
        
    }
    public void SetupProjectile(Attack attack)
    {
        damage = attack.attackDamage;
        moveSpeed = attack.moveSpeed;
        

    }
    private void Start()
    {
        Debug.Log(isEnemyProjectile + " is enemy");
        if (isEnemyProjectile)
        {
            sr.sprite = enemySprite;
        }
        else
        {
            sr.sprite = playerSprite;
        }
    }
    private void Update()
    {
        if(type == ProjectileType.simple || target == null)
        {
            transform.position += Time.deltaTime * moveSpeed * moveDirection.normalized;
        }
        else if (type == ProjectileType.homing)
        {
            Vector2 current = transform.right;
            Vector2 to = target.transform.position - transform.position;
            transform.right = Vector3.RotateTowards(current, to, homingTurnSpeed * Mathf.Deg2Rad * Time.deltaTime, 1); ;
            transform.position += moveSpeed * Time.deltaTime * transform.right;
        }
        
        
    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (isEnemyProjectile)
        {
            if (collision.CompareTag("Ship"))
            {
                collision.GetComponent<CombatUnit>().TakeDamage(damage);
                Destroy(gameObject);
            }
        }
        else
        {
            if (collision.CompareTag("EnemyShip"))
            {
                collision.GetComponent<CombatUnit>().TakeDamage(damage);
                Destroy(gameObject);
            }
        }
        
    }
}
public enum ProjectileType
{
    simple,homing
}
