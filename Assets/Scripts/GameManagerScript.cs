using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameManagerScript : MonoBehaviour
{
    [Header("Spawn")]
    [SerializeField] GameObject SpawnPoint1;
    [SerializeField] GameObject SpawnPoint2;
    [SerializeField] GameObject SpawnPoint3;
    [SerializeField] private GameObject[] platforms;
    [Tooltip("Intervalo mínimo entre spawns (segundos)")]
    [SerializeField] private float spawnIntervalMin = 1.0f;
    [Tooltip("Intervalo máximo entre spawns (segundos)")]
    [SerializeField] private float spawnIntervalMax = 2.0f;
    [Tooltip("Desvío vertical aleatorio al instanciar plataformas")]
    [SerializeField] private float spawnYVariance = 1.0f;

    [Header("GUI")]
    [SerializeField] TMP_Text gui;
    [SerializeField] private int startingLives = 3;

    private readonly List<GameObject> spawnPoints = new List<GameObject>(3);
    private float spawnTimer;
    private float nextSpawnTime;
    private int score;
    private int lives;
    private bool gameActive = true;

    void Start()
    {
        // Registrar puntos de spawn no nulos
        spawnPoints.Clear();
        if (SpawnPoint1 != null) spawnPoints.Add(SpawnPoint1);
        if (SpawnPoint2 != null) spawnPoints.Add(SpawnPoint2);
        if (SpawnPoint3 != null) spawnPoints.Add(SpawnPoint3);

        lives = Mathf.Max(0, startingLives);
        score = 0;
        spawnTimer = 0f;
        nextSpawnTime = UnityEngine.Random.Range(spawnIntervalMin, spawnIntervalMax);

        UpdateGUI();
    }

    void Update()
    {
        if (!gameActive) return;

        spawnTimer += Time.deltaTime;
        if (spawnTimer >= nextSpawnTime)
        {
            SpawnPlatform();
            spawnTimer = 0f;
            nextSpawnTime = UnityEngine.Random.Range(spawnIntervalMin, spawnIntervalMax);
        }
    }

    private void SpawnPlatform()
    {
        if (platforms == null || platforms.Length == 0) return;
        if (spawnPoints.Count == 0) return;

        // Elegir prefab y spawn point aleatoriamente
        GameObject prefab = platforms[UnityEngine.Random.Range(0, platforms.Length)];
        GameObject spawnPoint = spawnPoints[UnityEngine.Random.Range(0, spawnPoints.Count)];

        if (prefab == null || spawnPoint == null) return;

        Vector3 pos = spawnPoint.transform.position;
        pos.y += UnityEngine.Random.Range(-spawnYVariance, spawnYVariance);

        Instantiate(prefab, pos, Quaternion.identity);
    }

    private void UpdateGUI()
    {
        if (gui == null) return;
        gui.text = $"Puntos: {score}    Vidas: {lives}";
    }

    // API pública para que otros scripts notifiquen eventos
    public void AddScore(int amount = 1)
    {
        if (!gameActive) return;
        score += amount;
        UpdateGUI();
    }

    public void LoseLife()
    {
        if (!gameActive) return;

        lives = Mathf.Max(0, lives - 1);
        UpdateGUI();

        if (lives <= 0)
        {
            GameOver();
        }
    }

    public void RestartGame()
    {
        // Reiniciar estado básico; objetos en escena pueden necesitar restauración por separado.
        score = 0;
        lives = Mathf.Max(0, startingLives);
        gameActive = true;
        spawnTimer = 0f;
        nextSpawnTime = UnityEngine.Random.Range(spawnIntervalMin, spawnIntervalMax);
        UpdateGUI();
    }

    public void WinGame()
    {
        gameActive = false;
        if (gui != null)
        {
            gui.text = $"¡Has encontrado a tu hermano!\nPuntos: {score}";
        }
    }

    private void GameOver()
    {
        gameActive = false;
        if (gui != null)
        {
            gui.text = $"GAME OVER\nPuntos: {score}";
        }
    }
}
