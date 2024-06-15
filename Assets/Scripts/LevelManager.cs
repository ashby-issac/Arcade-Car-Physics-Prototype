using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class LevelManager 
{
    private TextMeshProUGUI gameOverText;
    private GameObject gameOverPanel;

    public LevelManager(TextMeshProUGUI gameOverText, GameObject gameOverPanel)
    {
        this.gameOverText = gameOverText;
        this.gameOverPanel = gameOverPanel;

        GameplayController.Instance.OnGameOver += LevelEndPanel;
    }

    /*
     * Activate GameOver Panel when car has: fallen off, 
     * or when level's complete or when time has run out
     */
    private void LevelEndPanel(string text)
    {
        //if (isGameOver) return;

        gameOverText.text = text;
        gameOverPanel.SetActive(true);
        //isGameOver = true;
    }
}
