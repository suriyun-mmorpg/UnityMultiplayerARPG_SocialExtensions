using LiteNetLibManager;
using System.Collections.Generic;

namespace MultiplayerARPG
{
    public class UIOnlineCharacters : UISocialGroup<UISocialCharacter>
    {
        protected override void OnEnable()
        {
            base.OnEnable();
            Refresh();
        }

        private void UpdateCharactersUIs(List<SocialCharacterData> foundCharacters)
        {
            if (foundCharacters == null)
                return;

            memberAmount = foundCharacters.Count;
            UpdateUIs();

            string selectedId = MemberSelectionManager.SelectedUI != null ? MemberSelectionManager.SelectedUI.Data.id : string.Empty;
            MemberSelectionManager.DeselectSelectedUI();
            MemberSelectionManager.Clear();

            UISocialCharacter tempUI;
            MemberList.Generate(foundCharacters, (index, data, ui) =>
            {
                tempUI = ui.GetComponent<UISocialCharacter>();
                tempUI.uiSocialGroup = this;
                tempUI.Data = data;
                tempUI.Show();
                MemberSelectionManager.Add(tempUI);
                if (selectedId.Equals(data.id))
                    tempUI.SelectByManager();
            });
            if (memberListEmptyObject != null)
                memberListEmptyObject.SetActive(foundCharacters.Count == 0);
        }

        public override bool CanKick(string characterId)
        {
            return false;
        }

        public override int GetMaxMemberAmount()
        {
            return 0;
        }

        public override int GetSocialId()
        {
            return 1;
        }

        public override bool IsLeader(string characterId)
        {
            return false;
        }

        public override bool OwningCharacterCanKick()
        {
            return false;
        }

        public override bool OwningCharacterIsLeader()
        {
            return false;
        }

        public void Refresh()
        {
            BaseGameNetworkManager.Singleton.RequestGetOnlineCharacters(GetOnlineCharactersCallback);
        }

        private void GetOnlineCharactersCallback(ResponseHandlerData responseHandler, AckResponseCode responseCode, ResponseSocialCharacterListMessage response)
        {
            if (responseCode.ShowUnhandledResponseMessageDialog(response.message)) return;
            UpdateCharactersUIs(response.characters);
        }
    }
}
