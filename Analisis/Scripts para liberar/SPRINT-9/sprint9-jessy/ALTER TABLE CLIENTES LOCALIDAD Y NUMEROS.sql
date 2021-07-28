IF COL_LENGTH('Clientes', 'numeroInterior') IS  NULL
BEGIN
    Alter table Clientes add numeroInterior varchar(20) 
END

IF COL_LENGTH('Clientes', 'numeroExterior') IS  NULL
BEGIN
    Alter table Clientes add numeroExterior varchar(20) 
END

IF COL_LENGTH('Clientes', 'localidad') IS  NULL
BEGIN
    Alter table Clientes add localidad varchar(250) 
END


