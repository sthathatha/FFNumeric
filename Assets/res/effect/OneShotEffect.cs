using UnityEngine;

public class OneShotEffect : MonoBehaviour
{
    private GameObject destroyObject;

    public OneShotEffect()
    {
        destroyObject = null;
    }

    /// <summary>
    /// �폜�I�u�W�F�N�g�w��
    /// </summary>
    /// <param name="obj"></param>
    public void SetDestroyObject(GameObject obj)
    {
        destroyObject = obj;
    }

    // Update is called once per frame
    void Update()
    {
        var anim = GetComponent<Animator>();
        var stateInfo = anim.GetCurrentAnimatorStateInfo(0);
        if (stateInfo.normalizedTime >= 1f)
        {
            if (destroyObject != null)
            {
                Destroy(destroyObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }
    }
}
