using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIController
{
    private AudioSource _winAudio;
    private AudioSource _gameOverAudio;
    
    private Text _playerHpText;
    private Text _enemyLeftText;
    private Text _hardBulletsLeftText;
    private Text _levelNameText;
    private Image _playerHpProgressBar;

    private GameObject _goPanel;
    private Text _goPanelText;
    
    private Game _game;
    private float _startPlayerHp;

    private float _hpSize;

    public UIController(Game game, AudioSource win, AudioSource gameOver, Text hpText, Image hpBar, 
        Text enemyLeftText, Text hardBulletsLeftText, Text levelNameText, GameObject goPanel)
    {
        _game = game;
        _winAudio = win;
        _gameOverAudio = gameOver;
        _playerHpText = hpText;
        _playerHpProgressBar = hpBar;
        _startPlayerHp = game.Player.Hp;
        _enemyLeftText = enemyLeftText;
        _hardBulletsLeftText = hardBulletsLeftText;
        _levelNameText = levelNameText;
        _goPanel = goPanel;
        _goPanelText = goPanel.GetComponentInChildren<Text>();
        _hpSize = 100.0f / _startPlayerHp;
    }

    public void UpdateUI()
    {
        _playerHpText.text = $"HP: {Mathf.Round(_game.Player.Hp * _hpSize)}";
        _playerHpProgressBar.fillAmount = _game.Player.Hp / _startPlayerHp;
        _enemyLeftText.text = $"X{_game.TotalEnemiesOnLevelCount}";
        _hardBulletsLeftText.text = $"X{_game.HardBulletsCount}";
        _levelNameText.text = $"Level: {_game.TempLevel.levelNumber} {_game.TempLevel.levelName}";
    }

    public IEnumerator GameOver()
    {
        _game.IsGameFinished = true;
        _gameOverAudio.Play();
        float waitForUI = 1f;
        
        yield return new WaitForSeconds(waitForUI);
        
        _goPanel.SetActive(true);
        _goPanelText.text = "GAME OVER";
        
        yield return new WaitForSeconds(_gameOverAudio.clip.length - waitForUI);
        
        SceneManager.LoadScene(0);
    }
    
    public IEnumerator Win()
    {
        _game.IsGameFinished = true;
        int newVal = _game.TempLevel.levelNumber;
        
        if (PlayerPrefs.GetInt("LevelPassed") <= newVal)
        {
            PlayerPrefs.SetInt("LevelPassed", newVal);   
        }

        _winAudio.Play();
        float waitForUI = 2f;
        
        yield return new WaitForSeconds(waitForUI);
        
        _goPanel.SetActive(true);
        _goPanelText.text = "YOU WON";
        
        yield return new WaitForSeconds(_winAudio.clip.length - waitForUI);
        
        SceneManager.LoadScene(0);
    }
}
