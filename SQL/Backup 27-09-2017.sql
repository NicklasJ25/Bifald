-- MySqlBackup.NET 2.0.9.2
-- Dump Time: 2017-09-27 18:07:28
-- --------------------------------------
-- Server version 5.7.19-log MySQL Community Server (GPL)


/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET @OLD_CHARACTER_SET_RESULTS=@@CHARACTER_SET_RESULTS */;
/*!40101 SET @OLD_COLLATION_CONNECTION=@@COLLATION_CONNECTION */;
/*!40101 SET NAMES utf8 */;
/*!40014 SET @OLD_UNIQUE_CHECKS=@@UNIQUE_CHECKS, UNIQUE_CHECKS=0 */;
/*!40014 SET @OLD_FOREIGN_KEY_CHECKS=@@FOREIGN_KEY_CHECKS, FOREIGN_KEY_CHECKS=0 */;
/*!40101 SET @OLD_SQL_MODE=@@SQL_MODE, SQL_MODE='NO_AUTO_VALUE_ON_ZERO' */;
/*!40111 SET @OLD_SQL_NOTES=@@SQL_NOTES, SQL_NOTES=0 */;


-- 
-- Definition of Afsluttede_pladser
-- 

DROP TABLE IF EXISTS `Afsluttede_pladser`;
CREATE TABLE IF NOT EXISTS `Afsluttede_pladser` (
  `Id` int(11) NOT NULL AUTO_INCREMENT,
  `Sagsnummer` varchar(45) NOT NULL,
  `Pladsnummer` varchar(45) NOT NULL,
  PRIMARY KEY (`Id`),
  KEY `Sagsnummer_idx` (`Sagsnummer`),
  KEY `Pladsnummer_idx` (`Pladsnummer`),
  CONSTRAINT `Pladsnummer` FOREIGN KEY (`Pladsnummer`) REFERENCES `Pladser` (`Pladsnummer`) ON DELETE CASCADE ON UPDATE CASCADE,
  CONSTRAINT `Sagsnummer` FOREIGN KEY (`Sagsnummer`) REFERENCES `Sager` (`Sagsnummer`) ON DELETE CASCADE ON UPDATE CASCADE
) ENGINE=InnoDB AUTO_INCREMENT=39 DEFAULT CHARSET=utf8;

-- 
-- Dumping data for table Afsluttede_pladser
-- 

/*!40000 ALTER TABLE `Afsluttede_pladser` DISABLE KEYS */;
INSERT INTO `Afsluttede_pladser`(`Id`,`Sagsnummer`,`Pladsnummer`) VALUES
(9,'Sagen (1)','E2.01'),
(10,'Sagen (1)','E2.02'),
(11,'Sagen (1)','E2.03'),
(12,'Sagen (1)','E2.04'),
(13,'Sagen (1)','E2.05'),
(14,'Sagen (1)','E2.06'),
(15,'Sagen (1)','E2.07'),
(38,'6543','E2.08');
/*!40000 ALTER TABLE `Afsluttede_pladser` ENABLE KEYS */;

-- 
-- Definition of Kasser
-- 

DROP TABLE IF EXISTS `Kasser`;
CREATE TABLE IF NOT EXISTS `Kasser` (
  `Id` int(11) NOT NULL AUTO_INCREMENT,
  `Sagsnummer` varchar(45) NOT NULL,
  `Antal` int(11) NOT NULL,
  `Hentet_leveret` varchar(45) NOT NULL,
  `Hentet_leveret_dato` date NOT NULL,
  PRIMARY KEY (`Id`),
  KEY `Sag_idx` (`Sagsnummer`),
  CONSTRAINT `Sagskasser` FOREIGN KEY (`Sagsnummer`) REFERENCES `sager` (`Sagsnummer`) ON DELETE CASCADE ON UPDATE CASCADE
) ENGINE=InnoDB AUTO_INCREMENT=13 DEFAULT CHARSET=utf8;

-- 
-- Dumping data for table Kasser
-- 

