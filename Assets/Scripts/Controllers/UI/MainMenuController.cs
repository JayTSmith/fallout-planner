using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

[RequireComponent(typeof(UIDocument))]
public class MainMenuController : MonoBehaviour
{
    private Button simBtn;
    private Button dataBtn;
    private Button buildBtn;

    // Start is called before the first frame update
    void Start()
    {
        UIDocument doc = GetComponent<UIDocument>();
        simBtn = doc.rootVisualElement.Q<Button>("simBtn");
        dataBtn = doc.rootVisualElement.Q<Button>("dataBtn");
        buildBtn = doc.rootVisualElement.Q<Button>("buildBtn");

        //simBtn.RegisterCallback<MouseDownEvent>(SimCallback);
        dataBtn.RegisterCallback<MouseDownEvent>(DataCallback);
        buildBtn.RegisterCallback<MouseDownEvent>(SimBCallback);
        simBtn.clickable.clicked += SimCallback;
        simBtn.text = "W";
    }

    void SimBCallback(MouseDownEvent evt) {
        SceneManager.LoadSceneAsync(2);
    }

    void SimCallback()
    {
        SceneManager.LoadSceneAsync(3);

    }

    void DataCallback(MouseDownEvent evt)
    {
        SceneManager.LoadSceneAsync(1);
    }
}
