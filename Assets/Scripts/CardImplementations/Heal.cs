public class Heal : AbstractSupportCard
{
    protected override void OnSupportCardActivation(Player player)
    {
        player.AddHealth(25);
    }
}