/*!40000 ALTER TABLE `Kasser` DISABLE KEYS */;
INSERT INTO `Kasser`(`Id`,`Sagsnummer`,`Antal`,`Hentet_leveret`,`Hentet_leveret_dato`) VALUES
(1,'1',20,'Leveret','2016-07-30 00:00:00'),
(2,'64sefs',20,'Leveret','2016-07-30 00:00:00'),
(3,'222',50,'Leveret','2016-08-04 00:00:00'),
(4,'Sagen',10,'Leveret','2016-08-04 00:00:00'),
(5,'EnFalckMere',15,'Leveret','2016-08-04 00:00:00'),
(6,'25954',26,'Leveret','2016-08-04 00:00:00'),
(7,'222',20,'Hentet','2016-08-05 00:00:00'),
(8,'Sagen (1)',20,'Leveret','2017-09-19 00:00:00'),
(9,'Sagen (2)',20,'Leveret','2017-09-19 00:00:00'),
(10,'test',20,'Leveret','2017-09-20 00:00:00'),
(11,'Gennem test af sag',20,'Leveret','2017-09-21 00:00:00'),
(12,'Gennem test af sag (1)',20,'Leveret','2017-09-21 00:00:00');
/*!40000 ALTER TABLE `Kasser` ENABLE KEYS */;

-- 
-- Definition of Kunder
-- 

