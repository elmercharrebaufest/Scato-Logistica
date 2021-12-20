cd /d %~dp0
START cmd.exe /k "Molinos.Scato.Web.deploy.cmd /Y -setParamFile:%~dp0Web\<ambiente>.DeployParameters.xml"