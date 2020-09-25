ssh e1on@5.180.138.37 "sudo systemctl stop loli-chat"
ssh e1on@5.180.138.37 "sudo rm -rf /usr/sbin/game/Plugins/*"
ssh e1on@5.180.138.37 "sudo rm -rf /usr/sbin/game/Lib/*"

scp ./Server/bin/Debug/* e1on@5.180.138.37:/usr/sbin/game/Lib
scp ./Server/bin/Debug/Server.dll e1on@5.180.138.37:/usr/sbin/game/Plugins

scp ./migration.sql e1on@5.180.138.37:/usr/sbin/game
ssh e1on@5.180.138.37 "mariadb -f -u user --password=game123 game < /usr/sbin/game/migration.sql"

ssh e1on@5.180.138.37 "sudo systemctl start loli-chat"