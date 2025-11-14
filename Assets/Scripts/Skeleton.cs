using UnityEngine;

public class Skeleton : MonoBehaviour
{
    public SwordSkeleton swordSkeleton;
    public int skeletonDamage;

    private void Start()
    {
        swordSkeleton.damage = skeletonDamage;
    }
}
