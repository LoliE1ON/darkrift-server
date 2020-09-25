SET NAMES utf8mb4;

CREATE TABLE IF NOT EXISTS  `users` (
    `id` int(255) NOT NULL AUTO_INCREMENT,
    `login` varchar(50) NOT NULL,
    `password` varchar(200) NOT NULL,
    PRIMARY KEY (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

ALTER TABLE `users` ADD COLUMN `registration_date` int(255) AFTER `password`;