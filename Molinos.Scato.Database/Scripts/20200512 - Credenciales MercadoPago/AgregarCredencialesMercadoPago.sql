USE [Scato]
GO
--QA
--https://www.mercadopago.com.ar/developers/es/guides/marketplace/api/create-marketplace/

--USUARIO VENDEDOR whitelisted para cobrar con este modelo
--Usuario: camara_sl_test Contraseña: qatest9590

--https://auth.mercadopago.com.ar/authorization?client_id=APP_ID&response_type=code&platform_id=mp&redirect_uri=https://scatoqa.molinosagro.com.ar/Scato.Web/PagoDeReciboMunicipal/AutorizarMercadoPago

--Cust_id o Collector_id: 493527931
--USUARIO COMPRADOR (Este va en el celular) → 
--Usuario: test_user_74197867@testuser.com Contraseña: qatest5478
--Usuario: tete7034344 Pass: qatest371 


--GENERAR LOCAL EN MERCADO PAGO y asociar garita!
--INSERT INTO [dbo].[CredencialMercadoPago]
--           ([PublicKey]
--           ,[AccessToken]
--           ,[AppId]
--           ,[SecretKey]
--           ,[RedirectUri]
--            )
       
--     VALUES
--           ('APP_USR-b2c152d2-d7f9-4ebb-8aa7-22ea879ff5c3'
--           ,'APP_USR-205309017804878-112706-58ce82778f97a1cb9e4568a396d036d7-493527931'
--           ,205309017804878
--           ,'iUOto2d6FOJwr8UOqdFcjgN3y5Oma2Df'
--           ,'https://scatoqa.molinosagro.com.ar/Scato.Web/PagoDeReciboMunicipal/AutorizarMercadoPago'
--		   )
--GO