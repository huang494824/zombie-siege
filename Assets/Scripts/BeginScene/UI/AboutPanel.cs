using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AboutPanel : BasePanel
{
    public TextMeshProUGUI txtVersion;
    public Button btnClose;
    public override void Init()
    {
        //从 Player Settings 中读取版本号
        txtVersion.text = "Demo " + Application.version;

        btnClose.onClick.AddListener(() =>
        {
            UIManager.Instance.HidePanel<AboutPanel>();
            UIManager.Instance.ShowPanel<BeginPanel>();
        });
    }
}
