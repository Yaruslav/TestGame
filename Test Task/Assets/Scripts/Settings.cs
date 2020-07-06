using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Settings : MonoBehaviour
{
    //General
    public static float minX;
    public static float maxX;
    public static float minZ;
    public static float maxZ;

    public static bool _paused;
    public static bool _gameStarted;
    public static bool _keyboardOnly;

    //Player
    public static bool _dead;
    public static bool _gameover;
    public static bool _invulnerable;
    public static int hp;
    public static int points;


    //Objects
    public GameObject Menu;
    public GameObject Player;
    public GameObject Asteroids;
    public GameObject UFO;

    //UI
    //Menu
    public Button btn_continue;
    public Text ControlButton;

    //Game
    public Text Points;
    public Slider HP;



    //MenuButtons
    public void Continue()
    {
        Menu.SetActive(false);
        _paused = false;
        Time.timeScale = 1;
    }
    public void New_Game()
    {
        if (_gameStarted)
        {
            SceneManager.LoadScene(0);
            _gameStarted = false;
        }
        else
        {
            if (_gameover)
            {
                SceneManager.LoadScene(0);
            }
            else
            {
                Time.timeScale = 1;
                Asteroids.SetActive(true);
                Player.SetActive(true);
                Menu.SetActive(false);
                _paused = false;
                btn_continue.interactable = true;
            }
        }
    }
    public void ChangeControl()
    {
        if (!_keyboardOnly)
        {
            _keyboardOnly = true;
            ControlButton.text = "Управление: клавиатура";
        }
        else
        {
            _keyboardOnly = false;
            ControlButton.text = "Управление: клавиатура + мышь";
        }

    }
    public void Exit()
    {
        Application.Quit();
    }




    //Game
    private void Start()
    {
        SetStartValues();
        GetScreenBorders();
    }

    private void GetScreenBorders()
    {
        Plane plane = new Plane(Vector3.up, Vector3.zero);

        Ray minRay = Camera.main.ViewportPointToRay(Vector2.zero);
        Ray maxRay = Camera.main.ViewportPointToRay(Vector2.one);

        float minHit;
        float maxHit;

        plane.Raycast(minRay, out minHit);
        plane.Raycast(maxRay, out maxHit);

        Vector3 minVector = minRay.GetPoint(minHit);
        Vector3 maxVector = maxRay.GetPoint(maxHit);

        minX = minVector.x;
        maxX = maxVector.x;

        minZ = minVector.z;
        maxZ = maxVector.z;
    }
    private void SetStartValues()
    {
        _keyboardOnly = true;
        _dead = false;
        _gameover = false;
        _invulnerable = false;
        _paused = true;
        _gameStarted = false;
        btn_continue.interactable = false;
        hp = 5;
        points = 0;
        Time.timeScale = 0;
    }


    private void CheckHP()
    {
        if (hp == 0)
        {
            GameOver();
            _gameover = true;
        }
    }
    private void GameOver()
    {
        var _bullets = GameObject.FindGameObjectsWithTag("Bullet");
        var _asteroids = GameObject.FindGameObjectsWithTag("Asteroid");
        var _ufoBullets = GameObject.FindGameObjectsWithTag("UFO_bullet");
        for (int i = 0; i < _asteroids.Length; i++)
        {
            if (_asteroids[i].GetComponent<Asteroid>()._big)
                _asteroids[i].SetActive(false);
            else
                Destroy(_asteroids[i]);
        }
        for (int i = 0; i < _bullets.Length; i++)
            _bullets[i].SetActive(false);
        for (int i = 0; i < _ufoBullets.Length; i++)
            _ufoBullets[i].SetActive(false);

        btn_continue.interactable = false;
        _gameStarted = false;
        HP.value = hp;

        Menu.SetActive(true);
        Player.SetActive(false);
        Asteroids.SetActive(false);
        UFO.SetActive(false);

        Camera.main.GetComponent<AudioSource>().loop = false;
    }

    private void Update()
    {
        CheckHP();
        Points.text = points.ToString();
        HP.value = hp;
    }

}
