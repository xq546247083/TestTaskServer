/*
SQLyog Ultimate v12.09 (64 bit)
MySQL - 5.7.12-log : Database - testtaskdatabase
*********************************************************************
*/

/*!40101 SET NAMES utf8 */;

/*!40101 SET SQL_MODE=''*/;

/*!40014 SET @OLD_UNIQUE_CHECKS=@@UNIQUE_CHECKS, UNIQUE_CHECKS=0 */;
/*!40014 SET @OLD_FOREIGN_KEY_CHECKS=@@FOREIGN_KEY_CHECKS, FOREIGN_KEY_CHECKS=0 */;
/*!40101 SET @OLD_SQL_MODE=@@SQL_MODE, SQL_MODE='NO_AUTO_VALUE_ON_ZERO' */;
/*!40111 SET @OLD_SQL_NOTES=@@SQL_NOTES, SQL_NOTES=0 */;
CREATE DATABASE /*!32312 IF NOT EXISTS*/`testtaskdatabase` /*!40100 DEFAULT CHARACTER SET utf8 */;

USE `testtaskdatabase`;

/*Table structure for table `lotterydraw` */

DROP TABLE IF EXISTS `lotterydraw`;

CREATE TABLE `lotterydraw` (
  `SerialNumber` int(11) NOT NULL AUTO_INCREMENT,
  `UserName` varchar(16) DEFAULT NULL,
  `UserFlag` varchar(32) DEFAULT NULL,
  `Points` int(11) DEFAULT '0',
  `LastLotteryDrawTime` datetime DEFAULT NULL,
  PRIMARY KEY (`SerialNumber`)
) ENGINE=InnoDB AUTO_INCREMENT=17 DEFAULT CHARSET=utf8;

/*Data for the table `lotterydraw` */

insert  into `lotterydraw`(`SerialNumber`,`UserName`,`UserFlag`,`Points`,`LastLotteryDrawTime`) values (2,'user1','f2b61d0b0e5b4a7ca3a6761d9cd503b1',0,NULL),(3,'user2','f2b61d0b0e5b4a7ca3a6761d9cd503b2',0,NULL),(4,'user6','f2b61d0b0e5b4a7ca3a6761d9cd503b6',20,'2017-04-10 11:30:52'),(5,'user24','f2b61d0b0e5b4a7ca3a6761d9cd50324',10,'2017-04-10 13:47:19'),(6,'user20','f2b61d0b0e5b4a7ca3a6761d9cd50320',0,NULL),(7,'user8','f2b61d0b0e5b4a7ca3a6761d9cd503b8',0,NULL),(8,'user9','f2b61d0b0e5b4a7ca3a6761d9cd503b9',0,NULL),(9,'user23','f2b61d0b0e5b4a7ca3a6761d9cd50323',0,NULL),(10,'user12','f2b61d0b0e5b4a7ca3a6761d9cd50312',0,NULL),(11,'user13','f2b61d0b0e5b4a7ca3a6761d9cd50313',0,NULL),(12,'user7','f2b61d0b0e5b4a7ca3a6761d9cd503b7',0,NULL),(13,'user3','f2b61d0b0e5b4a7ca3a6761d9cd503b3',10,'2017-04-10 11:10:13'),(14,'user4','f2b61d0b0e5b4a7ca3a6761d9cd503b4',50,'2017-04-10 13:48:41'),(15,'user5','f2b61d0b0e5b4a7ca3a6761d9cd503b5',60,'2017-04-10 13:46:58'),(16,'user16','f2b61d0b0e5b4a7ca3a6761d9cd50316',10,'2017-04-10 13:47:14');

/*Table structure for table `lotterydrawconfig` */

DROP TABLE IF EXISTS `lotterydrawconfig`;

CREATE TABLE `lotterydrawconfig` (
  `BeginTime` datetime DEFAULT NULL,
  `EndTime` datetime DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

/*Data for the table `lotterydrawconfig` */

insert  into `lotterydrawconfig`(`BeginTime`,`EndTime`) values ('2017-04-04 15:26:31','2017-04-30 15:26:35');

/*Table structure for table `userinfo` */

DROP TABLE IF EXISTS `userinfo`;

CREATE TABLE `userinfo` (
  `UserName` varchar(16) DEFAULT NULL,
  `UserFlag` varchar(32) NOT NULL,
  `DiamondNumber` int(11) DEFAULT '0',
  PRIMARY KEY (`UserFlag`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

/*Data for the table `userinfo` */

insert  into `userinfo`(`UserName`,`UserFlag`,`DiamondNumber`) values ('user10','f2b61d0b0e5b4a7ca3a6761d9cd50310',1000),('user11','f2b61d0b0e5b4a7ca3a6761d9cd50311',1000),('user12','f2b61d0b0e5b4a7ca3a6761d9cd50312',800),('user13','f2b61d0b0e5b4a7ca3a6761d9cd50313',900),('user14','f2b61d0b0e5b4a7ca3a6761d9cd50314',1000),('user15','f2b61d0b0e5b4a7ca3a6761d9cd50315',1000),('user16','f2b61d0b0e5b4a7ca3a6761d9cd50316',900),('user17','f2b61d0b0e5b4a7ca3a6761d9cd50317',1000),('user18','f2b61d0b0e5b4a7ca3a6761d9cd50318',1000),('user19','f2b61d0b0e5b4a7ca3a6761d9cd50319',1000),('user20','f2b61d0b0e5b4a7ca3a6761d9cd50320',700),('user21','f2b61d0b0e5b4a7ca3a6761d9cd50321',1000),('user22','f2b61d0b0e5b4a7ca3a6761d9cd50322',1000),('user23','f2b61d0b0e5b4a7ca3a6761d9cd50323',900),('user24','f2b61d0b0e5b4a7ca3a6761d9cd50324',200),('user25','f2b61d0b0e5b4a7ca3a6761d9cd50325',1000),('user1','f2b61d0b0e5b4a7ca3a6761d9cd503b1',0),('user2','f2b61d0b0e5b4a7ca3a6761d9cd503b2',0),('user3','f2b61d0b0e5b4a7ca3a6761d9cd503b3',0),('user4','f2b61d0b0e5b4a7ca3a6761d9cd503b4',300),('user5','f2b61d0b0e5b4a7ca3a6761d9cd503b5',400),('user6','f2b61d0b0e5b4a7ca3a6761d9cd503b6',100),('user7','f2b61d0b0e5b4a7ca3a6761d9cd503b7',700),('user8','f2b61d0b0e5b4a7ca3a6761d9cd503b8',800),('user9','f2b61d0b0e5b4a7ca3a6761d9cd503b9',900);

/*!40101 SET SQL_MODE=@OLD_SQL_MODE */;
/*!40014 SET FOREIGN_KEY_CHECKS=@OLD_FOREIGN_KEY_CHECKS */;
/*!40014 SET UNIQUE_CHECKS=@OLD_UNIQUE_CHECKS */;
/*!40111 SET SQL_NOTES=@OLD_SQL_NOTES */;
