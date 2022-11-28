using UnityEngine;
using UnityEngine.UI;

namespace DA_Assets.FCU.UI
{
    public class UIManagerButton : MonoBehaviour
    {
        [SerializeField] Text txtButton;
        [SerializeField] IdNameInstanceId model;
        public void SetModel(IdNameInstanceId _model)
        {
            model = _model;
            txtButton.text = model.Name;
        }
        public void OnClick()
        {
            StartCoroutine(UIManager.Instance.Show(model));
        }
    }
}
