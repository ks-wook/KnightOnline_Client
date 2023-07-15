using System.Collections;
using UnityEngine;
using UnityEngine.UI;

/*
 * �� ��ȯ �� �����κ��� ��Ŷ�� �޾Ƽ� ���� ���� �غ��ϱ�����
 * ����ũ �ε� ��
 */

public class FakeLoadingScene : MonoBehaviour
{
    [SerializeField]
    [Tooltip("����ũ �ε� ���� ǥ���� �ð�")]
    float FakeLoadingTime = 3f;

    [SerializeField]
    Image LoadingImg;

    [SerializeField]
    Slider LoadingProgressBar;

    [SerializeField]
    Text TipText;

    static string[] tips =
    {
        "Tip : ��� �׵θ��� �Ϲ� ���, ��� �׵θ��� ��� ��� �����Դϴ�.",
        "Tip : ���͵��� ������ �ٸ� ������ �����մϴ�.",
        "Tip : �ñر� �������� ���� á�� �� �ñر⸦ ������ �� �ֽ��ϴ�.",
        "Tip : �ٸ� �÷��̾��� �����Ͽ� ���̵� �������� Ŭ���� �غ�����!"
    };

    void Awake()
    {
        Init();
    }

    void Init()
    {
        int randomNum = UnityEngine.Random.Range(1, tips.Length);

        // �ε� �̹��� ����
        LoadingImg.sprite = Managers.Resource.Load<Sprite>("Textures/LoadingImg/LoadingImg" + randomNum);

        // tip ����
        TipText.text = tips[randomNum];


        Screen.SetResolution(960, 540, false);
        StartCoroutine(ViewFakeLoadingScene());

    }

    IEnumerator ViewFakeLoadingScene()
    {
        float currentTime = FakeLoadingTime;

        while (currentTime > 0f)
        {
            currentTime -= Time.deltaTime;

            // �̹����� ä���� ������ ����
            float fillAmount = 1 - currentTime / FakeLoadingTime;
            LoadingProgressBar.value = fillAmount;

            yield return null;
        }

        // ����ũ �ε� �� ����
        Destroy(gameObject, 1.0f);
    }
}
