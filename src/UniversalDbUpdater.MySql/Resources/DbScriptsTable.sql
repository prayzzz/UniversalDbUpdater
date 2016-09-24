CREATE TABLE `Infrastructure.DbScripts` (
	`Id` INT UNSIGNED NOT NULL AUTO_INCREMENT,
	`Date` DATE NOT NULL,
	`Name` VARCHAR(255) NOT NULL,
	`Description` TEXT NOT NULL,
	PRIMARY KEY (`Id`)
)
COLLATE='utf8_general_ci'
ENGINE=InnoDB;
