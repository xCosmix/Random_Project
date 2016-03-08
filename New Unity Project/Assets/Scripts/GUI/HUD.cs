using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class HUD : MonoBehaviour {

    public Image heartIcon;
    public Image aim;
    public Text score;
    public float aimMaxDist;
    public float aimMinDist;
    public Sprite heartFill;
    public Sprite heartEmpty;

    protected Player player;
    protected Vector2 playerScreenPos;

	// Use this for initialization
	void Awake () {
        player = Player.player;
	}

    void Update()
    {
        if (player == null) player = Player.player;
        playerScreenPos = RectTransformUtility.WorldToScreenPoint(Camera.main, player.transform.position);
        DrawAim();

        if (player.stats.life == 2)
        {
            heartIcon.sprite = heartFill;
        }
        else
        {
            heartIcon.sprite = heartEmpty;
        }

        string points = player.Score < 10 ? 
            "000" + player.Score : 
            player.Score < 100 ? 
            "00" + player.Score :
            player.Score < 1000 ? 
            "0" + player.Score : 
            player.Score+"";

        score.text = points;
    }
    private void DrawAim()
    {
        Vector2 target = player.AimDir.sqrMagnitude > 0.2f ? playerScreenPos + player.AimDir * aimMaxDist : playerScreenPos + player.AimDir * aimMinDist;
        Vector2 pos = (target - (Vector2)aim.transform.position) * Time.deltaTime * 6.0f;
        aim.transform.position += (Vector3)pos;
    }
}
