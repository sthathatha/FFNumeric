using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.UI;

public class CharacterSceneScript : MonoBehaviour
{
    public Image imgCharaPic;
    public TMP_Text txtPersonalityExp;
    public TMP_Text txtPersonalityName;
    public TMP_Text txtSkillExp;
    public TMP_Text txtSkillName;
    public TMP_Text txtSkillCharge;
    public TMP_Text txtSkillStock;
    public TMP_Text txtInitHp;
    public TMP_Text txtInitRange;
    public TMP_Text txtNameE;
    public TMP_Text txtNameJ;

    private List<ResourceRequest> textureList;
    private int displayCharacter;
    private Coroutine changeCoroutine;

    #region �\��������
    private struct DisplayStatus
    {
        public string nameE;
        public string nameJ;
        public int initHP;
        public int initRange;
        public string personalityName;
        public string personalityExplain;
        public string skillName;
        public string skillExplain;
        public int skillCharge;
        public int skillStock;
    }

    private readonly DisplayStatus STAT_DROWS = new DisplayStatus()
    {
        nameE = "DROWS",
        nameJ = "�h���V�[",
        initHP = (int)System.Math.Floor(Constant.DROWS_PARAM.InitHP),
        initRange = 1,
        personalityName = "�x���Z���N",
        personalityExplain = $"�ő�{Constant.SKILL_DROWS_ATTACK_LIMIT}��܂ŁA�ꌂ�œG��|������^�[�����I�������Ɉړ������������֒ǉ��U�����s��" +
        "\n�ǉ��U���œ|���Ȃ������ꍇ�͔������󂯂��A�^�[�����I������",
        skillName = "�J���l�[�W�h���C��",
        skillExplain = $"{Constant.SKILL_DROWS_KEEP_TURN}�^�[���̊ԁA�^����_���[�W��{(int)Constant.SKILL_DROWS_ATTACK_RATE * 100}���ɂȂ�",
        skillCharge = Constant.DROWS_PARAM.SkillGaugeLength,
        skillStock = Constant.DROWS_PARAM.SkillStockMax
    };
    private readonly DisplayStatus STAT_ERAPS = new DisplayStatus()
    {
        nameE = "ERAPS",
        nameJ = "�G��",
        initHP = (int)System.Math.Floor(Constant.ERAPS_PARAM.InitHP),
        initRange = 1,
        personalityName = "�R�m��",
        personalityExplain = "�^�[���J�n���ƓG�̌��j���ɃV�[���h�𐶐�����",
        skillName = "���ݏo������ɖ�������",
        skillExplain = "�V�[���h�̐������x���㏸����i�X�e�[�W���i���j",
        skillCharge = Constant.ERAPS_PARAM.SkillGaugeLength,
        skillStock = Constant.ERAPS_PARAM.SkillStockMax
    };
    private readonly DisplayStatus STAT_EXA = new DisplayStatus()
    {
        nameE = "EXA",
        nameJ = "�G�O�U",
        initHP = (int)System.Math.Floor(Constant.EXA_PARAM.InitHP),
        initRange = 1,
        personalityName = "�|����",
        personalityExplain = "�U�����A���E�̓G�ɂ��_���[�W��^����",
        skillName = "�M�����O�X�^�[",
        skillExplain = "�Ώۂƍ��E��100���_���[�W��^���A�^�����_���[�W����������\n�Ώۂ�|�����ꍇ�͈ړ�����",
        skillCharge = Constant.EXA_PARAM.SkillGaugeLength,
        skillStock = Constant.EXA_PARAM.SkillStockMax
    };
    private readonly DisplayStatus STAT_WORRA = new DisplayStatus()
    {
        nameE = "WORRA",
        nameJ = "�E�[��",
        initHP = (int)System.Math.Floor(Constant.WORRA_PARAM.InitHP),
        initRange = 1,
        personalityName = "�A��",
        personalityExplain = "�A���U�����s��\n�r���œ|�����ꍇ�A�����O�ɉ��̓G�ɍU������\n�������퓬�̏ꍇ�͑Ώۂ̎��͂U��",
        skillName = "�T�e���C�g�X�i�C�v",
        skillExplain = $"�퓬�͈͂�1������i�X�e�[�W���i���F�ő�{Constant.SKILL_WORRA_MAX_RANGE}�j" +
        $"\n�˒��ő厞�ɂ͍U���񐔂�1������i�X�e�[�W���i���j",
        skillCharge = Constant.WORRA_PARAM.SkillGaugeLength,
        skillStock = Constant.WORRA_PARAM.SkillStockMax
    };
    private readonly DisplayStatus STAT_KOOB = new DisplayStatus()
    {
        nameE = "KOOB",
        nameJ = "�N�[",
        initHP = (int)System.Math.Floor(Constant.KOOB_PARAM.InitHP),
        initRange = Constant.SKILL_KOOB_ATTACK_RANGE,
        personalityName = "�V��",
        personalityExplain = $"�������x��{(int)(Constant.SKILL_KOOB_POWUP_RATE * 100)}���ɂȂ�",
        skillName = "���O�i���N",
        skillExplain = $"{Constant.SKILL_KOOB_RAGNAROK_RANGE}�}�X���̓G�S����{(int)Constant.SKILL_KOOB_RAGNAROK_RATE * 100}���_���[�W��^���A{Constant.SKILL_KOOB_RAGNAROK_POW_INVALID_TURN}�^�[���̐����s��{Constant.SKILL_KOOB_RAGNAROK_PARALYZE_TURN}��̔����s��t�^����" +
        "\n�G��|�����ꍇ�A�c�����G�̋߂��Ɉړ�����",
        skillCharge = Constant.KOOB_PARAM.SkillGaugeLength,
        skillStock = Constant.KOOB_PARAM.SkillStockMax
    };
    private readonly DisplayStatus STAT_YOU = new DisplayStatus()
    {
        nameE = "YOU",
        nameJ = "�I",
        initHP = (int)System.Math.Floor(Constant.YOU_PARAM.InitHP),
        initRange = 1,
        personalityName = "���m��",
        personalityExplain = $"�󂯂�_���[�W��{(int)(Constant.SKILL_YOU_DAMAGE_RATE * 100)}���ɂȂ�" +
        $"\n�^����_���[�W��{(int)(Constant.SKILL_YOU_ATTACK_RATE * 100)}���ɂȂ�",
        skillName = "���e�v�M",
        skillExplain = $"{Constant.SKILL_YOU_KILL_RANGE}�}�X�ȓ��̑Ώ�1�̂𑦎������A�ړ�����B",
        skillCharge = Constant.YOU_PARAM.SkillGaugeLength,
        skillStock = Constant.YOU_PARAM.SkillStockMax
    };
    #endregion

