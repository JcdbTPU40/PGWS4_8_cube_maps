using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary>
/// ガラス玉メインメニュー。
/// マウスを重ねる（または ←→ キー）でガラス玉を選び、クリック（または Enter）でそのシーンを開く。
/// </summary>
public class MainMenuController : MonoBehaviour
{
    [SerializeField] Camera menuCamera = default!;
    [SerializeField] MenuOrb[] orbs = default!;
    [SerializeField] Text selectionLabel = default!;
    [SerializeField] string idleMessage = "ガラス玉を選んでください";

    int selected = -1;
    bool loading;

    void Update()
    {
        if (loading) return;

        var mouse = Mouse.current;
        var keyboard = Keyboard.current;
        bool confirm = false;

        // マウス: 動かしたときだけ選択を更新する（キーボードで選んだ状態を上書きしないため）
        if (mouse != null)
        {
            bool click = mouse.leftButton.wasPressedThisFrame;
            if (click || mouse.delta.ReadValue().sqrMagnitude > 0.0f)
            {
                int hit = PickOrb(mouse.position.ReadValue());
                selected = hit;
                confirm = click && hit >= 0;
            }
        }

        // キーボード: ←→ / A D で移動、Enter / Space で決定
        if (keyboard != null && orbs.Length > 0)
        {
            if (keyboard.rightArrowKey.wasPressedThisFrame || keyboard.dKey.wasPressedThisFrame)
                selected = selected < 0 ? 0 : (selected + 1) % orbs.Length;
            if (keyboard.leftArrowKey.wasPressedThisFrame || keyboard.aKey.wasPressedThisFrame)
                selected = selected < 0 ? orbs.Length - 1 : (selected + orbs.Length - 1) % orbs.Length;
            if (selected >= 0 && (keyboard.enterKey.wasPressedThisFrame || keyboard.spaceKey.wasPressedThisFrame))
                confirm = true;
        }

        for (int i = 0; i < orbs.Length; i++) orbs[i].Selected = (i == selected);
        if (selectionLabel != null)
            selectionLabel.text = selected >= 0 ? orbs[selected].DisplayName : idleMessage;

        if (confirm) Open(orbs[selected]);
    }

    int PickOrb(Vector2 screenPosition)
    {
        Ray ray = menuCamera.ScreenPointToRay(screenPosition);
        if (!Physics.Raycast(ray, out RaycastHit hit, 100.0f)) return -1;
        var orb = hit.collider.GetComponentInParent<MenuOrb>();
        return orb != null ? System.Array.IndexOf(orbs, orb) : -1;
    }

    void Open(MenuOrb orb)
    {
        loading = true;
        // 開いた先のシーンから Esc でここに戻れるようにしておく
        ReturnToMenu.Install(SceneManager.GetActiveScene().name);
        SceneManager.LoadScene(orb.SceneName);
    }
}
