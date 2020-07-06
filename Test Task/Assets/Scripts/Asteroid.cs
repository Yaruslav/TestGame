using System.Collections;
using UnityEngine;

public class Asteroid : MonoBehaviour
{
    private Vector3 _lastVel;


    public bool _big;
    public bool _medium;
    public bool _small;
    public GameObject Asteroid_medium;
    public GameObject Asteroid_small;
    public AudioClip[] Explosions;


    private void CheckPosition()
    {
        if (transform.position.x <= Settings.minX)
            transform.position = new Vector3(Settings.maxX, transform.position.y, transform.position.z);
        else if (transform.position.x >= Settings.maxX)
            transform.position = new Vector3(Settings.minX, transform.position.y, transform.position.z);

        if (transform.position.z <= Settings.minZ)
            transform.position = new Vector3(transform.position.x, transform.position.y, Settings.maxZ);
        else if (transform.position.z >= Settings.maxZ)
            transform.position = new Vector3(transform.position.x, transform.position.y, Settings.minZ);
    }

    private void SpawnMedium(GameObject _mediumAsteroid)
    {
        var _speed = Random.Range(100, 125);
        for (int i = 0; i < 2; i++)
        {
            var _asteroid = Instantiate(_mediumAsteroid);
            Rigidbody asteroidRB = _asteroid.GetComponent<Rigidbody>();
            asteroidRB.transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.z);
            asteroidRB.transform.rotation = Quaternion.Slerp(Random.rotation, Random.rotation, 0.5f);
            if (i == 0)
                asteroidRB.AddForce((_lastVel + Vector3.left) * _speed * 0.2f);
            else
                asteroidRB.AddForce((_lastVel + Vector3.right) * _speed * 0.2f);
            asteroidRB.GetComponent<Asteroid>()._big = false;
            asteroidRB.GetComponent<Asteroid>()._medium = true;
            asteroidRB.GetComponent<Asteroid>()._small = false;
        }
        gameObject.SetActive(false);

    }
    private void SpawnSmall(GameObject _smallAsteroid)
    {
        var _speed = Random.Range(125, 150);
        for (int i = 0; i < 2; i++)
        {
            var _asteroid = Instantiate(_smallAsteroid);
            Rigidbody asteroidRB = _asteroid.GetComponent<Rigidbody>();
            asteroidRB.transform.position = new Vector3(transform.position.x, transform.position.y - 0.25f, transform.position.z);
            asteroidRB.transform.rotation = Quaternion.Slerp(Random.rotation, Random.rotation, 0.5f);
            if (i == 0)
                asteroidRB.AddForce((_lastVel + Vector3.left * 5) * _speed * 0.2f);
            else
                asteroidRB.AddForce((_lastVel + Vector3.right * 5) * _speed * 0.2f);
            asteroidRB.GetComponent<Asteroid>()._big = false;
            asteroidRB.GetComponent<Asteroid>()._medium = false;
            asteroidRB.GetComponent<Asteroid>()._small = true;
        }
        Destroy(gameObject);
    }

    private void DeadByPlayerOrUFO()
    {
        if (_big)
        {
            EnemySpawner.count -= 4;
            gameObject.SetActive(false);
            Camera.main.GetComponent<AudioSource>().PlayOneShot(Explosions[2]);
        }
        else
        {
            if (_medium)
            {
                EnemySpawner.count -= 2;
                Camera.main.GetComponent<AudioSource>().PlayOneShot(Explosions[1]);
            }
            if (_small)
            {
                EnemySpawner.count--;
                Camera.main.GetComponent<AudioSource>().PlayOneShot(Explosions[0]);
            }
            Destroy(gameObject);
        }
    }
    private void DeadByBullet()
    {
        if (_small)
        {
            EnemySpawner.count--;
            if (EnemySpawner.count < 0) EnemySpawner.count = 0;
            Destroy(gameObject);
            Camera.main.GetComponent<AudioSource>().PlayOneShot(Explosions[0]);
            Settings.points += 100;
        }
        if (_medium)
        {
            SpawnSmall(Asteroid_small);
            Camera.main.GetComponent<AudioSource>().PlayOneShot(Explosions[1]);
            Settings.points += 50;
        }
        if (_big)
        {
            SpawnMedium(Asteroid_medium);
            Camera.main.GetComponent<AudioSource>().PlayOneShot(Explosions[2]);
            Settings.points += 20;
        }
    }

    private void Update()
    {
        CheckPosition();
    }

    private void OnTriggerEnter(Collider other)
    {
        if ((other.gameObject.CompareTag("Player") && !Settings._dead && !Settings._invulnerable) || other.gameObject.CompareTag("UFO"))
        {
            DeadByPlayerOrUFO();
            if (other.gameObject.CompareTag("UFO"))
            {
                other.gameObject.SetActive(false);
                EnemySpawner.ufo_dead = true;
            }
            else
                Settings._dead = true;
        }
        else
        {
            if (other.gameObject.CompareTag("Bullet"))
            {
                _lastVel = other.GetComponent<Rigidbody>().velocity;
                DeadByBullet();
                other.gameObject.SetActive(false);
            }
        }
    }

}
