alter table `eTRIKSdata`.`Variable_Reference_TBL` drop foreign key `FK_VariableRef_TBL_Dataset_TBL_ActivityDatasetId`;
alter table `Variable_Reference_TBL` drop foreign key `FK_VariableRef_TBL_VariableDef_TBL_VariableId`;
alter table `Derived_Method_TBL` drop foreign key `FK_DerivedMethod_TBL_VariableDef_TBL_DerivedVariable_OID`;
alter table `Variable_Reference_TBL` drop index `IX_VariableId`;
alter table `Variable_Reference_TBL` drop index `IX_ActivityDatasetId`;
alter table `Derived_Method_TBL` drop index `IX_DerivedVariable_OID`;
 alter table `Dataset_TBL` drop primary key ;
 alter table `Variable_Reference_TBL` drop primary key; 
 alter table `Variable_Definition_TBL` drop primary key; 
alter table `Variable_Definition_TBL` add column `Accession` longtext ;
alter table `Dataset_TBL` modify `OID` int not null auto_increment primary key; 
ALTER TABLE `eTRIKSdata`.`Variable_Reference_TBL` 
CHANGE COLUMN `VariableId` `VariableId` INT(10) NOT NULL ,
CHANGE COLUMN `ActivityDatasetId` `ActivityDatasetId` INT(10) NOT NULL ;
alter table `Variable_Definition_TBL` modify `OID` int not null  auto_increment primary key; 
alter table `Derived_Method_TBL` modify `DerivedVariable_OID` INT(10) NOT NULL;
alter table `Variable_Reference_TBL` add primary key  `PK_Variable_Reference_TBL` ( `VariableId`,`ActivityDatasetId`) ;
CREATE index  `IX_VariableId` on `Variable_Reference_TBL` (`VariableId` DESC) using HASH;
CREATE index  `IX_ActivityDatasetId` on `Variable_Reference_TBL` (`ActivityDatasetId` DESC) using HASH;
CREATE index  `IX_DerivedVariable_OID` on `Derived_Method_TBL` (`DerivedVariable_OID` DESC) using HASH;
alter table `Variable_Reference_TBL` add constraint `FK_VariableRef_Dataset_ActivityDatasetId`  foreign key (`ActivityDatasetId`) references `Dataset_TBL` ( `OID`)  on update cascade on delete cascade ;
alter table `Variable_Reference_TBL` add constraint `FK_VariableRef_VariableDef_VariableId`  foreign key (`VariableId`) references `Variable_Definition_TBL` ( `OID`)  on update cascade on delete cascade ;
alter table `Derived_Method_TBL` add constraint `FK_DerivedMethod_VariableDef_DerivedVarOID`  foreign key (`DerivedVariable_OID`) references `Variable_Definition_TBL` ( `OID`) ;
