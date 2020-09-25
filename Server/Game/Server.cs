using System;
using System.Collections.Generic;
using System.Linq;
using MySql.Data.MySqlClient;
using Server.Net;

namespace Server {
    
    using DarkRift;
    using DarkRift.Server;
    
    public class Server: Plugin {
        
        public override bool ThreadSafe => false;
        public override Version Version => new Version(1, 0, 0);
        public Dictionary<IClient, Player> players = new Dictionary<IClient, Player>();
        public Mysql DB;
        
        public Server(PluginLoadData pluginLoadData) : base(pluginLoadData) {

            // Connection to DB
            DB = DataBase.getInstance().mysql;
            DB.Connect(pluginLoadData.DatabaseManager.GetConnectionString("db"));

            ClientManager.ClientConnected += ClientConnected;
        }
        

        void ClientConnected(object sender, ClientConnectedEventArgs e) {
            
            players.Add(e.Client, new Player(e.Client.ID));
            
            using (DarkRiftWriter newPlayerWriter = DarkRiftWriter.Create()) {
                newPlayerWriter.Write(e.Client.ID);
                using (Message playerMessage = Message.Create(Tags.ConnectedPlayer, newPlayerWriter))
                    e.Client.SendMessage(playerMessage, SendMode.Reliable);
            }
            
            e.Client.MessageReceived += MessageReceived;
            ClientManager.ClientDisconnected += ClientDisconnected;

        }
        
        void ClientDisconnected(object sender, ClientDisconnectedEventArgs e) {
            
            players.Remove(e.Client);
            
            using (DarkRiftWriter writer = DarkRiftWriter.Create()) {
                writer.Write(e.Client.ID);
                using (Message message = Message.Create(Tags.DisconnectedPlayer, writer)) {
                    foreach (IClient client in ClientManager.GetAllClients())
                        client.SendMessage(message, SendMode.Reliable);
                }
            }
        }
        
        void MessageReceived(object sender, MessageReceivedEventArgs e) {
            using (Message message = e.GetMessage() as Message) {

                if (message.Tag == Tags.Authentication) Authentication(e);
                if (message.Tag == Tags.PlayerDetails) PlayerDetails(e);
                
            }
        }
        
        void Authentication(MessageReceivedEventArgs e) {
            using (Message message = e.GetMessage() as Message) {
                using (DarkRiftReader reader = message.GetReader()) {
                    
                    var login = reader.ReadString();
                    var password = reader.ReadString();
                    var avatarType = reader.ReadUInt16();
                    
                    var authResult = DB.Count("SELECT COUNT(*) FROM users WHERE login = @login AND password = @password", new Dictionary<string, string> {
                        { "@login", login },
                        { "@password", password },
                    });

                    if (authResult > 0) {
                        
                        // Сохраняем данные для нового игрока
                        players[e.Client].Name = login;
                        players[e.Client].Session = Guid.NewGuid().ToString("n").Substring(0, 20);
                        players[e.Client].Avatar = avatarType == 0
                            ? AvatarType.Neru
                            : (avatarType == 1 ? AvatarType.Kira : AvatarType.Miku);


                        // Уведомляем всех пользователей о новом игроке
                        using (DarkRiftWriter newPlayerWriter = DarkRiftWriter.Create()) {
                            newPlayerWriter.Write("");
                            newPlayerWriter.Write(players[e.Client].ID);
                            newPlayerWriter.Write(players[e.Client].Name);
                            newPlayerWriter.Write(players[e.Client].Avatar);

                            using (Message newPlayerMessage = Message.Create(Tags.SpawnNewPlayer, newPlayerWriter)) {
                                foreach (IClient client in ClientManager.GetAllClients().Where(x => x != e.Client))
                                    client.SendMessage(newPlayerMessage, SendMode.Reliable);
                            }
                        }

                        // Уведомляем нового игрока о удачной авторизации
                        using (DarkRiftWriter playerWriter = DarkRiftWriter.Create()) {
                            playerWriter.Write(players[e.Client].Session);
                            foreach (Player player in players.Values) {
                                playerWriter.Write(player.ID);
                                playerWriter.Write(player.Name);
                                playerWriter.Write(player.Avatar);
                            }

                            using (Message playerMessage = Message.Create(Tags.SpawnCurrentPlayer, playerWriter))
                                e.Client.SendMessage(playerMessage, SendMode.Reliable);
                        }

                    }
                    else {
                        
                        // Уведомляем нового игрока о НЕ удачной авторизации
                        using (DarkRiftWriter playerWriter = DarkRiftWriter.Create()) {
                            playerWriter.Write(players[e.Client].ID);
                            using (Message playerMessage = Message.Create(Tags.AuthorizationFail, playerWriter))
                                e.Client.SendMessage(playerMessage, SendMode.Reliable);
                        }
                    }



                }
            }
        }
        
        void PlayerDetails(MessageReceivedEventArgs e) {
            using (Message message = e.GetMessage() as Message) {
                using (DarkRiftReader reader = message.GetReader()) {
                    Player player = players[e.Client];

                    player.PositionX = reader.ReadSingle();
                    player.PositionY = reader.ReadSingle();
                    player.PositionZ = reader.ReadSingle();

                    player.AnimatorIsWalking = reader.ReadBoolean();
                    player.AnimatorIsRunning = reader.ReadBoolean();
                    player.AnimatorIsBackWalking = reader.ReadBoolean();
                    player.AnimatorIsRightWalking = reader.ReadBoolean();
                    player.AnimatorIsLeftWalking = reader.ReadBoolean();
                    player.AnimatorIsDirectRightWalking = reader.ReadBoolean();
                    player.AnimatorIsDirectLeftWalking = reader.ReadBoolean();
                    player.AnimatorIsJumpRun = reader.ReadBoolean();
                    
                    player.RotationX = reader.ReadSingle();
                    player.RotationY = reader.ReadSingle();
                    player.RotationZ = reader.ReadSingle();
                    player.RotationW = reader.ReadSingle(); 
                    
                    using (DarkRiftWriter writer = DarkRiftWriter.Create()) {
                        writer.Write(player.ID);
                        writer.Write(player.PositionX);
                        writer.Write(player.PositionY);
                        writer.Write(player.PositionZ);
                        writer.Write(player.AnimatorIsWalking);
                        writer.Write(player.AnimatorIsRunning);
                        writer.Write(player.AnimatorIsBackWalking);
                        writer.Write(player.AnimatorIsRightWalking);
                        writer.Write(player.AnimatorIsLeftWalking);
                        writer.Write(player.AnimatorIsDirectRightWalking);
                        writer.Write(player.AnimatorIsDirectLeftWalking);
                        writer.Write(player.AnimatorIsJumpRun);
                        writer.Write(player.RotationX);
                        writer.Write(player.RotationY);
                        writer.Write(player.RotationZ);
                        writer.Write(player.RotationW);
                        message.Serialize(writer);
                    }

                    foreach (IClient c in ClientManager.GetAllClients().Where(x => x != e.Client))
                        c.SendMessage(message, e.SendMode);
                }
            }
        }
        
    }
}