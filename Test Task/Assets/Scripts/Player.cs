using System.Collections;
using UnityEngine;

public class Player : MonoBehaviour
{
    private Rigidbody rb;

    private float _maxMoveSpeed = 2.5f;
    private float _rotationSpeed = 2f;
    private float _moveAcceleration = 0.1f;
    private float _moveSpeed = 0f;
    private float _shootSpeed = 10f;
    private float _timeCount = 1337;


    private float _horizontalInput;
    private float _verticalInput;
    private bool _clickInput;
    private bool _attackInput;
    private bool _attack2Input;
    private bool _pauseInput;

    private bool _played;

    public int _currentBullet;

    public Transform BulletPool;
    public AudioClip AC_shot;
    public GameObject Menu;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        Settings._gameStarted = true;
        transform.position = new Vector3(0, 0.5f, 0);
        transform.rotation = new Quaternion(0, 0, 0, 0);
    }

    private void GetPlayerInput()
    {
        _horizontalInput = Input.GetAxisRaw("Horizontal");
        _verticalInput = Input.GetAxisRaw("Forward");
        _clickInput = Input.GetMouseButton(1);
        _attackInput = Input.GetKeyDown(KeyCode.Space);
        _attack2Input = Input.GetMouseButtonDown(0);
        _pauseInput = Input.GetKeyDown(KeyCode.Escape);
    }
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


    private void MovePlayer()
    {

        if (Settings._keyboardOnly)
        {
            if (_verticalInput == 1)
            {
                if (_moveSpeed <= _maxMoveSpeed)
                    _moveSpeed += _moveAcceleration;
                Camera.main.GetComponent<AudioSource>().loop = true;
                if (!_played)
                    Camera.main.GetComponent<AudioSource>().Play();
                _played = true;
            }
            else
            {
                if (_moveSpeed > 0)
                    _moveSpeed -= _moveAcceleration * 0.25f;
                else _moveSpeed = 0;
                Camera.main.GetComponent<AudioSource>().loop = false;
                _played = false;
            }
            rb.velocity = transform.forward * _moveSpeed;
        }
        else
        {
            if (_clickInput || _verticalInput == 1)
            {
                if (_moveSpeed <= _maxMoveSpeed)
                    _moveSpeed += _moveAcceleration;
                Camera.main.GetComponent<AudioSource>().loop = true;
                if (!_played)
                    Camera.main.GetComponent<AudioSource>().Play();
                _played = true;
            }
            else
            {
                if (_moveSpeed > 0)
                    _moveSpeed -= _moveAcceleration * 0.25f;
                else _moveSpeed = 0;
                Camera.main.GetComponent<AudioSource>().loop = false;
                _played = false;
            }
            rb.velocity = transform.forward * _moveSpeed;
        }
    }
    private void RotatePlayer()
    {
        if (Settings._keyboardOnly)
        {
            float rotation = _horizontalInput * _rotationSpeed;
            transform.Rotate(Vector2.up * rotation);
        }
        else
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit = new RaycastHit();
            if (Physics.Raycast(ray, out hit))
            {
                var rotation = Quaternion.LookRotation(hit.point - transform.position);
                transform.rotation = Quaternion.Slerp(transform.rotation, rotation, Time.deltaTime * _rotationSpeed);
            }
        }
    }

    private void ShootPlayer()
    {
        if (_attackInput)
        {
            if (_timeCount == 1337)
                _timeCount = Time.time;
            Shot();
        }

        if (!Settings._keyboardOnly && _attack2Input)
        {
            if (_timeCount == 1337)
                _timeCount = Time.time;
            Shot();
        }
    }
    private void Shot()
    {
        Rigidbody bulletRB = BulletPool.GetChild(_currentBullet).GetComponent<Rigidbody>();
        bulletRB.gameObject.SetActive(true);
        bulletRB.transform.position = transform.position;
        bulletRB.velocity = transform.forward * _shootSpeed;
        _currentBullet++;
        Camera.main.GetComponent<AudioSource>().PlayOneShot(AC_shot);
    }


    private void Dead()
    {
        transform.position = new Vector3(0, 0.5f, 0);
        transform.rotation = new Quaternion(0, 0, 0, 0);
        Settings.hp--;
        var _bullets = GameObject.FindGameObjectsWithTag("Bullet");
        for (int i = 0; i < _bullets.Length; i++)
            _bullets[i].SetActive(false);
        if (Settings.hp != 0)
            StartCoroutine(Invulnerable());
        Settings._dead = false;
    }

    private IEnumerator Invulnerable()
    {
        var _invCount = Time.time;
        Settings._invulnerable = true;
        var MR = GetComponent<MeshRenderer>();
        while (Time.time - _invCount < 3)
        {
            MR.enabled = false;
            yield return new WaitForSeconds(0.25f);
            MR.enabled = true;
            yield return new WaitForSeconds(0.25f);
        }
        Settings._invulnerable = false;
    }


    private void Update()
    {
        if (!Settings._paused)
        {
            GetPlayerInput();
            CheckPosition();
            MovePlayer();
            RotatePlayer();
            ShootPlayer();
            if (_currentBullet >= BulletPool.childCount)
                if ((Time.time - _timeCount) >= 1)
                {
                    _currentBullet = 0;
                    _timeCount = 1337;
                }
            if (_pauseInput)
            {
                Time.timeScale = 0;
                Settings._paused = true;
                Menu.SetActive(true);
            }
            if (Settings._dead)
                Dead();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if ((other.gameObject.CompareTag("UFO") || other.gameObject.CompareTag("UFO_bullet")) && !Settings._invulnerable)
        {
            if (other.gameObject.CompareTag("UFO_bullet"))
                Settings._dead = true;
            else
                EnemySpawner.ufo_dead = true;
            other.gameObject.SetActive(false);
        }
    }



}


