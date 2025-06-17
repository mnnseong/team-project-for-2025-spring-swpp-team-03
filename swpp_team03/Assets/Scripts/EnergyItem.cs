using UnityEngine;

public class EnergyItem : MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {
        Debug.Log($"[충돌 감지] {other.name}"); // 로그 찍기
        Debug.Log($"[충돌 감지] {other.gameObject.tag}"); // 로그 찍기

        if (other.CompareTag("Player"))
        {
            Debug.Log("[플레이어 감지됨] 에너지 증가!");
			EffectManager.Instance.PlayEnergyRecover(transform.position);

            StatusBar status = FindObjectOfType<StatusBar>();
            if (status != null)
            {
                status.AddEnergy(10f);
            }

            Destroy(gameObject); // 아이템 제거
        }
    }
}
