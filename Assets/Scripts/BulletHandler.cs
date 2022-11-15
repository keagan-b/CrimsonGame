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
            CmdDoDamage(collision.gameObject, gameObject, damage);
        }
        else if (collision.tag != "Player")
        {
            CmdDestroySelf(gameObject);
        }
    }

    [Command(requiresAuthority = false)]
    void CmdDoDamage(GameObject enemy, GameObject bullet, float damage)
    {
        EnemyController ec = enemy.GetComponent<EnemyController>();
        ec.health -= damage;
        if (ec.health <= 0)
        {
            spawner.GetComponent<PlayerController>().gold += (int)Mathf.Ceil(ec.health / 5);
        }
        Destroy(bullet);
    }

    [Command(requiresAuthority = false)]
    void CmdDestroySelf(GameObject bullet)
    {
        Destroy(bullet);
    }
}
