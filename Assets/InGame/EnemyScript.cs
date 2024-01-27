using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyScript : CharacterScript
{
    // Start is called before the first frame update
    override protected void Start()
    {
        base.Start();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnClickEvent()
    {
        var system = GameObject.Find("GameSystem").GetComponent<GameSceneBaseScript>();

        if (!gameObject.transform.parent)
        {
            // �����폜��
            return;
        }
        var cellScr = this.gameObject.transform.parent.GetComponent<FieldCellScript>();
        var plrScr = system.GetPlayerObject().GetComponent<PlayerScript>();
        var attackRange = system.GetClickRange();

        var dist = Util.CalcLocationDistance(system.GetPlayerLocation(), cellScr.GetLocation());
        if (dist > attackRange)
        {
            // �˒��O�̓N���b�N����
            return;
        }

        system.EnemyClickEvent(this.gameObject);
    }
}
