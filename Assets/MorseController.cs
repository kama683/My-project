using UnityEngine;
using TMPro;
using System.Collections.Generic;
using UnityEngine.UI; // Обязательно добавь эту строку для работы с кнопками и картинками

public class MorseController : MonoBehaviour
{
    [Header("UI элементы")]
    public TextMeshProUGUI targetText; 
    public TextMeshProUGUI resultText;  
    public TextMeshProUGUI currentSymbolsText;
    public GameObject hintPanel; // Сюда перетащим наше окно с алфавитом

    [Header("Настройки")]
    public AudioSource audioSource;
    public AudioClip beepSound;
    public float dashThreshold = 0.25f; 
    public float letterPause = 0.6f;    

    private string[] levels = { "SOS", "AIR", "H2O", "OPEN" };
    private int currentLevelIndex = 0;

    private string currentLetterCode = ""; 
    private string translatedWord = "";   
    private float lastReleaseTime;
    private bool isPressing = false;
    private float pressTime;
    private bool letterProcessed = true;

    private Dictionary<string, char> morseDict = new Dictionary<string, char>() {
        {".-", 'A'}, {"-...", 'B'}, {"-.-.", 'C'}, {"-..", 'D'}, {".", 'E'},
        {"..-.", 'F'}, {"--.", 'G'}, {"....", 'H'}, {"..", 'I'}, {".---", 'J'},
        {"-.-", 'K'}, {".-..", 'L'}, {"--", 'M'}, {"-.", 'N'}, {"---", 'O'},
        {".--.", 'P'}, {"--.-", 'Q'}, {".-.", 'R'}, {"...", 'S'}, {"-", 'T'},
        {"..-", 'U'}, {"...-", 'V'}, {".--", 'W'}, {"-..-", 'X'}, {"-.--", 'Y'},
        {"--..", 'Z'}, {"-----", '0'}, {".----", '1'}, {"..---", '2'}, {"...--", '3'},
        {"....-", '4'}, {".....", '5'}, {"-....", '6'}, {"--...", '7'}, {"---..", '8'}, {"----.", '9'}
    };

    void Start() {
        UpdateLevelUI();
        if(hintPanel != null) hintPanel.SetActive(false); // Прячем подсказку в начале
    }

    void Update() {
        // Если открыта подсказка, ввод не считываем (чтобы не пищало случайно)
        if (hintPanel != null && hintPanel.activeSelf) return;

        if (Input.GetMouseButtonDown(0)) {
            pressTime = Time.time;
            isPressing = true;
            letterProcessed = false;
            if (audioSource) audioSource.Play();
        }

        if (Input.GetMouseButtonUp(0)) {
            isPressing = false;
            float duration = Time.time - pressTime;
            currentLetterCode += (duration < dashThreshold) ? "." : "-";
            currentSymbolsText.text = currentLetterCode;
            lastReleaseTime = Time.time;
            if (audioSource) audioSource.Stop();
        }

        if (!isPressing && !letterProcessed && (Time.time - lastReleaseTime > letterPause)) {
            ProcessLetter();
        }
    }

    void ProcessLetter() {
        letterProcessed = true;
        if (morseDict.ContainsKey(currentLetterCode)) {
            translatedWord += morseDict[currentLetterCode];
            resultText.text = translatedWord;
            CheckWord();
        }
        currentLetterCode = ""; 
        currentSymbolsText.text = "";
    }

    // --- НОВЫЕ ФУНКЦИИ ДЛЯ КНОПОК ---

    public void DeleteLastLetter() {
        if (translatedWord.Length > 0) {
            translatedWord = translatedWord.Substring(0, translatedWord.Length - 1);
            resultText.text = translatedWord;
        }
    }

    public void ToggleHint() {
        if (hintPanel != null) {
            bool isActive = hintPanel.activeSelf;
            hintPanel.SetActive(!isActive); // Включает, если выключено, и наоборот
        }
    }

    // --------------------------------

    void CheckWord() {
        if (translatedWord == levels[currentLevelIndex]) {
            currentLevelIndex++;
            if (currentLevelIndex < levels.Length) {
                translatedWord = "";
                resultText.text = "ПРИНЯТО!";
                Invoke("UpdateLevelUI", 1.0f); 
            } else {
                targetText.text = "СВЯЗЬ УСТАНОВЛЕНА!";
                resultText.text = "ПОБЕДА";
            }
        }
    }

    void UpdateLevelUI() {
        translatedWord = "";
        resultText.text = "";
        targetText.text = "ЦЕЛЬ: " + levels[currentLevelIndex];
    }
}