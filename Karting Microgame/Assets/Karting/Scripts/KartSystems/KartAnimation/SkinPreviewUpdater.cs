using KartGame.KartSystems;
using UnityEngine;

public class SkinPreviewUpdater : MonoBehaviour
{
    public Transform playerIdleParent; // PlayerIdle의 부모 (즉, KartVisual)
    public KartPlayerAnimator kartPlayerAnimator;

    public string targetName1 = "KartVisual"; // 교체할 자식 이름
    public string targetName2 = "PlayerIdle"; // 교체할 자식 이름

    private GameObject currentCharacter;

    public void ReplacePlayerIdle(GameObject characterPrefab)
    {
        // 기존 PlayerIdle 제거
        Transform oldParent = playerIdleParent.Find(targetName1);
        Transform old = oldParent.Find(targetName2);

        if (old != null)
        {
            DestroyImmediate(old.gameObject); // 즉시 제거
        }

        // 새 캐릭터 프리팹 생성 후 같은 이름으로 넣기
        currentCharacter = Instantiate(characterPrefab, oldParent);
        currentCharacter.name = targetName2; // 이름 통일! (애니메이터 타겟 매핑용)

        // 위치 초기화 (로컬 위치 0)
        currentCharacter.transform.localPosition = new Vector3(0.00942993164f, -0.200000003f, -0.0899999142f);
        // 🔥 스케일은 그대로 유지
    }

    public void ReplaceMainPlayerIdle(GameObject characterPrefab)
    {
        // 기존 PlayerIdle 제거
        Transform oldParent = playerIdleParent.Find(targetName1);
        Transform old = oldParent.Find(targetName2);

        if (old != null)
        {
            DestroyImmediate(old.gameObject); // 즉시 제거
        }

        // 새 캐릭터 프리팹 생성 후 같은 이름으로 넣기
        currentCharacter = Instantiate(characterPrefab, oldParent);
        currentCharacter.name = targetName2; // 이름 통일! (애니메이터 타겟 매핑용)

        // 위치 초기화 (로컬 위치 0)
        currentCharacter.transform.localPosition = new Vector3(0.00942993164f, -0.200000003f, -0.0899999142f);
        // 🔥 스케일은 그대로 유지

        var kartAnimator = currentCharacter.GetComponent<Animator>();
        kartPlayerAnimator.SetAnimator(kartAnimator);
    }
    
    //public void ReplacePlayerAnimator
}