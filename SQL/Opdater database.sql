CREATE TABLE Plads_historik (
	ID int NOT NULL auto_increment,
	Sagsnummer Varchar(45) NOT NULL,
    Pladsnummer Varchar(45) NOT NULL,
    Opret_afslut Varchar(10) NOT NULL,
    Dato datetime NOT NULL,
    PRIMARY KEY (ID),
    FOREIGN KEY (Sagsnummer) REFERENCES Sager(Sagsnummer),
    FOREIGN KEY (Pladsnummer) REFERENCES Pladser(Pladsnummer)
);

ALTER TABLE `Bifald`.`Sager` 
ADD COLUMN `Noter` VARCHAR(5000) NULL AFTER `Afsluttet`;