    /// <summary>
    /// �R���X�g���N�^
    /// </summary>
    public CharacterSceneScript()
    {
        textureList = new List<ResourceRequest>();
    }

    /// <summary>
    /// �폜
    /// </summary>
    public void OnDestroy()
    {
        foreach (var handle in textureList)
        {
            //if (handle.IsValid())
            if (handle != null)
            {
                Resources.UnloadAsset(handle.asset);
                //Addressables.Release(handle);
            }
        }

        textureList.Clear();
    }

    /// <summary>
    /// ������
    /// </summary>
    public void Start()
    {
        textureList.Add(Resources.LoadAsync<Sprite>("CharacterListScene/drows"));
        textureList.Add(Resources.LoadAsync<Sprite>("CharacterListScene/eraps"));
        textureList.Add(Resources.LoadAsync<Sprite>("CharacterListScene/exa"));
        textureList.Add(Resources.LoadAsync<Sprite>("CharacterListScene/worra"));
        textureList.Add(Resources.LoadAsync<Sprite>("CharacterListScene/koob"));
        textureList.Add(Resources.LoadAsync<Sprite>("CharacterListScene/you"));
        //textureList.Add(Addressables.LoadAssetAsync<Sprite>("charaui_drows_0"));
        //textureList.Add(Addressables.LoadAssetAsync<Sprite>("charaui_eraps_0"));
        //textureList.Add(Addressables.LoadAssetAsync<Sprite>("charaui_exa_0"));
        //textureList.Add(Addressables.LoadAssetAsync<Sprite>("charaui_worra_0"));
        //textureList.Add(Addressables.LoadAssetAsync<Sprite>("charaui_koob_0"));
        //textureList.Add(Addressables.LoadAssetAsync<Sprite>("charaui_you_0"));

        displayCharacter = (int)Constant.PlayerID.Drows;
        changeCoroutine = null;
        UpdateCharacterInfo(false);
    }

