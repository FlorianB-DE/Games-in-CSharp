using UnityEngine;

public abstract class AbstractSupportCard : ICard
{
    public void OnActivation(ICard.PlayOutData data)
    {
        OnSupportCardActivation(data.Player);
    }

    protected void AddParticleSystem(GameObject target, Material material)
    {
    }

    protected abstract void OnSupportCardActivation(Player player);
}