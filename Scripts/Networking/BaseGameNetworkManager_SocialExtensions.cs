using Cysharp.Threading.Tasks;
using LiteNetLib;
using LiteNetLibManager;
using UnityEngine;

namespace MultiplayerARPG
{
    public partial class BaseGameNetworkManager
    {
        [Header("Social extensions")]
        public ushort getOnlineCharacterDataRequestType = 1500;

        [DevExtMethods("RegisterMessages")]
        public void RegisterClientMessages_SocialExtensions()
        {
            RegisterRequestToServer<RequestGetOnlineCharacterDataMessage, ResponseGetOnlineCharacterDataMessage>(getOnlineCharacterDataRequestType, HandleRequestGetOnlineCharacterData);
        }

        public UniTaskVoid HandleRequestGetOnlineCharacterData(RequestHandlerData requestHandler, RequestGetOnlineCharacterDataMessage request, RequestProceedResultDelegate<ResponseGetOnlineCharacterDataMessage> result)
        {
            if (!GameInstance.ServerUserHandlers.TryGetPlayerCharacterById(request.characterId, out IPlayerCharacterData playerCharacter))
            {
                result.InvokeError(new ResponseGetOnlineCharacterDataMessage()
                {
                    message = UITextKeys.UI_ERROR_CHARACTER_NOT_FOUND,
                });
                return default;
            }
            PlayerCharacterData resultPlayerCharacter = playerCharacter.CloneTo(new PlayerCharacterData(), true, false, false, false, false, true, false, false, false, false, false, false, false, false);
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
    }
}
