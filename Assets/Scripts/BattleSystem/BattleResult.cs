// Assets/Scripts/BattleSystem/BattleResult.cs
public enum BattleOutcome { Victory, Defeat, Draw }

public class BattleResult
{
    public BattleOutcome Outcome { get; set; }
    public int TowersDestroyed { get; set; }
    public int TowersLost { get; set; }
    public float BattleTime { get; set; }

    public bool PlayerWon => Outcome == BattleOutcome.Victory;
}