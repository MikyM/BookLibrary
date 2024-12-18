create database keycloak; 

create user keycloak with password 'password'; 

alter database keycloak owner to keycloak;

create user booklibrary with password 'password'; 

alter database booklibrary owner to booklibrary;