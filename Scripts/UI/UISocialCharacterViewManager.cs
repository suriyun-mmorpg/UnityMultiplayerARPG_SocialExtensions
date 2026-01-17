using LiteNetLibManager;
using System.Collections.Generic;
using UnityEngine;

namespace MultiplayerARPG
{
    public class UISocialCharacterViewManager : MonoBehaviour
    {
        [System.Serializable]
        public struct Entry
        {
            public Transform container;
            public RenderTexture renderTexture;
        }
        public Entry[] entries = new Entry[0];
        private List<BaseCharacterModel> _characterModels = new List<BaseCharacterModel>();

        private void Update()
        {
            for (int i = 0; i < _characterModels.Count; ++i)
            {
                _characterModels[i].UpdateAnimation(Time.deltaTime);
            }
        }

        public void LoadCharacter(int index, string characterId, out RenderTexture renderTexture, ResponseDelegate<ResponseGetOnlineCharacterDataMessage> callback)
        {
            renderTexture = null;
            if (index < 0 || index >= entries.Length)
                return;
            renderTexture = entries[index].renderTexture;
            _characterModels.Clear();
            ClearContainer(index);
            BaseGameNetworkManager.Singleton.RequestGetOnlineCharacterData(new RequestGetOnlineCharacterDataMessage()
            {
                characterId = characterId,
            }, (requestHandler, responseCode, response) =>
            {
                if (callback != null)
                    callback.Invoke(requestHandler, responseCode, response);
                if (responseCode != AckResponseCode.Success)
                    return;
                BaseCharacterModel characterModel = response.character.InstantiateModel(entries[index].container);
                if (characterModel != null)
                {
                    characterModel.SetupModelBodyParts(response.character);
                    characterModel.SetEquipItemsImmediately(response.character.EquipItems, response.character.SelectableWeaponSets, response.character.EquipWeaponSet, true);
                    characterModel.gameObject.SetActive(true);
                    _characterModels.Add(characterModel);
                }
            });
        }

        public void ClearContainer(int index)
        {
            if (index < 0 || index >= entries.Length)
                return;
            entries[index].container.DestroyChildren();
        }
    }
}
