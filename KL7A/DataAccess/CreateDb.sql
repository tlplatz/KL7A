CREATE TABLE YearlySetting
(
	Id int identity (1,1),
	Name nvarchar(255) not null,
	Classification nvarchar(255) not null,
	[Date] datetime not null,
	Indicator nvarchar(5) not null,
	NumericIndicator nvarchar(5) not null,
	constraint PKYearlySettings primary key (Id),
	constraint UKYearlySettings unique (Name, [Date])
)

CREATE TABLE Wiring
(
	Id int identity(1,1),
	Plate1 nvarchar (36) not null,
	Plate2 nvarchar (36) not null,
	MonthlyId int not null,
	constraint PKWiring primary key (Id)
)

CREATE TABLE RotorWiringDefinition
(
	WiringId int not null,
	Ordinal int not null,
	Value nvarchar (36) not null,
	constraint PKRotorWiringDef primary key (WiringId, Ordinal)
)

CREATE TABLE NotchDefinition
(
	WiringId int not null,
	Ordinal int not null,
	Value nvarchar(36) not null,
	constraint PKNotchDef primary key (WiringId, Ordinal)
)

CREATE TABLE MoveDefinition
(
	WiringId int not null,
	Ordinal int not null,
	Value nvarchar(36) not null,
	constraint PKMoveDef primary key (WiringId, Ordinal)
)

CREATE TABLE MonthlySetting
(
	Id int identity (1,1),
	Name nvarchar(255) not null,
	Classification nvarchar(255) not null,
	[Date] datetime not null,
	Indicator nvarchar(5) not null,
	NumericIndicator nvarchar(5) not null,
	YearlySettingId int not null,
	constraint PKMonthlySettings primary key (Id),
	constraint UKMonthlySettings unique (YearlySettingId, [Date])
)

CREATE TABLE DailySetting
(
	Id int identity(1,1),
	[Date] datetime not null,
	Indicator nvarchar(5) not null,
	NumericIndicator nvarchar(5) not null,
	StartPosition nvarchar(8) not null,
	[Check] nvarchar(10) not null,
	MonthlySettingId int not null,
	constraint PKSettings primary key (Id),
	constraint UKSettings unique (MonthlySettingId, [Date])
)	        

CREATE TABLE RotorSetting
(
	Id int identity (1,1),
	Ordinal int not null,
	RotorName int not null,
	AlphabetRingPosition int not null,
	NotchRingName int not null,
	NotchRingPosition int not null,
	DailySettingId int not null,
	constraint PKRotorSetting primary key (Id),
	constraint UKRotorSetting unique (DailySettingId, Ordinal)
)

alter table Wiring add constraint FKWiringMonthly foreign key (MonthlyId) references MonthlySetting(Id)
alter table RotorWiringDefinition add constraint FKWiringRotor foreign key (WiringId) references Wiring(Id)
alter table NotchDefinition add constraint FKWiringNotch foreign key (WiringId) references Wiring(Id)
alter table MoveDefinition add constraint FKWiringMove foreign key (WiringId) references Wiring(Id)
alter table MonthlySetting add constraint FKMonthYear foreign key (YearlySettingId) references YearlySetting(Id)
alter table DailySetting add constraint FKDailyMonth foreign key (MonthlySettingId) references MonthlySetting(Id)
alter table RotorSetting add constraint FKRotorDaily foreign key (DailySettingId) references DailySetting(Id)

