using UnityEngine;
using UnityEngine.UI;

public class LargerEffectAnim : MonoBehaviour
{
    public float animationDuration; // 애니메이션 지속 시간
    private Vector3 initialScale;
    private Image image;
    private float elapsedTime;

    private void Start()
    {
        image = GetComponent<Image>();
        initialScale = transform.localScale;
        elapsedTime = 0f;
    }

    private void Update()
    {
        elapsedTime += Time.deltaTime;

        if (elapsedTime > animationDuration)
            elapsedTime = animationDuration;

        float t = elapsedTime / animationDuration;
        float scale = Mathf.Lerp(0f, 1f, t);
        transform.localScale = initialScale * scale;
    }
}