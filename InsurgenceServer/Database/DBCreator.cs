using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;

namespace InsurgenceServer.Database
{
    public static class DBCreator
    {
        public static void CreateTables()
        {
            var conn = new OpenConnection();
            if (!conn.IsConnected())
            {
                conn.Close();
                return;
            }
            new MySqlCommand(
                "CREATE TABLE IF NOT EXISTS `countermetrics` (`id` int(11) NOT NULL,`name` varchar(255) DEFAULT NULL,`value` int(11) DEFAULT NULL, PRIMARY KEY(`id`))",
                conn.Connection).ExecuteNonQuery();
            new MySqlCommand(
                "CREATE TABLE IF NOT EXISTS `friend_list` ( `user_id` int(11) NOT NULL, `friends` varchar(255) DEFAULT NULL, PRIMARY KEY (`user_id`))",
                conn.Connection).ExecuteNonQuery();
            new MySqlCommand(
                "CREATE TABLE IF NOT EXISTS `GTS` (  `id` int(11) NOT NULL AUTO_INCREMENT,  `Offer` json DEFAULT NULL,  `Request` json DEFAULT NULL,  `Accepted` tinyint(1) NOT NULL DEFAULT '0'," + 
                "  `Result` json DEFAULT NULL,  `user_id` int(11) DEFAULT NULL,  `username` varchar(255) DEFAULT NULL,  `OfferLevel` int(11) NOT NULL DEFAULT '0',  PRIMARY KEY (`id`))",
                conn.Connection).ExecuteNonQuery();
            new MySqlCommand(
                "CREATE TABLE IF NOT EXISTS `users` (  `user_id` int(10) unsigned NOT NULL AUTO_INCREMENT,  `username` varchar(32) COLLATE utf8_unicode_ci NOT NULL," + 
                "  `password` varchar(11) COLLATE utf8_unicode_ci NOT NULL,  `email` varchar(50) COLLATE utf8_unicode_ci NOT NULL,  `usergroup` int(10) NOT NULL DEFAULT '0'," + 
                "  `banned` tinyint(1) NOT NULL DEFAULT '0',  `uniquecode` char(8) COLLATE utf8_unicode_ci NOT NULL DEFAULT '0',  `base` varchar(10000) COLLATE utf8_unicode_ci DEFAULT NULL," + 
                "  `sprite` varchar(15) COLLATE utf8_unicode_ci NOT NULL,  `admin` tinyint(1) DEFAULT '0',  PRIMARY KEY (`user_id`),  UNIQUE KEY `username` (`username`))",
                conn.Connection).ExecuteNonQuery();
            new MySqlCommand(
                "CREATE TABLE IF NOT EXISTS `ips` (  `user_id` int(10) unsigned NOT NULL,  `ip` varchar(15) COLLATE utf8_unicode_ci NOT NULL,  `ipban` tinyint(1) DEFAULT NULL," + 
                "  PRIMARY KEY (`user_id`,`ip`),  CONSTRAINT `ips_ibfk_1` FOREIGN KEY (`user_id`) REFERENCES `users` (`user_id`) ON DELETE CASCADE)",
                conn.Connection).ExecuteNonQuery();
            new MySqlCommand(
                "CREATE TABLE IF NOT EXISTS `user_data` (  `user_id` int(10) unsigned NOT NULL,  `lastlogin` datetime NOT NULL,  `guild_id` int(10) unsigned DEFAULT NULL," + 
                "  PRIMARY KEY (`user_id`),  CONSTRAINT `user_data_ibfk_1` FOREIGN KEY (`user_id`) REFERENCES `users` (`user_id`) ON DELETE CASCADE)",
                conn.Connection).ExecuteNonQuery();
            new MySqlCommand(
                "CREATE TABLE IF NOT EXISTS `tradelog` (  `user1` char(255) DEFAULT NULL,  `user2` char(255) DEFAULT NULL,  `pokemon1` json DEFAULT NULL,  `pokemon2` json DEFAULT NULL," + 
                "  `time` datetime DEFAULT NULL,  `i` smallint(10) NOT NULL AUTO_INCREMENT,  PRIMARY KEY (`i`))",
                conn.Connection).ExecuteNonQuery();
            new MySqlCommand(
                "CREATE TABLE IF NOT EXISTS `wondertradelog`( `id` int(10) unsigned NOT NULL AUTO_INCREMENT,`username` varchar(255) DEFAULT NULL, `pokemon` varchar(255) DEFAULT NULL," + 
                " `time` datetime DEFAULT NULL, PRIMARY KEY (`id`))", 
                conn.Connection).ExecuteNonQuery();
        }
    }
}
