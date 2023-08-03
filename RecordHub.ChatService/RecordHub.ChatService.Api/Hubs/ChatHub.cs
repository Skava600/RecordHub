using Microsoft.AspNetCore.SignalR;
using RecordHub.ChatService.Application.Services;
using RecordHub.ChatService.Domain.Models;

namespace RecordHub.ChatService.Api.Hubs
{
    public class ChatHub : Hub
    {
        private readonly string _botUser;
        private readonly IDictionary<string, UserConnection> _connections;
        private readonly IRoomsService _roomsService;

        public ChatHub(IDictionary<string, UserConnection> connections, IRoomsService roomsService)
        {
            _botUser = "MyChat Bot";
            _connections = connections;
            _roomsService = roomsService;
        }

        public override Task OnDisconnectedAsync(Exception exception)
        {
            if (_connections.TryGetValue(Context.ConnectionId, out UserConnection userConnection))
            {
                _connections.Remove(Context.ConnectionId);
                Clients.Group(userConnection.Room).SendAsync("ReceiveMessage", _botUser, $"{userConnection.User} has left", DateTime.Now);
                SendUsersConnected(userConnection.Room);
            }

            return base.OnDisconnectedAsync(exception);
        }

        public async Task JoinRoom(UserConnection userConnection)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, userConnection.Room);

            _connections[Context.ConnectionId] = userConnection;

            string message = $"{userConnection.User} has joined {userConnection.Room}";


            var room = await _roomsService.GetAsync(userConnection.Room);
            if (room == null)
            {
                room = await _roomsService.CreateAsync(new Room
                {
                    Messages = new List<Message>(),
                    RoomId = userConnection.Room,
                });
            }
            foreach (var chatMessage in room.Messages)
            {
                await Clients.Caller.SendAsync("ReceiveMessage", chatMessage.Name, chatMessage.Text, chatMessage.DateSent);
            }

            await Clients.Group(userConnection.Room).SendAsync("ReceiveMessage", _botUser, message, DateTime.Now);
            await SendUsersConnected(userConnection.Room);
        }

        public async Task SendMessage(string message)
        {
            if (_connections.TryGetValue(Context.ConnectionId, out UserConnection userConnection))
            {
                await Clients.Group(userConnection.Room).SendAsync("ReceiveMessage", userConnection.User, message, DateTime.Now);

                await _roomsService.AddMessagesAsync(
                    userConnection.Room,
                    new List<Message>
                    {
                        new Message(message, userConnection.User)
                    });
            }
        }

        public Task SendUsersConnected(string room)
        {
            var users = _connections.Values
                .Where(c => c.Room == room)
                .Select(c => c.User);

            return Clients.Group(room).SendAsync("UsersInRoom", users);
        }
    }
}
