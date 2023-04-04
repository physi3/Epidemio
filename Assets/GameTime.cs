using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class GameTime : MonoBehaviour
{
    public static GameTime instance;

    public float hoursEllapsed = 0;
    public float currentHourLength = 5f;

    [Header("References")]
    [SerializeField] TextMeshProUGUI timeUI;
    [SerializeField] TextMeshProUGUI dayUI;
    [SerializeField] Camera mainCamera;
    [Header("Time Controls")]
    [SerializeField] Button pause;
    [SerializeField] Button play;
    [SerializeField] Button fastforward;

    public static float TimeOfDay {
        get => instance.hoursEllapsed % 24;
    }

    public static float DeltaTime {
        get => Time.deltaTime / instance.currentHourLength;
    }

    public static string ReadableTime {
        get => $"{Mathf.FloorToInt(TimeOfDay):00}:{(TimeOfDay % 1f)*60:00}";
    }

    public static float SunHeight {
        get => -Mathf.Cos(Mathf.PI * TimeOfDay / 12f);
    }

    private void Awake()
    {
        instance = this;

        // Time controls
        pause.onClick.AddListener(Pause);
        play.onClick.AddListener(Play);
        fastforward.onClick.AddListener(Fastforward);
    }

    void Update()
    {
        hoursEllapsed += Time.deltaTime / currentHourLength;
        timeUI.text = ReadableTime;
        dayUI.text = $"Day {Mathf.FloorToInt(hoursEllapsed / 24)}";

        float currentColour = (7.5f + (2.5f * SunHeight)) / 100f;
        mainCamera.backgroundColor = new Color(currentColour, currentColour, currentColour);
    }

    void Pause() {
        currentHourLength = float.PositiveInfinity;
    }

    void Play() {
        currentHourLength = 5f;
    }

    void Fastforward() {
        currentHourLength = 1f;
    }
}
