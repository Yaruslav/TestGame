using System.Collections;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    //Asteroids
    public static int count;

    public Transform AsteroidPool;
    public GameObject Asteroid_big;

    //UFO
    public static bool ufo_dead;
    public GameObject UFO;


    private void Start()
    {
        ufo_dead = true;
        count = 0;
        StartCoroutine(Spawn(Asteroid_big, 2));
        StartCoroutine(UFO_Spawn());
    }


    private IEnumerator Spawn(GameObject Asteroid, int _asteroidAmount)
    {
        while (!Settings._gameover)
        {
            var rnd = Random.Range(-1f, 1f);
            rnd = (rnd > 0) ? 1 : -1;
            var _asteroid = 0;
            if (count == 0)
            {
                while (_asteroid != _asteroidAmount)
                {
                    var _speed = Random.Range(75, 100);
                    if (AsteroidPool.childCount < _asteroidAmount)
                    {
                        Instantiate(Asteroid, AsteroidPool);
                        AsteroidPool.GetChild(_asteroid).gameObject.SetActive(false);
                    }
                    Rigidbody asteroidRB = AsteroidPool.GetChild(_asteroid).GetComponent<Rigidbody>();
                    asteroidRB.velocity = Vector3.zero;
                    asteroidRB.gameObject.SetActive(true);
                    asteroidRB.transform.position = new Vector3(Random.Range(Settings.minX, Settings.maxX), 0.5f, Random.Range(Settings.minZ, Settings.maxZ));
                    asteroidRB.transform.rotation = Quaternion.Slerp(Random.rotation, Random.rotation, 0.5f);
                    asteroidRB.AddForce(new Vector3(rnd, 0, rnd) * _speed);
                    _asteroid++;
                    count += 4;
                }
                _asteroidAmount++;
            }
            yield return new WaitForSeconds(2);
        }
    }
    private IEnumerator UFO_Spawn()
    {
        while (!Settings._gameover)
        {
            if (ufo_dead)
            {
                yield return new WaitForSeconds(Random.Range(20, 41));
                Rigidbody ufoRB = UFO.GetComponent<Rigidbody>();
                UFO.SetActive(true);
                var _speed = 1.5f;
                var rnd = Random.Range(-1f, 1f);
                rnd = (rnd > 0) ? (Settings.maxX - 0.1f) : (Settings.minX + 0.1f);
                UFO.transform.position = new Vector3(rnd, 0.5f, Random.Range(Settings.minZ * 0.8f, Settings.maxZ * 0.8f));
                if (rnd < 0)
                    ufoRB.AddForce(Vector3.right * _speed, ForceMode.Impulse);
                else
                    ufoRB.AddForce(Vector3.left * _speed, ForceMode.Impulse);
                ufo_dead = false;
            }
            yield return new WaitForSeconds(0);
        }
    }



}
