using UnityEngine;
using UnityEngine.UI;

namespace MultiplayerARPG
{
    [RequireComponent(typeof(UISocialCharacter))]
    public class UISocialCharacterView : MonoBehaviour
    {
        public UISocialCharacterViewManager manager;
        public int index = -1;
        public string id = string.Empty;
        public RawImage view;
        public GameObject[] noCharacterPlaceholders = new GameObject[0];
        private UISocialCharacter _uiSocialCharacter;
        private RenderTexture _renderTexture;

        private void Start()
        {
            _uiSocialCharacter = GetComponent<UISocialCharacter>();
            SetShowNoCharacterPlaceHolders(true);
        }

        private void OnEnable()
        {
            Clear();
        }

        private void OnDisable()
        {
            Clear();
        }

        private void Update()
        {
            if (index != transform.GetSiblingIndex() || !id.Equals(_uiSocialCharacter.Data.id))
            {
                index = transform.GetSiblingIndex();
                id = _uiSocialCharacter.Data.id;
                SetShowNoCharacterPlaceHolders(true);
                view.enabled = false;
                manager.LoadCharacter(index, id, out RenderTexture renderTexture, (requestHandler, responseCode, response) =>
                {
                    SetShowNoCharacterPlaceHolders(responseCode != LiteNetLibManager.AckResponseCode.Success);
                    view.enabled = responseCode == LiteNetLibManager.AckResponseCode.Success;
                    view.texture = _renderTexture;
                });
                _renderTexture = renderTexture;
            }
        }

        public void Clear()
        {
            if (index >= 0)
                manager.ClearContainer(index);
            index = -1;
            id = string.Empty;
        }

        public void SetShowNoCharacterPlaceHolders(bool isShow)
        {
            for (int i = 0; i < noCharacterPlaceholders.Length; ++i)
            {
                noCharacterPlaceholders[i].SetActive(isShow);
            }
        }
    }
}
