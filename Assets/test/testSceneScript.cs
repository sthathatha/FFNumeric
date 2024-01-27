using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class testSceneScript : MonoBehaviour
{
    protected GameObject player;
    protected GameObject enemy;

    protected int gameState;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.Find("player");
        enemy = GameObject.Find("slime");

        gameState = 0;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="enm"></param>
    public IEnumerator EnemyClick(GameObject enm)
    {
        if (gameState != 0)
        {
            yield return null;
        }
        gameState = 1;

        var enmScript = enm.GetComponent<CharacterScript>();
        var plrScript = player.GetComponent<CharacterScript>();

        while (true)
        {
            // ÉvÉåÉCÉÑÅ[çUåÇ
            var pAtk = plrScript.GetAttackNum();
            enmScript.Damage(pAtk);

            plrScript.AnimateAttack();
            yield return new WaitForSeconds(0.4f);

            if (!enmScript.IsAlive())
            {
                // ê¨í∑
                plrScript.AddHp(enmScript.GetMaxHp());
                break;
            }

            // ìGçUåÇ
            var eAtk = enmScript.GetAttackNum();
            plrScript.Damage(eAtk);

            enmScript.AnimateAttack();
            yield return new WaitForSeconds(0.4f);

            if (!plrScript.IsAlive())
            {
                // GameOver
                break;
            }
        }

        gameState = 0;
    }
}
