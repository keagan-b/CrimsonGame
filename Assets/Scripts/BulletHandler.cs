using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletHandler : MonoBehaviour
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

    private void OnTriggerStay(Collider collision)
    {
        Debug.Log("collider hit.");
        if (collision.tag == "Enemy")
        {
            collision.GetComponent<EnemyController>().health -= damage;
            Destroy(gameObject);
        }
    }
}
