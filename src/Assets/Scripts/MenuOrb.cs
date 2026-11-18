using UnityEngine;

/// <summary>
/// メインメニューの項目になるガラス玉。
/// 選択されると屈折率と色分散をなめらかに上げ、少し浮かせて大きくする。
/// （講義の「スクリプトからシェーダパラメータを操作する」の応用）
/// </summary>
public class MenuOrb : MonoBehaviour
{
    [SerializeField] string sceneName = "";
    [SerializeField] string displayName = "";

    [Header("屈折率 (Refract Dispersion Shader Graph の _IOR)")]
    [SerializeField, Range(1.0f, 2.5f)] float idleIOR = 1.15f;
    [SerializeField, Range(1.0f, 2.5f)] float selectedIOR = 1.5f;

    [Header("色分散 (_Dispersion)")]
    [SerializeField, Range(0.0f, 0.2f)] float idleDispersion = 0.008f;
    [SerializeField, Range(0.0f, 0.2f)] float selectedDispersion = 0.04f;

    [Header("動き")]
    [SerializeField] float selectedScale = 1.25f;
    [SerializeField] float selectedLift = 0.25f;
    [SerializeField] float bobAmplitude = 0.05f;
    [SerializeField, Range(0.0f, 2.0f)] float bobFrequency = 0.25f;
    [SerializeField] float response = 5.0f;

    static readonly int IorId = Shader.PropertyToID("_IOR");
    static readonly int DispersionId = Shader.PropertyToID("_Dispersion");

    Renderer orbRenderer = default!;
    MaterialPropertyBlock block = default!;
    Vector3 basePosition;
    Vector3 baseScale;
    float blend;   // 0: 非選択, 1: 選択
    float phase;   // 玉ごとに揺れのタイミングをずらす

    public bool Selected { get; set; }
    public string SceneName => sceneName;
    public string DisplayName => displayName;

    void Awake()
    {
        orbRenderer = GetComponent<Renderer>();
        block = new MaterialPropertyBlock();
        basePosition = transform.position;
        baseScale = transform.localScale;
        phase = basePosition.x * 1.7f + basePosition.z * 0.9f;
        Apply(0.0f);
    }

    void Update()
    {
        float target = Selected ? 1.0f : 0.0f;
        blend = Mathf.MoveTowards(blend, target, response * Time.deltaTime * Mathf.Max(0.05f, Mathf.Abs(target - blend)));
        Apply(Mathf.SmoothStep(0.0f, 1.0f, blend));
    }

    void Apply(float t)
    {
        // マテリアルを複製せずに、この玉だけのパラメータを渡す
        orbRenderer.GetPropertyBlock(block);
        block.SetFloat(IorId, Mathf.Lerp(idleIOR, selectedIOR, t));
        block.SetFloat(DispersionId, Mathf.Lerp(idleDispersion, selectedDispersion, t));
        orbRenderer.SetPropertyBlock(block);

        float bob = bobAmplitude * Mathf.Sin(2.0f * Mathf.PI * bobFrequency * Time.time + phase);
        transform.position = basePosition + Vector3.up * (bob + selectedLift * t);
        transform.localScale = baseScale * Mathf.Lerp(1.0f, selectedScale, t);
    }
}
