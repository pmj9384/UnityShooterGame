using UnityEngine;

public class ZombieHandTrigger : MonoBehaviour
{
    public Zombie zombie; // 좀비 스크립트 참조

    private void Awake()
    {
        if (zombie == null)
        {
        zombie = GetComponentInParent<Zombie>();
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            var player = other.GetComponent<LivingEntity>();
            if (player != null && !player.IsDead)
            {
                zombie.AttackTarget(player); 
            }
        }
    }
}