    /// <summary>
    /// �E�{�^��
    /// </summary>
    public void ChangeRight()
    {
        if (changeCoroutine != null)
        {
            return;
        }

        SoundManager.GetInstance().PlaySE(SoundManager.SeID.SYSTEM_MOVE);
        changeCoroutine = StartCoroutine(ChangeRightCoroutine());
    }

    public IEnumerator ChangeRightCoroutine()
    {
        imgCharaPic.GetComponent<Animator>().SetTrigger("leftDisapp");
        yield return new WaitForSeconds(0.25f);

        displayCharacter++;
        if (displayCharacter > (int)Constant.PlayerID.You)
        {
            displayCharacter = (int)Constant.PlayerID.Drows;
        }
        yield return textureList[displayCharacter];

        UpdateCharacterInfo();

        changeCoroutine = null;

        imgCharaPic.GetComponent<Animator>().SetTrigger("rightApp");
    }

    /// <summary>
    /// ���{�^��
    /// </summary>
    public void ChangeLeft()
    {
        if (changeCoroutine != null)
        {
            return;
        }

        SoundManager.GetInstance().PlaySE(SoundManager.SeID.SYSTEM_MOVE);
        changeCoroutine = StartCoroutine(ChangeLeftCoroutine());
    }

    public IEnumerator ChangeLeftCoroutine()
    {
        imgCharaPic.GetComponent<Animator>().SetTrigger("rightDisapp");
        yield return new WaitForSeconds(0.25f);

        displayCharacter--;
        if (displayCharacter < (int)Constant.PlayerID.Drows)
        {
            displayCharacter = (int)Constant.PlayerID.You;
        }
        yield return textureList[displayCharacter];

        UpdateCharacterInfo();

        changeCoroutine = null;

        imgCharaPic.GetComponent<Animator>().SetTrigger("leftApp");
    }

    /// <summary>
    /// displayCharacter�̏���\��
    /// </summary>
    /// <param name="picture">�摜�ύX</param>
    private void UpdateCharacterInfo(bool picture = true)
    {
        DisplayStatus disp;
        switch ((Constant.PlayerID)displayCharacter)
        {
            case Constant.PlayerID.Drows: disp = STAT_DROWS; break;
            case Constant.PlayerID.Eraps: disp = STAT_ERAPS; break;
            case Constant.PlayerID.Exa: disp = STAT_EXA; break;
            case Constant.PlayerID.Worra: disp = STAT_WORRA; break;
            case Constant.PlayerID.Koob: disp = STAT_KOOB; break;
            default: disp = STAT_YOU; break;
        }

        if (picture)
        {
            //imgCharaPic.sprite = textureList[displayCharacter].Result;
            imgCharaPic.sprite = textureList[displayCharacter].asset as Sprite;
        }

        txtPersonalityName.SetText(disp.personalityName);
        txtPersonalityExp.SetText(disp.personalityExplain);
        txtSkillName.SetText(disp.skillName);
        txtSkillExp.SetText(disp.skillExplain);
        txtSkillCharge.SetText(disp.skillCharge.ToString());
        txtSkillStock.SetText(disp.skillStock > 0 ? disp.skillStock.ToString() : "����");
        txtInitHp.SetText(disp.initHP.ToString());
        txtInitRange.SetText(disp.initRange.ToString());
        txtNameE.SetText(disp.nameE);
        txtNameJ.SetText(disp.nameJ);
    }

    /// <summary>
    /// �߂�
    /// </summary>
    public void BackButton()
    {
        SoundManager.GetInstance().PlaySE(SoundManager.SeID.SYSTEM_CANCEL);
        ManagerScript.GetInstance().ChangeScene("TitleScene");
    }
}
