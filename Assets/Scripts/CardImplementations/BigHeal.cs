public class BigHeal : AbstractSupportCard
{
    protected override void OnSupportCardActivation(Player player)
    {
        player.AddHealth(50);
    }
}
