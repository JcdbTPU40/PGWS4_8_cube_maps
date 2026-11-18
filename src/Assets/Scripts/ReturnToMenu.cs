using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

/// <summary>
/// メニューから開いたシーンで Esc を押すとメニューに戻る。
/// メニュー側が Install() で生成し、シーンをまたいで生き残る。
/// </summary>
public class ReturnToMenu : MonoBehaviour
{
    static ReturnToMenu instance;

    string menuSceneName = "";
    GUIStyle hintStyle;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    static void ResetStatics() => instance = null;

    public static void Install(string menuSceneName)
    {
        if (instance == null)
        {
            var go = new GameObject("Return To Menu");
            DontDestroyOnLoad(go);
            instance = go.AddComponent<ReturnToMenu>();
        }
        instance.menuSceneName = menuSceneName;
    }

    void Update()
    {
        var keyboard = Keyboard.current;
        if (keyboard == null || !keyboard.escapeKey.wasPressedThisFrame) return;

        string target = menuSceneName;
        instance = null;
        Destroy(gameObject);
        SceneManager.LoadScene(target);
    }

    void OnGUI()
    {
        hintStyle ??= new GUIStyle(GUI.skin.label) { fontSize = 18, normal = { textColor = Color.white } };
        GUI.Label(new Rect(16.0f, 12.0f, 400.0f, 30.0f), "Esc: メニューへ戻る", hintStyle);
    }
}
