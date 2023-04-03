using UnityEngine;
using UnityEngine.UI;

public class BuyRobot : MonoBehaviour
{
    public Text nameRobot;
    public Text[] resources = new Text[ResourcesStat.countResource];
    public float[] resourcesValue = new float[ResourcesStat.countResource];
    public PlayerInterface player;
    public void OnBuy()
    {
        if(player == null)
        {
            Debug.LogError("нет игрока, чтобы покупать");
            return;
        }
        if(player.MyBases.Count == 0)
        {
            Debug.LogError("нет ни одной базы, чтобы покупать");
            return;
        }
        player.MyBases[player.targetBase].ValidateCreateRobot(resourcesValue, nameRobot.text);
    }
}
