using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Inspector : MonoBehaviour
{
    public static Inspector instance;
    [SerializeField] CameraController cameraController;
    [SerializeField] TextMeshProUGUI title;
    [SerializeField] TextMeshProUGUI body;
    public int frameOfLastInspection = 0;

    IInspectable inspectable;
    private void Awake()
    {
        instance = this;
        SetVisibility(false);
    }

    public void Inspect(IInspectable _inspectable)
    {
        inspectable = _inspectable;
        frameOfLastInspection = Time.frameCount;

        if (_inspectable is Agent agent)
            cameraController.FollowTarget(agent.transform);
    }

    public void Update()
    {
        Dictionary<string, string> inspectionData = inspectable.InspectionData();
        title.text = inspectionData["title"];
        body.text = inspectionData["body"];
    }

    public void SetVisibility(bool visible)
    {
        gameObject.SetActive(visible);
    }
}

public interface IInspectable
{
    public Dictionary<string, string> InspectionData();
}