DROP TABLE IF EXISTS `Kunder`;
CREATE TABLE IF NOT EXISTS `Kunder` (
  `Id` int(11) NOT NULL AUTO_INCREMENT,
  `Navn` varchar(45) NOT NULL,
  `Adresse_fra` varchar(45) NOT NULL,
  `Adresse_til` varchar(45) DEFAULT NULL,
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB AUTO_INCREMENT=15 DEFAULT CHARSET=utf8;

-- 
-- Dumping data for table Kunder
-- 

/*!40000 ALTER TABLE `Kunder` DISABLE KEYS */;
INSERT INTO `Kunder`(`Id`,`Navn`,`Adresse_fra`,`Adresse_til`) VALUES
(1,'Nicklas Storm','En ny vej',''),
(4,'Falck','En vej','En anden vej'),
(5,'Bob Andersen','Villavejen',''),
(6,'Falck','Falckvej',''),
(7,'Falck','FalckVej','FalckSlutVej'),
(8,'Falck','FalckVejIgen',''),
(9,'Nicklas Storm','Min nye vej',''),
(10,'test','test','test'),
(11,'test','test','test'),
(12,'test','test 2','test 3'),
(13,'test ','test 1','test 2'),
(14,'test','test 1','test 2');
/*!40000 ALTER TABLE `Kunder` ENABLE KEYS */;

-- 
-- Definition of Pladser
-- 

DROP TABLE IF EXISTS `Pladser`;
CREATE TABLE IF NOT EXISTS `Pladser` (
  `Pladsnummer` varchar(45) NOT NULL,
  `Sagsnummer` varchar(45) DEFAULT NULL,
  `Type` varchar(5) NOT NULL,
  PRIMARY KEY (`Pladsnummer`),
  KEY `Sag_idx` (`Sagsnummer`),
  CONSTRAINT `Sagspladser` FOREIGN KEY (`Sagsnummer`) REFERENCES `sager` (`Sagsnummer`) ON DELETE CASCADE ON UPDATE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- 
-- Dumping data for table Pladser
-- 

/*!40000 ALTER TABLE `Pladser` DISABLE KEYS */;
INSERT INTO `Pladser`(`Pladsnummer`,`Sagsnummer`,`Type`) VALUES
('1.1','Sagen (2)','Lift'),
('1.2','Sagen (2)','Lift'),
('1.3','EnFalckMere','Lift'),
('1.4','Sagen (2)','Lift'),
('1.5','Sagen (2)','Lift'),
('1.6','Sagen (2)','Lift'),
('2.1','test','Lift'),
('2.2','test','Lift'),
('2.3','test','Lift'),
('2.4','test','Lift'),
('2.5','test','Lift'),
('2.6','Gennem test af sag','Lift'),
('3.1','Gennem test af sag (1)','Lift'),
('3.2',NULL,'Lift'),
('3.3',NULL,'Lift'),
('3.4',NULL,'Lift'),
('3.5',NULL,'Lift'),
('3.6',NULL,'Lift'),
('4.1',NULL,'Lift'),
('4.2',NULL,'Lift'),
('4.3',NULL,'Lift'),
('4.4',NULL,'Lift'),
('4.5',NULL,'Lift'),
('4.6',NULL,'Lift'),
('5.1',NULL,'Lift'),
('5.2',NULL,'Lift'),
('5.3',NULL,'Lift'),
('5.4',NULL,'Lift'),
('5.5',NULL,'Lift'),
('5.6',NULL,'Lift'),
('6.1',NULL,'Lift'),
('6.2',NULL,'Lift'),
('6.3',NULL,'Lift'),
('6.4',NULL,'Lift'),
('6.5',NULL,'Lift'),
('6.6',NULL,'Lift'),
('7.1',NULL,'Lift'),
('7.10',NULL,'Lift'),
('7.2',NULL,'Lift'),
('7.3',NULL,'Lift'),
('7.4',NULL,'Lift'),
('7.5',NULL,'Lift'),
('7.6',NULL,'Lift'),
('7.7',NULL,'Lift'),
('7.8',NULL,'Lift'),
('7.9',NULL,'Lift'),
('8.1',NULL,'Lift'),
('8.10',NULL,'Lift'),
('8.2',NULL,'Lift'),
('8.3',NULL,'Lift'),
('8.4',NULL,'Lift'),
('8.5',NULL,'Lift'),
('8.6',NULL,'Lift'),
('8.7',NULL,'Lift'),
('8.8',NULL,'Lift'),
('8.9',NULL,'Lift'),
('9.1',NULL,'Lift'),
('9.10',NULL,'Lift'),
('9.2',NULL,'Lift'),
('9.3',NULL,'Lift'),
('9.4',NULL,'Lift'),
('9.5',NULL,'Lift'),
('9.6',NULL,'Lift'),
('9.7',NULL,'Lift'),
('9.8',NULL,'Lift'),
('9.9',NULL,'Lift'),
('A1.01','Gennem test af sag','Plads'),
('A1.02','Gennem test af sag','Plads'),
('A1.03','64sefs','Plads'),
('A1.04','64sefs','Plads'),
('A1.05','64sefs','Plads'),
('A1.06','Gennem test af sag','Plads'),
('A1.07','Gennem test af sag','Plads'),
('A1.08','Gennem test af sag','Plads'),
('A1.09','Gennem test af sag','Plads'),
('A1.10','Gennem test af sag','Plads'),
('A1.11',NULL,'Plads'),
('A1.12',NULL,'Plads'),
('A1.13',NULL,'Plads'),
('A1.14',NULL,'Plads'),
('A1.15',NULL,'Plads'),
('A2.01',NULL,'Plads'),
('A2.02',NULL,'Plads'),
('A2.03',NULL,'Plads'),
('A2.04',NULL,'Plads'),
('A2.05',NULL,'Plads'),
('A2.06',NULL,'Plads'),
('A2.07',NULL,'Plads'),
('A2.08',NULL,'Plads'),
('A2.09',NULL,'Plads'),
('A2.10',NULL,'Plads'),
('A2.11',NULL,'Plads'),
('A2.12',NULL,'Plads'),
('A2.13',NULL,'Plads'),
('A2.14',NULL,'Plads'),
('A2.15',NULL,'Plads'),
('A3.01',NULL,'Plads'),
('A3.02',NULL,'Plads'),
('A3.03',NULL,'Plads'),
('A3.04',NULL,'Plads'),
('A3.05',NULL,'Plads'),
('A3.06',NULL,'Plads'),
('A3.07','222','Plads'),
('A3.08',NULL,'Plads'),
('A3.09',NULL,'Plads'),
('A3.10',NULL,'Plads'),
('A3.11',NULL,'Plads'),
('A3.12',NULL,'Plads'),
('A3.13',NULL,'Plads'),
('A3.14',NULL,'Plads'),
('A3.15',NULL,'Plads'),
('B1.01',NULL,'Plads'),
('B1.02',NULL,'Plads'),
('B1.03',NULL,'Plads'),
('B1.04',NULL,'Plads'),
('B1.05',NULL,'Plads'),
('B1.06',NULL,'Plads'),
('B1.07',NULL,'Plads'),
('B1.08',NULL,'Plads'),
('B1.09',NULL,'Plads'),
('B1.10',NULL,'Plads'),
('B1.11',NULL,'Plads'),
('B1.12',NULL,'Plads'),
('B2.01',NULL,'Plads'),
('B2.02',NULL,'Plads'),
('B2.03',NULL,'Plads'),
('B2.04',NULL,'Plads'),
('B2.05',NULL,'Plads'),
('B2.06',NULL,'Plads'),
('B2.07',NULL,'Plads'),
('B2.08',NULL,'Plads'),
('B2.09',NULL,'Plads'),
('B2.10',NULL,'Plads'),
('B2.11',NULL,'Plads'),
('B2.12',NULL,'Plads'),
('B3.01',NULL,'Plads'),
('B3.02',NULL,'Plads'),
('B3.03',NULL,'Plads'),
('B3.04',NULL,'Plads'),
('B3.05',NULL,'Plads'),
('B3.06',NULL,'Plads'),
('B3.07',NULL,'Plads'),
('B3.08',NULL,'Plads'),
('B3.09',NULL,'Plads'),
('B3.10',NULL,'Plads'),
('B3.11',NULL,'Plads'),
('B3.12',NULL,'Plads'),
('C1.01',NULL,'Plads'),
('C1.02',NULL,'Plads'),
('C1.03',NULL,'Plads'),
('C1.04',NULL,'Plads'),
('C1.05',NULL,'Plads'),
('C1.06',NULL,'Plads'),
('C1.07',NULL,'Plads'),
('C1.08',NULL,'Plads'),
('C1.09',NULL,'Plads'),
('C1.10',NULL,'Plads'),
('C1.11',NULL,'Plads'),
('C1.12',NULL,'Plads'),
('C2.01',NULL,'Plads'),
('C2.02',NULL,'Plads'),
('C2.03',NULL,'Plads'),
('C2.04',NULL,'Plads'),
('C2.05',NULL,'Plads'),
('C2.06',NULL,'Plads'),
('C2.07',NULL,'Plads'),
('C2.08',NULL,'Plads'),
('C2.09',NULL,'Plads'),
('C2.10',NULL,'Plads'),
('C2.11',NULL,'Plads'),
('C2.12',NULL,'Plads'),
('C3.01',NULL,'Plads'),
('C3.02',NULL,'Plads'),
('C3.03',NULL,'Plads'),
('C3.04',NULL,'Plads'),
('C3.05',NULL,'Plads'),
('C3.06',NULL,'Plads'),
('C3.07',NULL,'Plads'),
('C3.08',NULL,'Plads'),
('C3.09',NULL,'Plads'),
('C3.10',NULL,'Plads'),
('C3.11',NULL,'Plads'),
('C3.12',NULL,'Plads'),
('D1.01',NULL,'Plads'),
('D1.02',NULL,'Plads'),
('D1.03',NULL,'Plads'),
('D1.04',NULL,'Plads'),
('D1.05',NULL,'Plads'),
('D1.06',NULL,'Plads'),
('D1.07',NULL,'Plads'),
('D1.08',NULL,'Plads'),
('D1.09',NULL,'Plads'),
('D1.10',NULL,'Plads'),
('D1.11',NULL,'Plads'),
('D1.12',NULL,'Plads'),
('D2.01',NULL,'Plads'),
('D2.02',NULL,'Plads'),
('D2.03',NULL,'Plads'),
('D2.04',NULL,'Plads'),
('D2.05',NULL,'Plads'),
('D2.06',NULL,'Plads'),
('D2.07',NULL,'Plads'),
('D2.08',NULL,'Plads'),
('D2.09','Sagen','Plads'),
('D2.10',NULL,'Plads'),
('D2.11',NULL,'Plads'),
('D2.12',NULL,'Plads'),
('D3.01',NULL,'Plads'),
('D3.02',NULL,'Plads'),
('D3.03',NULL,'Plads'),
('D3.04',NULL,'Plads'),
('D3.05',NULL,'Plads'),
('D3.06',NULL,'Plads'),
('D3.07',NULL,'Plads'),
('D3.08',NULL,'Plads'),
('D3.09',NULL,'Plads'),
('D3.10',NULL,'Plads'),
('D3.11',NULL,'Plads'),
('D3.12',NULL,'Plads'),
('E1.01',NULL,'Plads'),
('E1.02',NULL,'Plads'),
('E1.03','Sagen','Plads'),
('E1.04',NULL,'Plads'),
('E1.05',NULL,'Plads'),
('E1.06',NULL,'Plads'),
('E1.07',NULL,'Plads'),
('E1.08',NULL,'Plads'),
('E1.09',NULL,'Plads'),
('E1.10',NULL,'Plads'),
('E1.11',NULL,'Plads'),
('E1.12',NULL,'Plads'),
('E2.01',NULL,'Plads'),
('E2.02',NULL,'Plads'),
('E2.03',NULL,'Plads'),
('E2.04',NULL,'Plads'),
('E2.05',NULL,'Plads'),
('E2.06',NULL,'Plads'),
('E2.07',NULL,'Plads'),
('E2.08','test','Plads'),
('E2.09','6543','Plads'),
('E2.10','6543','Plads'),
('E2.11','6543','Plads'),
('E2.12','6543','Plads'),
('E3.01','6543','Plads'),
('E3.02','6543','Plads'),
('E3.03','6543','Plads'),
('E3.04','Sagen','Plads'),
('E3.05','6543','Plads'),
('E3.06','6543','Plads'),
('E3.07','test','Plads'),
('E3.08','test','Plads'),
('E3.09','test','Plads'),
('E3.10','test','Plads'),
('E3.11','test','Plads'),
('E3.12','test','Plads');
/*!40000 ALTER TABLE `Pladser` ENABLE KEYS */;

-- 
-- Definition of Sager
-- 

DROP TABLE IF EXISTS `Sager`;
CREATE TABLE IF NOT EXISTS `Sager` (
  `Sagsnummer` varchar(45) NOT NULL,
  `KundeId` int(11) NOT NULL,
  `Opbevaring_startdato` date NOT NULL,
  `Opbevaring_slutdato` date DEFAULT NULL,
  `Afsluttet` tinyint(1) NOT NULL DEFAULT '0',
  `Noter` varchar(5000) DEFAULT NULL,
  PRIMARY KEY (`Sagsnummer`),
  KEY `Kundesager_idx` (`KundeId`),
  CONSTRAINT `Kundesager` FOREIGN KEY (`KundeId`) REFERENCES `kunder` (`Id`) ON DELETE CASCADE ON UPDATE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- 
-- Dumping data for table Sager
-- 

/*!40000 ALTER TABLE `Sager` DISABLE KEYS */;
INSERT INTO `Sager`(`Sagsnummer`,`KundeId`,`Opbevaring_startdato`,`Opbevaring_slutdato`,`Afsluttet`,`Noter`) VALUES
('1',1,'2016-07-30 00:00:00',NULL,1,NULL),
('222',5,'2016-08-04 00:00:00',NULL,0,NULL),
('25954',8,'2016-08-04 00:00:00',NULL,1,NULL),
('64sefs',4,'2016-07-30 00:00:00',NULL,0,NULL),
('6543',9,'2017-03-19 00:00:00','2017-09-20 00:00:00',0,NULL),
('EnFalckMere',7,'2016-08-04 00:00:00',NULL,0,NULL),
('Gennem test af sag',13,'2017-09-21 00:00:00',NULL,0,'Tester\r\nlige \r\nnoter'),
('Gennem test af sag (1)',14,'2017-09-21 00:00:00',NULL,0,'note'),
('Sagen',6,'2016-08-04 00:00:00',NULL,0,NULL),
('Sagen (1)',10,'2017-09-19 00:00:00','2017-09-20 00:00:00',1,''),
('Sagen (2)',11,'2017-09-19 00:00:00',NULL,0,''),
('test',12,'2017-09-20 00:00:00','2017-09-21 00:00:00',0,'Tester\r\nlige\r\nnoterne\r\n:D');
/*!40000 ALTER TABLE `Sager` ENABLE KEYS */;


/*!40101 SET SQL_MODE=@OLD_SQL_MODE */;
/*!40014 SET FOREIGN_KEY_CHECKS=@OLD_FOREIGN_KEY_CHECKS */;
/*!40014 SET UNIQUE_CHECKS=@OLD_UNIQUE_CHECKS */;
/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
/*!40111 SET SQL_NOTES=@OLD_SQL_NOTES */;


-- Dump completed on 2017-09-27 18:07:29
-- Total time: 0:0:0:0:127 (d:h:m:s:ms)
