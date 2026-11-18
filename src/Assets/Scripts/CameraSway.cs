using UnityEngine;

/// <summary>
/// カメラを左右にゆっくり揺らす。
/// 視点が動くとガラス玉の屈折・反射の見え方が変わるので、静止画より効果が伝わる。
/// </summary>
public class CameraSway : MonoBehaviour
{
    [SerializeField] Vector3 lookTarget = new Vector3(0.0f, 2.5f, 0.0f);
    [SerializeField] float amplitude = 0.8f;
    [SerializeField] float verticalAmplitude = 0.15f;
    [SerializeField, Range(0.0f, 1.0f)] float frequency = 0.05f;

    Vector3 basePosition;

    void Start()
    {
        basePosition = transform.position;
    }

    void LateUpdate()
    {
        float angle = 2.0f * Mathf.PI * frequency * Time.time;
        transform.position = basePosition
            + transform.right * (amplitude * Mathf.Sin(angle))
            + Vector3.up * (verticalAmplitude * Mathf.Sin(2.0f * angle));
        transform.LookAt(lookTarget);
    }
}
