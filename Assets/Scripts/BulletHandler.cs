using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class BulletHandler : NetworkBehaviour
{
    public float duration = 10f;
    float despawnTime;

    public float damage = 100f;

    // Start is called before the first frame update
    void Start()
    {
        despawnTime = Time.time + duration;
    }

    // Update is called once per frame
    void Update()
    {
        if (despawnTime <= Time.time)
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter(Collider collision)
    {
        if (collision.tag == "Enemy" && isServer)
        {
            CmdDoDamage(collision.gameObject, gameObject, damage);
        }
    }

    [Command(requiresAuthority = false)]
    void CmdDoDamage(GameObject enemy, GameObject bullet, float damage)
    {
        enemy.GetComponent<EnemyController>().health -= damage;
        Destroy(bullet);
    }
}
