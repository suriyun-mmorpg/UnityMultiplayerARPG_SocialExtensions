using Cysharp.Threading.Tasks;
using Insthync.DevExtension;
using LiteNetLibManager;
using System.Collections.Generic;
using UnityEngine;

namespace MultiplayerARPG
{
    public partial class BaseGameNetworkManager
    {
        [Header("Social extensions")]
        [SerializeField]
        protected ushort getOnlineCharacterDataRequestType = 1500;
        [SerializeField]
        protected ushort getOnlineCharactersRequestType = 1501;

        [DevExtMethods("RegisterMessages")]
        public virtual void RegisterClientMessages_SocialExtensions()
        {
            RegisterRequestToServer<RequestGetOnlineCharacterDataMessage, ResponseGetOnlineCharacterDataMessage>(getOnlineCharacterDataRequestType, HandleRequestGetOnlineCharacterData);
            RegisterRequestToServer<EmptyMessage, ResponseSocialCharacterListMessage>(getOnlineCharactersRequestType, HandleRequestGetOnlineCharacters);
        }

        public virtual UniTaskVoid HandleRequestGetOnlineCharacterData(RequestHandlerData requestHandler, RequestGetOnlineCharacterDataMessage request, RequestProceedResultDelegate<ResponseGetOnlineCharacterDataMessage> result)
        {
            if (!GameInstance.ServerUserHandlers.TryGetPlayerCharacterById(request.characterId, out IPlayerCharacterData playerCharacter))
            {
                result.InvokeError(new ResponseGetOnlineCharacterDataMessage()
                {
                    message = UITextKeys.UI_ERROR_CHARACTER_NOT_FOUND,
                });
                return default;
            }
            PlayerCharacterData resultPlayerCharacter = playerCharacter.CloneTo(new PlayerCharacterData(), true, false, false, false, false, true, false, false, false, false, false, false, false, false, false);
            result.InvokeSuccess(new ResponseGetOnlineCharacterDataMessage()
            {
                character = resultPlayerCharacter,
            });
            return default;
        }

        public bool RequestGetOnlineCharacterData(RequestGetOnlineCharacterDataMessage data, ResponseDelegate<ResponseGetOnlineCharacterDataMessage> callback)
        {
            return ClientSendRequest(getOnlineCharacterDataRequestType, data, responseDelegate: callback);
        }

        public virtual UniTaskVoid HandleRequestGetOnlineCharacters(RequestHandlerData requestHandler, EmptyMessage request, RequestProceedResultDelegate<ResponseSocialCharacterListMessage> result)
        {
            List<SocialCharacterData> onlineCharacters = new List<SocialCharacterData>();
            foreach (IPlayerCharacterData playerCharacterData in GameInstance.ServerUserHandlers.GetPlayerCharacters())
            {
                onlineCharacters.Add(SocialCharacterData.Create(playerCharacterData));
            }
            result.InvokeSuccess(new ResponseSocialCharacterListMessage()
            {
                characters = onlineCharacters,
            });
            return default;
        }

        public bool RequestGetOnlineCharacters(ResponseDelegate<ResponseSocialCharacterListMessage> callback)
        {
            return ClientSendRequest(getOnlineCharactersRequestType, EmptyMessage.Value, responseDelegate: callback);
        }
    }
}
