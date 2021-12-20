cd /d %~dp0
START cmd.exe /k "Molinos.Scato.WebOperaciones.deploy.cmd /Y -setParamFile:%~dp0WebOperaciones\<ambiente>.DeployParameters.xml"