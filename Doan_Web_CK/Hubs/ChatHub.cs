using Doan_Web_CK.Models;
using Doan_Web_CK.Repository;
using Microsoft.AspNetCore.SignalR;
namespace Doan_Web_CK.Hubs
{
    public class ChatHub : Hub
    {
        private readonly IChatRoomRepository _chatRoomRepository;
        private readonly IMessageRepository _messageRepository;
        private readonly IAccountRepository _accountRepository;
        public ChatHub(IChatRoomRepository chatRoomRepository, IMessageRepository messageRepository, IAccountRepository accountRepository)
        {
            _chatRoomRepository = chatRoomRepository;
            _messageRepository = messageRepository;
            _accountRepository = accountRepository;
        }
        public async Task SendMessage(string user, string message)
        {
            await Clients.All.SendAsync("ReceiveMessage", user, message);
        }

        public async Task SendToUser(string user, string receiverConnectionId, string message)
        {
            var sender = await _accountRepository.GetByIdAsync(user);
            var receiver = await _accountRepository.GetByIdAsync(receiverConnectionId);
            var leftOrRight = "right";

            var chatroom = await _chatRoomRepository.GetByUsersIdAsync(sender.Id, receiver.Id);
            if (chatroom != null)
            {
                var newMessage = new Message
                {
                    UserName = sender.UserName,
                    UserImageUrl = sender.ImageUrl,
                    Text = message,
                    Time = DateTime.Now,
                    userId = sender.Id,
                    ApplicationUser = sender,
                    ChatRoomId = chatroom.Id,
                };
                await _chatRoomRepository.AddMessagesAsync(chatroom, newMessage);
                if (sender.Id != newMessage.userId)
                {
                    leftOrRight = "left";
                }
            }
            //await Clients.Users([user, receiverConnectionId]).SendAsync("ReceiveMessage", user, message, sender.ImageUrl, leftOrRight);
            await Clients.User(user).SendAsync("ReceiveMessage", user, message, sender.ImageUrl, "right");
            await Clients.User(receiverConnectionId).SendAsync("ReceiveMessage", receiverConnectionId, message, sender.ImageUrl, "left");
        }

        public string GetConnectionId() => Context.UserIdentifier;
    }
}
