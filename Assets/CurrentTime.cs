using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class CurrentTime : MonoBehaviour
{
    TextMeshProUGUI textMesh;

    private void Awake()
    {
        textMesh = GetComponent<TextMeshProUGUI>();
    }

    private void Update()
    {
        textMesh.text = $"{Time.timeSinceLevelLoad:0.0} seconds";
    }
}
