cd /d %~dp0
START cmd.exe /k "Molinos.Scato.ServiciosWeb.deploy.cmd /Y -setParamFile:%~dp0ServiciosWeb\<ambiente>.DeployParameters.xml"