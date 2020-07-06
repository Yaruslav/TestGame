using System.Collections;
using UnityEngine;

public class UFO : MonoBehaviour
{
    private int _bulletIndex;
    private float _shootSpeed;

    public Transform BulletPool;
    public GameObject Player;


    private void Start()
    {
        _bulletIndex = 0;
        _shootSpeed = 10f;
        StartCoroutine(Shot());
    }

    private void CheckPosition()
    {
        if (transform.position.x <= Settings.minX)
            gameObject.SetActive(false);
        else if (transform.position.x >= Settings.maxX)
            gameObject.SetActive(false);
    }

    private IEnumerator Shot()
    {
        while (!EnemySpawner.ufo_dead)
        {
            yield return new WaitForSeconds(Random.Range(2f, 5f));
            Rigidbody bulletRB = BulletPool.GetChild(_bulletIndex).GetComponent<Rigidbody>();
            bulletRB.gameObject.SetActive(true);
            bulletRB.transform.position = transform.position;
            bulletRB.velocity = Vector3.zero;
            bulletRB.velocity = transform.forward * _shootSpeed;
            _bulletIndex++;
            if (_bulletIndex == 4) _bulletIndex = 0;
        }
    }

    private void Update()
    {
        CheckPosition();
        gameObject.transform.LookAt(Player.transform);
        gameObject.transform.rotation = new Quaternion(0, transform.rotation.y, 0, transform.rotation.w);
    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Bullet"))
        {
            EnemySpawner.ufo_dead = true;
            Settings.points += 200;
            gameObject.SetActive(false);
        }
    }

}
