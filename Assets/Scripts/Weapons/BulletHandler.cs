using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class BulletHandler : NetworkBehaviour
{
    public Vector3 target;
    Vector3 normalized;
    public float speed = 10f;
    public float duration = 5f;
    float despawnTime;
    public GameObject spawner;

    public float damage = 100f;

    // Start is called before the first frame update
    void Start()
    {
        despawnTime = Time.time + duration;
        normalized = (target - transform.position).normalized;
    }

    // Update is called once per frame
    void Update()
    {
        if (!isServer)
            return;

        if (despawnTime <= Time.time)
        {
            Destroy(gameObject);
        }

        transform.position += normalized * speed * Time.deltaTime;
    }

    private void OnTriggerEnter(Collider collision)
    {
        if (collision.tag == "Enemy" && isServer)
        {
            DoDamage(collision.gameObject, gameObject, damage);
        }
        else if (collision.tag != "Player")
        {
            DestroySelf(gameObject);
        }
    }

    void DoDamage(GameObject enemy, GameObject bullet, float damage)
    {
        EnemyController ec = enemy.GetComponent<EnemyController>();
        ec.health -= damage;
        if (ec.health <= 0)
        {
            int level = (int)Mathf.Ceil(ec.damage / 5);
            spawner.GetComponent<PlayerController>().gold += Mathf.Abs(Random.Range(level - 2, level + 2));
        }
        NetworkServer.Destroy(bullet);
    }

    void DestroySelf(GameObject bullet)
    {
        NetworkServer.Destroy(bullet);
    }
}
