using System.Collections;
using UnityEngine;
using UnityEngine.UI;

/*
 * 씬 전환 시 서버로부터 패킷을 받아서 게임 씬을 준비하기위한
 * 페이크 로딩 씬
 */

public class FakeLoadingScene : MonoBehaviour
{
    [SerializeField]
    [Tooltip("페이크 로딩 씬을 표시할 시간")]
    float FakeLoadingTime = 3f;

    [SerializeField]
    Image LoadingImg;

    [SerializeField]
    Slider LoadingProgressBar;

    [SerializeField]
    Text TipText;

    static string[] tips =
    {
        "Tip : 흰색 테두리는 일반 등급, 녹색 테두리는 희귀 등급 무기입니다.",
        "Tip : 몬스터들은 저마다 다른 공격을 시전합니다.",
        "Tip : 궁극기 게이지가 가득 찼을 때 궁극기를 시전할 수 있습니다.",
        "Tip : 다른 플레이어들과 협동하여 레이드 컨텐츠를 클리어 해보세요!"
    };

    void Awake()
    {
        Init();
    }

    void Init()
    {
        int randomNum = UnityEngine.Random.Range(1, tips.Length);

        // 로딩 이미지 설정
        LoadingImg.sprite = Managers.Resource.Load<Sprite>("Textures/LoadingImg/LoadingImg" + randomNum);

        // tip 설정
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

            // 이미지의 채워진 정도를 갱신
            float fillAmount = 1 - currentTime / FakeLoadingTime;
            LoadingProgressBar.value = fillAmount;

            yield return null;
        }

        // 페이크 로딩 씬 종료
        Destroy(gameObject, 1.0f);
    }
}
